
namespace SoundEventLink.Runtime.MasterData
{
	public class SoundEventLinkData
	{
		public string Id { get; set; }

		public SoundEventLinkType Type { get; set; }

		public string GraphName { get; set; }

		public object[] ExposedParameterList { get; set; }
	}
}