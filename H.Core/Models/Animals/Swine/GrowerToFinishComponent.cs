using H.Infrastructure;

namespace H.Core.Models.Animals.Swine
{
    /// <summary>
    /// Represents a "Grower-to-finish" swine production system
    /// </summary>
    public class GrowerToFinishComponent : AnimalComponentBase
    {
        #region Constructors

        public GrowerToFinishComponent()
        {
            this.ComponentNameDisplayString = H.Core.Properties.Resources.TitleGrowerToFinish;
            this.ComponentCategory = ComponentCategory.Swine;
            this.ComponentType = ComponentType.SwineGrowers;
            this.ComponentDescriptionString = H.Core.Properties.Resources.ToolTipGrowerToFinishProductionSystem;
        }

        #endregion
    }
}
