using H.Infrastructure;

namespace H.Core.Models.Animals.Swine
{
    public class FarrowToWeanComponent : AnimalComponentBase
    {
        #region Fields

        #endregion

        #region Constructors

        public FarrowToWeanComponent()
        {
            this.ComponentNameDisplayString = ComponentType.FarrowToWean.GetDescription();
            this.ComponentCategory = ComponentCategory.Swine;
            this.ComponentType = ComponentType.FarrowToWean;
            this.ComponentDescriptionString = H.Core.Properties.Resources.ToolTipFarrowToWeanProductionSystem;
        }

        #endregion

        #region Properties

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion

        #region Event Handlers

        #endregion
    }
}