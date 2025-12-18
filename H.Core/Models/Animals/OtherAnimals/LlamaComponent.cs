using H.Infrastructure;

namespace H.Core.Models.Animals.OtherAnimals
{
    public class LlamaComponent : AnimalComponentBase
    {
        public LlamaComponent()
        {
            this.ComponentNameDisplayString = ComponentType.Llamas.GetDescription();
            this.ComponentDescriptionString = Properties.Resources.ToolTipLlamasComponent;
            this.ComponentCategory = ComponentCategory.OtherLivestock;
            this.ComponentType = ComponentType.Llamas;
        }
    }
}