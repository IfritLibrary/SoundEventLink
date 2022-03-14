using System;
using System.Linq;
using GraphProcessor;

namespace SoundEventLink.Runtime.Node.Primitives
{
	[Serializable, NodeMenuItem("Primitives/PlayCount")]
	public class PlayCountNode : BaseNode
	{
		[Input("Key"), ShowAsDrawer] public string key;

		[Output("Out")]
		public int output;

		public override string name => "PlayCount";

		protected override void Process()
		{
			output = SoundEventLink.Instance.PlayAudioDataList.Count(data => data.Key == key);
		}
	}
}