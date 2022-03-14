using System;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;
using Random = UnityEngine.Random;
namespace SoundEventLink.Runtime.Node
{
	[Serializable, NodeMenuItem("Random/RandomSelect")]
	public class RandomSelectNode : BaseNode
	{
		[Input(name = "Audio Clip1"), ShowAsDrawer]
		public AudioClip _audioClip1;

		[Input(name = "Audio Clip2"), ShowAsDrawer]
		public AudioClip _audioClip2;

		[Input(name = "Audio Clip3"), ShowAsDrawer]
		public AudioClip _audioClip3;

		[Input(name = "Audio Clip4"), ShowAsDrawer]
		public AudioClip _audioClip4;

		[Input(name = "Audio Clip5"), ShowAsDrawer]
		public AudioClip _audioClip5;

		[Input(name = "Audio Clip6"), ShowAsDrawer]
		public AudioClip _audioClip6;

		[Input(name = "Audio Clip7"), ShowAsDrawer]
		public AudioClip _audioClip7;

		[Input(name = "Audio Clip8"), ShowAsDrawer]
		public AudioClip _audioClip8;

		[Output(name = "Output")]
		public AudioClip _output;

		public override string name => "RandomSelect";

		protected override void Process()
		{
			var list = new List<AudioClip>();
			if (_audioClip1 != null)
				list.Add(_audioClip1);
			if (_audioClip2 != null)
				list.Add(_audioClip2);
			if (_audioClip3 != null)
				list.Add(_audioClip3);
			if (_audioClip4 != null)
				list.Add(_audioClip4);
			if (_audioClip5 != null)
				list.Add(_audioClip5);
			if (_audioClip6 != null)
				list.Add(_audioClip6);
			if (_audioClip7 != null)
				list.Add(_audioClip7);
			if (_audioClip8 != null)
				list.Add(_audioClip8);
			_output = list[Random.Range(0, list.Count)];
		}
	}
}