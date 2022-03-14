using System;
using System.Collections.Generic;
using UnityEngine;
namespace GraphProcessor
{
	[Serializable]
	public class ExposedParameterWorkaround : ScriptableObject
	{
		public BaseGraph graph;
		[SerializeReference]
		public List<ExposedParameter> parameters = new List<ExposedParameter>();
	}
}