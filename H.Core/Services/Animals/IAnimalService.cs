using System.Collections.Generic;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Services.Animals
{
    public interface IAnimalService
    {
        List<AnimalComponentEmissionsResults> GetAnimalResults(Farm farm);
        List<AnimalComponentEmissionsResults> GetAnimalResults(AnimalType animalType, Farm farm);

        AnimalGroupEmissionResults GetResultsForGroup(AnimalGroup animalGroup, Farm farm,
            AnimalComponentBase animalComponent);

        AnimalGroupEmissionResults GetResultsForManagementPeriod(AnimalGroup animalGroup, Farm farm,
            AnimalComponentBase animalComponent, ManagementPeriod managementPeriod);

        List<GroupEmissionsByMonth> GetGroupEmissionsFromGrazingAnimals(
            List<AnimalComponentEmissionsResults> results,
            GrazingViewItem grazingViewItem);

        /// <summary>
        ///     Selects the management periods and associated emissions for animals that are grazing on pasture according to
        ///     Chapter 11/Appendix methodology
        /// </summary>
        List<ManagementPeriod> GetGrazingManagementPeriods(
            AnimalGroup animalGroup,
            FieldSystemComponent fieldSystemComponent);
    }
}