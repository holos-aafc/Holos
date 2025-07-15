using H.Core.Emissions.Results;
using H.Core.Models.LandManagement.Fields;
using System.Collections.Generic;
using System.Linq;

namespace H.Core.Calculators.Nitrogen.NitrogenService
{
    public partial class NitrogenService
    {
        /// <summary>
        /// Equation 5.6.2-1
        ///
        /// (kg N ha^-1)
        /// </summary>
        public double CalculateManureNitrogenInputsFromGrazingAnimals(FieldSystemComponent fieldSystemComponent,
            CropViewItem cropViewItem,
            List<AnimalComponentEmissionsResults> results)
        {
            var totalNitrogenExcretedByAnimals = 0d;
            var totalAmmoniaEmissions = 0d;
            var totalLeaching = 0d;
            var totalN2ON = 0d;


            var grazingViewItems = fieldSystemComponent.CropViewItems.Where(y => y.CropType == cropViewItem.CropType).SelectMany(x => x.GrazingViewItems).ToList();

            var grazingItems = grazingViewItems.Where(x => x.Start.Year == cropViewItem.Year).ToList();

            foreach (var grazingViewItem in grazingItems)
            {
                var emissionsFromGrazingAnimals = _animalService.GetGroupEmissionsFromGrazingAnimals(results, grazingViewItem);
                foreach (var groupEmissionsByMonth in emissionsFromGrazingAnimals)
                {
                    totalNitrogenExcretedByAnimals += groupEmissionsByMonth.MonthlyAmountOfNitrogenExcreted;
                    totalAmmoniaEmissions += groupEmissionsByMonth.MonthlyNH3FromGrazingAnimals;
                    totalLeaching += groupEmissionsByMonth.MonthlyManureLeachingN2ONEmission;
                    totalN2ON += (groupEmissionsByMonth.MonthlyManureDirectN2ONEmission + groupEmissionsByMonth.MonthlyManureIndirectN2ONEmission);
                }
            }

            var result = (totalNitrogenExcretedByAnimals - (totalN2ON + (CoreConstants.ConvertToNH3N(totalAmmoniaEmissions)) + totalLeaching)) / cropViewItem.Area;

            return result < 0 ? 0 : result;
        }

        public void CalculateManureNitrogenInputByGrazingAnimals(FieldSystemComponent fieldSystemComponent,
            IEnumerable<AnimalComponentEmissionsResults> results, List<CropViewItem> cropViewItems)
        {
            foreach (var cropViewItem in cropViewItems)
            {
                cropViewItem.TotalNitrogenInputFromManureFromAnimalsGrazingOnPasture = this.CalculateManureNitrogenInputsFromGrazingAnimals(fieldSystemComponent, cropViewItem, results.ToList());
            }
        }
    }
}