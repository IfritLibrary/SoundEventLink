// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY DATASHEET. DO NOT CHANGE IT.
// </auto-generated>

using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector.Editor;
#endif
namespace SoundEventLink
{
    [Serializable]
    public class SoundEventLinkData : ScriptableObject
    {
        [LabelText("タイプ")]
        [ShowInInspector]
        [field: SerializeField, HideInInspector]
        public global::SoundEventLink.SoundEventLinkType Type { get; set; }

        [LabelText("グラフ")]
        [ShowInInspector]
        [field: SerializeField, HideInInspector]
        public global::SoundEventLink.NodeGraphProcessor.Runtime.Graph.SoundEventLinkGraph Graph { get; set; }

        [LabelText("indexList")]
        [ShowInInspector]
        [field: SerializeField, HideInInspector]
        public global::SoundEventLink.ExposedType[] ExposedIndexList { get; set; }

        [LabelText("bool")]
        [ShowInInspector]
        [field: SerializeField, HideInInspector]
        public global::System.Boolean[] ExposedBoolList { get; set; }

        [LabelText("float")]
        [ShowInInspector]
        [field: SerializeField, HideInInspector]
        public global::System.Single[] ExposedFloatList { get; set; }

        [LabelText("object")]
        [ShowInInspector]
        [field: SerializeField, HideInInspector]
        public global::UnityEngine.Object[] ExposedObjectList { get; set; }

        [LabelText("string")]
        [ShowInInspector]
        [field: SerializeField, HideInInspector]
        public global::System.String[] ExposedStringList { get; set; }

        [LabelText("reference")]
        [ShowInInspector]
        [field: SerializeField, HideInInspector]
        public global::SoundEventLink.SoundEventLinkData Reference { get; set; }

    }
    public static class SoundEventLinkDataAccessor
    {
        public static global::Cysharp.Threading.Tasks.UniTask<global::DataSheet.AddressableResource<SoundEventLinkData>> GetAsync(EnumSoundEventLinkData type)
        {
            return GetAsync(type.ToString());
        }
        public static global::Cysharp.Threading.Tasks.UniTask<global::DataSheet.AddressableResource<SoundEventLinkData>> GetAsync(string name)
        {
            return global::DataSheet.AddressableResource<SoundEventLinkData>.LoadAsync($"SoundEventLinkData/{name}");
        }
    }
}
#if UNITY_EDITOR
namespace SoundEventLink.GeneratedEditor
{
    public class CreateSoundEventLinkDataWindow : OdinEditorWindow
    {
        private OdinMenuEditorWindow _odinWindow;
        public static void Open(OdinMenuEditorWindow odinWindow)
        {
            var window = GetWindow<CreateSoundEventLinkDataWindow>(nameof(CreateSoundEventLinkDataWindow));
            window._odinWindow = odinWindow;
            window.name = "";
            window.Show();
        }
        [ShowInInspector, ValidateInput(nameof(ValidateName), "Nameを入力してください")]
        public string Name { get; set; }
        private bool ValidateName(string n) => !string.IsNullOrEmpty(n);
        public bool ValidData => !string.IsNullOrEmpty(Name);

        [Button, EnableIf(nameof(ValidData))]
        private void Create()
        {
            var value = ScriptableObject.CreateInstance<SoundEventLink.SoundEventLinkData>();
            ((ScriptableObject)value).name = Name;
            Directory.CreateDirectory(Path.Combine("Assets", @"DataSheetGenerated/Data\SoundEventLink/SoundEventLinkData"));
            AssetDatabase.CreateAsset(value, Path.Combine("Assets", @"DataSheetGenerated/Data\SoundEventLink/SoundEventLinkData", $"{Name}.asset"));
            AssetDatabase.Refresh();
            _odinWindow.ForceMenuTreeRebuild();
            Close();
        }
    }
    public class SoundEventLinkDataWindow : OdinMenuEditorWindow
    {
        [MenuItem("Tools/SoundEventLink")]
        public static void ShowWindow() => GetWindow<SoundEventLinkDataWindow>(nameof(SoundEventLinkDataWindow)).Show();
        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(true)
            { DefaultMenuStyle = { IconSize = 28.0f }, Config = { DrawSearchToolbar = true } };

            var dataNames = new List<string>();
            foreach (var data in AssetDatabase.FindAssets("t:SoundEventLink.SoundEventLinkData").Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath<global::SoundEventLink.SoundEventLinkData>))
            {
                DataSheet.Editor.AddressableUtil.AddAssetToGroup(data, $"SoundEventLinkData/{data.name}", "SoundEventLinkData");
                tree.Add(data.name, data);
                dataNames.Add(data.name);
            }
            // Create Enum For DBAccessor
            var scriptPath = Path.Combine(Application.dataPath, @"DataSheetGenerated/Scripts\SoundEventLink\EnumSoundEventLinkData.cs");
            var sb = new StringBuilder();
            sb.AppendLine(@"// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY DATASHEET. DO NOT CHANGE IT.
// </auto-generated>
");
            sb.AppendLine("namespace SoundEventLink{");
            sb.AppendLine("public enum EnumSoundEventLinkData{");
            foreach(var name in dataNames) sb.AppendLine($"    {name} = {name.GetHashCode()},");
            sb.AppendLine("}");
            sb.AppendLine("}");
            var sbTxt = sb.ToString();
            if(File.Exists(scriptPath))
            {
                if(File.ReadAllText(scriptPath) != sbTxt)
                {
                    File.WriteAllText(scriptPath, sbTxt);
                    AssetDatabase.Refresh();
                }
            }
            else
            {
                File.WriteAllText(scriptPath, sbTxt);
                AssetDatabase.Refresh();
            }
            tree.SortMenuItemsByName();
            return tree;
        }
        protected override void OnBeginDrawEditors()
        {
            SirenixEditorGUI.BeginHorizontalToolbar();
            if (SirenixEditorGUI.ToolbarButton("Create Item")) CreateSoundEventLinkDataWindow.Open(this);
            SirenixEditorGUI.EndHorizontalToolbar();
        }
    }
    public class SoundEventLinkDataTypeGetter : global::DataSheet.Editor.IVariableTypeGetter
    {
        public global::DataSheet.Editor.VariableType GetVariableType() => new global::DataSheet.Editor.VariableType(typeof(global::SoundEventLink.SoundEventLinkData));
    }
}
#endif
