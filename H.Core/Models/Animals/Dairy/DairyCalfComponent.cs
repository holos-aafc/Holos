using H.Infrastructure;

namespace H.Core.Models.Animals.Dairy
{
    public class DairyCalfComponent : AnimalComponentBase
    {
        #region Constructors

        public DairyCalfComponent()
        {
            ComponentNameDisplayString = ComponentType.DairyCalf.GetDescription();
            ComponentCategory = ComponentCategory.Dairy;
            ComponentType = ComponentType.DairyCalf;
        }

        #endregion
    }
}