using H.Infrastructure;

namespace H.Core.Models.Animals.OtherAnimals
{
    public class AlpacaComponent : AnimalComponentBase
    {
        #region Constructors

        public AlpacaComponent()
        {
            this.ComponentNameDisplayString = ComponentType.Alpaca.GetDescription();
            this.ComponentDescriptionString = Properties.Resources.ToolTipAlpacaComponent;
            this.ComponentCategory = ComponentCategory.OtherLivestock;
            this.ComponentType = ComponentType.Alpaca;
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
