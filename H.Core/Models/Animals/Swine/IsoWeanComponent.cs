using H.Core.Properties;

namespace H.Core.Models.Animals.Swine
{
    public class IsoWeanComponent : AnimalComponentBase
    {
        #region Constructors

        public IsoWeanComponent()
        {
            ComponentNameDisplayString = Resources.TitleIsoWean;
            ComponentCategory = ComponentCategory.Swine;
            ComponentType = ComponentType.IsoWean;
            ComponentDescriptionString = Resources.ToolTipIsoWeanProductionSystem;
        }

        #endregion
    }
}