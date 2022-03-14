using System;
using UnityEngine;
namespace GraphProcessor
{
	/// <summary>
	///     Element that overlays the graph like the blackboard
	/// </summary>
	[Serializable]
	public class PinnedElement
	{
		public static readonly Vector2 defaultSize = new Vector2(150, 200);
		public SerializableType editorType;
		public bool opened = true;

		public Rect position = new Rect(Vector2.zero, defaultSize);

		public PinnedElement(Type editorType) => this.editorType = new SerializableType(editorType);
	}
}