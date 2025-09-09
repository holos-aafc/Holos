using H.Core.Properties;

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
            ComponentNameDisplayString = Resources.TitleBackgroundingComponentDisplayString;
            ComponentCategory = ComponentCategory.BeefProduction;
            ComponentType = ComponentType.Backgrounding;
            ComponentDescriptionString = Resources.ToolTipBackgroundingComponent;
        }

        #endregion
    }
}