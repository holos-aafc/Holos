#region Imports

#endregion

using H.Infrastructure;

namespace H.Core.Models.Animals.Swine
{
    /// <summary>
    /// </summary>
    public class SwineFinishersComponent : AnimalComponentBase
    {
        #region Fields

        #endregion

        #region Constructors

        public SwineFinishersComponent()
        {
            this.ComponentNameDisplayString = ComponentType.SwineFinishers.GetDescription();
            this.ComponentCategory = ComponentCategory.Swine;
            this.ComponentType = ComponentType.SwineFinishers;
            this.ComponentDescriptionString = H.Core.Properties.Resources.ToolTipFarrowToFinishProductionSystem;
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