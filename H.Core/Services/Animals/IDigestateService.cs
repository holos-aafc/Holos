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
        double MaximumAmountOfDigestateAvailableForLandApplication(DateTime dateTime, Farm farm, DigestateState digestateState);

        DigestateTank GetTank(
            Farm farm,
            int year, 
            DigestateState state,
            List<DigestorDailyOutput> dailyDigestorResults);

        List<DigestorDailyOutput> GetDailyResults(Farm farm);
    }
}