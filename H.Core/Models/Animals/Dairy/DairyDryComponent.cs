using H.Infrastructure;

namespace H.Core.Models.Animals.Dairy
{
    public class DairyDryComponent : AnimalComponentBase
    {
        #region Fields        

        #endregion

        #region Constructors

        public DairyDryComponent()
        {
            this.ComponentNameDisplayString = ComponentType.DairyDry.GetDescription();
            this.ComponentCategory = ComponentCategory.Dairy;
            this.ComponentType = ComponentType.DairyDry;
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
