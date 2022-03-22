using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Models.Animals;

namespace H.Core.Calculators.Infrastructure
{
    public class ADCalculator
    {
        #region Fields
        #endregion

        #region Constructors
        #endregion

        #region Properties
        #endregion

        #region Public Methods

        public void CalculateFlowsFromNonFreshManure(Farm farm,
            List<AnimalComponentEmissionsResults> animalComponentEmissionsResults)
        {
            var component = farm.Components.OfType<AnaerobicDigestionComponent>().SingleOrDefault();
            if (component == null)
            {
                // This farm doesn't have an AD
                return;
            }

            // Go over all results and get group emissions by day that have AD set as the manure handling system
            foreach (var animalComponentEmissionsResult in animalComponentEmissionsResults)
            {
                foreach (var animalGroupEmissionResults in animalComponentEmissionsResult.EmissionResultsForAllAnimalGroupsInComponent)
                {
                   
                }
            }


            // Get a list of the manure substrates the user inputs into the system
            var manureSubstrates = component.AnaerobicDigestionViewItem.ManureSubstrateViewItems.ToList();

            var totalFlowRateOfStoredManureEnteringSystem = this.CalculateFlowOfTotalMassOfStoredManureEnteringDigester(manureSubstrates.Select(x => x.FlowRate));

            // TODO: need to get this value from only the components the user has specified will be used for stored manure inputs
            var totalManureAvailableForLandApplication = animalComponentEmissionsResults.TotalVolumeOfManureAvailableForLandApplication();
            var flowOfStoredManure = this.CalculateTotalFlowFromStoredManure(
                totalVolumeOfLandManure: totalManureAvailableForLandApplication,
                proportion: 1); // TODO

            // There may be multiple entries for the same type of substrate (i.e. 2 entries for beef cattle manure), so we group by residue type.
            var manureSubstratesGroupedByAnimalType = component.AnaerobicDigestionViewItem.ManureSubstrateViewItems.GroupBy(x => x.AnimalType).ToList();

            // A dictionary of calculated volatile solids for each type of stored (non-fresh) animal manure (allows for detailed reporting by residue type).
            var totalSolidsFlowByType = new Dictionary<AnimalType, double>();
            var volatileSolidsFlowByType = new Dictionary<AnimalType, double>();
            var nitrogenFlowByType = new Dictionary<AnimalType, double>();
            var carbonFlowByType = new Dictionary<AnimalType, double>();

            /*
             * When the user is specifying non-fresh manure, they need to tell us where it is coming from (i.e. which component, which group, and which management period). This
             * is the only way we can access the number of animals daily emissions etc.
             */
            var managementPeriod = new ManagementPeriod();

            // Go over all the different groups of residue types and get the total TS, VS, N, etc. for each type of residue
            foreach (var substrateGroupedByAnimalType in manureSubstratesGroupedByAnimalType)
            {
                var currentAnimalManureType = substrateGroupedByAnimalType.Key;

                var totalSolidsByType = 0.0;
                var volatileSolidsByType = 0.0;
                var nitrogenByType = 0.0;
                var carbonByType = 0.0;

                // All items in this loop have the same residue type
                foreach (var animalManureSubstrate in substrateGroupedByAnimalType)
                {
                    totalSolidsByType += this.CalculateFlowOfVSEnteringDigesterFromStoredManure(
                        flowRate: animalManureSubstrate.FlowRate,
                        totalSolidsConcentration: animalManureSubstrate.TotalSolids);

                    // Need VS rate from liquid manure

                    volatileSolidsByType += this.CalculateVolatileSolidsFlowFromStoredManure(
                        volatileSolids: animalManureSubstrate.VolatileSolids,
                        reductionFactor: 1, // TODO
                        numberOfAnimals: managementPeriod.NumberOfAnimals,
                        1);// TODO
                }
            }
        }

        public void CalculateFlowsFromFarmSubstrates(Farm farm, List<AnimalComponentEmissionsResults> animalComponentEmissionsResults)
        {
            var component = farm.Components.OfType<AnaerobicDigestionComponent>().SingleOrDefault();
            if (component == null)
            {
                // This farm doesn't have an AD
                return;
            }

            // Get a list of the crop residues/farm residues the user inputs into the system
            var residueSubstrates = component.AnaerobicDigestionViewItem.FarmResiduesSubstrateViewItems.ToList();

            // TODO: Add flow rate column to farm residue grid view. Also, unit attributes/comments needed for FarmResidueSubstrateViewItem class.
            var flowRateOfResidueSubstrates = this.CalculateFlowOfTotalMassOfCropResidueEnteringDigester(residueSubstrates.Select(x => x.FlowRate));

            // There may be multiple entries for the same type of farm residue (i.e. 2 entries for OatStraw), so we group by residue type.
            var farmResidueSubstratesGroupedByType = component.AnaerobicDigestionViewItem.FarmResiduesSubstrateViewItems.GroupBy(x => x.FarmResidueType).ToList();

            // A dictionary of calculated volatile solids for each type of farm crop residue (allows for detailed reporting by residue type).
            var totalSolidsFlowByType = new Dictionary<FarmResidueType, double>();
            var volatileSolidsFlowByType = new Dictionary<FarmResidueType, double>();
            var nitrogenFlowByType = new Dictionary<FarmResidueType, double>();
            var carbonFlowByType = new Dictionary<FarmResidueType, double>();

            // Go over all the different groups of residue types and get the total TS, VS, N, etc. for each type of residue
            foreach (var substratesGroupedByType in farmResidueSubstratesGroupedByType)
            {
                var currentResidueType = substratesGroupedByType.Key;

                var totalSolidsByType = 0.0;
                var volatileSolidsByType = 0.0;
                var nitrogenByType = 0.0;
                var carbonByType = 0.0;

                // All items in this loop have the same residue type
                foreach (var substrateViewItem in substratesGroupedByType)
                {
                    totalSolidsByType += this.CalculateFlowOfCropResidueTotalSolidsEnteringDigester(
                    flowRateOfSubstrateEnteringDigester: substrateViewItem.FlowRate,
                    totalSolidsConcentrationOfSubstrate: substrateViewItem.TotalSolids);

                    volatileSolidsByType += this.CalculateFlowOfCropResidueVolatileSolidsEnteringDigester(
                    flowRateOfSubstrateEnteringDigester: substrateViewItem.FlowRate,
                    volatileSolidsConcentrationOfSubstrate: substrateViewItem.VolatileSolids);

                    nitrogenByType += this.CalculateFlowOfCropResidueNitrogenEnteringDigester(
                    flowRateOfSubstrateEnteringDigester: substrateViewItem.FlowRate,
                    nitrogenConcentrationOfSubstrate: substrateViewItem.TotalNitrogen);

                    carbonByType += this.CalculateFlowOfTotalCarbonEnteringDigesterCropResidue(
                    flowRateOfSubstrateEnteringDigester: substrateViewItem.FlowRate,
                    carbonConcentrationOfSubstrate: substrateViewItem.TotalCarbon); // TODO: Add column for Carbon to farm residue grid view but set visibility to collapsed since we don't have default values yet.
                }

                // Local sums for this type of residue
                totalSolidsFlowByType.Add(currentResidueType, totalSolidsByType);
                volatileSolidsFlowByType.Add(currentResidueType, volatileSolidsByType);
                nitrogenFlowByType.Add(currentResidueType, nitrogenByType);
                carbonFlowByType.Add(currentResidueType, carbonByType);
            }

            // These are the total sums for all substrates entered into system                          
            var totalSolidsFlowFromAllResidues = totalSolidsFlowByType.Sum(x => x.Value);
            var volatileSolidsFlowFromAllResidues = volatileSolidsFlowByType.Sum(x => x.Value);
            var nitrogenFlowFromAllResidues = nitrogenFlowByType.Sum(x => x.Value);
            var carbonFlowRateFromAllResidues = carbonFlowByType.Sum(x => x.Value);
        }

