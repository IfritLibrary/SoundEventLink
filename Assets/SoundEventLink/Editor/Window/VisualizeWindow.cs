using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace SoundEventLink.Editor.Window
{
	public class VisualizeWindow : EditorWindow
	{
		[MenuItem("Window/SoundEventLink/DuckingVisualize")]
		private static void ShowWindow()
		{
			var window = GetWindow<VisualizeWindow>();
			window.titleContent = new GUIContent("DuckingVisualize");
			window.Show();
		}

		private enum Tab
		{
			Main,
			Ducking
		}
		private Tab _tab = Tab.Main;
		
		private readonly List<IWindow> _window = new List<IWindow>();
		public VisualizeWindow()
		{
			_window.Add(new MainWindow());
			_window.Add(new DuckingWindow());
		}

		private void OnEnable()
		{
			_window[(int)_tab].OnEnable(this);
		}

		private void OnDisable()
		{
			_window[(int)_tab].OnDisable(this);
		}

		private void Update()
		{
			_window[(int)_tab].Update(this);
		}

		[SerializeField] private string _playKey = "";
		[SerializeField] private Vector3 _position = Vector3.zero;
		[SerializeField] private List<string> _customProperties = new List<string>();

		private void OnGUI()
		{
			// 再生出来るようにする
			using (new EditorGUILayout.VerticalScope())
			{
				var so = new SerializedObject(this);
				so.Update();
				
				EditorGUILayout.PropertyField(so.FindProperty(nameof(_playKey)));
				EditorGUILayout.PropertyField(so.FindProperty(nameof(_position)));
				EditorGUILayout.PropertyField(so.FindProperty(nameof(_customProperties)));
				so.ApplyModifiedProperties();
				
				if (Runtime.SoundEventLink.Instance != null)
				{
					if (GUILayout.Button("Play"))
						Runtime.SoundEventLink.Instance.Play(_playKey, _position);
					if (GUILayout.Button("Clear Cache"))
						Runtime.SoundEventLink.Instance.ClearCache();
				}
			}

			using (new EditorGUILayout.HorizontalScope()) {
				GUILayout.FlexibleSpace();
				
				EditorGUI.BeginChangeCheck();
				var beforeTab = _tab;
				_tab = (Tab)GUILayout.Toolbar((int)_tab, Styles.TabToggles, Styles.TabButtonStyle, Styles.TabButtonSize);

				if (EditorGUI.EndChangeCheck())
				{
					_window[(int)beforeTab].OnDisable(this);
					_window[(int)_tab].OnEnable(this);
				}
				GUILayout.FlexibleSpace();
			}
			
			_window[(int)_tab].OnGUI(this);
		}
		
		// Style定義
		private static class Styles
		{
			private static GUIContent[] _tabToggles;
			public static GUIContent[] TabToggles =>  _tabToggles ??= Enum.GetNames(typeof(Tab)).Select(x => new GUIContent(x)).ToArray();
        
			public static readonly GUIStyle TabButtonStyle = "LargeButton";

			// GUI.ToolbarButtonSize.FitToContentsも設定できる
			public const GUI.ToolbarButtonSize TabButtonSize = GUI.ToolbarButtonSize.Fixed;
		}
	}
}