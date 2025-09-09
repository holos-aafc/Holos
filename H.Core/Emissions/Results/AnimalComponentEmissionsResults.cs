﻿using System.Collections.Generic;
using System.Linq;
using H.Core.Models;

namespace H.Core.Emissions.Results
{
    /// <summary>
    ///     A class to hold a collection of <see cref="AnimalGroupEmissionResults" /> and properties that hold the totals from
    ///     this collection of results - these total will be the emissions
    ///     from all animal groups in the component.
    /// </summary>
    public class AnimalComponentEmissionsResults
    {
        #region Methods

        public List<GroupEmissionsByDay> GetDailyEmissions()
        {
            var result = new List<GroupEmissionsByDay>();

            foreach (var emissionResultsForAnimalComponent in EmissionResultsForAllAnimalGroupsInComponent)
            foreach (var groupEmissionsByMonth in emissionResultsForAnimalComponent.GroupEmissionsByMonths)
                result.AddRange(groupEmissionsByMonth.DailyEmissions);

            return result;
        }

        #endregion

        #region Properties

        public double TotalCarbonDioxideEquivalentsFromAllGroupsInComponent
        {
            get
            {
                var result = 0d;

                foreach (var groupEmissionResults in EmissionResultsForAllAnimalGroupsInComponent)
                    result += groupEmissionResults.TotalCarbonDioxideEquivalentEmissionsFromAnimalGroup;

                return result;
            }
        }

        /// <summary>
        ///     Equation 3.1.3-1
        ///     Equation 3.2.3-1
        ///     Equation 3.3.2-1
        ///     Equation 3.4.2-1
        ///     Total enteric CH4 emissions from all animal groups in component over all months
        ///     (kg CH4)
        /// </summary>
        public double TotalEntericMethaneEmission
        {
            get
            {
                var result = 0d;

                foreach (var animalGroupEmissionResult in EmissionResultsForAllAnimalGroupsInComponent)
                    result += animalGroupEmissionResult.GroupEmissionsByMonths.Sum(x =>
                        x.MonthlyEntericMethaneEmission);

                return result;
            }
        }

        public double TotalEntericMethaneAsCO2e =>
            TotalEntericMethaneEmission * CoreConstants.CH4ToCO2eConversionFactor;

        /// <summary>
        ///     Equation 4.1.3-10
        ///     Equation 5.4.1-1
        ///     Total manure CH4 emissions from all animal groups in component over all months
        ///     (kg CH4)
        /// </summary>
        public double TotalManureMethaneEmission
        {
            get
            {
                var result = 0d;

                foreach (var animalGroupEmissionResult in EmissionResultsForAllAnimalGroupsInComponent)
                    result += animalGroupEmissionResult.GroupEmissionsByMonths.Sum(x => x.MonthlyManureMethaneEmission);

                return result;
            }
        }

        /// <summary>
        ///     Equation 4.1.3-11
        ///     Total volatile solids produced from all animal groups in component over all months
        ///     (kg)
        /// </summary>
        public double TotalVolatileSolids
        {
            get
            {
                var result = 0d;

                foreach (var animalGroupEmissionResult in EmissionResultsForAllAnimalGroupsInComponent)
                    result += animalGroupEmissionResult.GroupEmissionsByMonths.Sum(x => x.MonthlyVolatileSolids);

                return result;
            }
        }

        /// <summary>
        ///     Equation 5.4.2-1
        ///     Total manure direct N2O-N emissions from all animal groups in component over all months
        ///     (kg N2O-N)
        /// </summary>
        public double TotalManureDirectN2ONEmission
        {
            get
            {
                var result = 0d;

                foreach (var animalGroupEmissionResult in EmissionResultsForAllAnimalGroupsInComponent)
                    result += animalGroupEmissionResult.GroupEmissionsByMonths.Sum(x =>
                        x.MonthlyManureDirectN2ONEmission);

                return result;
            }
        }

