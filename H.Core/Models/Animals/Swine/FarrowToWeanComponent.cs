using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Models.Animals.Swine
{
    public class FarrowToWeanComponent : AnimalComponentBase
    {
        #region Constructors

        public FarrowToWeanComponent()
        {
            ComponentNameDisplayString = ComponentType.FarrowToWean.GetDescription();
            ComponentCategory = ComponentCategory.Swine;
            ComponentType = ComponentType.FarrowToWean;
            ComponentDescriptionString = Resources.ToolTipFarrowToWeanProductionSystem;
        }

        #endregion
    }
}