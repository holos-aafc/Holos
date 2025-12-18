using System.Collections.Generic;
using H.Core.Emissions.Results;
using H.Core.Models;

namespace H.Core.Calculators.Infrastructure
{
    public interface IADCalculator
    {
        List<DigestorDailyOutput> CalculateResults(Farm farm, List<AnimalComponentEmissionsResults> animalComponentEmissionsResults);
    }
}