        /// <summary>
        ///     Total manure direct N2O emissions from all animal groups in component over all months
        ///     Equation 3.1.6-3
        ///     Equation 3.2.8-3
        ///     Equation 3.3.4-3
        ///     (kg N2O)
        /// </summary>
        public double TotalManureDirectN2OEmission
        {
            get
            {
                var result = 0d;

                foreach (var animalGroupEmissionResult in EmissionResultsForAllAnimalGroupsInComponent)
                    result += animalGroupEmissionResult.GroupEmissionsByMonths.Sum(
                        x => x.MonthlyManureDirectN2OEmission);

                return result;
            }
        }

        /// <summary>
        ///     Equation 5.4.2-2
        ///     (kg N2O-N)
        /// </summary>
        public double TotalManureVolatilizationN2ONEmission
        {
            get
            {
                var result = 0d;

                foreach (var animalGroupEmissionResult in EmissionResultsForAllAnimalGroupsInComponent)
                    result += animalGroupEmissionResult.GroupEmissionsByMonths.Sum(x =>
                        x.MonthlyManureVolatilizationN2ONEmission);

                return result;
            }
        }

        /// <summary>
        ///     Total manure volatilization emissions from all animal groups in component over all months
        ///     (kg N2O)
        /// </summary>
        public double TotalManureVolatilizationN2OEmission
        {
            get
            {
                var result = 0d;

                foreach (var animalGroupEmissionResult in EmissionResultsForAllAnimalGroupsInComponent)
                    result += animalGroupEmissionResult.GroupEmissionsByMonths.Sum(x =>
                        x.MonthlyManureVolatilizationN2OEmission);

                return result;
            }
        }

        /// <summary>
        ///     Equation 5.4.2-3
        ///     Total N2O-N manure leaching emissions from all animal groups in component over all months
        ///     (kg N2O-N)
        /// </summary>
        public double TotalManureLeachingN2ONEmission
        {
            get
            {
                var result = 0d;

                foreach (var animalGroupEmissionResult in EmissionResultsForAllAnimalGroupsInComponent)
                    result += animalGroupEmissionResult.GroupEmissionsByMonths.Sum(x =>
                        x.MonthlyManureLeachingN2ONEmission);

                return result;
            }
        }

        /// <summary>
        ///     Total N2O manure leaching emissions from all animal groups in component over all months
        ///     Equation 3.1.6-5
        ///     Equation 3.2.8-5
        ///     Equation 3.3.4-5
        ///     (kg N2O)
        /// </summary>
        public double TotalManureLeachingN2OEmission
        {
            get
            {
                var result = 0d;

                foreach (var animalGroupEmissionResult in EmissionResultsForAllAnimalGroupsInComponent)
                    result += animalGroupEmissionResult.GroupEmissionsByMonths.Sum(x =>
                        x.MonthlyManureLeachingN2OEmission);

                return result;
            }
        }

        /// <summary>
        ///     Equation 5.4.2-4
        ///     Total manure indirect N2O-N emissions from all animal groups in component over all months
        ///     (kg N2O-N)
        /// </summary>
        public double TotalManureIndirectN2ONEmission =>
            TotalManureVolatilizationN2ONEmission + TotalManureLeachingN2ONEmission;

        /// <summary>
        ///     Total manure indirect N2O emissions from all animal groups in component over all months
        ///     (kg N2O)
        /// </summary>
        public double TotalManureIndirectN2OEmission =>
            TotalManureVolatilizationN2OEmission + TotalManureLeachingN2OEmission;

        /// <summary>
        ///     Equation 5.4.2-5
        ///     Total N2O-N manure emissions from all animal groups in component over all months
        ///     (kg N2O-N)
        /// </summary>
        public double TotalManureN2ONEmission => TotalManureDirectN2ONEmission + TotalManureIndirectN2ONEmission;

