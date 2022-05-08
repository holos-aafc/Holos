#region Imports

#endregion

using H.Infrastructure;

namespace H.Core.Models.Animals.Dairy
{
    /// <summary>
    /// </summary>
    public class DairyComponent : AnimalComponentBase
    {
        #region Fields        

        #endregion

        #region Constructors

        public DairyComponent()
        {
            this.ComponentNameDisplayString = ComponentType.Dairy.GetDescription();
            this.ComponentDescriptionString = Properties.Resources.ToolTipDairyComponent;
            this.ComponentCategory = ComponentCategory.Dairy;
            this.ComponentType = ComponentType.Dairy;
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