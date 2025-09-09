using H.Core.Properties;

namespace H.Core.Models.Animals.Swine
{
    /// <summary>
    ///     Represents a "Grower-to-finish" swine production system
    /// </summary>
    public class GrowerToFinishComponent : AnimalComponentBase
    {
        #region Constructors

        public GrowerToFinishComponent()
        {
            ComponentNameDisplayString = Resources.TitleGrowerToFinish;
            ComponentCategory = ComponentCategory.Swine;
            ComponentType = ComponentType.SwineGrowers;
            ComponentDescriptionString = Resources.ToolTipGrowerToFinishProductionSystem;
        }

        #endregion
    }
}