        /// <summary>
        ///     Total N2O manure emissions from all animal groups in component over all months
        ///     (kg N2O)
        /// </summary>
        public double TotalManureN2OEmission => TotalManureDirectN2OEmission + TotalManureIndirectN2OEmission;

        /// <summary>
        ///     Total energy CO2 from all animals
        ///     (kg CO2)
        /// </summary>
        public double TotalEnergyCO2Emissions
        {
            get
            {
                var result = 0d;

                foreach (var animalGroupEmissionResult in EmissionResultsForAllAnimalGroupsInComponent)
                    result += animalGroupEmissionResult.GroupEmissionsByMonths.Sum(x => x.MonthlyEnergyCarbonDioxide);

                return result;
            }
        }

        /// <summary>
        ///     Total manure ammonia emissions from housing from all animal groups in component over all months
        ///     (kg NH3)
        /// </summary>
        public double TotalAmmoniaEmissionsFromHousingSystem
        {
            get
            {
                var result = 0d;

                foreach (var animalGroupEmissionResult in EmissionResultsForAllAnimalGroupsInComponent)
                    result += animalGroupEmissionResult.GroupEmissionsByMonths.Sum(x =>
                        x.TotalAmmoniaEmissionsFromHousingSystem);

                return result;
            }
        }

        /// <summary>
        ///     Total manure ammonia emissions from storage from all animal groups in component over all months
        ///     (kg NH3)
        /// </summary>
        public double TotalAmmoniaEmissionsFromStorageSystem
        {
            get
            {
                var result = 0d;

                foreach (var animalGroupEmissionResult in EmissionResultsForAllAnimalGroupsInComponent)
                    result += animalGroupEmissionResult.GroupEmissionsByMonths.Sum(x =>
                        x.TotalAmmoniaEmissionsFromStorageSystem);

                return result;
            }
        }

        /// <summary>
        ///     (kg NH3)
        /// </summary>
        public double TotalAmmoniaEmissions =>
            TotalAmmoniaEmissionsFromHousingSystem + TotalAmmoniaEmissionsFromStorageSystem;

        /// <summary>
        ///     Total CO2 emissions from all animal groups in the component over all months. Note: for animals, the total CO2 is
        ///     equal to the total energy CO2
        ///     (kg CO2)
        /// </summary>
        public double TotalCarbonDioxide
        {
            get
            {
                var result = 0d;

                foreach (var groupEmissionResults in EmissionResultsForAllAnimalGroupsInComponent)
                    result += groupEmissionResults.TotalCarbonDioxideFromAnimals;

                return result;
            }
        }

        /// <summary>
        ///     Total N20_directmanure
        ///     (kg N2O)
        /// </summary>
        public double TotalDirectNitrousOxideEmission => TotalManureDirectN2OEmission;

        /// <summary>
        ///     Total N20_indirectmanure
        ///     (kg N2O)
        /// </summary>
        public double TotalIndirectN2OEmission => TotalManureIndirectN2OEmission;

        /// <summary>
        ///     Total N20_manure
        ///     Equation 3.1.7-3
        ///     (kg N2O)
        /// </summary>
        public double TotalNitrousOxideEmission => TotalManureN2OEmission;

        /// <summary>
        ///     (kg C)
        /// </summary>
        public double TotalAmountOfCarbonInStoredManure
        {
            get
            {
                var result = 0d;

                foreach (var animalGroupEmissionResult in EmissionResultsForAllAnimalGroupsInComponent)
                    result += animalGroupEmissionResult.GroupEmissionsByMonths.Sum(x =>
                        x.TotalAmountOfCarbonInStoredManureAtEndOfMonth);

                return result;
            }
        }

