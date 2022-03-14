using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataSheet.Editor;
using Sirenix.OdinInspector.Editor;
using SoundEventLink.NodeGraphProcessor.Runtime.Graph;
using SoundEventLink.Runtime;
using UnityEditor;
using UnityEngine;

namespace SoundEventLink.Editor
{
    [InitializeOnLoad]
    public static class DataSheetInitializer
    {
        static DataSheetInitializer()
        {
            var path = Path.GetFullPath("Packages/jp.ifrit.soundeventlink/DataSheet");
            if (Directory.Exists(path))
            {
                DataSheetManager.LoadDataSettingsDirectory(path);   
            }

            path = Path.Combine(Application.dataPath, "SoundEventLink/DataSheet");
            if (Directory.Exists(path))
            {
                DataSheetManager.LoadDataSettingsDirectory(path);
            }
        }
    }

    [CustomEditor(typeof(SoundEventLinkData))]
    public class SoundEventLinkDataCustomEditor : OdinEditor
    {
        private void RefreshExposedParameter()
        {
            var data = target as SoundEventLinkData;
            if (data == null)
                return;

            var boolCount = 0;
            var floatCount = 0;
            var stringCount = 0;
            var objectCount = 0;
            var indexList = new List<ExposedType>();
            
            if (data.Graph == null)
                return;
            var exParams = data.Graph.exposedParameters.Where(parameter => !parameter.name.StartsWith("Custom"));
            foreach (var type in exParams.Select(t => t.GetValueType()))
            {
                if (type == typeof(bool))
                {
                    indexList.Add(ExposedType.Bool);
                    boolCount++;
                }
                else if (type == typeof(float))
                {
                    indexList.Add(ExposedType.Float);
                    floatCount++;
                }
                else if (type == typeof(string))
                {
                    indexList.Add(ExposedType.String);
                    stringCount++;
                }
                else
                {
                    indexList.Add(ExposedType.Object);
                    objectCount++;
                }
            }

            data.ExposedIndexList = indexList.ToArray();
            data.ExposedBoolList = new bool[boolCount];
            data.ExposedFloatList = new float[floatCount];
            data.ExposedStringList = new string[stringCount];
            data.ExposedObjectList = new Object[objectCount];
        }
        
        public override void OnInspectorGUI()
        {
            var data = target as SoundEventLinkData;
            if (data == null)
                return;
            
            data.Type = (SoundEventLinkType)EditorGUILayout.EnumPopup(data.Type);

            if (data.Type == SoundEventLinkType.なし)
                return;

            if (data.Type == SoundEventLinkType.参照)
            {
                data.Reference = EditorGUILayout.ObjectField(data.Reference, typeof(SoundEventLinkData), false) as SoundEventLinkData;
                if (data.Reference == data)
                    data.Reference = null;
            }
            else if (data.Type == SoundEventLinkType.オリジナル)
            {
                EditorGUI.BeginChangeCheck();

                data.Graph = EditorGUILayout.ObjectField(data.Graph, typeof(SoundEventLinkGraph), false) as SoundEventLinkGraph;

                if (EditorGUI.EndChangeCheck())
                    RefreshExposedParameter();

                if (data.Graph == null)
                    return;

                var exParams = data.Graph.exposedParameters.Where(parameter => !parameter.name.StartsWith("Custom"));
                var boolCnt = -1;
                var floatCnt = -1;
                var stringCnt = -1;
                var objectCnt = -1;
                foreach (var t in exParams)
                {
                    GUILayout.Label(t.name, GUILayout.Width(80));
                    var type = t.GetValueType();

                    if (type == typeof(bool))
                    {
                        boolCnt++;
                        if (data.ExposedBoolList.Length <= boolCnt)
                        {
                            RefreshExposedParameter();
                            return;
                        }

                        data.ExposedBoolList[boolCnt] = EditorGUILayout.Toggle(data.ExposedBoolList[boolCnt]);
                    }
                    else if (type == typeof(float))
                    {
                        floatCnt++;
                        if (data.ExposedFloatList.Length <= floatCnt)
                        {
                            RefreshExposedParameter();
                            return;
                        }

                        data.ExposedFloatList[floatCnt] = EditorGUILayout.FloatField(data.ExposedFloatList[floatCnt]);
                    }
                    else if (type == typeof(string))
                    {
                        stringCnt++;
                        if (data.ExposedStringList.Length <= stringCnt)
                        {
                            RefreshExposedParameter();
                            return;
                        }

                        data.ExposedStringList[stringCnt] = EditorGUILayout.TextField(data.ExposedStringList[stringCnt]);
                    }
                    else
                    {
                        objectCnt++;
                        if (data.ExposedObjectList.Length <= objectCnt)
                        {
                            RefreshExposedParameter();
                            return;
                        }

                        data.ExposedObjectList[objectCnt] = EditorGUILayout.ObjectField(data.ExposedObjectList[objectCnt],
                            t.GetValueType(), false);
                    }
                }
            }
        }
    }
}