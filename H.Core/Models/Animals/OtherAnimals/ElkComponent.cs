using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Models.Animals.OtherAnimals
{
    public class ElkComponent : AnimalComponentBase
    {
        #region Constructors

        public ElkComponent()
        {
            ComponentNameDisplayString = ComponentType.Elk.GetDescription();
            ComponentDescriptionString = Resources.ToolTipElkComponent;
            ComponentCategory = ComponentCategory.OtherLivestock;
            ComponentType = ComponentType.Elk;
        }

        #endregion
    }
}