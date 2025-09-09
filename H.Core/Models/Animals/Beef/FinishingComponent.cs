using H.Core.Properties;

namespace H.Core.Models.Animals.Beef
{
    /// <summary>
    /// </summary>
    public class FinishingComponent : AnimalComponentBase
    {
        #region Constructors

        public FinishingComponent()
        {
            ComponentNameDisplayString = Resources.TitleFinishingComponentDisplayString;
            ComponentDescriptionString = Resources.ToolTipFinishingComponent;
            ComponentCategory = ComponentCategory.BeefProduction;
            ComponentType = ComponentType.Finishing;
        }

        #endregion
    }
}