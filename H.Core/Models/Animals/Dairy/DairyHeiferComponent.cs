using H.Infrastructure;

namespace H.Core.Models.Animals.Dairy
{
    public class DairyHeiferComponent : AnimalComponentBase
    {
        #region Fields        

        #endregion

        #region Constructors

        public DairyHeiferComponent()
        {
            this.ComponentNameDisplayString = ComponentType.DairyHeifer.GetDescription();
            this.ComponentCategory = ComponentCategory.Dairy;
            this.ComponentType = ComponentType.DairyHeifer;
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
