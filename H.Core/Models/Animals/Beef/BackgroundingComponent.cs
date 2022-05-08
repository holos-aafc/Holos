#region Imports

#endregion

namespace H.Core.Models.Animals.Beef
{
    /// <summary>
    /// </summary>
    public class BackgroundingComponent : AnimalComponentBase
    {
        #region Constructors

        public BackgroundingComponent()
        {
            //base.ComponentNameDisplayString = Properties.Resources.TitleFinishingComponentDisplayString;
            this.ComponentNameDisplayString = Properties.Resources.TitleBackgroundingComponentDisplayString;
            this.ComponentCategory = ComponentCategory.BeefProduction;
            this.ComponentType = ComponentType.Backgrounding;
            this.ComponentDescriptionString = Properties.Resources.ToolTipBackgroundingComponent;
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