        public void CaculateResults(Farm farm, List<AnimalComponentEmissionsResults> animalComponentEmissionsResults)
        {
        }

        #endregion

        #region Private Methods
        #endregion

        #region Equations

        #region 4.9.1 For Fresh/raw Livestock Manure Entering The Digester

        /// <summary>
        /// Eq. 4.9.1-2
        /// </summary>
        public double CalculateTotalFreshMassEnteringDigester(
            double totalVolumeOfLandManure,
            double propotion)
        {
            return totalVolumeOfLandManure * propotion;
        }

        /// <summary>
        /// Eq. 4.9.1-3
        /// </summary>
        public double CalculateTotalSolidsEnteringDigester(
            double totalMassFlowRate,
            double concentration)
        {
            return totalMassFlowRate * concentration;
        }

        /// <summary>
        /// Eq. 4.9.1-4
        /// </summary>
        /// <param name="volatileSolids">Volatile solids (kg head^-1 day^-1). </param>
        /// <param name="numberOfAnimals">Number of animals</param>
        /// <param name="proportionTotalManureAddedToAD">Proportion of total manure produced added to the AD system</param>
        /// <returns>Flow rate of volatile solids in substrate entering the digester (kg day-1)</returns>
        public double CalculateFlowOfVolatileSolidsEnteringDigesterFromFreshManure(double volatileSolids,
            double numberOfAnimals,
            double proportionTotalManureAddedToAD)
        {
            return volatileSolids * numberOfAnimals * proportionTotalManureAddedToAD;
        }

        /// <summary>
        /// Eq. 4.9.1-5
        /// </summary>
        /// <param name="totalNitrogenExcreted">Total amount of N excreted (kg N day-1)</param>
        /// <param name="totalNitrogenAddedFromBeddingMaterial">Total amount of N added from bedding materials (kg N day-1)</param>
        /// <param name="proportionTotalManureAddedToAD">Proportion of total manure produced added to the AD system</param>
        /// <returns>Flow rate of total N in substrate entering the digester (kg day-1)</returns>
        public double CalculateFlowOfTotalNitrogenEnteringDigesterFromFreshManure(double totalNitrogenExcreted,
                                                                   double totalNitrogenAddedFromBeddingMaterial,
                                                                   double proportionTotalManureAddedToAD)
        {
            return (totalNitrogenExcreted + totalNitrogenAddedFromBeddingMaterial) * proportionTotalManureAddedToAD;
        }

        /// <summary>
        /// Eq. 4.9.1-6
        /// </summary>
        /// <param name="dailyOrganicNitrogenInStoredManure">Flow rate of organic N in substrate entering the digester (kg day-1)</param>
        /// <param name="proportionTotalManureAddedToAD">Proportion of total manure produced added to the AD system</param>
        /// <returns>Daily organic N in stored manure (kg N day-1) for beef and dairy cattle.</returns>
        public double CalculateFlowOfOrganicNEnteringDigesterBeefDairyFreshManure(double dailyOrganicNitrogenInStoredManure,
                                                                     double proportionTotalManureAddedToAD)
        {
            return dailyOrganicNitrogenInStoredManure * proportionTotalManureAddedToAD;
        }

        /// <summary>
        /// Eq. 4.9.1-7
        /// </summary>
        /// <param name="totalNitrogenExcreted">Total amount of N excreted (kg N day-1)</param>
        /// <param name="totalAmmonicalNitrogenExcretionRate">Total ammonical N (TAN) excretion rate (kg TAN head-1 day-1). For broilers, layers and turkeys, default TANexcretion_rate values are used </param>
        /// <param name="numberOfAnimals">Number of animals (broilers, layers or turkeys)</param>
        /// <param name="proportionTotalManureAddedToAD">Proportion of total manure produced added to the AD system</param>
        /// <returns>Daily organic N in stored manure (kg N day-1) for Poultry</returns>
        public double CalculateFlowOfOrganicNEnteringDigesterPoultryFreshManure(double totalNitrogenExcreted,
                                                                     double totalAmmonicalNitrogenExcretionRate,
                                                                     double numberOfAnimals,
                                                                     double proportionTotalManureAddedToAD)
        {
            return (totalNitrogenExcreted - (totalAmmonicalNitrogenExcretionRate * numberOfAnimals)) * proportionTotalManureAddedToAD;
        }

