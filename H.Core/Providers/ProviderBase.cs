using H.Infrastructure;

namespace H.Core.Providers
{
    public abstract class ProviderBase
    {
        #region Properties
        public bool IsInitialized { get; set; } 
        #endregion

        #region Public Methods

        public double ParseDouble(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return 0;
            }

            var culture = InfrastructureConstants.EnglishCultureInfo;

            var result = double.Parse(value, culture);

            return result;
        }

        public int ParseInt(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return 0;
            }

            var culture = InfrastructureConstants.EnglishCultureInfo;

            var result = int.Parse(value, culture);

            return result;
        }

        #endregion
    }
}