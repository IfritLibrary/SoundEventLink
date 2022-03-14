using System;
using GraphProcessor;
using UnityEngine;
using UnityEngine.Audio;

namespace SoundEventLink.Runtime
{
	[Serializable]
	public class AudioClipParameter : ExposedParameter
	{
		[SerializeField] private AudioClip val;

		public override object value
		{
			get => val;
			set => val = (AudioClip)value;
		}

		public override Type GetValueType() => typeof(AudioClip);
	}

	[Serializable]
	public class AudioMixerParameter : ExposedParameter
	{
		[SerializeField] private AudioMixerGroup val;

		public override object value
		{
			get => val;
			set => val = (AudioMixerGroup)value;
		}

		public override Type GetValueType() => typeof(AudioMixerGroup);
	}
}