        /// <summary>
        /// Eq. 4.9.1-8
        /// </summary>
        /// <param name="totalAmmonicalNitrogenExcretionRate">Total ammonical N (TAN) excretion rate (kg TAN head-1 day-1).</param>
        /// <param name="numberOfAnimals">Number of animals</param>
        /// <param name="proportionTotalManureAddedToAD">Proportion of total manure produced added to the AD system</param>
        /// <returns>Flow rate of TAN in substrate entering the digester (kg day-1) for beef cattle, dairy cattle, broilers, layers and turkeys. </returns>
        public double CalculateFlowOfTANEnteringDigesterFreshManure(double totalAmmonicalNitrogenExcretionRate,
                                                         double numberOfAnimals,
                                                         double proportionTotalManureAddedToAD)
        {
            return totalAmmonicalNitrogenExcretionRate * numberOfAnimals * proportionTotalManureAddedToAD;
        }

        /// <summary>
        /// Eq. 4.9.1-9
        /// </summary>
        /// <param name="totalCarbonAddedInManure">Total amount of C added in manure (including bedding) (kg N day-1)</param>
        /// <param name="proportionTotalManureAddedToAD">Proportion of total manure produced added to the AD system</param>
        /// <returns>Flow rate of total C in substrate entering the digester (kg day^-1)</returns>
        public double CalculateFlowOfTotalCarbonEnteringDigesterFreshManure(double totalCarbonAddedInManure,
                                                                 double proportionTotalManureAddedToAD)
        {
            return totalCarbonAddedInManure * proportionTotalManureAddedToAD;
        }

        #endregion

        #region 4.9.1 For Crop Residues Entering The Digester

        /// <summary>
        /// Eq 4.9.1-11
        /// </summary>
        /// <param name="flowRatesOfSubstrates">A collection of flow rate of substrate i entering the digester (kg day-1)</param>
        /// <returns>Total flow rate of substrate entering the digester (kg day-1)</returns>
        public double CalculateFlowOfTotalMassOfCropResidueEnteringDigester(IEnumerable<double> flowRatesOfSubstrates)
        {
            return flowRatesOfSubstrates.Sum();
        }

        /// <summary>
        /// Eq 4.9.1-8
        /// </summary>
        /// <param name="flowRateOfSubstrateEnteringDigester">Flow rate of substrate i entering the digester (kg day-1)</param>
        /// <param name="totalSolidsConcentrationOfSubstrate">Total solids concentration of substrate I (kg kg-1) </param>
        /// <returns>Flow rate of total solids entering the digester (kg day-1)</returns>
        public double CalculateFlowOfCropResidueTotalSolidsEnteringDigester(double flowRateOfSubstrateEnteringDigester,
                                                                 double totalSolidsConcentrationOfSubstrate)
        {

            return flowRateOfSubstrateEnteringDigester * totalSolidsConcentrationOfSubstrate;
        }

        /// <summary>
        /// Eq 4.8.1-9
        /// </summary>
        /// <param name="flowRateOfSubstrateEnteringDigester">Flow rate of substrate i entering the digester (kg day-1)</param>
        /// <param name="volatileSolidsConcentrationOfSubstrate">Flow rate of volatile solids in substrate entering the digester (kg day-1)</param>
        /// <returns>VS concentration of substrate i (kg kg-1) </returns>
        public double CalculateFlowOfCropResidueVolatileSolidsEnteringDigester(double flowRateOfSubstrateEnteringDigester,
                                                                    double volatileSolidsConcentrationOfSubstrate)
        {
            return flowRateOfSubstrateEnteringDigester * volatileSolidsConcentrationOfSubstrate;
        }

        /// <summary>
        /// Eq 4.8.1-10
        /// </summary>
        /// <param name="flowRateOfSubstrateEnteringDigester">Flow rate of substrate i entering the digester (kg day-1)</param>
        /// <param name="nitrogenConcentrationOfSubstrate">N concentration of substrate i (kg kg-1) </param>
        /// <returns>Flow rate of total N in substrate entering the digester (kg day-1)</returns>
        public double CalculateFlowOfCropResidueNitrogenEnteringDigester(double flowRateOfSubstrateEnteringDigester,
                                                                   double nitrogenConcentrationOfSubstrate)
        {
            return flowRateOfSubstrateEnteringDigester * nitrogenConcentrationOfSubstrate;
        }

        /// <summary>
        /// Eq 4.8.1-11
        /// </summary>
        /// <param name="flowRateOfSubstrateEnteringDigester">Flow rate of substrate i entering the digester (kg day-1)</param>
        /// <param name="organicNConcentrationOfSubstrate">Organic N concentration of substrate i (kg kg-1)</param>
        /// <returns>Daily organic N in stored manure (kg N day-1)</returns>
        public double CalculateFlowOfCropResidueOrganicNitrogenEnteringDigester(double flowRateOfSubstrateEnteringDigester,
                                                                     double organicNConcentrationOfSubstrate)
        {
            return flowRateOfSubstrateEnteringDigester * organicNConcentrationOfSubstrate;
        }

        /// <summary>
        /// Eq 4.8.1-12
        /// </summary>
        /// <param name="flowRateOfSubstrateEnteringDigester">Flow rate of substrate i entering the digester (kg day-1)</param>
        /// <param name="totalAmmonicalNitrogenConcentrationOfSubstrate">TAN concentration of substrate i (kg kg-1) </param>
        /// <returns>Flow rate of TAN in substrate entering the digester (kg day-1) </returns>
        public double CalculateFlowOfCropResidueTANEnteringDigester(double flowRateOfSubstrateEnteringDigester,
                                                         double totalAmmonicalNitrogenConcentrationOfSubstrate)
        {
            return flowRateOfSubstrateEnteringDigester * totalAmmonicalNitrogenConcentrationOfSubstrate;
        }

        /// <summary>
        /// Eq 4.8.1-13
        /// </summary>
        /// <param name="flowRateOfSubstrateEnteringDigester">Flow rate of substrate i entering the digester (kg day-1)</param>
        /// <param name="carbonConcentrationOfSubstrate">C concentration of substrate i (kg kg-1). A default value of 0.45 kg kg-1 is used for all crop residues;
        /// default values of XX, XX and XX are used for municipal sewage sludge, food waste and used vegetable oil, respectively</param>
        /// <returns>Flow rate of total C in substrate entering the digester </returns>
        public double CalculateFlowOfTotalCarbonEnteringDigesterCropResidue(double flowRateOfSubstrateEnteringDigester,
                                                                            double carbonConcentrationOfSubstrate)
        {

            return flowRateOfSubstrateEnteringDigester * carbonConcentrationOfSubstrate;
        }

        #endregion

