using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Models.Animals.Dairy
{
    /// <summary>
    /// </summary>
    public class DairyComponent : AnimalComponentBase
    {
        #region Constructors

        public DairyComponent()
        {
            ComponentNameDisplayString = ComponentType.Dairy.GetDescription();
            ComponentDescriptionString = Resources.ToolTipDairyComponent;
            ComponentCategory = ComponentCategory.Dairy;
            ComponentType = ComponentType.Dairy;
        }

        #endregion
    }
}