#region Imports

#endregion

namespace H.Core.Models.Animals.Beef
{
    /// <summary>
    /// </summary>
    public class FinishingComponent : AnimalComponentBase
    {
        #region Constructors

        public FinishingComponent()
        {
            this.ComponentNameDisplayString = Properties.Resources.TitleFinishingComponentDisplayString;
            this.ComponentDescriptionString = Properties.Resources.ToolTipFinishingComponent;
            this.ComponentCategory = ComponentCategory.BeefProduction;
            this.ComponentType = ComponentType.Finishing;
        }

        #endregion

        #region Fields        

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