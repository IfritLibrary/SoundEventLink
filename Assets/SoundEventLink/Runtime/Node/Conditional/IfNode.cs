using System;
using GraphProcessor;

namespace SoundEventLink.Runtime.Node
{
	[Serializable, NodeMenuItem("Conditional/If")]
	public class IfNode : BaseNode
	{
		[Input(name = "Condition"), ShowAsDrawer]
		public bool _condition;
		[Input(name = "False"), ShowAsDrawer] public object _falseValue = null;
		[Input(name = "True"), ShowAsDrawer] public object _trueValue = null;

		[Output("Output")] public object _output;

		public override string name => "If";

		protected override void Process()
		{
			_output = _condition ? _trueValue : _falseValue;
		}
	}
}