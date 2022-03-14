using System;
using GraphProcessor;
namespace SoundEventLink.Runtime.Node.Conditional
{
	[Serializable, NodeMenuItem("Conditional/EqualInt")]
	public class EqualNodeInt : BaseNode
	{
		[Input("値1"), ShowAsDrawer] public int _value1;
		[Input("値2"), ShowAsDrawer] public int _value2;

		[Output("Output")] public bool _output;
		
		public override string name => "等しい";

		protected override void Process()
		{
			_output = _value1 == _value2;
		}
	}
	
	[Serializable, NodeMenuItem("Conditional/EqualString")]
	public class EqualStringNode : BaseNode
	{
		[Input("値1"), ShowAsDrawer] public string _value1;
		[Input("値2"), ShowAsDrawer] public string _value2;

		[Output("Output")] public bool _output;
		
		public override string name => "等しい";

		protected override void Process()
		{
			_output = _value1 == _value2;
		}
	}
}