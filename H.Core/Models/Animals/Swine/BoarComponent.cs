using H.Infrastructure;

namespace H.Core.Models.Animals.Swine
{
    public class BoarComponent : AnimalComponentBase
    {
        #region Constructors

        public BoarComponent()
        {
            ComponentNameDisplayString = ComponentType.Boar.GetDescription();
            ComponentCategory = ComponentCategory.Swine;
            ComponentType = ComponentType.Boar;
        }

        #endregion
    }
}