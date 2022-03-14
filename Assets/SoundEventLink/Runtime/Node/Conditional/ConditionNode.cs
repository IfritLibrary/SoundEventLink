using System;
using GraphProcessor;
using UnityEngine;
using UnityEngine.Rendering;
namespace SoundEventLink.Runtime.Node.Conditional
{
	[Serializable, NodeMenuItem("Conditional/Condition")]
	public class ConditionNode : BaseNode
	{
		[Input("Compare Function"), ShowAsDrawer]
		public CompareFunction _compareOperator;

		[Output("Output")] public bool _output;
		[Input(name = "Value1"), ShowAsDrawer] public float _value1Value;
		[Input(name = "Value2"), ShowAsDrawer] public float _value2Value;

		public override string name => "Condition";

		protected override void Process()
		{
			switch (_compareOperator)
			{
			case CompareFunction.Disabled:
			case CompareFunction.Never:
				_output = false;
				break;
			case CompareFunction.Less:
				_output = _value1Value > _value2Value;
				break;
			case CompareFunction.Equal:
				_output = Mathf.Approximately(_value1Value, _value2Value);
				break;
			case CompareFunction.LessEqual:
				_output = _value1Value >= _value2Value;
				break;
			case CompareFunction.Greater:
				_output = _value1Value < _value2Value;
				break;
			case CompareFunction.NotEqual:
				_output = !Mathf.Approximately(_value1Value, _value2Value);
				break;
			case CompareFunction.GreaterEqual:
				_output = _value1Value <= _value2Value;
				break;
			case CompareFunction.Always:
				_output = true;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}
}