using H.Infrastructure;

namespace H.Core.Models.Animals.Dairy
{
    public class DairyCalfComponent : AnimalComponentBase
    {
        #region Fields        

        #endregion

        #region Constructors

        public DairyCalfComponent() 
        {
            this.ComponentNameDisplayString = ComponentType.DairyCalf.GetDescription();
            this.ComponentCategory = ComponentCategory.Dairy;
            this.ComponentType = ComponentType.DairyCalf;
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
