using System.Linq;
using GraphProcessor;
using SoundEventLink.Runtime.Node;
using SoundEventLink.Runtime.Node.Output;
using UnityEngine;

namespace SoundEventLink.Runtime
{
	[CreateAssetMenu(menuName = "SoundEventLink/Graph", order = 0)]
	public class SoundEventLinkGraph : BaseGraph
	{
		protected override void OnEnable()
		{
			base.OnEnable();
		}
	}
}