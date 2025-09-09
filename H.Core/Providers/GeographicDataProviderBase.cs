using H.Core.Providers.Soil;

namespace H.Core.Providers
{
    /// <summary>
    /// </summary>
    public abstract class GeographicDataProviderBase : ProviderBase
    {
        #region Fields

        protected readonly EcodistrictDefaultsProvider ecodistrictDefaultsProvider = new EcodistrictDefaultsProvider();

        #endregion
    }
}