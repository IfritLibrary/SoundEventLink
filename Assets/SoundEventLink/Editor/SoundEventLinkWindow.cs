// using System;
// using Cysharp.Threading.Tasks;
// using ScriptableObjectSheet.Editor;
// using UnityEditor;
// using UnityEngine;
//
// namespace SoundEventLink.Editor
// {
// 	public class SoundEventLinkWindow : ScriptableObjectSheetForceDefaultWindow<SoundEventLinkSheet>
// 	{
// 		private string _testKey = "";
// 		public override string OutputPath => "Assets/Work/SoundEventLink";
//
// 		protected new void OnGUI()
// 		{
// 			using (var _ = new EditorGUILayout.HorizontalScope())
// 			{
// 				if (Runtime.SoundEventLink.Instance != null && Camera.main != null)
// 				{
// 					_testKey = EditorGUILayout.TextField("テスト再生キー", _testKey);
//
// 					if (GUILayout.Button("再生", GUILayout.Width(40)))
// 						Runtime.SoundEventLink.Instance.Play(_testKey, Camera.main.transform.position).Forget();
// 				}
// 			}
//
// 			base.OnGUI();
// 		}
//
// 		[MenuItem("Tools/ScriptableObjectSheet/SoundEventLink")]
// 		private static void ShowWindow()
// 		{
// 			if (Environment.GetEnvironmentVariable("PROJECT_TEMP") == "")
// 			{
// 				EditorUtility.DisplayDialog("Error", "環境変数が設定されてません\n適切な方法で起動してください", "OK");
// 				return;
// 			}
//
// 			var window = GetWindow<SoundEventLinkWindow>();
// 			window.titleContent = new GUIContent("SoundEventLink");
// 			window.Show();
// 		}
// 	}
// }