        #region 4.8.1.3 For Livestock Manure Stored For A Period Of Time Prior To Entering The Digester

        /// <summary>
        /// Eq 4.9.1-15
        /// </summary>
        /// <param name="flowRatesOfSubstrates">A collection of flow rate of substrate i entering the digester (kg day-1)</param>
        /// <returns>Total flow rate of substrate entering the digester (kg day-1)</returns>
        public double CalculateFlowOfTotalMassOfStoredManureEnteringDigester(IEnumerable<double> flowRatesOfSubstrates)
        {
            return flowRatesOfSubstrates.Sum();
        }

        /// <summary>
        /// Eq. 4.9.1-16
        /// </summary>
        public double CalculateTotalFlowFromStoredManure(
            double totalVolumeOfLandManure,
            double proportion)
        {
            return totalVolumeOfLandManure * proportion;
        }

        /// <summary>
        /// Eq. 4.9.1-17
        /// </summary>
        public double CalculateTotalSolidsEnteringDigesterFromStoredManure(
            double totalMassFlowRate,
            double concentration)
        {
            return totalMassFlowRate * concentration;
        }

        /// <summary>
        /// Eq. 4.9.1-19
        /// </summary>
        /// <param name="volatileSolids">Volatile solids excreted (kg head-1 day-1)</param>
        /// <param name="reductionFactor">Fixed reduction in VS in stored solid manure entering the digester following a pre-digester storage period</param>
        /// <param name="numberOfAnimals">Number of animals</param>
        /// <param name="proportionTotalManureAddedToAD">Proportion of total manure produced added to the AD system</param>
        /// <returns>Flow rate of volatile solids in stored solid manure entering the digester from previously stored solid manure (kg day-1)</returns>
        public double CalculateVolatileSolidsFlowFromStoredManure(
            double volatileSolids,
            double reductionFactor,
            double numberOfAnimals,
            double proportionTotalManureAddedToAD)
        {
            return  volatileSolids * (1 - reductionFactor) * numberOfAnimals * proportionTotalManureAddedToAD;
        }

        /// <summary>
        /// Eq 4.9.1-18
        /// </summary>
        /// <param name="sumVolatileSolidsLoaded">Sum of volatile solids (kg) into storage system across all days of pre-digester storage period</param>
        /// <param name="sumVolatileSolidsConsumed">Sum of volatile solids (kg) consumed across all days of pre-digester storage period.</param>
        /// <param name="proportionTotalManureAddedToAD">Proportion of total manure produced added to the AD system</param>
        /// <returns>Flow rate of volatile solids in substrate entering the digester (kg day-1)</returns>
        public double CalculateVolatileSolidsFlowFromStoredLiquidManure(double sumVolatileSolidsLoaded,
                                                                              double sumVolatileSolidsConsumed,
                                                                              double proportionTotalManureAddedToAD)
        {
            return (sumVolatileSolidsLoaded - sumVolatileSolidsConsumed) * proportionTotalManureAddedToAD;
        }

        /// <summary>
        /// Eq 4.9.1-17
        /// </summary>
        /// <param name="flowRate"></param>
        /// <param name="totalSolidsConcentration"></param>
        /// <returns></returns>
        public double CalculateFlowOfVSEnteringDigesterFromStoredManure(
            double flowRate,
            double totalSolidsConcentration)
        {
            return flowRate * totalSolidsConcentration;
        }

        /// <summary>
        /// Eq 4.8.1-15
        /// </summary>
        /// <param name="sumVolatileSolidsLoaded">Sum of volatile solids (kg) into storage system across all days of pre-digester storage period. Daily VSloaded is the same as daily VS (kg VS produced head-1 day-1)</param>
        /// <param name="sumVolatileSolidsConsumed">Sum of volatile solids (kg) consumed across all days of pre-digester storage period</param>
        /// <param name="proportionTotalManureAddedToAD">Proportion of total manure produced added to the AD system</param>
        /// <returns>Flow rate of volatile solids in substrate entering the digester (kg day-1)</returns>
        public double CalculateFlowOfVSEnteringDigesterFromSolidManureSystem(double sumVolatileSolidsLoaded,
                                                                             double sumVolatileSolidsConsumed,
                                                                             double proportionTotalManureAddedToAD)
        {
            return (sumVolatileSolidsLoaded - sumVolatileSolidsConsumed) * proportionTotalManureAddedToAD;
        }

        /// <summary>
        /// Eq 4.9.1-20
        /// </summary>
        /// <param name="nitrogenAvailalbleForLandApplication"></param>
        /// <param name="proportion"></param>
        /// <returns>Flow rate of total N in substrate entering the digester (kg day-1)</returns>
        public double CalculateFlowRateOfTotalNitrogenEnteringDigesterFromStoredManure(double nitrogenAvailalbleForLandApplication,
            double proportion)
        {
            return nitrogenAvailalbleForLandApplication * proportion;
        }

        /// <summary>
        /// Eq 4.9.1-21
        /// </summary>
        /// <param name="organicNitrogenAvailableForLandApplication"></param>
        /// <param name="proportion"></param>
        /// <returns></returns>
        public double CalculateFlowOfOrganicNitrogenFromStoredManureBeefDairy(
            double organicNitrogenAvailableForLandApplication,
            double proportion)
        {
            return organicNitrogenAvailableForLandApplication * proportion;
        }

        /// <summary>
        /// Eq 4.9.1-22
        /// </summary>
        /// <returns></returns>
        public double CalculateFlowOfOrganicNitrogenFromStoredPoultryManure(
            double nitrogenFlowFromSubstrate,
            double tanFlowFromSubstrate)
        {
            return nitrogenFlowFromSubstrate - tanFlowFromSubstrate;
        }

        /// <summary>
        /// Equation 4.9.1-23
        /// </summary>
        /// <param name="tanAvailableForLandApplication"></param>
        /// <param name="proportion"></param>
        /// <returns></returns>
        public double CalculateFlowOfTanFromStoredManureBeefDairy(
            double tanAvailableForLandApplication,
            double proportion)
        {
            return tanAvailableForLandApplication * proportion;
        }

