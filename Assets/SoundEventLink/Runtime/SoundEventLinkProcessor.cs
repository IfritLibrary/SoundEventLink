using System.Collections.Generic;
using System.Linq;
using GraphProcessor;
using SoundEventLink.Runtime.Node;
using SoundEventLink.Runtime.Node.Output;

namespace SoundEventLink.Runtime
{
	public class SoundEventLinkProcessor : BaseGraphProcessor
	{
		private List<BaseNode> _processList;

		public SoundEventLinkProcessor(BaseGraph graph) : base(graph)
		{
		}
		public BGMResultNode[] BGMResultNodeList { get; private set; }
		public SEResultNode[] SEResultList { get; private set; }
		public DuckingNode[] DuckingNodeList { get; private set; }

		public override void UpdateComputeOrder()
		{
			_processList = graph.nodes.OrderBy(n => n.computeOrder).ToList();
		}

		public override void Run()
		{
			foreach (var process in _processList)
				process.OnProcess();

			BGMResultNodeList = _processList.OfType<BGMResultNode>().ToArray();
			SEResultList = _processList.OfType<SEResultNode>().ToArray();

			DuckingNodeList = _processList.OfType<DuckingNode>().ToArray();
		}
	}
}