using UnityEditor;
using UnityEngine;
namespace SoundEventLink.Editor.Window
{
	internal class DuckingWindow : IWindow
	{
		private DuckingVisualizeTreeView _treeView;
		private object _splitterState;
		private int _interval;

		public void Update(VisualizeWindow window)
		{
			if (_interval++ % 120 == 0)
			{
				_treeView.ReloadAndSort();
				window.Repaint();
			}
		}
		public void OnEnable(VisualizeWindow window)
		{
			_splitterState = SplitterGUILayout.CreateSplitterState(new[]
			{
				75f, 25f
			}, new[]
			{
				32, 32
			}, null);
			_treeView = new DuckingVisualizeTreeView();
		}
		public void OnDisable(VisualizeWindow window)
		{
		}

		private Vector2 _tableScroll;
		public void OnGUI(VisualizeWindow window)
		{
			SplitterGUILayout.BeginVerticalSplit(_splitterState);

			using (var _ = new EditorGUILayout.VerticalScope())
			{
				using var scope = new EditorGUILayout.ScrollViewScope(_tableScroll);
				_tableScroll = scope.scrollPosition;

				var controlRect = EditorGUILayout.GetControlRect(GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
				_treeView.OnGUI(controlRect);
			}

			using (var _ = new EditorGUILayout.VerticalScope())
			{
				foreach (var iD in _treeView.state.selectedIDs)
				{
					GUILayout.Label(iD.ToString());
				}
			}
			SplitterGUILayout.EndVerticalSplit();
		}
	}
}