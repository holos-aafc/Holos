using H.Infrastructure;

namespace H.Core.Models.Animals.Dairy
{
    public class DairyHeiferComponent : AnimalComponentBase
    {
        #region Constructors

        public DairyHeiferComponent()
        {
            ComponentNameDisplayString = ComponentType.DairyHeifer.GetDescription();
            ComponentCategory = ComponentCategory.Dairy;
            ComponentType = ComponentType.DairyHeifer;
        }

        #endregion
    }
}