using H.Infrastructure;

namespace H.Core.Models.Animals.Dairy
{
    public class DairyLactatingComponent : AnimalComponentBase
    {
        #region Constructors

        public DairyLactatingComponent()
        {
            ComponentNameDisplayString = ComponentType.DairyLactating.GetDescription();
            ComponentCategory = ComponentCategory.Dairy;
            ComponentType = ComponentType.DairyLactating;
        }

        #endregion
    }
}