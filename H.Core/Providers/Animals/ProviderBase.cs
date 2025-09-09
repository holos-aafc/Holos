#region Imports

using H.Core.Converters;

#endregion

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// </summary>
    public abstract class ProviderBase
    {
        #region Constructors

        protected ProviderBase()
        {
            _animalTypeStringConverter = new AnimalTypeStringConverter();
            _provinceStringConverter = new ProvinceStringConverter();
            _componentTypeStringConverter = new ComponentTypeStringConverter();
            _productionStageStringConverter = new ProductionStageStringConverter();
        }

        #endregion

        #region Fields

        protected readonly AnimalTypeStringConverter _animalTypeStringConverter;
        protected readonly ProvinceStringConverter _provinceStringConverter;
        protected readonly ComponentTypeStringConverter _componentTypeStringConverter;
        protected readonly ProductionStageStringConverter _productionStageStringConverter;

        #endregion
    }
}