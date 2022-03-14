namespace SoundEventLink.Editor.Window
{
	internal interface IWindow
	{
		void Update(VisualizeWindow    window);
		void OnEnable(VisualizeWindow  window);
		void OnDisable(VisualizeWindow window);
		void OnGUI(VisualizeWindow     window);
	}

}