using H.Infrastructure;

namespace H.Core.Models.Animals.Dairy
{
    public class DairyBullComponent : AnimalComponentBase
    {
        #region Fields        

        #endregion

        #region Constructors

        public DairyBullComponent()
        {
            this.ComponentNameDisplayString = ComponentType.DairyBulls.GetDescription();
            this.ComponentCategory = ComponentCategory.Dairy;
            this.ComponentType = ComponentType.DairyBulls;
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
