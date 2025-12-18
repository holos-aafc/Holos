using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Models.Animals.Swine
{
    public class FarrowToFinishComponent : AnimalComponentBase
    {
        #region Constructors

        public FarrowToFinishComponent()
        {
            ComponentNameDisplayString = ComponentType.FarrowToFinish.GetDescription();
            ComponentCategory = ComponentCategory.Swine;
            ComponentType = ComponentType.FarrowToFinish;

            ComponentDescriptionString = Resources.ToolTipFarrowToFinishProductionSystem;
        }

        #endregion
    }
}