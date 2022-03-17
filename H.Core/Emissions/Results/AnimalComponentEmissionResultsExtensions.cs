using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using H.Core.Models;
using H.Core.Models.Animals;

namespace H.Core.Emissions.Results
{
    public static class AnimalComponentEmissionResultsExtensions
    {
        public static List<AnimalComponentEmissionsResults> GetByCategory(this IEnumerable<AnimalComponentEmissionsResults> animalComponentEmissionsResults, ComponentCategory category)
        {
            return animalComponentEmissionsResults.Where(result => result.Component.ComponentCategory == category).ToList();
        }

        /// <summary>
        /// (CH4)
        /// </summary>
        public static double TotalEntericAndManureMethane(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            return results.Sum(result => result.TotalEntericAndManureMethane);
        }

        /// <summary>
        /// (CO2)
        /// </summary>
        public static double TotalCarbonDioxideEmissionsFromManureSpreading(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            return results.Sum(result => result.TotalCarbonDioxideEmissionsFromManureApplication);
        }

        public static double TotalMilkProduced(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            return results.Sum(result => result.TotalMilkProduced);
        }

        public static double TotalBeefProduced(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            return results.Sum(result => result.TotalBeefProduced);
        }

        public static double TotalFatAndProteinCorrectMilk(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            return results.Sum(result => result.TotalFatAndProteinCorrectedMilkProduced);
        }

        /// <summary>
        /// (CO2e)
        /// </summary>
        public static double TotalMethaneAsCarbonDioxideEquivalents(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            return results.TotalEntericAndManureMethane() * CoreConstants.CH4ToCO2eConversionFactor;
        }

        /// <summary>
        /// (kg CH4)
        /// </summary>
        public static double TotalEntericMethane(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            return results.Sum(result => result.TotalEntericMethaneEmission);
        }

        /// <summary>
        /// Equation 4.4.1-1
        /// 
        /// (kg CH4)
        /// </summary>
        public static double TotalManureMethane(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            return results.Sum(result => result.TotalManureMethaneEmission);
        }

        /// <summary>
        /// Equation 4.4.2-1
        /// 
        /// (kg N2O-N)
        /// </summary>
        public static double TotalDirectManureN2ON(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            return results.Sum(result => result.TotalManureDirectN2ONEmission);
        }

        /// <summary>
        /// Equation 4.4.2-2
        /// 
        /// (kg NH3)
        /// </summary>
        public static double TotalAmmoniaEmissionsFromHousing(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            return results.Sum(result => result.TotalAmmoniaEmissionsFromHousingSystem);
        }

        /// <summary>
        /// Equation 4.4.2-3
        /// 
        /// (kg NH3)
        /// </summary>
        public static double TotalAmmoniaEmissionsFromStorage(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            return results.Sum(result => result.TotalAmmoniaEmissionsFromStorageSystem);
        }

        /// <summary>
        /// Equation 4.4.2-4
        /// 
        /// (kg NH3)
        /// </summary>
        public static double TotalAmmoniaEmissions(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            return results.Sum(result => result.TotalAmmoniaEmissions);
        }

        /// <summary>
        /// Equation 4.4.2-5
        /// 
        /// (kg N2O-N)
        /// </summary>
        public static double TotalN2ONVolatilization(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            return results.Sum(result => result.TotalManureVolatilizationN2ONEmission);
        }

        /// <summary>
        /// Equation 4.4.2-6
        /// 
        /// (kg N2O-N)
        /// </summary>
        public static double TotalN2ONLeaching(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            return results.Sum(result => result.TotalManureLeachingN2ONEmission);
        }

        /// <summary>
        /// Equation 4.4.2-7
        /// 
        /// (kg N2O-N)
        /// </summary>
        public static double TotalN2ONIndirectManure(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            return results.Sum(result => result.TotalManureIndirectN2ONEmission);
        }

        /// <summary>
        /// Equation 4.4.2-8
        /// 
        /// (kg N2O-N)
        /// </summary>
        public static double TotalN2ONManure(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            return results.Sum(result => result.TotalManureN2ONEmission);
        }

        /// <summary>
        /// Equation 4.4.3-1
        /// 
        /// (kg N2O)
        /// </summary>
        public static double TotalDirectN2OManure(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            return results.Sum(result => result.TotalManureDirectN2OEmission);
        }

        /// <summary>
        /// Equation 4.4.3-2
        /// 
        /// (kg N2O)
        /// </summary>
        public static double TotalIndirectN2OManure(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            return results.Sum(result => result.TotalManureIndirectN2OEmission);
        }

        /// <summary>
        /// Equation 4.4.3-3
        /// 
        /// (kg N2O)
        /// </summary>
        public static double TotalManureN2O(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            return results.Sum(result => result.TotalManureN2OEmission);
        }

        /// <summary>
        /// Equation 4.5.1-1
        /// 
        /// (kg C)
        /// </summary>
        public static double TotalAmountOfCarbonInStoredManure(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            return results.Sum(result => result.TotalAmountOfCarbonInStoredManure);
        }

