using H.Infrastructure;

namespace H.Core.Models.Animals.Swine
{
    public class LactatingSowsComponent : AnimalComponentBase
    {
        #region Constructors

        public LactatingSowsComponent()
        {
            this.ComponentNameDisplayString = ComponentType.SwineLactatingSows.GetDescription();
            this.ComponentCategory = ComponentCategory.Swine;
            this.ComponentType = ComponentType.SwineLactatingSows;
        }

        #endregion
    }
}