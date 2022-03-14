using DataSheet.Editor;
using SoundEventLink.NodeGraphProcessor.Runtime.Graph;

namespace SoundEventLink.NodeGraphProcessor.Editor
{
    public class SoundEventLinkGraphTypeGetter : IVariableTypeGetter
    {
        public VariableType GetVariableType() => new VariableType(typeof(SoundEventLinkGraph),"SoundEventLink.NodeGraphProcessor");
    }
}