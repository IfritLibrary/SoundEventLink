using System;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;
using Random = UnityEngine.Random;
namespace SoundEventLink.Runtime.Node
{
	/// <summary>
	/// 同じ選択を二度連続でしない
	/// </summary>
	[Serializable, NodeMenuItem("Random/Shuffle")]
	public class ShuffleNode : BaseNode
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

		private AudioClip _beforeSelect;

		[Output(name = "Output")]
		public AudioClip _output;

		public override string name => "Shuffle";

		protected override void Process()
		{
			var list = new List<AudioClip>();
			if (_audioClip1 != null && _audioClip1 != _beforeSelect)
				list.Add(_audioClip1);
			if (_audioClip2 != null && _audioClip2 != _beforeSelect)
				list.Add(_audioClip2);
			if (_audioClip3 != null && _audioClip3 != _beforeSelect)
				list.Add(_audioClip3);
			if (_audioClip4 != null && _audioClip4 != _beforeSelect)
				list.Add(_audioClip4);
			if (_audioClip5 != null && _audioClip5 != _beforeSelect)
				list.Add(_audioClip5);
			if (_audioClip6 != null && _audioClip6 != _beforeSelect)
				list.Add(_audioClip6);
			if (_audioClip7 != null && _audioClip7 != _beforeSelect)
				list.Add(_audioClip7);
			if (_audioClip8 != null && _audioClip8 != _beforeSelect)
				list.Add(_audioClip8);
			_output       = list[Random.Range(0, list.Count)];
			_beforeSelect = _output;
		}
	}
}