        /// <summary>
        /// Eq 4.9.1-24
        /// </summary>
        /// <param name="totalAmmonicalNitrogenExcretionRate">Total ammonical N (TAN) excretion rate (kg TAN head-1 day-1).</param>
        /// <param name="indirectNLossesFromManureInHousingNH3">Indirect N losses from manure in housing via NH3 volatilization during the pre-AD stage (kg NH3-N day-1)</param>
        /// <param name="indirectNLossesFromManureInStorageNH3">Indirect N losses from manure in storage via NH3 volatilization during the pre-AD stage. </param>
        /// <param name="numberOfAnimals">The total number of animals.</param>
        /// <param name="proportionTotalManureAddedToAD">Proportion of total manure produced added to the AD system</param>
        /// <returns>Flow rate of TAN in substrate entering the digester (kg day-1) for beef cattle, dairy cattle, broilers, layers and turkeys.</returns>
        public double CalculateFlowOfTANEnteringDigesterFromStoredPoultryManure(double totalAmmonicalNitrogenExcretionRate,
            double indirectNLossesFromManureInHousingNH3,
            double indirectNLossesFromManureInStorageNH3,
            double numberOfAnimals,
            double proportionTotalManureAddedToAD)
        {
            return (totalAmmonicalNitrogenExcretionRate * numberOfAnimals -
                    (indirectNLossesFromManureInHousingNH3 + indirectNLossesFromManureInStorageNH3)) * proportionTotalManureAddedToAD;
        }

        /// <summary>
        /// Eq 4.8.1-17
        /// </summary>
        /// <param name="totalNitrogenExcreted">Total amount of N excreted (kg N day-1)</param>
        /// <param name="volatilizationFraction">Volatilization fraction, default values for sheep, swine, poultry, and other livestock groups are used (Table 36)</param>
        /// <returns>Indirect N losses from manure in housing via NH3 volatilization during the pre-AD storage stage (kg NH3-N day-1) for sheep, swine, ducks and geese, goats, llamas and alpacas, deer and elk, horses, mules and bison</returns>
        public double CalculateIndirectNLossesFromManureInHousingViaNH3(double totalNitrogenExcreted,
                                                                        double volatilizationFraction)
        {
            return totalNitrogenExcreted * volatilizationFraction;
        }

        /// <summary>
        /// Eq 4.8.1-18
        /// </summary>
        /// <param name="dailyOrganicNitrogenInStoredManure">Daily organic N in stored manure (kg N day-1)</param>
        /// <param name="directNitrousOxideToNitrogenEmissions">Direct N2O-N emissions from manure during the pre-AD stage (kg N2O-N day-1)</param>
        /// <param name="indirectNLossesFromManureInHousingNH3">Indirect N losses from manure in housing via NH3 volatilization during the pre-AD stage (kg NH3-N day-1)</param>
        /// <param name="indirectNLossesFromManureInStorageNH3">Indirect N losses from manure in storage via NH3 volatilization during the pre-AD stage.</param>
        /// <param name="indirectNLossesFromManureInHousingLeaching"></param>
        /// <param name="proportionTotalManureAddedToAD">Proportion of total manure produced added to the AD system</param>
        /// <returns>Flow rate of organic N in substrate entering the digester (kg day-1)</returns>
        public double CalculateFlowOrganicNitrogenEnteringDigesterBeefDairyCattle(double dailyOrganicNitrogenInStoredManure,
                                                                                  double directNitrousOxideToNitrogenEmissions,
                                                                                  double indirectNLossesFromManureInHousingNH3,
                                                                                  double indirectNLossesFromManureInStorageNH3,
                                                                                  double indirectNLossesFromManureInHousingLeaching,
                                                                                  double proportionTotalManureAddedToAD)

        {
            return (dailyOrganicNitrogenInStoredManure -
                    ((directNitrousOxideToNitrogenEmissions + indirectNLossesFromManureInHousingNH3 + indirectNLossesFromManureInStorageNH3 + indirectNLossesFromManureInHousingLeaching) -
                    (indirectNLossesFromManureInHousingNH3 + indirectNLossesFromManureInStorageNH3))) * proportionTotalManureAddedToAD;
        }

        /// <summary>
        /// Eq 4.8.1-19
        /// </summary>
        /// <param name="totalNitrogenExcreted">Total amount of N excreted (kg N day-1)</param>
        /// <param name="totalAmmonicalNitrogenExcretionRate">Total ammonical N (TAN) excretion rate (kg TAN head-1 day-1). For broilers, layers and turkeys, default TANexcretion_rate values are used</param>
        /// <param name="numberOfAnimals">Number of animals (broilers, layers or turkeys)</param>
        /// <param name="proportionTotalManureAddedToAD">Proportion of total manure produced added to the AD system</param>
        /// <returns>Flow rate of organic N in substrate entering the digester (kg day-1)</returns>
        public double CalculateFlowOrganicNitrogenEnteringDigesterPoultry(double totalNitrogenExcreted,
                                                                          double totalAmmonicalNitrogenExcretionRate,
                                                                          double numberOfAnimals,
                                                                          double proportionTotalManureAddedToAD)
        {
            return (totalNitrogenExcreted - (totalAmmonicalNitrogenExcretionRate * numberOfAnimals)) * proportionTotalManureAddedToAD;
        }



        /// <summary>
        /// Eq 4.8.1-21
        /// </summary>
        /// <param name="totalCarbonInManure">Total amount of C added in manure (including bedding) (kg N day-1)</param>
        /// <param name="carbonLostAsCH4">Carbon lost as methane during manure management prior to entering the digester </param>
        /// <param name="proportionTotalManureAddedToAD">Proportion of total manure produced added to the AD system</param>
        /// <returns>Flow rate of total C in substrate entering the digester (kg day-1)</returns>
        public double CalculateFlowOfTotalCarbonEnteringDigester(double totalCarbonInManure,
                                                                 double carbonLostAsCH4,
                                                                 double proportionTotalManureAddedToAD)
        {
            return (totalCarbonInManure - carbonLostAsCH4) * proportionTotalManureAddedToAD;
        }

        #endregion

        #region 4.8.2.1 Flow Of Biodegradable Volatile Solids And Methane Potential

