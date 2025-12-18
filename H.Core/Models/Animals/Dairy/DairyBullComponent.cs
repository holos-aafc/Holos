using H.Infrastructure;

namespace H.Core.Models.Animals.Dairy
{
    public class DairyBullComponent : AnimalComponentBase
    {
        #region Constructors

        public DairyBullComponent()
        {
            ComponentNameDisplayString = ComponentType.DairyBulls.GetDescription();
            ComponentCategory = ComponentCategory.Dairy;
            ComponentType = ComponentType.DairyBulls;
        }

        #endregion
    }
}