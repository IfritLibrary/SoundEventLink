using System;
using GraphProcessor;

namespace SoundEventLink.Runtime.Node.Output
{
	[Serializable, NodeMenuItem("Custom/Ducking")]
	public class DuckingNode : BaseNode
	{
		[Input("Audio"), ShowAsDrawer] public float _audio;
		[Input("Audio Parameter"), ShowAsDrawer] public string _audioParameter;
		[Input("Fade OutTime"), ShowAsDrawer] public float _inTime;
		[Input("Fade InTime"), ShowAsDrawer] public float _outTime;
		[Input("Priority"), ShowAsDrawer] public int _priority;
	}
}