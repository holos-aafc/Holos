using H.Infrastructure;

namespace H.Core.Models.Animals.Dairy
{
    public class DairyLactatingComponent : AnimalComponentBase
    {
        #region Fields        

        #endregion

        #region Constructors

        public DairyLactatingComponent()
        {
            this.ComponentNameDisplayString = ComponentType.DairyLactating.GetDescription();
            this.ComponentCategory = ComponentCategory.Dairy;
            this.ComponentType = ComponentType.DairyLactating;
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
