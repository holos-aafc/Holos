using System;
using H.Core.Enumerations;

namespace H.Core.Providers.Climate
{
    public interface IIndoorTemperatureProvider
    {
        double GetTemperature(DateTime dateTime);
    }
}