using System.Collections.Generic;
using System.Linq;
using Unity.Jobs;
// using Unity.Entities;

namespace GraphProcessor
{

	/// <summary>
	///     Graph processor
	/// </summary>
	public class ProcessGraphProcessor : BaseGraphProcessor
	{
		private List<BaseNode> processList;

		/// <summary>
		///     Manage graph scheduling and processing
		/// </summary>
		/// <param name="graph">Graph to be processed</param>
		public ProcessGraphProcessor(BaseGraph graph) : base(graph) {}

		public override void UpdateComputeOrder()
		{
			processList = graph.nodes.OrderBy(n => n.computeOrder).ToList();
		}

		/// <summary>
		///     Schedule the graph into the job system
		/// </summary>
		public override void Run()
		{
			var count = processList.Count;

			for (var i = 0; i < count; i++)
				processList[i].OnProcess();

			JobHandle.ScheduleBatchedJobs();
		}
	}
}