        /// <summary>
        ///     TAN available for land application
        ///     (kg TAN)
        /// </summary>
        public double TotalTANAvailableForLandApplication
        {
            get
            {
                var result = 0d;

                foreach (var animalGroupEmissionResult in EmissionResultsForAllAnimalGroupsInComponent)
                    result += animalGroupEmissionResult.GroupEmissionsByMonths.Sum(x =>
                        x.TotalTANAvailableForLandApplicationAtEndOfMonth);

                return result;
            }
        }

        /// <summary>
        ///     (kg N)
        /// </summary>
        public double TotalOrganicNitrogenAvailableForLandApplication
        {
            get
            {
                var result = 0d;

                foreach (var animalGroupEmissionResult in EmissionResultsForAllAnimalGroupsInComponent)
                    result += animalGroupEmissionResult.GroupEmissionsByMonths.Sum(x =>
                        x.TotalOrganicNitrogenAvailableForLandApplicationAtEndOfMonth);

                return result;
            }
        }

        /// <summary>
        ///     (1000 kg wet weight for solid manure, 1000 L for liquid manure)
        /// </summary>
        public double TotalVolumeOfManureAvailableForLandApplication
        {
            get
            {
                var result = 0d;

                foreach (var animalGroupEmissionResult in EmissionResultsForAllAnimalGroupsInComponent)
                    result += animalGroupEmissionResult.GroupEmissionsByMonths.Sum(x =>
                        x.TotalVolumeOfManureAvailableForLandApplication);

                return result;
            }
        }

        /// <summary>
        ///     (kg)
        /// </summary>
        public double TotalMilkProduced
        {
            get
            {
                var result = 0d;

                foreach (var animalGroupEmissionResult in EmissionResultsForAllAnimalGroupsInComponent)
                    result += animalGroupEmissionResult.GroupEmissionsByMonths.Sum(x => x.MonthlyMilkProduction);

                return result;
            }
        }

        /// <summary>
        ///     (kg)
        /// </summary>
        public double TotalBeefProduced
        {
            get
            {
                var result = 0d;

                foreach (var animalGroupEmissionResult in EmissionResultsForAllAnimalGroupsInComponent)
                    result += animalGroupEmissionResult.GroupEmissionsByMonths.Sum(x => x.MonthlyBeefProduced);

                return result;
            }
        }

        /// <summary>
        ///     FPCM
        /// </summary>
        public double TotalFatAndProteinCorrectedMilkProduced
        {
            get
            {
                var result = 0d;

                foreach (var animalGroupEmissionResult in EmissionResultsForAllAnimalGroupsInComponent)
                    result += animalGroupEmissionResult.GroupEmissionsByMonths.Sum(x =>
                        x.MonthlyFatAndProteinCorrectedMilkProduction);

                return result;
            }
        }

        /// <summary>
        ///     (kg N)
        /// </summary>
        public double TotalAvailableManureNitrogenAvailableForLandApplication
        {
            get
            {
                var result = 0d;

                foreach (var animalGroupEmissionResult in EmissionResultsForAllAnimalGroupsInComponent)
                    result += animalGroupEmissionResult.GroupEmissionsByMonths.Sum(x =>
                        x.TotalAmountOfNitrogenInStoredManureAvailableForMonth);

                return result;
            }
        }

        /// <summary>
        ///     (kg CH4 year^-1)
        /// </summary>
        public double TotalEntericAndManureMethane => TotalEntericMethaneEmission + TotalManureMethaneEmission;

        /// <summary>
        ///     (kg N2O year^-1)
        /// </summary>
        public double TotalNitrousOxide => TotalDirectNitrousOxideEmission + TotalIndirectN2OEmission;

        /// <summary>
        ///     (kWh year^-1)
        /// </summary>
        public double TotalElectricityProducedFromAnaerobicDigestion
        {
            get
            {
                var result = 0d;

                foreach (var animalGroupEmissionResult in EmissionResultsForAllAnimalGroupsInComponent)
                    result += animalGroupEmissionResult.GroupEmissionsByMonths.Sum(x =>
                        x.MonthlyElectricityProducedFromAnaerobicDigestion);

                return result;
            }
        }

