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
            animalTypeStringConverter = new AnimalTypeStringConverter();
            provinceStringConverter = new ProvinceStringConverter();
        }

        #endregion

        #region Fields

        protected readonly AnimalTypeStringConverter animalTypeStringConverter;
        protected readonly ProvinceStringConverter provinceStringConverter;

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