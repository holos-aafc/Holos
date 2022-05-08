using H.Infrastructure;

namespace H.Core.Models.Animals.OtherAnimals
{
    public class ElkComponent : AnimalComponentBase
    {
        #region Constructors

        public ElkComponent()
        {
            this.ComponentNameDisplayString = ComponentType.Elk.GetDescription();
            this.ComponentDescriptionString = Properties.Resources.ToolTipElkComponent;
            this.ComponentCategory = ComponentCategory.OtherLivestock;
            this.ComponentType = ComponentType.Elk;
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
