using H.Core.Properties;

namespace H.Core.Models.Animals.Beef
{
    /// <summary>
    /// </summary>
    public class CowCalfComponent : AnimalComponentBase
    {
        #region Constructors

        public CowCalfComponent()
        {
            ComponentCategory = ComponentCategory.BeefProduction;
            ComponentType = ComponentType.CowCalf;
            ComponentNameDisplayString = Resources.TitleCowCalfComponentDisplayString;
            ComponentDescriptionString = Resources.ToolTipCowCalfComponent;
        }

        #endregion
    }
}