using UnityEngine;

namespace SoundEventLink.Runtime
{
	public class PlayAudioData
	{
		public bool IsBgm { get; set; }
		public string Key { get; set; }
		public AudioSource AudioSource { get; set; }
	}
}