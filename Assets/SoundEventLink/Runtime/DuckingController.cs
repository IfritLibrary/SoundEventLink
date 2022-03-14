using System;
using System.Collections.Generic;
using System.Linq;
using SoundEventLink.Runtime.Node.Output;
using UnityEngine;
using UnityEngine.Audio;
namespace SoundEventLink.Runtime
{
	public class DuckingController
	{
		private readonly AudioMixer _mixer;

		public DuckingController(AudioMixer mixer) => _mixer = mixer;

		private readonly Dictionary<PlayAudioData, string> _audioDataToParameter = new Dictionary<PlayAudioData, string>();
		private readonly Dictionary<string, List<Data>> _dataList = new Dictionary<string, List<Data>>();
		private readonly DataComparer _dataComparer = new DataComparer();

		private readonly List<int> _deleteIndex = new List<int>();

		/// <summary>
		/// FadeIn は優先度高いものがFadeといえば絶対にFadeする
		/// FadeOut は優先度が低いものが他に残っていたとしたらそちらに処理を委ねる
		/// </summary>
		public void Update()
		{
			var time = Time.time;
			foreach (var pair in _dataList)
			{
				if (pair.Value.Count == 0)
					continue;

				_deleteIndex.Clear();
				
				for (var i = 0; i < pair.Value.Count; i++)
				{
					var value = pair.Value[i];
					if (value.IsFadeIn)
					{
						// Fadeなし
						if (Mathf.Approximately(value.Node._inTime, 0))
						{
							value.IsFadeIn = false;
							value.Volume   = value.Node._audio;
						}
						else
						{
							if (value.FadeInStartVolume == null)
							{
								_mixer.GetFloat(value.Node._audioParameter, out var volume);
								value.FadeInStartVolume = volume;
							}
							var weight = (time - value.Time) / value.Node._inTime;
							if (weight >= 1)
							{
								value.IsFadeIn = false;
								value.Volume   = value.Node._audio;
							}
							else
								value.Volume = Mathf.Lerp(value.FadeInStartVolume.Value, value.Node._audio, weight);
						}
					}
					if (value.IsFadeOut)
					{						
						// Fadeなし
						if (Mathf.Approximately(value.Node._outTime, 0))
						{
							value.Volume   = value.InitVolume;
							_deleteIndex.Add(i);
							if(pair.Value.Count > i + 1)
								continue;
						}
						else
						{
							// Fade中だが下位のPriorityでまだ動いているのでそっちに委任
							if (pair.Value.Count > i + 1)
							{
								var weight = (time - value.Time) / value.Node._outTime;
								if (weight >= 1)
								{
									value.Volume = value.InitVolume;
									_deleteIndex.Add(i);
								}

								// FadeOut状態出ないものが下位で動いている
								if (pair.Value.Skip(i + 1).Any(v => !v.IsFadeOut))
									continue;
							}
							else
							{
								// FadeOutが始まった初回なのでデータを初期化
								if (value.FadeOutStartVolume == null)
								{
									_mixer.GetFloat(value.Node._audioParameter, out var volume);
									value.FadeOutStartVolume = volume;
									value.Time               = time;
								}
								var weight = (time - value.Time) / value.Node._outTime;
								if (weight >= 1)
								{
									value.Volume = value.InitVolume;
									_deleteIndex.Add(i);
									if (pair.Value.Count > i + 1)
										continue;
								}
								else
									value.Volume = Mathf.Lerp(value.FadeOutStartVolume.Value, value.InitVolume, weight);
							}
						}
					}

					_mixer.SetFloat(value.Node._audioParameter, value.Volume);
					break;
				}

				for (var i = _deleteIndex.Count - 1; i >= 0; i--)
				{
					pair.Value.RemoveAt(_deleteIndex[i]);
				}
			}
		}

		public void AddData(DuckingNode duckingNode, PlayAudioData audioData)
		{
			_audioDataToParameter.Add(audioData, duckingNode._audioParameter);

			if (_dataList.ContainsKey(duckingNode._audioParameter))
			{
				var list = _dataList[duckingNode._audioParameter];

				// 開始時のVolumeを覚えるを続けているとデータが全て同じ値になるはず
				float volume;
				if (list.Count == 0)
				{
					if (!_mixer.GetFloat(duckingNode._audioParameter, out volume))
					{
						Debug.LogError($"[SoundEventLink] 「{duckingNode._audioParameter}」がAudioMixerにありません");
						return;
					}
				}
				else
				{
					volume = list[0].InitVolume;
				}
				var data = new Data
				{
					InitVolume = volume,
					Node       = duckingNode,
					AudioData  = audioData
				};
				list.Add(data);
				list.Sort(_dataComparer);
			}
			else
			{
				if (!_mixer.GetFloat(duckingNode._audioParameter, out var volume))
				{
					Debug.LogError($"[SoundEventLink] 「{duckingNode._audioParameter}」がAudioMixerにありません");
					return;
				}
				var data = new Data
				{
					InitVolume = volume,
					Node       = duckingNode,
					AudioData  = audioData
				};
				_dataList.Add(duckingNode._audioParameter, new List<Data> { data });
			}
		}

		public void StopAudio(PlayAudioData data)
		{
			if (!_audioDataToParameter.ContainsKey(data))
				return;
			var audioParameter = _audioDataToParameter[data];
			_audioDataToParameter.Remove(data);
			var list = _dataList[audioParameter];
			var idx  = list.FindIndex(d => d.AudioData == data);
			list[idx].IsFadeOut = true;
		}

		private class DataComparer : IComparer<Data>
		{
			public int Compare(Data x, Data y)
			{
				return x.Node._priority - y.Node._priority;
			}
		}
		private class Data
		{
			/// <summary>
			/// 初期値、Ducking終了後もとに戻す値
			/// </summary>
			public float InitVolume;

			/// <summary>
			/// FadeIn,FadeOutを管理するタイマー
			/// </summary>
			public float Time = UnityEngine.Time.time;

			/// <summary>
			/// FadeIn開始時の値
			/// </summary>
			public float? FadeInStartVolume;

			/// <summary>
			/// FadeOut開始時の値
			/// </summary>
			public float? FadeOutStartVolume;

			/// <summary>
			/// FadeIn中
			/// </summary>
			public bool IsFadeIn = true;

			/// <summary>
			/// FadeOut中
			/// </summary>
			public bool IsFadeOut = false;

			/// <summary>
			/// 現在のVolume
			/// </summary>
			public float Volume;

			public DuckingNode Node;
			public PlayAudioData AudioData;
		}
		
		#if UNITY_EDITOR
		public void ForEachActiveDucking(Action<string, float, float, float, int> action)
		{
			var time = Time.time;
			foreach (var pair in _dataList)
			{
				foreach (var data in pair.Value)
				{
					action(pair.Key,
						data.IsFadeIn ? (time  - data.Time) / data.Node._inTime : 1.0f,
						data.IsFadeOut ? (time - data.Time) / data.Node._outTime : 0.0f,
						data.Volume,
						data.Node._priority);
				}
			}
		}
		#endif
	}
}