        /// <summary>
        /// Eq 4.8.2-1
        /// </summary>
        /// <param name="flowRatesOfSubstrateEnteringDigester">Amounts of substrate i entering the digester (kg day-1). The equations sums all these values together to calculate the final result.</param>
        /// <param name="biodegradeableFractionOfVSForSubstrate">Biodegradable fraction of VS for substrate i</param>
        /// <returns>Flow rate of biodegradable volatile solids (kg day-1)</returns>
        public double CalculateFlowRateBiodegradeableVolatileSolids(IEnumerable<double> flowRatesOfSubstrateEnteringDigester,
                                                                    double biodegradeableFractionOfVSForSubstrate)
        {
            var sumFlowRatesEnteringDigester = flowRatesOfSubstrateEnteringDigester.Sum();
            return sumFlowRatesEnteringDigester * biodegradeableFractionOfVSForSubstrate;
        }

        /// <summary>
        /// Eq 4.8.2-2
        /// </summary>
        /// <param name="flowRatesOfDegradedVSDuringDigestion">Flow rate of biodegradable volatile solids. The equations sums all these values together to calculate the final result.</param>
        /// <param name="biomethanePotentialOfSubstrate">Theoretical biomethane potential of substrate i (Nm3 kg VS-1)</param>
        /// <returns>Total CH4 production (Nm3 day-1), where Nm3 are normal metres cubed</returns>
        public double CalculateTotalCH4Production(IEnumerable<double> flowRatesOfDegradedVSDuringDigestion,
                                                  double biomethanePotentialOfSubstrate)
        {
            var sumFlowRatesDegradedVS = flowRatesOfDegradedVSDuringDigestion.Sum();
            return sumFlowRatesDegradedVS * biomethanePotentialOfSubstrate;
        }

        /// <summary>
        /// Eq 4.8.2-3
        /// </summary>
        /// <param name="flowRateBiodegradeableVSInSubstrate">Flow rate of biodegradable VS in substrate i (Nm3 VS day-1)</param>
        /// <param name="hydrolysisRateSubstrateDuringDigestion">Hydrolysis rate of substrate i during digestion (day-1)</param>
        /// <param name="hydraulicRetentionTime">Hydraulic retention time (days)</param>
        /// <returns>Flow rate of degraded VS during digestion (kg VS day-1)</returns>
        public double CalculateFlowRateOfDegradedVolatileSolidsDuringDigestion(double flowRateBiodegradeableVSInSubstrate,
                                                                               double hydrolysisRateSubstrateDuringDigestion,
                                                                               double hydraulicRetentionTime)
        {
            return flowRateBiodegradeableVSInSubstrate -
                  (flowRateBiodegradeableVSInSubstrate / ((1 + hydrolysisRateSubstrateDuringDigestion) * hydraulicRetentionTime));
        }
        #endregion

        #region 4.8.2.2 Biogas Production

        /// <summary>
        /// Eq 4.8.2-4
        /// </summary>
        /// <param name="methaneProductionOfSubstrate">Methane production of substrate i (Nm3 day-1)</param>
        /// <param name="methaneFractionInBiogas">Fraction of methane in the bioags for substrate i (Table 46)</param>
        /// <returns>Biogas production of substrate i (Nm3 day-1)</returns>
        public double CalculateBiogasProductionOfSubstrate(double methaneProductionOfSubstrate,
                                                           double methaneFractionInBiogas)
        {
            return methaneProductionOfSubstrate / methaneFractionInBiogas;
        }

        /// <summary>
        /// Eq 4.8.2-5
        /// </summary>
        /// <param name="biogasProductionsOfSubstrates">A collection of Biogas production of substrate i values. Calculated using <see cref="CalculateBiogasProductionOfSubstrate(double, double)"/></param>
        /// <returns>Total biogas production upon co-digestion of multiple substrates (Nm3 day-1)</returns>
        public double CalculateTotalBiogasProduction(IEnumerable<double> biogasProductionsOfSubstrates)
        {
            return biogasProductionsOfSubstrates.Sum();
        }

        #endregion

        #region 4.8.2.3 Carbon Dioxide Production

        /// <summary>
        /// Eq 4.8.2-6
        /// </summary>
        /// <param name="biogasProductionOfSubstrate"></param>
        /// <param name="methaneProductionOfSubstrate"></param>
        /// <returns>Carbon dioxide production from substrate i (Nm3 day-1)</returns>
        public double CalculateCarbonDioxideProductionFromSubstrate(double biogasProductionOfSubstrate,
                                                                    double methaneProductionOfSubstrate)
        {
            return (biogasProductionOfSubstrate - methaneProductionOfSubstrate);
        }

        /// <summary>
        /// Eq 4.8.2-7
        /// </summary>
        /// <param name="totalBiogasProduction">Total biogas production upon co-digestion of multiple substrates (Nm3 day-1). Calculated using <see cref="CalculateTotalBiogasProduction"/></param>
        /// <param name="totalCH4Production"></param>
        /// <returns>Total CO2 production upon co-digestion of multiple substrates (Nm3 day-1)</returns>
        public double CalculateTotalCarbonDioxideProduction(double totalBiogasProduction,
                                                            double totalCH4Production)
        {
            return totalBiogasProduction - totalCH4Production;
        }

        #endregion

        #region 4.8.2.4 Reactor Dimensioning

        /// <summary>
        /// Eq 4.8.2-8
        /// </summary>
        /// <param name="totalFlowRateSubstrate">Total flow rate of substrate entering the digester (m3 day-1)</param>
        /// <param name="hydraulicRetentionDays">Hydraulic retention time (days)</param>
        /// <returns>Reactor volume (m3)</returns>
        public double CalculateReactorVolume(double totalFlowRateSubstrate,
                                             double hydraulicRetentionDays)
        {
            return hydraulicRetentionDays / totalFlowRateSubstrate;
        }

        /// <summary>
        /// Eq 4.8.2-9
        /// </summary>
        /// <param name="totalFlowRateVolatileSolids">Total flow rate of VS entering the digester (m3 day-1)</param>
        /// <param name="reactorVolume">Reactor volume (m3)</param>
        /// <returns>Organic loading rate (kg VS m-3 d-1) (upper limit: 3.5, average: 1.6 </returns>
        public double CalculateOrganicLoadingRate(double totalFlowRateVolatileSolids,
                                                  double reactorVolume)
        {
            return totalFlowRateVolatileSolids / reactorVolume;
        }

        #endregion

        #region 4.8.2.5 Volorization of Methane