        /// <summary>
        /// Equation 4.5.2-2
        /// 
        /// (kg TAN)
        /// </summary>
        public static double TotalTANAvailableForLandApplication(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            return results.Sum(result => result.TotalTANAvailableForLandApplication);
        }


        /// <summary>
        /// Equation 4.5.2-4
        /// 
        /// (kg N)
        /// </summary>
        public static double TotalOrganicNitrogenAvailableForLandApplication(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            return results.Sum(result => result.TotalOrganicNitrogenAvailableForLandApplication);
        }

        /// <summary>
        /// Equation 4.5.2-5
        /// 
        /// (kg N)
        /// </summary>
        public static double TotalNitrogenAvailableForLandApplication(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            return results.Sum(result => result.TotalAvailableManureNitrogenInStoredManureAvailableForLandApplication);
        }

        /// <summary>
        /// N2O
        /// </summary>
        public static double TotalNitrousOxide(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            return results.Sum(result => result.TotalNitrousOxide);
        }

        public static double TotalEnergyCarbonDioxide(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            return results.Sum(result => result.TotalEnergyCO2Emissions);
        }

        /// <summary>
        /// (CO2e)
        /// </summary>
        public static double TotalNitrousOxideAsCarbonDioxideEquivalents(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            return results.TotalNitrousOxide() * CoreConstants.N2OToCO2eConversionFactor;
        }

        /// <summary>
        /// (kg N2O)
        /// </summary>
        public static double TotalDirectNitrousOxide(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            return results.Sum(x => x.TotalDirectNitrousOxideEmission);
        }

        /// <summary>
        /// (kg N2O)
        /// </summary>
        public static double TotalIndirectNitrousOxide(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            return results.Sum(x => x.TotalIndirectN2OEmission);
        }

        /// <summary>
        /// (CO2e)
        /// </summary>
        public static double TotalCarbonDioxideEquivalents(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            return results.TotalNitrousOxideAsCarbonDioxideEquivalents() +
                   results.TotalEnergyCarbonDioxide() +
                   results.TotalMethaneAsCarbonDioxideEquivalents();
        }

        /// <summary>
        /// Note: for animals, the total CO2 is equal to the total energy CO2
        /// 
        /// (kg CO2)
        /// </summary>
        public static double TotalCarbonDioxide(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            return results.Sum(x => x.TotalCarbonDioxide);
        }


        /// <summary>
        /// Equation 4.5.2-5
        ///
        /// Total available manure N available for land application (organic N and TAN)
        /// 
        /// (kg N year^-1)
        /// </summary>
        public static double TotalAvailableManureNitrogenInStoredManureAvailableForLandApplication(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            var total = 0.0;

            foreach (var result in results)
            {
                total += result.TotalAvailableManureNitrogenInStoredManureAvailableForLandApplication;
            }

            return total;
        }

        /// <summary>
        /// (kg N)
        /// </summary>
        public static double TotalOrganicNitrogenInStoredManure(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            var result = 0d;

            foreach (var item in results)
            {
                foreach (var animalGroupEmissionResult in item.EmissionResultsForAllAnimalGroupsInComponent)
                {
                    result += animalGroupEmissionResult.GroupEmissionsByMonths.Sum(x => x.MonthlyOrganicNitrogenInStoredManure);
                }
            }

            return result;
        }

        /// <summary>
        /// (kg for solid manure, L for liquid manure)
        /// </summary>
        public static double TotalVolumeOfManureAvailableForLandApplication(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            var result = 0d;

            foreach (var item in results)
            {
                foreach (var animalGroupEmissionResult in item.EmissionResultsForAllAnimalGroupsInComponent)
                {
                    result += animalGroupEmissionResult.GroupEmissionsByMonths.Sum(x => x.TotalVolumeOfManureAvailableForLandApplication);
                }
            }

            return result;
        }

        public static double TotalLambProduced(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            var total = 0.0;

            foreach (var result in results)
            {
                foreach (var animalGroupEmissionResults in result.EmissionResultsForAllAnimalGroupsInComponent)
                {
                    foreach (var animalGroupEmissionsByMonth in animalGroupEmissionResults.GroupEmissionsByMonths)
                    {
                        total += animalGroupEmissionsByMonth.MonthlyLambProduced;
                    }
                }
            }

            return total;
        }

        public static int TotalManagementDays(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            var total = 0;

            foreach (var result in results)
            {
                if (result.Component is AnimalComponentBase animalComponent)
                {
                    foreach (var animalGroup in animalComponent.Groups)
                    {
                        foreach (var managementPeriod in animalGroup.ManagementPeriods)
                        {
                            total += managementPeriod.Duration.Days;
                        }
                    }
                }
            }

            return total;
        }

        public static int TotalNumberOfAnimalsDuringManagementDays(this IEnumerable<AnimalComponentEmissionsResults> results)
        {
            var total = 0;

            foreach (var result in results)
            {
                if (result.Component is AnimalComponentBase animalComponent)
                {
                    foreach (var animalGroup in animalComponent.Groups)
                    {
                        total += animalGroup.TotalNumberOfAnimalsSurviving();
                    }
                }
            }

            return total;
        }
    }
}