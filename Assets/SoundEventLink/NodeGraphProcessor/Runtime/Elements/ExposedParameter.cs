using System;
using System.Collections.Generic;
using UnityEngine;
namespace GraphProcessor
{
	[Serializable]
	public class ExposedParameter : ISerializationCallbackReceiver
	{

		private static Dictionary<Type, Type> exposedParameterTypeCache = new Dictionary<Type, Type>();

		public string guid; // unique id to keep track of the parameter
		public bool input = true;
		public string name;
		[Obsolete("Use value instead")]
		public SerializableObject serializedValue;
		[SerializeReference]
		public Settings settings;
		[Obsolete("Use GetValueType()")]
		public string type;
		public string shortType => GetValueType()?.Name;

		public virtual object value { get; set; }

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			// SerializeReference migration step:
		#pragma warning disable CS0618
			if (serializedValue?.value != null) // old serialization system can't serialize null values
			{
				value = serializedValue.value;
				Debug.Log("Migrated: " + serializedValue.value + " | " + serializedValue.serializedName);
				serializedValue.value = null;
			}
		#pragma warning restore CS0618
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize() {}

		public void Initialize(string name, object value)
		{
			guid          = Guid.NewGuid().ToString(); // Generated once and unique per parameter
			settings      = CreateSettings();
			settings.guid = guid;
			this.name     = name;
			this.value    = value;
		}

		protected virtual Settings CreateSettings() => new Settings();
		public virtual Type GetValueType() => value.GetType();
		internal ExposedParameter Migrate()
		{
			if (exposedParameterTypeCache.Count == 0)
			{
				foreach (var type in AppDomain.CurrentDomain.GetAllTypes())
				{
					if (type.IsSubclassOf(typeof(ExposedParameter)) && !type.IsAbstract)
					{
						var paramType = Activator.CreateInstance(type) as ExposedParameter;
						exposedParameterTypeCache[paramType.GetValueType()] = type;
					}
				}
			}
		#pragma warning disable CS0618 // Use of obsolete fields
			var oldType = Type.GetType(type);
		#pragma warning restore CS0618
			if (oldType == null || !exposedParameterTypeCache.TryGetValue(oldType, out var newParamType))
				return null;

			var newParam = Activator.CreateInstance(newParamType) as ExposedParameter;

			newParam.guid          = guid;
			newParam.name          = name;
			newParam.input         = input;
			newParam.settings      = newParam.CreateSettings();
			newParam.settings.guid = guid;

			return newParam;

		}

		public static bool operator ==(ExposedParameter param1, ExposedParameter param2)
		{
			if (ReferenceEquals(param1, null) && ReferenceEquals(param2, null))
				return true;
			if (ReferenceEquals(param1, param2))
				return true;
			if (ReferenceEquals(param1, null))
				return false;
			if (ReferenceEquals(param2, null))
				return false;

			return param1.Equals(param2);
		}

		public static bool operator !=(ExposedParameter param1, ExposedParameter param2) => !(param1 == param2);

		public bool Equals(ExposedParameter parameter) => guid == parameter.guid;

		public override bool Equals(object obj)
		{
			if (obj == null || !GetType().Equals(obj.GetType()))
				return false;
			return Equals((ExposedParameter)obj);
		}

		public override int GetHashCode() => guid.GetHashCode();

		public ExposedParameter Clone()
		{
			var clonedParam = Activator.CreateInstance(GetType()) as ExposedParameter;

			clonedParam.guid     = guid;
			clonedParam.name     = name;
			clonedParam.input    = input;
			clonedParam.settings = settings;
			clonedParam.value    = value;

			return clonedParam;
		}
		[Serializable]
		public class Settings
		{
			public bool expanded = false;

			[SerializeField]
			public string guid;
			public bool isHidden = false;

			public override bool Equals(object obj)
			{
				if (obj is Settings s && s != null)
					return Equals(s);
				return false;
			}

			public virtual bool Equals(Settings param)
				=> isHidden == param.isHidden && expanded == param.expanded;

			public override int GetHashCode() => base.GetHashCode();
		}
	}

	[Serializable]
	public class FloatParameter : ExposedParameter
	{
		public enum FloatMode
		{
			Default,
			Slider
		}

		[SerializeField]
		private float val;

		public override object value { get => val; set => val = (float)value; }
		protected override Settings CreateSettings() => new FloatSettings();

		[Serializable]
		public class FloatSettings : Settings
		{
			public float max = 1;
			public float min = 0;
			public FloatMode mode;

			public override bool Equals(Settings param)
				=> base.Equals(param) && mode == ((FloatSettings)param).mode && min == ((FloatSettings)param).min && max == ((FloatSettings)param).max;
		}
	}

	[Serializable]
	public class Vector3Parameter : ExposedParameter
	{
		[SerializeField]
		private Vector3 val;

		public override object value { get => val; set => val = (Vector3)value; }
	}

	[Serializable]
	public class IntParameter : ExposedParameter
	{
		public enum IntMode
		{
			Default,
			Slider
		}

		[SerializeField]
		private int val;

		public override object value { get => val; set => val = (int)value; }
		protected override Settings CreateSettings() => new IntSettings();

		[Serializable]
		public class IntSettings : Settings
		{
			public int max = 10;
			public int min = 0;
			public IntMode mode;

			public override bool Equals(Settings param)
				=> base.Equals(param) && mode == ((IntSettings)param).mode && min == ((IntSettings)param).min && max == ((IntSettings)param).max;
		}
	}

	[Serializable]
	public class StringParameter : ExposedParameter
	{
		[SerializeField]
		private string val;

		public override object value { get => val; set => val = (string)value; }
		public override Type GetValueType() => typeof(string);
	}
}