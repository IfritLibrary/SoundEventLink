using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GraphProcessor;
using SoundEventLink.Runtime.Node;
using SoundEventLink.Runtime.Node.Output;
using UnityEditor;
using UnityEngine;

namespace SoundEventLink.Editor
{
	public class SoundEventLinkGraphWindow : BaseGraphWindow
	{
		protected override void InitializeWindow(BaseGraph graph)
		{
			// ウィンドウのタイトルを適当に設定
			var fileName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(graph));
			titleContent = new GUIContent(ObjectNames.NicifyVariableName(fileName));

			// グラフを編集するためのビューであるGraphViewを設定
			if (graphView == null) graphView = new SoundEventLinkGraphView(this);
			rootView.Add(graphView);
		}

		protected override void InitializeGraphView(BaseGraphView view)
		{
			view.OpenPinned<ExposedParameterView>();
		}
	}

	public class SoundEventLinkGraphView : BaseGraphView
	{
		public SoundEventLinkGraphView(EditorWindow window) : base(window)
		{
		}

		protected override bool canDeleteSelection => !selection.Any(e => e is ResultNodeView);

		public override IEnumerable<(string path, Type type)> FilterCreateNodeMenuEntries()
		{
			foreach (var nodeMenuItem in NodeProvider.GetNodeMenuEntries())
			{
				//if (nodeMenuItem.type == typeof(SEResultNode)) continue;

				yield return nodeMenuItem;
			}
		}
	}
}