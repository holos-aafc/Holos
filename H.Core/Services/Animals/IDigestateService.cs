using System;
using System.Collections.Generic;
using H.Core.Calculators.Infrastructure;
using H.Core.Enumerations;
using H.Core.Models;

namespace H.Core.Services.Animals
{
    public interface IDigestateService
    { 
        void Initialize(Farm farm);
        DateTime GetDateOfMaximumAvailableDigestate(Farm farm, DigestateState state, int year);
        List<DigestorDailyOutput> GetDailyResults(Farm farm);
        DigestateTank GetTank(Farm farm, DateTime targetDate);
    }
}