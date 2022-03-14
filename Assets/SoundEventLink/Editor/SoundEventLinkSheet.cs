using System.Collections.Generic;
using System.Linq;
using ScriptableObjectSheet.Editor;
using SoundEventLink.Runtime;
using SoundEventLink.Runtime.MasterData;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace SoundEventLink.Editor
{
	public class SoundEventLinkSheet : ScriptableObjectSheetBase
	{
		public enum ExposedType
		{
			Bool,
			Float,
			String,
			Object
		}

		public List<bool> _exposedBoolList = new List<bool>();
		public List<float> _exposedFloatList = new List<float>();
		public List<ExposedType> _exposedIndexList = new List<ExposedType>();
		public List<Object> _exposedObjectList = new List<Object>();
		public List<string> _exposedStringList = new List<string>();
		public SoundEventLinkGraph _graph;
		public SoundEventLinkSheet _reference;
		
		public SoundEventLinkType _type = SoundEventLinkType.None;

		public SoundEventLinkData Data
		{
			get
			{
				var result = new SoundEventLinkData
				{
					Id = name
				};
				var data = this;
				if (_type == SoundEventLinkType.None)
					return result;
				if (_type == SoundEventLinkType.Reference)
				{
					if (_reference == null)
						return result;
					data = _reference;
				}
				
				
				result.GraphName = AddressableUtil.AddAssetToGroup(data._graph, "SEL");
				if (!data._exposedIndexList.Any())
					return result;
				result.ExposedParameterList = new object[data._exposedIndexList.Count];

				var boolCnt   = 0;
				var floatCnt  = 0;
				var stringCnt = 0;
				var objectCnt = 0;
				for (var i = 0; i < data._exposedIndexList.Count; i++)
				{
					result.ExposedParameterList[i] = data._exposedIndexList[i] switch
					{
						ExposedType.Bool   => data._exposedBoolList[boolCnt++],
						ExposedType.Float  => data._exposedFloatList[floatCnt++],
						ExposedType.String => data._exposedStringList[stringCnt++],
						ExposedType.Object => data._exposedObjectList[objectCnt++] switch
						{
							AudioClip clip        => AddressableUtil.AddAssetToGroup(clip, "SEL"),
							AudioMixerGroup group => group.name,
							_                     => null
						},
						_ => result.ExposedParameterList[i]
					};
				}

				return result;
			}
		}

		private void RefreshExposedParameter()
		{
			_exposedBoolList.Clear();
			_exposedFloatList.Clear();
			_exposedStringList.Clear();
			_exposedObjectList.Clear();
			_exposedIndexList.Clear();

			if (_graph == null)
				return;
			var exParams = _graph.exposedParameters.Where(parameter => !parameter.name.StartsWith("Custom"));
			foreach (var type in exParams.Select(t => t.GetValueType()))
			{
				if (type == typeof(bool))
				{
					_exposedIndexList.Add(ExposedType.Bool);
					_exposedBoolList.Add(default);
				}
				else if (type == typeof(float))
				{
					_exposedIndexList.Add(ExposedType.Float);
					_exposedFloatList.Add(default);
				}
				else if (type == typeof(string))
				{
					_exposedIndexList.Add(ExposedType.String);
					_exposedStringList.Add(default);
				}
				else
				{
					_exposedIndexList.Add(ExposedType.Object);
					_exposedObjectList.Add(default);
				}
			}
		}

		public override void OnSheetGUI()
		{
			_type = (SoundEventLinkType)EditorGUILayout.EnumPopup(_type, GUILayout.Width(100));

			if (_type == SoundEventLinkType.None)
				return;

			if (_type == SoundEventLinkType.Reference)
			{
				_reference = EditorGUILayout.ObjectField(_reference, typeof(SoundEventLinkSheet), false) as SoundEventLinkSheet;
				if (_reference == this)
					_reference = null;
			}
			else if (_type == SoundEventLinkType.Original)
			{
				EditorGUI.BeginChangeCheck();

				_graph = EditorGUILayout.ObjectField(_graph, typeof(SoundEventLinkGraph), false, GUILayout.Width(150)) as SoundEventLinkGraph;

				if (EditorGUI.EndChangeCheck())
					RefreshExposedParameter();

				if (_graph == null)
					return;

				var exParams  = _graph.exposedParameters.Where(parameter => !parameter.name.StartsWith("Custom"));
				var boolCnt   = -1;
				var floatCnt  = -1;
				var stringCnt = -1;
				var objectCnt = -1;
				foreach (var t in exParams)
				{
					GUILayout.Label(t.name, GUILayout.Width(80));
					var type = t.GetValueType();

					if (type == typeof(bool))
					{
						boolCnt++;
						if (_exposedBoolList.Count <= boolCnt)
						{
							RefreshExposedParameter();
							return;
						}
						_exposedBoolList[boolCnt] = EditorGUILayout.Toggle(_exposedBoolList[boolCnt], GUILayout.Width(100));
					}
					else if (type == typeof(float))
					{
						floatCnt++;
						if (_exposedFloatList.Count <= floatCnt)
						{
							RefreshExposedParameter();
							return;
						}
						_exposedFloatList[floatCnt] = EditorGUILayout.FloatField(_exposedFloatList[floatCnt], GUILayout.Width(100));
					}
					else if (type == typeof(string))
					{
						stringCnt++;
						if (_exposedStringList.Count <= stringCnt)
						{
							RefreshExposedParameter();
							return;
						}
						_exposedStringList[stringCnt] = EditorGUILayout.TextField(_exposedStringList[stringCnt], GUILayout.Width(100));
					}
					else
					{
						objectCnt++;
						if (_exposedObjectList.Count <= objectCnt)
						{
							RefreshExposedParameter();
							return;
						}
						_exposedObjectList[objectCnt] = EditorGUILayout.ObjectField(_exposedObjectList[objectCnt],
							t.GetValueType(), false, GUILayout.Width(100));
					}
				}
			}
		}
	}
}