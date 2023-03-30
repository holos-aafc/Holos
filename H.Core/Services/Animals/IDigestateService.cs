using System;
using System.Collections.Generic;
using H.Core.Calculators.Infrastructure;
using H.Core.Enumerations;
using H.Core.Models;

namespace H.Core.Services.Animals
{
    public interface IDigestateService
    {
        DateTime GetDateOfMaximumAvailableDigestate(Farm farm, DigestateState state, int year, List<DigestorDailyOutput> digestorDailyOutputs);
        DigestateTank GetTank(Farm farm, DateTime targetDate, List<DigestorDailyOutput> dailyOutputs);
        List<DigestorDailyOutput> GetDailyResults(Farm farm);
    }
}