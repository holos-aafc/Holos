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
            ComponentNameDisplayString = ComponentType.OtherLivestock.GetDescription();
            ComponentCategory = ComponentCategory.OtherLivestock;
            ComponentType = ComponentType.OtherLivestock;
        }

        #endregion
    }
}