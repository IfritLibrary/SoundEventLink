using System;
using UnityEngine;
namespace GraphProcessor
{
	[Serializable]
	public class SerializableEdge : ISerializationCallbackReceiver
	{
		public string GUID;

		public string inputFieldName;

		[NonSerialized]
		public BaseNode inputNode;

		[SerializeField]
		private string inputNodeGUID;

		[NonSerialized]
		public NodePort inputPort;

		// Use to store the id of the field that generate multiple ports
		public string inputPortIdentifier;
		public string outputFieldName;

		[NonSerialized]
		public BaseNode outputNode;
		[SerializeField]
		private string outputNodeGUID;
		[NonSerialized]
		public NodePort outputPort;
		public string outputPortIdentifier;

		[SerializeField]
		private BaseGraph owner;

		//temporary object used to send port to port data when a custom input/output function is used.
		[NonSerialized]
		public object passThroughBuffer;

		public void OnBeforeSerialize()
		{
			if (outputNode == null || inputNode == null)
				return;

			outputNodeGUID = outputNode.GUID;
			inputNodeGUID  = inputNode.GUID;
		}

		public void OnAfterDeserialize() {}

		public static SerializableEdge CreateNewEdge(BaseGraph graph, NodePort inputPort, NodePort outputPort)
		{
			var edge = new SerializableEdge();

			edge.owner                = graph;
			edge.GUID                 = Guid.NewGuid().ToString();
			edge.inputNode            = inputPort.owner;
			edge.inputFieldName       = inputPort.fieldName;
			edge.outputNode           = outputPort.owner;
			edge.outputFieldName      = outputPort.fieldName;
			edge.inputPort            = inputPort;
			edge.outputPort           = outputPort;
			edge.inputPortIdentifier  = inputPort.portData.identifier;
			edge.outputPortIdentifier = outputPort.portData.identifier;

			return edge;
		}

		//here our owner have been deserialized
		public void Deserialize()
		{
			if (!owner.nodesPerGUID.ContainsKey(outputNodeGUID) || !owner.nodesPerGUID.ContainsKey(inputNodeGUID))
				return;

			outputNode = owner.nodesPerGUID[outputNodeGUID];
			inputNode  = owner.nodesPerGUID[inputNodeGUID];
			inputPort  = inputNode.GetPort(inputFieldName, inputPortIdentifier);
			outputPort = outputNode.GetPort(outputFieldName, outputPortIdentifier);
		}

		public override string ToString() => $"{outputNode.name}:{outputPort.fieldName} -> {inputNode.name}:{inputPort.fieldName}";
	}
}