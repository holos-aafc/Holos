using H.Infrastructure;
using System.Collections.Generic;
using System.Globalization;

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

            var d = double.Parse(value, culture);

            return d;
        }

        #endregion
    }
}