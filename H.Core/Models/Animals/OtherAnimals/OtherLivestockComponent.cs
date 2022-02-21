#region Imports

#endregion

using H.Infrastructure;

namespace H.Core.Models.Animals.OtherAnimals
{
    /// <summary>
    /// </summary>
    public class OtherLivestockComponent : AnimalComponentBase
    {
        #region Constructors

        public OtherLivestockComponent()
        {
            this.ComponentNameDisplayString = ComponentType.OtherLivestock.GetDescription();
            this.ComponentCategory = ComponentCategory.OtherLivestock;
            this.ComponentType = ComponentType.OtherLivestock;
        }

        #endregion

        #region Fields

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