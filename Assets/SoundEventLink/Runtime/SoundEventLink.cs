using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DataSheet;
using SoundEventLink.Runtime.Node.Output;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.Pool;

namespace SoundEventLink.Runtime
{
    public class SoundEventLink : MonoBehaviour
    {
        private readonly Dictionary<string, IDisposable> _addressableCache = new();

        // Ducking用データ
        private AddressableResource<AudioMixer> _audioMixer;
        internal DuckingController DuckingController;
        internal ObjectPool<AudioSource> Pool;

        public static SoundEventLink Instance { get; private set; }

        internal List<PlayAudioData> PlayAudioDataList { get; } = new();

        private void Awake()
        {
            Instance = this;
            Pool = new ObjectPool<AudioSource>(CreatePooledItem,
                source => source.gameObject.SetActive(true),
                source => source.gameObject.SetActive(false),
                source => Destroy(source.gameObject),
                true, 10, 512);
            
            _audioMixer = AddressableResource<AudioMixer>.Load("SoundEventLinkData/AudioMixer");
            DuckingController = new DuckingController(_audioMixer?.Value);
        }

        public void Update()
        {
            DuckingController.Update();
        }

        private void OnDestroy()
        {
            _audioMixer?.Dispose();
            ClearCache();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            var obj = new GameObject(nameof(SoundEventLink));
            Instance = obj.AddComponent<SoundEventLink>();
            DontDestroyOnLoad(obj);
        }

        public void ClearCache()
        {
            foreach (var pair in _addressableCache)
                pair.Value.Dispose();
            _addressableCache.Clear();
        }

        private async UniTask<SoundEventLinkData> GetData(string eventKey)
        {
#if SOUNDEVENTLINK__SOUNDEVENTLINKDATA_GENERATED
            if (_addressableCache.TryGetValue(eventKey, out var find))
            {
                return (find as AddressableResource<SoundEventLinkData>)?.Value;
            }

            var accessor = await SoundEventLinkDataAccessor.GetAsync(eventKey);
            _addressableCache.Add(eventKey, accessor);
            return accessor.Value;
#else
            return null;
#endif
        }
#if SOUNDEVENTLINK__SOUNDEVENTLINKDATA_GENERATED
        public UniTask Play(EnumSoundEventLinkData eventKey, Vector3 position, params object[] customProperty) => Play(eventKey.ToString(), position, customProperty);
#endif

        public async UniTask Play(string eventKey, Vector3 position, params object[] customProperty)
        {
#if SOUNDEVENTLINK__SOUNDEVENTLINKDATA_GENERATED
            var result = await GetData(eventKey);

            // Graphデータがない場合は再生しない
            if (result.Graph == null)
                return;

            var graphData = result.Graph;

            var boolCnt = 0;
            var floatCnt = 0;
            var stringCnt = 0;
            var objCnt = 0;

            for (var i = 0; i < graphData.exposedParameters.Count; i++)
            {
                var exposedParameter = graphData.exposedParameters[i];

                switch (result.ExposedIndexList[i])
                {
                    case ExposedType.Bool:
                        exposedParameter.value = result.ExposedBoolList[boolCnt];
                        boolCnt++;
                        break;
                    case ExposedType.Float:
                        exposedParameter.value = result.ExposedFloatList[floatCnt];
                        floatCnt++;
                        break;
                    case ExposedType.String:
                        exposedParameter.value = result.ExposedStringList[stringCnt];
                        stringCnt++;
                        break;
                    case ExposedType.Object:
                        exposedParameter.value = result.ExposedObjectList[objCnt];
                        objCnt++;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            var processor = new SoundEventLinkProcessor(graphData);
            processor.Run();

            foreach (var bgmResultNode in processor.BGMResultNodeList)
                PlayBGM(eventKey, bgmResultNode).Forget();

            await UniTask.WhenAll(processor.SEResultList
                .Where(res => res._audioClip != null)
                .Select(res => PlaySE(eventKey, position, res, processor.DuckingNodeList)));
#else
            await UniTask.Yield();
#endif
        }

        private async UniTask EndCheck(PlayAudioData source)
        {
            while (true)
            {
                await UniTask.Yield();
                if (source.AudioSource.isPlaying) continue;
                Pool.Release(source.AudioSource);

                PlayAudioDataList.Remove(source);
                DuckingController.StopAudio(source);

                break;
            }
        }

        private async UniTask PlaySE(string key, Vector3 position, SEResultNode data, IEnumerable<DuckingNode> duckingNodeList)
        {
            await UniTask.Delay((int)(data._delay * 1000));

            var audioSource = Pool.Get();
            audioSource.clip = data._audioClip;
            audioSource.outputAudioMixerGroup = data._audioMixerGroup;
            audioSource.volume = data._volume;
            audioSource.transform.position = position;
            audioSource.spatialBlend = 1.0f;
            audioSource.loop = false;
            audioSource.Play();
            var playData = new PlayAudioData
            {
                IsBgm = false,
                Key = key,
                AudioSource = audioSource,
            };

            foreach (var duckingNode in duckingNodeList) DuckingController.AddData(duckingNode, playData);

            PlayAudioDataList.Add(playData);
            await EndCheck(playData);
        }

        private async UniTask PlayBGM(string key, BGMResultNode node)
        {
            var audioSource = Pool.Get();
            audioSource.clip = node._audioClip;
            audioSource.outputAudioMixerGroup = node._audioMixerGroup;
            audioSource.volume = 0;
            audioSource.spatialBlend = 0.0f;
            audioSource.loop = true;
            audioSource.Play();

            var playData = new PlayAudioData
            {
                IsBgm = true,
                Key = key,
                AudioSource = audioSource,
            };
            var playBgm = PlayAudioDataList.FirstOrDefault(data => data.IsBgm);
            PlayAudioDataList.Add(playData);

            if (playBgm != null)
            {
                if (node._fadeTime == 0)
                    playBgm.AudioSource.Stop();
                else
                {
                    var startTime = Time.time;
                    var endTime = startTime + node._fadeTime;

                    while (endTime > Time.time)
                    {
                        var weight = (Time.time - startTime) / node._fadeTime;
                        playBgm.AudioSource.volume = 1.0f - weight;
                        audioSource.volume = weight;
                        await UniTask.Yield();
                    }

                    playBgm.AudioSource.Stop();
                }
            }

            audioSource.volume = 1;
            await EndCheck(playData);
        }

        private static AudioSource CreatePooledItem()
        {
            var go = new GameObject("Pooled Audio Source");
            var source = go.AddComponent<AudioSource>();
            return source;
        }
    }
}