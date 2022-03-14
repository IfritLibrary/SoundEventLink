// using System.Collections.Generic;
// using System.Linq;
// using ScriptableObjectSheet.Editor;
// using UnityEngine;
// #if SOSHEETMM_GENERATED
// using SOSheetMasterMemory.Generated;
// using UnityEditor;
// #endif
// namespace SoundEventLink.Editor
// {
// 	public class SoundEventLinkBuilder : IScriptableObjectBuilder<SoundEventLinkSheet>
// 	{
// 		public void BuildImpl(IEnumerable<object> objectList)
// 		{
// 			
// #if SOSHEETMM_GENERATED
// 			var mixer = AssetDatabase.FindAssets("t:AudioMixer");
// 			if (mixer.Length != 1)
// 			{
// 				Debug.LogError("AudioMixerの数が合いません " + mixer.Length);
// 				return;
// 			}
// 			AddressableUtil.AddAssetToGroup(mixer[0], "SEL/AudioMixer", "SEL");
// 			
// 			var list = objectList
// 				.Select(obj => (SoundEventLinkSheet)obj)
// 				.Select(data => data.Data)
// 				.ToList();
// 			
// 			using var builder = MasterDataAccessor.GetBuilder();
// 			builder.ReplaceAll(list);
// #else
// 			Debug.LogError("ScriptableObjectSheetMasterMemory Is Not Generated");
// #endif
// 		}
// 		public GenerateArguments Arguments => null;
// 	}
// }