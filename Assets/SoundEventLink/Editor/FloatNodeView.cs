using GraphProcessor;
using SoundEventLink.Runtime.Node;
using SoundEventLink.Runtime.Node.Primitives;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SoundEventLink.Editor
{
	[NodeCustomEditor(typeof(FloatNode))]
	public class FloatNodeView : BaseNodeView
	{
		public override void Enable()
		{
			var floatNode = nodeTarget as FloatNode;

			var floatField = new DoubleField
			{
				value = floatNode.input
			};

			floatNode.onProcessed += () => floatField.value = floatNode.input;

			floatField.RegisterValueChangedCallback(v => {
				owner.RegisterCompleteObjectUndo("Updated floatNode input");
				floatNode.input = (float)v.newValue;
			});

			controlsContainer.Add(floatField);
		}
	}
}