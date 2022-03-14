using System;
using GraphProcessor;
namespace SoundEventLink.Runtime.Node.Primitives
{
	[Serializable, NodeMenuItem("Primitives/Float")]
	public class FloatNode : BaseNode
	{
		public float input;
		[Output("Out")]
		public float output;

		public override string name => "Float";

		protected override void Process() => output = input;
	}
	
	[Serializable, NodeMenuItem("Primitives/String")]
	public class StringNode : BaseNode
	{
		public string input;
		[Output("Out")]
		public string output;

		public override string name => "String";

		protected override void Process() => output = input;
	}
	
	
}