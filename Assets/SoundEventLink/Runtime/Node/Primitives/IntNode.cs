using System;
using GraphProcessor;
namespace SoundEventLink.Runtime.Node.Primitives
{
	[Serializable, NodeMenuItem("Primitives/Int")]
	public class IntNode : BaseNode
	{
		public int input;
		[Output("Out")]
		public int output;

		public override string name => "Int";

		protected override void Process() => output = input;
	}
}