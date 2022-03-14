using UnityEditor;
using UnityEngine;
namespace GraphProcessor
{
	[ExecuteAlways]
	public class DeleteCallback : UnityEditor.AssetModificationProcessor
	{
		private static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions options)
		{
			var graph = AssetDatabase.LoadAssetAtPath(path, typeof(BaseGraph));

			if (graph != null)
			{
				foreach (var graphWindow in Resources.FindObjectsOfTypeAll<BaseGraphWindow>())
					graphWindow.OnGraphDeleted();
			}

			return AssetDeleteResult.DidNotDelete;
		}
	}

}