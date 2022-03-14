using System;
using GraphProcessor;
namespace SoundEventLink.Runtime.Node.Conditional
{
	[Serializable, NodeMenuItem("Conditional/Less")]
	public class LessNode : BaseNode
	{
		[Input("小さい"), ShowAsDrawer] public int _minValue;
		[Input("大きい"), ShowAsDrawer] public int _maxValue;

		[Output("Output")] public bool _output;

		public override string name => "LessNode";

		protected override void Process()
		{
			_output = _minValue < _maxValue;
		}
	}
}