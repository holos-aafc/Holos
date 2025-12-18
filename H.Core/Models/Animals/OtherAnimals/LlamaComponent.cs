using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Models.Animals.OtherAnimals
{
    public class LlamaComponent : AnimalComponentBase
    {
        public LlamaComponent()
        {
            ComponentNameDisplayString = ComponentType.Llamas.GetDescription();
            ComponentDescriptionString = Resources.ToolTipLlamasComponent;
            ComponentCategory = ComponentCategory.OtherLivestock;
            ComponentType = ComponentType.Llamas;
        }
    }
}