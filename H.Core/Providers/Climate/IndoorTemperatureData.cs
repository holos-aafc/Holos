using H.Core.Enumerations;

namespace H.Core.Providers.Climate
{
    public class IndoorTemperatureData : MonthlyValueBase<double>
    {
        #region Properties

        public Province Province { get; set; }

        #endregion
    }
}