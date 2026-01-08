using H.Core.Emissions.Results;
using H.Core.Models;
using System.Collections.Generic;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Calculators.Nitrogen
{
    public partial class N2OEmissionFactorCalculator
    {
        public double GetDirectN2ONFromGrazingAnimals(Farm farm, CropViewItem currentYearResults,
            List<AnimalComponentEmissionsResults> animalComponentEmissionsResultsList)
        {
            var result = 0d;
            var field = farm.GetFieldSystemComponent(currentYearResults.FieldSystemComponentGuid);

            foreach (var emissionsResults in animalComponentEmissionsResultsList)
            {
                foreach (var animalGroupEmissionResults in emissionsResults.EmissionResultsForAllAnimalGroupsInComponent)
                {
                    foreach (var groupEmissionsByMonth in animalGroupEmissionResults.GroupEmissionsByMonths)
                    {
                        var managementPeriod = groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod;
                        if (field.IsGrazingManagementPeriodFromPasture(managementPeriod) && managementPeriod.Start.Year == currentYearResults.Year)
                        {
                            var emissions = groupEmissionsByMonth.MonthlyManureDirectN2ONEmission;
                            result += emissions;
                        }

                    }
                }
            }

            return result;
        }

        public double GetLeachingN2ONFromGrazingAnimals(Farm farm, CropViewItem currentYearResults,
            List<AnimalComponentEmissionsResults> animalComponentEmissionsResultsList)
        {
            var result = 0d;
            var field = farm.GetFieldSystemComponent(currentYearResults.FieldSystemComponentGuid);

            foreach (var emissionsResults in animalComponentEmissionsResultsList)
            {
                foreach (var animalGroupEmissionResults in emissionsResults.EmissionResultsForAllAnimalGroupsInComponent)
                {
                    foreach (var groupEmissionsByMonth in animalGroupEmissionResults.GroupEmissionsByMonths)
                    {
                        var managementPeriod = groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod;
                        if (field.IsGrazingManagementPeriodFromPasture(managementPeriod) && managementPeriod.Start.Year == currentYearResults.Year)
                        {
                            var emissions = groupEmissionsByMonth.MonthlyManureLeachingN2ONEmission;
                            result += emissions;
                        }

                    }
                }
            }

            return result;
        }

        public double GetActualLeachingN2ONFromGrazingAnimals(Farm farm, CropViewItem currentYearResults,
            List<AnimalComponentEmissionsResults> animalComponentEmissionsResultsList)
        {
            var result = 0d;
            var field = farm.GetFieldSystemComponent(currentYearResults.FieldSystemComponentGuid);

            foreach (var emissionsResults in animalComponentEmissionsResultsList)
            {
                foreach (var animalGroupEmissionResults in emissionsResults.EmissionResultsForAllAnimalGroupsInComponent)
                {
                    foreach (var groupEmissionsByMonth in animalGroupEmissionResults.GroupEmissionsByMonths)
                    {
                        var managementPeriod = groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod;
                        if (field.IsGrazingManagementPeriodFromPasture(managementPeriod) && managementPeriod.Start.Year == currentYearResults.Year)
                        {
                            var emissions = groupEmissionsByMonth.MonthlyManureNitrateLeachingN2ONEmission;
                            result += emissions;
                        }

                    }
                }
            }

            return result;
        }

        public double GetVolatilizationN2ONFromGrazingAnimals(Farm farm, CropViewItem currentYearResults,
            List<AnimalComponentEmissionsResults> animalComponentEmissionsResultsList)
        {
            var result = 0d;
            var field = farm.GetFieldSystemComponent(currentYearResults.FieldSystemComponentGuid);

            foreach (var emissionsResults in animalComponentEmissionsResultsList)
            {
                foreach (var animalGroupEmissionResults in emissionsResults.EmissionResultsForAllAnimalGroupsInComponent)
                {
                    foreach (var groupEmissionsByMonth in animalGroupEmissionResults.GroupEmissionsByMonths)
                    {
                        var managementPeriod = groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod;
                        if (field.IsGrazingManagementPeriodFromPasture(managementPeriod) && managementPeriod.Start.Year == currentYearResults.Year)
                        {
                            var emissions = groupEmissionsByMonth.MonthlyManureVolatilizationN2ONEmission;
                            result += emissions;
                        }

                    }
                }
            }

            return result;
        }

        public double GetVolatilizationNH3FromGrazingAnimals(Farm farm, CropViewItem currentYearResults,
            List<AnimalComponentEmissionsResults> animalComponentEmissionsResultsList)
        {
            var result = 0d;
            var field = farm.GetFieldSystemComponent(currentYearResults.FieldSystemComponentGuid);

            foreach (var emissionsResults in animalComponentEmissionsResultsList)
            {
                foreach (var animalGroupEmissionResults in emissionsResults.EmissionResultsForAllAnimalGroupsInComponent)
                {
                    foreach (var groupEmissionsByMonth in animalGroupEmissionResults.GroupEmissionsByMonths)
                    {
                        var managementPeriod = groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod;
                        if (field.IsGrazingManagementPeriodFromPasture(managementPeriod) && managementPeriod.Start.Year == currentYearResults.Year)
                        {
                            var emissions = groupEmissionsByMonth.MonthlyNH3FromGrazingAnimals;
                            result += emissions;
                        }

                    }
                }
            }

            return result;
        }
    }
}