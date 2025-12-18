using H.Infrastructure;

namespace H.Core.Models.Animals.Dairy
{
    public class DairyDryComponent : AnimalComponentBase
    {
        #region Constructors

        public DairyDryComponent()
        {
            ComponentNameDisplayString = ComponentType.DairyDry.GetDescription();
            ComponentCategory = ComponentCategory.Dairy;
            ComponentType = ComponentType.DairyDry;
        }

        #endregion
    }
}