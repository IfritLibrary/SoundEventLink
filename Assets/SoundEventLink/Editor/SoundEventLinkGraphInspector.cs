using GraphProcessor;
using SoundEventLink.NodeGraphProcessor.Runtime.Graph;
using SoundEventLink.Runtime;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace SoundEventLink.Editor
{
	[CustomEditor(typeof(SoundEventLinkGraph), true)]
	public class SoundEventLinkGraphInspector : GraphInspector
	{
		// ダブルクリックでウィンドウが開かれるように
		[OnOpenAsset(0)]
		public static bool OnBaseGraphOpened(int instanceID, int line)
		{
			var asset = EditorUtility.InstanceIDToObject(instanceID) as SoundEventLinkGraph;

			if (asset == null) return false;

			var window = EditorWindow.GetWindow<SoundEventLinkGraphWindow>();
			window.InitializeGraph(asset);
			return true;
		}

		protected override void CreateInspector()
		{
			base.CreateInspector();

			root.Add(new Button(() => {
				var graph     = target as SoundEventLinkGraph;
				var processor = new SoundEventLinkProcessor(graph);
				processor.Run();
				foreach (var resultNode in processor.SEResultList)
				{
					Debug.Log(resultNode);
				}
			})
			{
				text = "Test Process"
			});
		}
	}
}