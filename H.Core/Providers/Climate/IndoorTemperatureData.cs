using H.Core.Enumerations;
using H.Core.Providers.Temperature;

namespace H.Core.Providers.Climate
{
    public class IndoorTemperatureData : TemperatureData
    {
        #region Properties

        public Province Province { get; set; }

        #endregion
    }
}