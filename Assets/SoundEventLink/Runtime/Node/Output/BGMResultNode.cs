using System;
using GraphProcessor;
using UnityEngine;
using UnityEngine.Audio;

namespace SoundEventLink.Runtime.Node.Output
{
    [Serializable, NodeMenuItem("Custom/BGMResult")]
    public class BGMResultNode : BaseNode
    {
        [Input(name = "Audio Clip"), ShowAsDrawer]
        public AudioClip _audioClip;
		
        [Input(name = "Audio Group"), ShowAsDrawer]
        public AudioMixerGroup _audioMixerGroup;
		
        [Input(name = "Volume"), ShowAsDrawer]
        public float _volume = 1;

        [Input(name = "FadeTime"), ShowAsDrawer]
        public float _fadeTime = 0;
		
        public override string name => "BGMResult";
    }
}