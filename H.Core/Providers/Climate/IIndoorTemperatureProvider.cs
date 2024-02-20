using System;
using H.Core.Enumerations;

namespace H.Core.Providers.Climate
{
    public interface IIndoorTemperatureProvider
    {
        IndoorTemperatureData GetIndoorTemperature(Province province);
        double GetIndoorTemperatureForMonth(Province province, Months months);
    }
}