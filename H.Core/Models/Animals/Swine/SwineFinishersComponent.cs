using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Models.Animals.Swine
{
    /// <summary>
    /// </summary>
    public class SwineFinishersComponent : AnimalComponentBase
    {
        #region Constructors

        public SwineFinishersComponent()
        {
            ComponentNameDisplayString = ComponentType.SwineFinishers.GetDescription();
            ComponentCategory = ComponentCategory.Swine;
            ComponentType = ComponentType.SwineFinishers;
            ComponentDescriptionString = Resources.ToolTipFarrowToFinishProductionSystem;
        }

        #endregion
    }
}