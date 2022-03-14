using System;
using GraphProcessor;
using UnityEngine;
using UnityEngine.Audio;

namespace SoundEventLink.Runtime.Node.Output
{
	[Serializable, NodeMenuItem("Custom/SEResult")]
	public class SEResultNode : BaseNode
	{
		[Input(name = "Audio Clip"), ShowAsDrawer]
		public AudioClip _audioClip;

		[Input(name = "Audio Group"), ShowAsDrawer]
		public AudioMixerGroup _audioMixerGroup;

		[Input(name = "Delay"), ShowAsDrawer]
		public float _delay;

		[Input(name = "Volume"), ShowAsDrawer]
		public float _volume = 1;

		public override string name => "SEResult";

		public override string ToString() => $"{(_audioClip == null ? "null" : _audioClip.name)} volume:{_volume} delay:{_delay}";
	}
}