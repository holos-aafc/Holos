using H.Infrastructure;

namespace H.Core.Models.Animals.Swine
{
    public class FarrowToFinishComponent : AnimalComponentBase
    {
        #region Constructors

        public FarrowToFinishComponent()
        {
            this.ComponentNameDisplayString = ComponentType.FarrowToFinish.GetDescription();
            this.ComponentCategory = ComponentCategory.Swine;
            this.ComponentType = ComponentType.FarrowToFinish;

            this.ComponentDescriptionString = H.Core.Properties.Resources.ToolTipFarrowToFinishProductionSystem;
        }

        #endregion
    }
}