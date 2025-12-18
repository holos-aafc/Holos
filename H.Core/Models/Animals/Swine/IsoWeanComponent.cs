namespace H.Core.Models.Animals.Swine
{
    public class IsoWeanComponent : AnimalComponentBase
    {
        #region Constructors

        public IsoWeanComponent()
        {
            this.ComponentNameDisplayString = H.Core.Properties.Resources.TitleIsoWean;
            this.ComponentCategory = ComponentCategory.Swine;
            this.ComponentType = ComponentType.IsoWean;
            this.ComponentDescriptionString = H.Core.Properties.Resources.ToolTipIsoWeanProductionSystem;
        }

        #endregion
    }
}