        /// <summary>
        /// Eq 4.8.2-10
        /// </summary>
        /// <param name="totalCH4Production">Total CH4 production (Nm3 day-1)</param>
        /// <param name="fractionFugitiveMethaneLosses">Fraction of fugitive methane losses through digester equipment. A default value of 0.03 is used</param>
        /// <returns>Recoverable CH4 (Nm3 day-1)</returns>
        public double CalculateRecoverableCH4(double totalCH4Production,
                                              double fractionFugitiveMethaneLosses = 0.03)
        {
            return totalCH4Production * (1 - fractionFugitiveMethaneLosses);
        }

        /// <summary>
        /// Eq 4.8.2-11
        /// </summary>
        /// <param name="totalCH4Production"></param>
        /// <param name="calorificValueCH4">Calorific value of CH4 (MJ Nm-3). A default value of 35.17 is used</param>
        /// <param name="conversionCoefficient">Conversion coefficient kWh to MJ (MJ kWh-1). A default value of 3.6 is used</param>
        /// <returns>Total primary energy production (kWh day-1)</returns>
        public double CalculateTotalPrimaryEnergyProduction(double totalCH4Production,
                                                            double calorificValueCH4 = 35.17,
                                                            double conversionCoefficient = 3.6)
        {
            return totalCH4Production * (calorificValueCH4 / conversionCoefficient);
        }

        /// <summary>
        /// Eq 4.8.2-12
        /// </summary>
        /// <param name="totalPrimaryEnergyProduction">Total primary energy production (kWh day-1). Calculated using <see cref="CalculateTotalPrimaryEnergyProduction(double, double, double)"/></param>
        /// <param name="fractionPrimaryEnergyToElectricity">Fraction of primary energy converted to electricity through CHP. A default value of 0.4 is used</param>
        /// <returns>Total electricity production through CHP (kWh day-1)</returns>
        public double CalculateTotalElectricityProductionCHP(double totalPrimaryEnergyProduction,
                                                             double fractionPrimaryEnergyToElectricity = 0.4)
        {
            return totalPrimaryEnergyProduction * fractionPrimaryEnergyToElectricity;
        }

        /// <summary>
        /// Eq 4.8.2-13
        /// </summary>
        /// <param name="totalPrimaryEnergyProduction">Total primary energy production (kWh day-1). Calculated using <see cref="CalculateTotalPrimaryEnergyProduction(double, double, double)"</param>
        /// <param name="fractionPrimaryEnergyToHeat">Fraction of primary energy converted to heat through CHP. A default value of 0.5 is used</param>
        /// <returns>Total heat production through CHP (kWh day-1)</returns>
        public double CalculateTotalHeatProductionCHP(double totalPrimaryEnergyProduction,
                                                      double fractionPrimaryEnergyToHeat = 0.5)
        {
            return totalPrimaryEnergyProduction * fractionPrimaryEnergyToHeat;
        }

        /// <summary>
        /// Eq 4.8.2-14
        /// </summary>
        /// <param name="recoverableCH4">Recoverable CH4 (Nm3 day-1)</param>
        /// <param name="fractionMethaneLostInUpgradingPlants">	Fraction of methane lost in upgrading plants. A default value of 0.0081 is used</param>
        /// <returns>Potential CH4 injection to the gas grid (kWh day-1)</returns>
        public double CalculatePotentialCH4InjectionToGasGrid(double recoverableCH4,
                                                              double fractionMethaneLostInUpgradingPlants = 0.0081)
        {
            return recoverableCH4 * fractionMethaneLostInUpgradingPlants;
        }
        #endregion

        #region 4.8.3 Production of Digestate And Its Composition


        /// <summary>
        /// Eq 4.8.3-1
        /// </summary>
        /// <param name="flowRatesSubstrate">A collection of values denoting the flow rate of substrate i entering the digester (t day-1)</param>
        /// <returns>Returns the sum value of the parameter. Denotes the flow rate of total mass of digestate (t day-1)</returns>
        public double CalculateFlowRateTotalMassOfDigestate(IEnumerable<double> flowRatesSubstrate)
        {
            return flowRatesSubstrate.Sum();
        }

        /// <summary>
        /// Eq 4.8.3-2
        /// </summary>
        /// <param name="flowRateVolatileSolidsSubstrate">Flow rate of VS in substrate entering the digester (kg day-1)</param>
        /// <param name="flowRateVolatideSolidsDiscarded">Flow rate of VS degraded during digestion (kg day-1)</param>
        /// <returns>Flow rate of VS in digestate (kg day-1)</returns>
        public double CalculateFlowRateVolatileSolidsInDigestate(double flowRateVolatileSolidsSubstrate,
                                                                 double flowRateVolatideSolidsDiscarded)
        {
            return flowRateVolatileSolidsSubstrate - flowRateVolatideSolidsDiscarded;
        }

        /// <summary>
        /// Eq 4.8.3-3
        /// </summary>
        /// <param name="flowRatesTotalNitrogenInSubstrate">A collection of values denoting the flow rate of total N in substrate i entering the digester (kg day-1)</param>
        /// <returns>Flow rate of total N in digestate (kg day-1)</returns>
        public double CalculateFlowRateTotalNitrogenInDigestate(IEnumerable<double> flowRatesTotalNitrogenInSubstrate)
        {
            return flowRatesTotalNitrogenInSubstrate.Sum();
        }


        // 4.8.3-4
        // 4.8.3-5
        // 4.8.3-6
        // 4.8.3-7
        


        #endregion

        #region 4.8.4 Solid-Liquid Separation Of Digestate

        /// <summary>
        /// Eq 4.8.4-1
        /// </summary>
        /// <param name="fractionRawMaterialCoefficient">Separation coefficient: fraction of raw material in solid fraction following solid-liquid separation</param>
        /// <param name="flowRateDigestate">Flow rate of digestate (t day-1)</param>
        /// <returns>Flow rate of liquid fraction of digestate (t day-1)</returns>
        public double CalculateFlowRateLiquidFractionDigestate(double fractionRawMaterialCoefficient,
                                                               double flowRateDigestate)
        {
            return (1 - fractionRawMaterialCoefficient) * flowRateDigestate;
        }

        /// <summary>
        /// Eq 4.8.4-2
        /// </summary>
        /// <param name="fractionRawMaterialCoefficient"></param>
        /// <param name="flowRateDigestate">Flow rate of digestate (t day-1)</param>
        /// <returns>Flow rate of solid fraction of digestate (t day-1)</returns>
        public double CalculateFlowRateSolidFractionDigestate(double fractionRawMaterialCoefficient,
                                                              double flowRateDigestate)
        {
            return fractionRawMaterialCoefficient * flowRateDigestate;
        }

