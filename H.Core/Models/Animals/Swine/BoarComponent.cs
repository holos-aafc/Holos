using H.Infrastructure;

namespace H.Core.Models.Animals.Swine
{
    public class BoarComponent : AnimalComponentBase
    {
        #region Fields

        #endregion

        #region Constructors

        public BoarComponent()
        {
            this.ComponentNameDisplayString = ComponentType.Boar.GetDescription();
            this.ComponentCategory = ComponentCategory.Swine;
            this.ComponentType = ComponentType.Boar;
        }

        #endregion

        #region Properties

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion

        #region Event Handlers

        #endregion
    }
}
