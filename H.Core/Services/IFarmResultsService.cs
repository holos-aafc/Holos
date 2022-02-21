using System.Collections.Generic;
using H.Core.Emissions.Results;
using H.Core.Models;

namespace H.Core.Services
{
    public interface IFarmResultsService
    {
        Farm ReplicateFarm(Farm farm);
        List<Farm> ReplicateFarms(IEnumerable<Farm> farms);
        Farm Create();
        FarmEmissionResults CalculateFarmEmissionResults(Farm farm);
        List<FarmEmissionResults> CalculateFarmEmissionResults(IEnumerable<Farm> farms);

        bool CropEconomicDataApplied { get; set; }
    }
}