        /// <summary>
        ///     (kWh year^-1)
        /// </summary>
        public double TotalHeatProducedFromAnaerobicDigestion
        {
            get
            {
                var result = 0d;

                foreach (var animalGroupEmissionResult in EmissionResultsForAllAnimalGroupsInComponent)
                    result += animalGroupEmissionResult.GroupEmissionsByMonths.Sum(x =>
                        x.MonthlyHeatProducedFromAnaerobicDigestion);

                return result;
            }
        }

        /// <summary>
        ///     (kWh year^-1)
        /// </summary>
        public double TotalMethaneInjectedIntoGridFromAnaerobicDigestion
        {
            get
            {
                var result = 0d;

                foreach (var animalGroupEmissionResult in EmissionResultsForAllAnimalGroupsInComponent)
                    result += animalGroupEmissionResult.GroupEmissionsByMonths.Sum(x =>
                        x.MonthlyMethaneInjectionIntoGrid);

                return result;
            }
        }

        /// <summary>
        ///     Equation 4.6.2-8
        ///     (kg NH3-N)
        /// </summary>
        public double TotalAmmoniacalNitrogenFromLandAppliedManureForComponent
        {
            get
            {
                var result = 0d;

                foreach (var animalGroupEmissionResult in EmissionResultsForAllAnimalGroupsInComponent)
                    result += animalGroupEmissionResult.GroupEmissionsByMonths.Sum(x =>
                        x.TotalAmmoniacalNitrogenFromLandAppliedManure);

                return result;
            }
        }

        /// <summary>
        ///     Equation 4.6.2-9
        ///     (kg NH3)
        /// </summary>
        public double TotalAmmoniaFromLandAppliedManureForComponent
        {
            get
            {
                var result = 0d;

                foreach (var animalGroupEmissionResult in EmissionResultsForAllAnimalGroupsInComponent)
                    result += animalGroupEmissionResult.GroupEmissionsByMonths.Sum(x =>
                        x.TotalAmmoniaFromLandAppliedManure);

                return result;
            }
        }

        /// <summary>
        ///     Equation 4.6.4-2
        ///     (kg N2O-N)
        /// </summary>
        public double TotalN2ONLeachingEmissionsFromLandAppliedManureForComponent
        {
            get
            {
                var result = 0d;

                foreach (var animalGroupEmissionResult in EmissionResultsForAllAnimalGroupsInComponent)
                    result += animalGroupEmissionResult.GroupEmissionsByMonths.Sum(x =>
                        x.TotalMonthlyLeachingEmissionsFromLandAppliedManure);

                return result;
            }
        }

        /// <summary>
        ///     Equation 4.6.4-3
        ///     (kg N2O)
        /// </summary>
        public double TotalN2OLeachingEmissionsFromLandAppliedManureForComponent
        {
            get
            {
                var result = 0d;

                foreach (var animalGroupEmissionResult in EmissionResultsForAllAnimalGroupsInComponent)
                    result += animalGroupEmissionResult.GroupEmissionsByMonths.Sum(x =>
                        x.TotalMonthlyN2OLeachingEmissionsFromLandAppliedManureFromLandAppliedManure);

                return result;
            }
        }

        /// <summary>
        ///     The farm which created the emissions.
        /// </summary>
        public Farm Farm { get; set; }

        /// <summary>
        ///     The animal component which created the emissions.
        /// </summary>
        public ComponentBase Component { get; set; }

        /// <summary>
        ///     A collection of <see cref="AnimalGroupEmissionResults" />. There will be one instance for each animal group in the
        ///     component.
        /// </summary>
        public List<AnimalGroupEmissionResults> EmissionResultsForAllAnimalGroupsInComponent { get; set; } =
            new List<AnimalGroupEmissionResults>();

        #endregion
    }
}