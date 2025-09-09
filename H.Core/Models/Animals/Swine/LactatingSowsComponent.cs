using H.Infrastructure;

namespace H.Core.Models.Animals.Swine
{
    public class LactatingSowsComponent : AnimalComponentBase
    {
        #region Constructors

        public LactatingSowsComponent()
        {
            ComponentNameDisplayString = ComponentType.SwineLactatingSows.GetDescription();
            ComponentCategory = ComponentCategory.Swine;
            ComponentType = ComponentType.SwineLactatingSows;
        }

        #endregion
    }
}