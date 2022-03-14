using UnityEditor;
using UnityEngine;
namespace SoundEventLink.Editor.Window
{
	internal class MainWindow : IWindow
	{
		public void Update(VisualizeWindow window)
		{
			
		}
		public void OnEnable(VisualizeWindow window)
		{
		}
		public void OnDisable(VisualizeWindow window)
		{
		}
		public void OnGUI(VisualizeWindow window)
		{
			if (Runtime.SoundEventLink.Instance != null)
			{
				foreach (var audioData in Runtime.SoundEventLink.Instance.PlayAudioDataList)
				{
					using var _ = new EditorGUILayout.HorizontalScope();
					GUILayout.Label(audioData.Key, GUILayout.Width(120));
					GUILayout.Label(audioData.AudioSource.transform.position.ToString(), GUILayout.Width(80));
					GUILayout.Label(audioData.AudioSource.clip.name, GUILayout.Width(80));
				}
			}
		}
	}
}