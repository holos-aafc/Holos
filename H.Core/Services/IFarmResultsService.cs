using System.Collections.Generic;
using H.Core.Emissions.Results;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Services
{
    public interface IFarmResultsService
    {
        bool CropEconomicDataApplied { get; set; }
        Farm ReplicateFarm(Farm farm);
        List<Farm> ReplicateFarms(IEnumerable<Farm> farms);
        FarmEmissionResults CalculateFarmEmissionResults(Farm farm);
        List<FarmEmissionResults> CalculateFarmEmissionResults(IEnumerable<Farm> farms);
        List<CropViewItem> CalculateFieldResults(Farm farm);
    }
}