        /// <summary>
        /// Eq 4.8.4-3
        /// </summary>
        /// <param name="fractionVolatileSolidsCoefficient">Separation coefficient: fraction of VS in the solid fraction following solid-liquid </param>
        /// <param name="flowRateInDigestate">Flow rate of digestate (t day-1)</param>
        /// <returns>Flow rate of VS in the liquid fraction of digestate (kg day-1)</returns>
        public double CalculateFlowRateVolatileSolidsInDigestateLiquidFraction(double fractionVolatileSolidsCoefficient,
                                                                               double flowRateInDigestate)
        {
            return (1 - fractionVolatileSolidsCoefficient) * flowRateInDigestate;
        }

        /// <summary>
        /// Eq 4.8.4-4
        /// </summary>
        /// <param name="fractionVolatileSolidsCoefficient">Separation coefficient: fraction of VS in the solid fraction following solid-liquid </param>
        /// <param name="flowRateInDigestate">Flow rate of digestate (t day-1)</param>
        /// <returns>Flow rate of VS in the solid fraction of digestate (kg day-1)</returns>
        public double CalculateFlowRateVolatileSolidsInDigestateSolidFraction(double fractionVolatileSolidsCoefficient,
                                                                              double flowRateInDigestate)
        {
            return fractionVolatileSolidsCoefficient * flowRateInDigestate;
        }

        /// <summary>
        /// Eq 4.8.4-5
        /// </summary>
        /// <param name="fractionTANCoefficient">Separation coefficient: fraction of TAN in the solid fraction following solid-liquid separation (Table 47</param>
        /// <param name="flowRateTANInDigestate">Flow rate of TAN in digestate (kg day-1)</param>
        /// <returns>Flow rate of TAN in the liquid fraction of digestate (kg day-1)</returns>
        public double CalculateFlowRateTANInDigestateLiquidFraction(double fractionTANCoefficient,
                                                                    double flowRateTANInDigestate)
        {
            return (1 - fractionTANCoefficient) * flowRateTANInDigestate;
        }

        /// <summary>
        /// Eq 4.8.4-6
        /// </summary>
        /// <param name="fractionTANCoefficient">Separation coefficient: fraction of TAN in the solid fraction following solid-liquid separation (Table 47</param>
        /// <param name="flowRateTANInDigestate">Flow rate of TAN in digestate (kg day-1)</param>
        /// <returns>Flow rate of TAN in the solid fraction of digestate (kg day-1)</returns>
        public double CalculateFlowRateTANInDigestateSolidFraction(double fractionTANCoefficient,
                                                                    double flowRateTANInDigestate)
        {
            return fractionTANCoefficient * flowRateTANInDigestate;
        }

        /// <summary>
        /// Eq 4.8.4-7
        /// </summary>
        /// <param name="fractionOrganicNitrogenCoefficient">Separation coefficient: fraction of organic N in the solid fraction following solid-liquid separation (Table 47)</param>
        /// <param name="flowRateOrganicNitrogenInDigestate">Flow rate of organic N in digestate (kg day-1)</param>
        /// <returns>Flow rate of organic N in the liquid fraction of digestate (kg day-1)</returns>
        public double CalculateFlowRateOrganicNitrogenInDigestateLiquidFraction(double fractionOrganicNitrogenCoefficient,
                                                                                double flowRateOrganicNitrogenInDigestate)
        {
            return (1 - fractionOrganicNitrogenCoefficient) * flowRateOrganicNitrogenInDigestate;
        }

        /// <summary>
        /// Eq 4.8.4-8
        /// </summary>
        /// <param name="fractionOrganicNitrogenCoefficient">Separation coefficient: fraction of organic N in the solid fraction following solid-liquid separation (Table 47)</param>
        /// <param name="flowRateOrganicNitrogenInDigestate">Flow rate of organic N in digestate (kg day-1)</param>
        /// <returns>Flow rate of organic N in the solid fraction of digestate (kg day-1)</returns>
        public double CalculateFlowRateOrganicNitrogenInDigestateSolidFraction(double fractionOrganicNitrogenCoefficient,
                                                                               double flowRateOrganicNitrogenInDigestate)
        {
            return fractionOrganicNitrogenCoefficient * flowRateOrganicNitrogenInDigestate;
        }
        #endregion

        #region 4.8.4.1 Storage Of Digestate

        /// <summary>
        /// Eq 4.8.4-9
        /// </summary>
        /// <param name="methaneEmissionFactor">Methane emission factor for digestate storage (g m-3 day-1) </param>
        /// <param name="storageVolume">Storage volume of digestate (m3)</param>
        /// <returns>Methane emissions during digestate storage (kg day-1)</returns>
        public double CalculateMethaneEmissionsDuringDigestateStorage(double methaneEmissionFactor,
                                                                      double storageVolume)
        {
            return methaneEmissionFactor * storageVolume;
        }

        /// <summary>
        /// Eq 4.8.4-10
        /// </summary>
        /// <param name="nitrousOxideEmissionFactor">Nitrous oxide emission factor for digestate storage (g m-3 day-1). </param>
        /// <param name="storageVolume">Storage volume of digestate (m3)</param>
        /// <returns>Nitrous oxide emissions during digestate storage (kg day-1)</returns>
        public double CalculateNitrousOxideEmissionsDuringDigestateStorage(double nitrousOxideEmissionFactor,
                                                                           double storageVolume)
        {
            return nitrousOxideEmissionFactor * storageVolume;
        }

        /// <summary>
        /// Eq 4.8.4-11
        /// </summary>
        /// <param name="ammoniaEmissionFactor">Ammonia emission factor for digestate storage (g m-3 day-1)</param>
        /// <param name="storageVolume">Storage volume of digestate (m3)</param>
        /// <returns>Ammonia emissions during digestate storage (kg day-1)</returns>
        public double CalculateAmmoniaEmissionsDuringDigestateStorage(double ammoniaEmissionFactor,
                                                                      double storageVolume)
        {
            return ammoniaEmissionFactor * storageVolume;
        }
        #endregion

        #endregion
    }
}
