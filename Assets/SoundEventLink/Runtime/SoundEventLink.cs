using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
#if SOSHEETMM_GENERATED
using SOSheetMasterMemory.Generated;
#else
#pragma warning disable CS1998
#endif
using SoundEventLink.Runtime.Node;
using SoundEventLink.Runtime.Node.Output;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.Pool;
using Object = UnityEngine.Object;
namespace SoundEventLink.Runtime
{
	public class SoundEventLink : MonoBehaviour
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Initialize()
		{
			var obj = new GameObject(nameof(SoundEventLink));
			Instance = obj.AddComponent<SoundEventLink>();
			Instance.SetMixer(Addressables.LoadAssetAsync<AudioMixer>("SEL/AudioMixer").WaitForCompletion());
			DontDestroyOnLoad(obj);
		}
		
		public static SoundEventLink Instance { get; private set; }

		// Ducking用データ
		private AudioMixer _audioMixer;
		internal DuckingController DuckingController;
		
		internal List<PlayAudioData> PlayAudioDataList { get; } = new List<PlayAudioData>();
		internal ObjectPool<AudioSource> Pool;
		private readonly Dictionary<string, object> _addressableCache = new Dictionary<string, object>();

		private void SetMixer(AudioMixer mixer)
		{
			_audioMixer       = mixer;
			DuckingController = new DuckingController(mixer);
		}
		
		private void Awake()
		{
			Instance = this;
			Pool = new ObjectPool<AudioSource>(CreatePooledItem,
				source => source.gameObject.SetActive(true),
				source => source.gameObject.SetActive(false),
				source => Destroy(source.gameObject),
				true, 10, 512);
		}

		public void Update()
		{
			DuckingController.Update();
		}

		private void OnDestroy()
		{
			Addressables.Release(_audioMixer);
			ClearCache();
		}

		public void ClearCache()
		{
			foreach (var pair in _addressableCache)
			{
				Addressables.Release(pair.Value);
			}
			_addressableCache.Clear();
		}

		public async UniTask Play(string eventKey, Vector3 position, params object[] customProperty)
		{
			#if SOSHEETMM_GENERATED
			if (MasterDataAccessor.Database.SoundEventLinkDataTable.TryFindById(eventKey, out var result))
			{
				// Graphデータがない場合は再生しない
				if (result.GraphName == null)
					return;

				var graphData = await LoadAsync<SoundEventLinkGraph>(result.GraphName);
				
				var resultIdx = 0;
				var customIdx = 0;
				foreach (var exposedParameter in graphData.exposedParameters)
				{
					var type = exposedParameter.GetValueType();
					if (exposedParameter.name.StartsWith("Custom" + customIdx))
					{
						if (type != customProperty[customIdx].GetType())
						{
							Debug.LogError($"SoundEventLink Play Error! Dont match customProperty Custom{customIdx}:need [{type}] arg [{customProperty[customIdx].GetType()}]");
							return;
						}
						exposedParameter.value = customProperty[customIdx];
						customIdx++;
					}
					else
					{
						if (type == typeof(AudioClip))
						{
							var clipAo = await LoadAsync<AudioClip>((string)result.ExposedParameterList[resultIdx]);
							exposedParameter.value = clipAo;
						}
						else if (type == typeof(AudioMixerGroup))
							exposedParameter.value = _audioMixer.FindMatchingGroups((string)result.ExposedParameterList[resultIdx]).FirstOrDefault();
						else
							exposedParameter.value = result.ExposedParameterList[resultIdx];
						resultIdx++;
					}
				}
				
				if (result.ExposedParameterList != null)
				{
					for (var i = 0; i < result.ExposedParameterList.Length; i++)
					{
						var type = graphData.exposedParameters[i].GetValueType();
						if (type == typeof(AudioClip))
						{
							var clipAo = await LoadAsync<AudioClip>((string)result.ExposedParameterList[i]);
							graphData.exposedParameters[i].value = clipAo;
						}
						else if (type == typeof(AudioMixerGroup))
							graphData.exposedParameters[i].value = _audioMixer.FindMatchingGroups((string)result.ExposedParameterList[i]).FirstOrDefault();
						else
							graphData.exposedParameters[i].value = result.ExposedParameterList[i];
					}
				}
				var processor = new SoundEventLinkProcessor(graphData);
				processor.Run();

				foreach (var bgmResultNode in processor.BGMResultNodeList)
				{
					PlayBGM(eventKey, bgmResultNode).Forget();
				}
				
				await UniTask.WhenAll(processor.SEResultList
				                               .Where(res => res._audioClip != null)
				                               .Select(res => PlaySE(eventKey, position, res, processor.DuckingNodeList)));
			}
			else
			{
			#if UNITY_EDITOR
				CreateScriptableObject(eventKey);
			#endif
			}
#else
			Debug.LogError("ScriptableObjectSheetMasterMemory Is Not Generated");
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
			audioSource.clip                  = data._audioClip;
			audioSource.outputAudioMixerGroup = data._audioMixerGroup;
			audioSource.volume                = data._volume;
			audioSource.transform.position    = position;
			audioSource.spatialBlend = 1.0f;
			audioSource.loop = false;
			audioSource.Play();
			var playData = new PlayAudioData
			{
				IsBgm = false,
				Key         = key,
				AudioSource = audioSource
			};

			foreach (var duckingNode in duckingNodeList)
			{
				DuckingController.AddData(duckingNode, playData);
			}

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
				AudioSource = audioSource
			};
			var playBgm = PlayAudioDataList.FirstOrDefault(data => data.IsBgm);
			PlayAudioDataList.Add(playData);

			if (playBgm != null)
			{
				if (node._fadeTime == 0)
				{
					playBgm.AudioSource.Stop();
				}
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

		private async UniTask<T> LoadAsync<T>(string address)
		{
			if (_addressableCache.TryGetValue(address, out var value))
				return  (T)value;
			var result = await Addressables.LoadAssetAsync<T>(address);
			_addressableCache.Add(address, result);
			return result;
		}
		
		private static AudioSource CreatePooledItem()
		{
			var go     = new GameObject("Pooled Audio Source");
			var source = go.AddComponent<AudioSource>();
			return source;
		}
		
	#if UNITY_EDITOR
		private static void CreateScriptableObject(string keyName)
		{
			const string baseInfo = @"%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_Script: {fileID: 11500000, guid: 445e02a76cc1482bb13984b007faaa0e, type: 3}
  m_Name: ";
			File.WriteAllText("Assets/Work/SoundEventLink/" + keyName + ".asset", baseInfo + keyName);
		}
	#endif
	}
}