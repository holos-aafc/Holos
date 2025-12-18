using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Models.Animals.OtherAnimals
{
    public class AlpacaComponent : AnimalComponentBase
    {
        #region Constructors

        public AlpacaComponent()
        {
            ComponentNameDisplayString = ComponentType.Alpaca.GetDescription();
            ComponentDescriptionString = Resources.ToolTipAlpacaComponent;
            ComponentCategory = ComponentCategory.OtherLivestock;
            ComponentType = ComponentType.Alpaca;
        }

        #endregion
    }
}