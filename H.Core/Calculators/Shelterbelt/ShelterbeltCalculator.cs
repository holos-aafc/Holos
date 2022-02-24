using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper.Configuration.Conventions;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Shelterbelt;
using H.Core.Providers.Shelterbelt;
using H.Infrastructure;

namespace H.Core.Calculators.Shelterbelt
{
    public class ShelterbeltCalculator
    {
        #region Fields

        private readonly ShelterbeltAgTRatioProvider _shelterbeltAgTRatioProvider = new ShelterbeltAgTRatioProvider();

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the A value for aboveground biomass estimation.
        /// </summary>
        public double GetA(TreeSpecies species)
        {
            var entry = ShelterbeltAllometricTableProvider.GetShelterbeltAllometricTable().Find(x => x.TreeSpecies == species);
            if (entry == null)
            {
                throw new Exception("Species does not exist in table: " + species.GetDescription());
            }
            else
            {
                return entry.A;
            }
        }

        /// <summary>
        /// Gets the B value for aboveground biomass estimation.
        /// </summary>
        public double GetB(TreeSpecies species)
        {
            var entry = ShelterbeltAllometricTableProvider.GetShelterbeltAllometricTable().Find(x => x.TreeSpecies == species);
            if (entry == null)
            {
                throw new Exception("Species does not exist in table: " + species.GetDescription());
            }
            else
            {
                return entry.B;
            }
        }

        public double AverageTwo(double one, double two)
        {
            return (one + two) / 2.0;
        }

        /// <summary>
        /// Builds <see cref="TrannumData"/>s for the <see cref="ShelterbeltComponent"/> and calculates biomass for each group of tree in each row of the shelterbelt.
        /// </summary>
        public void CalculateInitialResults(ShelterbeltComponent component)
        {
            component.BuildTrannums();

            this.CalculateInitialResults(component.TrannumData);
        }

        public void CalculateInitialResults(
            IList<TrannumData> trannumData)
        {
            // Group trannums by tree groups since we could have two groups of trees with the same year of observation in the same row
            var groups = trannumData.GroupBy(x => x.TreeGroupGuid);
            foreach (var treeGroup in groups)
            {
                /*
                 * User has entered the circumference of the trees at the year of observation only (year of measurement). We can only calculate biomass/carbon
                 * for this one year. Once we have these calculated biomass/carbon values, we then compare these against the lookup values for total living biomass and
                 * get a ratio of actual growth to ideal (lookup) growth.
                 *
                 * With this ratio we then lookup values for TEC, BIOM, DOM for all other years and multiply by the calculated ratio to get estimated growth in past/future years.
                 */

                var yearOfObservationTrannum = treeGroup.Single(x => Math.Abs(x.Year - x.YearOfObservation) < double.Epsilon);

                // Calculate biomass carbon for the year of observation
                this.CalculateBiomassPerTreeAtYearOfObservation(yearOfObservationTrannum);

                // We now check if the farm location is inside of Saskatchewan or not - this will determine which lookup table we use for biomass/carbon values
                yearOfObservationTrannum.CanLookupByEcodistrict = ShelterbeltEcodistrictToClusterLookupProvider.CanLookupByEcodistrict(yearOfObservationTrannum.EcodistrictId);

                // Now we calculate the ratio of real growth compared to ideal growth
                yearOfObservationTrannum.RealGrowthRatio = this.CalculateRealGrowthRatio(yearOfObservationTrannum);

                // Now that we have calculated the real growth ratio, we assign this to all other years and indicate the table lookup method we will use
                foreach (var trannum in trannumData)
                {
                    trannum.RealGrowthRatio = yearOfObservationTrannum.RealGrowthRatio;
                    trannum.CanLookupByEcodistrict = yearOfObservationTrannum.CanLookupByEcodistrict;
                }

                // Finally, calculate estimated growth based on real growth ratio for all other years (i.e. from plant date to the cut date)
                foreach (var data in trannumData)
                {
                    this.CalculateEstimatedGrowth(data);
                }
            }
        }

        /// <summary>
        /// Calulcates the actual biomass of trees at the year of observation. Performs calucation of biomass only - not carbon.
        /// </summary>
        public void CalculateBiomassPerTreeAtYearOfObservation(TrannumData trannumData)
        {
            // Calculate biomass per tree first
            if (trannumData.TreeSpecies == TreeSpecies.AverageConifer)
            {
                trannumData.TotalBiomassPerTree = this.CalculateAverageConiferBiomass(trannumData);
            }
            else if (trannumData.TreeSpecies == TreeSpecies.AverageDeciduous)
            {
                trannumData.TotalBiomassPerTree = this.CalculateAverageDeciduousBiomass(trannumData);
            }
            else
            {
                trannumData.TotalBiomassPerTree = this.CalculateBiomassOfTrees(trannumData);
            }

            // Biomass_treetype
            trannumData.BiomassPerTreeType = this.CalculateBiomassPerTreeType(
                biomassPerTree: trannumData.TotalBiomassPerTree,
                treeCount: trannumData.TreeCount);

            // TLB_treetype
            trannumData.TotalLivingBiomassPerTreeTypePerStandardLength = this.CalculateTotalLivingBiomassPerStandardLength(
                biomassOfAllTrees: trannumData.BiomassPerTreeType,
                rowLength: trannumData.RowLength);
        }

        /// <summary>
        /// Compares the actual growth with that of the ideal growth for trees of the same age and species.
        /// </summary>
        public double CalculateRealGrowthRatio(TrannumData trannumData)
        {
            // Convert total living biomass to total living carbon
            trannumData.TotalLivingCarbonPerTreeTypePerStandardLength = this.CalculateTotalLivingCarbonPerTreeType(trannumData.TotalLivingBiomassPerTreeTypePerStandardLength);

            var age = trannumData.Age;
            var result = 0d;

            /*
             * Lookup tables do not have total living biomass for first three years (in some situations). If user has a year of observation that is in these first few years of growth (i.e. age = 1, 2, 3)
             * it will not be possible to calculate the real growth ratio. Go foward in time until we get a non-zero value and use that for comparison.
             */

            var biomasCarbonPerKilometerMegagrams = 0d;

            var biomasCarbonPerStandardLengthInKilograms = 0d;
            if (trannumData.CanLookupByEcodistrict)
            {
                do
                {
                    // Get total living biomass carbon of an ideal tree
                    biomasCarbonPerKilometerMegagrams = ShelterbeltCarbonDataProvider.GetLookupValue(
                        treeSpecies: trannumData.TreeSpecies,
                        ecodistrictId: trannumData.EcodistrictId,
                        percentMortality: trannumData.PercentMortality,
                        mortalityLow: trannumData.PercentMortalityLow,
                        mortalityHigh: trannumData.PercentMortalityHigh,
                        age: age,
                        column: ShelterbeltCarbonDataProviderColumns.Biom_Mg_C_km);

                    age++;
                } while (biomasCarbonPerKilometerMegagrams == 0 && age < CoreConstants.ShelterbeltCarbonTablesMaximumAge);
            }
            else
            {
                // If we are outside of Saskatchewan, we won't have access to the cluster id that is need to lookup live biomass values, instead we lookup values by hardiness zone instead.
                do
                {
                    // Get total living biomass carbon of an ideal tree
                    biomasCarbonPerKilometerMegagrams = ShelterbeltHardinessZoneLookupProvider.GetLookupValue(
                        treeSpecies: trannumData.TreeSpecies,
                        hardinessZone: trannumData.HardinessZone,
                        percentMortality: trannumData.PercentMortality,
                        mortalityLow: trannumData.PercentMortalityLow,
                        mortalityHigh: trannumData.PercentMortalityHigh,
                        age: age,
                        column: ShelterbeltCarbonDataProviderColumns.Biom_Mg_C_km);

                    age++;
                } while (biomasCarbonPerKilometerMegagrams == 0 && age < CoreConstants.ShelterbeltCarbonTablesMaximumAge);
            }

            // Convert total living biomass carbon to kilograms
            biomasCarbonPerStandardLengthInKilograms = biomasCarbonPerKilometerMegagrams * 1000;

            result = this.CalculateRealGrowthRatio(
                calculatedTotalLivingCarbonKilogramPerStandardLength: trannumData.TotalLivingCarbonPerTreeTypePerStandardLength,
                lookupTotalLivingCarbonKilogramsPerStandardLength: biomasCarbonPerStandardLengthInKilograms);

            return result;
        }

        /// <summary>
        /// For all years except the year of observation/measurement, we need to estimate growth based on the calculated real growth ratio. Calculated the estimated
        /// growth (living biomass carbon) for the particular species at the given year.
        /// </summary>
        public void CalculateEstimatedGrowth(TrannumData trannumData)
        {
            var totalEcosystemCarbon = 0d;
            var deadOrganicMatter = 0d;

            if (trannumData.TreeSpecies == TreeSpecies.AverageConifer)
            {
                /*
                 * Take the average of white spruce and scots pine
                 */

                trannumData.TreeSpecies = TreeSpecies.WhiteSpruce;

                var whiteSpruceTotalEcosystemCarbon = this.GetIdealTotalEcosystemCarbon(trannumData);
                var whiteSpruceDeadOrganicMatterCarbon = this.GetIdealDeadOrganicMatter(trannumData);

                trannumData.TreeSpecies = TreeSpecies.ScotsPine;

                var scotsPineTotalEcosystemCarbon = this.GetIdealTotalEcosystemCarbon(trannumData);
                var scotsPineDeadOrganicMatterCarbon = this.GetIdealDeadOrganicMatter(trannumData);

                // Change back to original species
                trannumData.TreeSpecies = TreeSpecies.AverageConifer;

                totalEcosystemCarbon = AverageTwo(whiteSpruceTotalEcosystemCarbon, scotsPineTotalEcosystemCarbon);
                deadOrganicMatter = AverageTwo(whiteSpruceDeadOrganicMatterCarbon, scotsPineDeadOrganicMatterCarbon);

            }
            else if (trannumData.TreeSpecies == TreeSpecies.AverageDeciduous)
            {
                /*
                 * Take the average of manitoba maple and green ash
                 */

                trannumData.TreeSpecies = TreeSpecies.ManitobaMaple;

                var manitobaMapleTotalEcoystemCarbon = this.GetIdealTotalEcosystemCarbon(trannumData);
                var manitobaMapleDeadOrganicMatterCarbon = this.GetIdealDeadOrganicMatter(trannumData);

                trannumData.TreeSpecies = TreeSpecies.GreenAsh;

                var greenAshTotalEcosystemCarbon = this.GetIdealTotalEcosystemCarbon(trannumData);
                var greenAshDeadOrganicMatterCarbon = this.GetIdealDeadOrganicMatter(trannumData);

                // Change back to original species
                trannumData.TreeSpecies = TreeSpecies.AverageDeciduous;

                totalEcosystemCarbon = AverageTwo(greenAshTotalEcosystemCarbon, manitobaMapleTotalEcoystemCarbon);
                deadOrganicMatter = AverageTwo(greenAshDeadOrganicMatterCarbon, manitobaMapleDeadOrganicMatterCarbon);
            }
            else
            {
                totalEcosystemCarbon = this.GetIdealTotalEcosystemCarbon(trannumData);
                deadOrganicMatter = this.GetIdealDeadOrganicMatter(trannumData);
            }

            var livingBiomass = totalEcosystemCarbon - deadOrganicMatter;

            var deadOrganicMatterRatio = deadOrganicMatter * trannumData.RealGrowthRatio;
            var livingBiomassRatio = livingBiomass * trannumData.RealGrowthRatio;

            // Calculate the estimated biomass carbon based on the real growth ratio
            trannumData.EstimatedBiomassCarbonBasedOnRealGrowth = livingBiomassRatio;

            // Calculate the estimated dead organic matter based on the real growth ratio
            trannumData.EstimatedDeadOrganicMatterBasedOnRealGrowth = deadOrganicMatterRatio;
        }

        /// <summary>
        /// Once the yearly values have been calculated, we can then total up the biomass/carbon/CO2 values for each shelterbelt during each year.
        /// </summary>
        public List<TrannumResultViewItem> TotalResultsForEachYear(
            IEnumerable<ShelterbeltComponent> shelterbeltComponents)
        {
            var results = new List<TrannumResultViewItem>();

            foreach (var component in shelterbeltComponents)
            {
                var resultsForComponent = new List<TrannumResultViewItem>();

                // There may be several rows each with multiple groups. We need to get a list of distinct years so we can get the totals for each year.
                var distinctYears = component.TrannumData.Select(x => x.Year).Distinct();
                foreach (var year in distinctYears)
                {
                    var resultViewItem = new TrannumResultViewItem();

                    var viewItemsForYear = component.TrannumData.Where(x => Math.Abs(x.Year - year) < double.Epsilon).ToList();

                    resultViewItem.ShelterbeltComponent = component;
                    resultViewItem.Year = (int)year;

                    resultViewItem.TotalLivingBiomassCarbon = this.CalculateTotalShelterbeltBiomassCarbon(viewItemsForYear) / 1000; // Convert to Mg
                    resultViewItem.TotalDeadOrganicMatterCarbon = this.CalculateTotalDeadOrganicMatter(viewItemsForYear) / 1000; // Convert to Mg
                    resultViewItem.TotalEcosystemCarbon = this.CalculateTotalEcosystemCarbon(
                        biomassCarbon: resultViewItem.TotalLivingBiomassCarbon,
                        deadOrganicMatterCarbon: resultViewItem.TotalDeadOrganicMatterCarbon);

                    resultsForComponent.Add(resultViewItem);
                }

                results.AddRange(resultsForComponent);

                // Next we calculate the changes in the total ecosystem carbon, living biomass carbon, and dead organic matter carbon from year to year
                for (int i = 0; i < resultsForComponent.Count; i++)
                {
                    if (i == 0)
                    {
                        var resultViewItem = resultsForComponent.ElementAt(0);
                        resultViewItem.TotalDeadOrganicMatterChange = 0;
                        resultViewItem.TotalEcosystemCarbonChange = 0;
                        resultViewItem.TotalLivingBiomassCarbonChange = 0;

                        continue;
                    }

                    var previousYearViewItem = resultsForComponent.ElementAt(i - 1);
                    var currentYearViewItem = resultsForComponent.ElementAt(i);

                    currentYearViewItem.TotalDeadOrganicMatterChange = (currentYearViewItem.TotalDeadOrganicMatterCarbon - previousYearViewItem.TotalDeadOrganicMatterCarbon);
                    currentYearViewItem.TotalEcosystemCarbonChange = (currentYearViewItem.TotalEcosystemCarbon - previousYearViewItem.TotalEcosystemCarbon);
                    currentYearViewItem.TotalLivingBiomassCarbonChange = (currentYearViewItem.TotalLivingBiomassCarbon - previousYearViewItem.TotalLivingBiomassCarbon);
                }
            }

            return results;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// An 'average' coniferous tree is the average biomass carbon of white spruce and scots pine at the same age
        /// </summary>
        private double CalculateAverageConiferBiomass(TrannumData trannumData)
        {
            trannumData.TreeSpecies = TreeSpecies.WhiteSpruce;

            var whiteSpruceBiomass = this.CalculateBiomassOfTrees(trannumData);

            trannumData.TreeSpecies = TreeSpecies.ScotsPine;

            var scotsPineBiomass = this.CalculateBiomassOfTrees(trannumData);

            var result = this.AverageTwo(scotsPineBiomass, whiteSpruceBiomass);

            trannumData.TreeSpecies = TreeSpecies.AverageConifer;

            return result;
        }

        /// <summary>
        /// An 'average' deciduous tree is the average biomass carbon of green ash and manitoba maple trees at the same age
        /// </summary>
        private double CalculateAverageDeciduousBiomass(TrannumData trannumData)
        {
            trannumData.TreeSpecies = TreeSpecies.ManitobaMaple;

            var manitobaMapleBiomass = this.CalculateBiomassOfTrees(trannumData);

            trannumData.TreeSpecies = TreeSpecies.GreenAsh;

            var greenAshBiomass = this.CalculateBiomassOfTrees(trannumData);

            var result = this.AverageTwo(greenAshBiomass, manitobaMapleBiomass);

            trannumData.TreeSpecies = TreeSpecies.AverageDeciduous;

            return result;
        }

        private double CalculateBiomassOfTrees(TrannumData trannumData)
        {
            // Get the two lookup values (a, b) that will be used to calculate the above ground biomass of the tree
            var a = this.GetA(trannumData.TreeSpecies);
            var b = this.GetB(trannumData.TreeSpecies);

            // At this point, each trannum will have had a default circumference based on ideal trees. The user can now override the circumference values
            // for each individual year and the view model will recalculate the biomass/carbon values.
            var circumferencePerTree = trannumData.CircumferenceData.UserCircumference;

            var agtRatio = _shelterbeltAgTRatioProvider.GetAboveGroundBiomassTotalTreeBiomassRatio(
                treeSpecies: trannumData.TreeSpecies,
                age: trannumData.Age);

            // AboveGroundBiomass_tree
            var aboveGroundBiomassPerTree = this.CalculateAboveGroundBiomassPerTree(
                circumference: circumferencePerTree,
                coefficientA: a,
                coefficientB: b);

            // Total biomass per tree (aboveground and belowground)
            var totalBiomassPerTree = this.CalculateTotalBiomassOfTree(
                aboveGroundBiomassOfTree: aboveGroundBiomassPerTree,
                aboveGroundBiomassRatio: agtRatio);

            return totalBiomassPerTree;
        }

        #endregion

        #region Algorithms

        /// <summary>
        /// Equation 2.1.6-10 
        ///
        /// Calculates the ratio of user specified growth (biomass carbon) over ideal tree growth (biomass carbon).
        /// </summary>
        public double CalculateRealGrowthRatio(
            double calculatedTotalLivingCarbonKilogramPerStandardLength,
            double lookupTotalLivingCarbonKilogramsPerStandardLength)
        {
            if (lookupTotalLivingCarbonKilogramsPerStandardLength == 0)
            {
                // Assume value is not available and return 1 to indicate we are assuming an ideal tree for this year
                return 1;
            }

            return calculatedTotalLivingCarbonKilogramPerStandardLength / lookupTotalLivingCarbonKilogramsPerStandardLength;
        }

        /// <summary>
        /// Equation 2.1.6-11
        ///
        /// Calculates the mortality of a row of trees given the total live count and the total planted count.
        /// </summary>
        public double CalculatePercentMortalityOfALinearPlanting(
            List<double> plantedTreeCountAllSpecies,
            List<double> liveTreeCountAllSpecies)
        {
            return 100.0 * (plantedTreeCountAllSpecies.Sum() - liveTreeCountAllSpecies.Sum()) /
                   plantedTreeCountAllSpecies.Sum();
        }

        /// <summary>
        /// Equation 2.1.6-12
        ///
        /// There are only 3 lookup values in the table for mortality. Convert user defined mortality to one of these lookup values.
        /// </summary>
        public int CalculateMortalityLow(double percentMortalityOfALinearPlanting)
        {
            if (percentMortalityOfALinearPlanting >= 30.0)
                return 30;
            else if (percentMortalityOfALinearPlanting >= 15.0)
                return 15;
            else
                return 0;
        }

        /// <summary>
        /// Equation 2.1.6-13
        ///
        /// There are only 3 lookup values in the table for mortality.
        /// </summary>
        public int CalculateMortalityHigh(int mortalityLow)
        {
            switch (mortalityLow)
            {
                case 0:
                    return 15;
                case 15:
                    return 30;
                case 30:
                    return 50;
                default:
                    throw new Exception(nameof(ShelterbeltCalculator) + "." + nameof(CalculateMortalityHigh) + "(int " +
                                        nameof(mortalityLow) + ") must only receive the following values: 0, 15 or 30");
            }
        }

        /// <summary>
        /// Equation 2.1.6-2
        /// </summary>
        /// <returns>Aboveground biomass per tree (kg tree^-1)</returns>
        public double CalculateAboveGroundBiomassPerTree(
            double circumference,
            double coefficientA,
            double coefficientB)
        {
            var aboveGroundCarbonStocksPerTree = coefficientA * Math.Pow(circumference / 3.14159, coefficientB);

            return aboveGroundCarbonStocksPerTree;
        }

        /// <summary>
        /// Equation 2.1.6-3
        /// </summary>
        public double CalculateCircumferenceFromDiameter(double diameter)
        {
            return 3.14159 * diameter;
        }

        /// <summary>
        /// Equation 2.1.6-3 (b)
        /// </summary>
        public double CalculateDiameterFromCircumference(double circumference)
        {
            return circumference / 3.14159;
        }

        /// <summary>
        /// Equation 2.1.6-6
        /// </summary>
        public double CalculateAverageCircumference(List<double> treeCircumferences)
        {
            var averageCircumference = 0.0;
            foreach (var circumference in treeCircumferences)
            {
                averageCircumference += circumference;
            }

            averageCircumference /= treeCircumferences.Count;

            return averageCircumference;
        }

        /// <summary>
        /// Equation 2.1.6-1
        /// </summary>
        /// <param name="aboveGroundBiomassOfTree">Above ground biomass of tree (kg tree^-1)</param>
        /// <param name="aboveGroundBiomassRatio">Fraction of aboveground over total biomass (aboveground + belowground)</param>
        /// <returns>Total tree biomass (aboveground + belowground) (kg tree^-1)</returns>
        public double CalculateTotalBiomassOfTree(
            double aboveGroundBiomassOfTree,
            double aboveGroundBiomassRatio)
        {
            return aboveGroundBiomassOfTree / aboveGroundBiomassRatio;
        }

        /// <summary>
        /// Equation 2.1.6-7
        /// </summary>
        public double CalculateTreeCount(
            double rowLength,
            double treeSpacing,
            double percentMortality)
        {
            return (rowLength / treeSpacing) * ((100 - percentMortality) / 100.0);
        }

        /// <summary>
        /// Equation 2.1.6-9
        /// </summary>
        /// <param name="biomassPerTree">Total tree biomass (aboveground + belowground) (kg tree^-1)</param>
        /// <param name="treeCount">Total number of trees (of same type) within the row</param>
        /// <returns>Biomass of trees of a particular species within a row (kg row^-1)</returns>
        public double CalculateBiomassPerTreeType(
            double biomassPerTree,
            double treeCount)
        {
            return biomassPerTree * treeCount;
        }

        /// <summary>
        /// Equation 2.1.6-10
        /// </summary>
        /// <param name="biomassOfAllTrees">Biomassof trees of the same species within a linear planting (kg planting-1)</param>
        /// <param name="rowLength">Length of the </param>
        /// <returns>Total tree biomass per standard length linear planting (kg km^-1)</returns>
        public double CalculateTotalLivingBiomassPerStandardLength(
            double biomassOfAllTrees,
            double rowLength)
        {
            return biomassOfAllTrees * (rowLength / 1000.0);
        }

        /// <summary>
        /// Equation 2.1.6-10
        /// </summary>
        /// <param name="biomassPerKilometer">Biomass (kg)</param>
        /// <returns>Total carbon in the living biomass per standard length linear planting (kg C km^-1)</returns>
        public double CalculateTotalLivingCarbonPerTreeType(
            double biomassPerKilometer)
        {
            return biomassPerKilometer * CoreConstants.CarbonConcentrationOfTrees;
        }

        /// <summary>
        /// Equation 2.3.1-4
        /// </summary>
        public double CalculateTreeCircumference(List<double> circumferences)
        {
            var treeCircumference = 0.0;
            foreach (var circumference in circumferences)
            {
                treeCircumference += circumference * circumference;
            }

            treeCircumference = Math.Sqrt(treeCircumference);
            return treeCircumference;
        }

        /// <summary>
        /// Equation 2.3.1-7
        /// </summary>
        public double CalculateTreeSpacing(
            double rowLength,
            double treeCount)
        {
            return rowLength / treeCount;
        }

        /// <summary>
        /// Equation 2.3.1-8
        /// 
        /// Calculates the total above ground carbon for a number of trees of the same type
        /// </summary>
        public double CalculateCarbonForTreetype(
            double aboveGroundCarbonStocksPerTree,
            double treeCount)
        {
            return aboveGroundCarbonStocksPerTree * treeCount;
        }

        /// <summary>
        /// Equation 2.3.1-9
        /// </summary>
        public double CalculateCarbonForLinearPlanting(List<double> aboveGroundCarbonForTreetypesInPlanting)
        {
            var aboveGroundCarbonForLinearPlanting = 0.0;
            foreach (var carbon in aboveGroundCarbonForTreetypesInPlanting)
            {
                aboveGroundCarbonForLinearPlanting += carbon;
            }

            return aboveGroundCarbonForLinearPlanting;
        }

        /// <summary>
        ///    Equation 2.3.3-1
        /// </summary>
        public double CalculateCarbonForShelterbelt(List<double> aboveGroundCarbonForLinearPlantingsInShelterbelt)
        {
            var aboveGroundCarbonForShelterbelt = 0.0;
            foreach (var carbon in aboveGroundCarbonForLinearPlantingsInShelterbelt)
            {
                aboveGroundCarbonForShelterbelt += carbon;
            }

            return aboveGroundCarbonForShelterbelt;
        }

        /// <summary>
        ///  Equation 2.3.4-1
        /// </summary>
        public double CalculateCarbonDioxideSequesteredInShelterbelt(double totalAboveGroundCarbonForShelterbelt)
        {
            return totalAboveGroundCarbonForShelterbelt * CoreConstants.ConvertFromCToCO2 * -1.0;
        }

        /// <summary>
        /// Equation 2.1.6-26
        /// </summary>
        /// <returns>The total dead organic matter carbon (kg C km^-1)</returns>
        private double GetIdealDeadOrganicMatter(TrannumData trannumData)
        {
            var deadOrganicMatterMegagrams = 0d;
            if (trannumData.CanLookupByEcodistrict)
            {
                // We can lookup by ecodistrict->cluster id mapping
                deadOrganicMatterMegagrams = ShelterbeltCarbonDataProvider.GetLookupValue(
                    treeSpecies: trannumData.TreeSpecies,
                    ecodistrictId: trannumData.EcodistrictId,
                    percentMortality: trannumData.PercentMortality,
                    mortalityLow: (int)trannumData.PercentMortalityLow,
                    mortalityHigh: (int)trannumData.PercentMortalityHigh,
                    age: trannumData.Age,
                    column: ShelterbeltCarbonDataProviderColumns.Dom_Mg_C_km);
            }
            else
            {
                // We need to lookup values by hardiness zone
                deadOrganicMatterMegagrams = ShelterbeltHardinessZoneLookupProvider.GetLookupValue(
                    treeSpecies: trannumData.TreeSpecies,
                    hardinessZone: trannumData.HardinessZone,
                    percentMortality: trannumData.PercentMortality,
                    mortalityLow: trannumData.PercentMortalityLow,
                    mortalityHigh: trannumData.PercentMortalityHigh,
                    age: trannumData.Age,
                    column: ShelterbeltCarbonDataProviderColumns.Dom_Mg_C_km);
            }

            var deadOrganicMatterKilograms = deadOrganicMatterMegagrams * 1000;

            return deadOrganicMatterKilograms;
        }

        /// <summary>
        /// Equation 2.1.6-26
        /// </summary>
        /// <returns>The total ecosystem carbon (kg C km^-1)</returns>
        private double GetIdealTotalEcosystemCarbon(TrannumData trannumData)
        {
            var totalEcosystemCarbonMegagrams = 0d;
            if (trannumData.CanLookupByEcodistrict)
            {
                // We can lookup by ecodistrict->cluster id mapping
                totalEcosystemCarbonMegagrams = ShelterbeltCarbonDataProvider.GetLookupValue(
                    treeSpecies: trannumData.TreeSpecies,
                    ecodistrictId: trannumData.EcodistrictId,
                    percentMortality: trannumData.PercentMortality,
                    mortalityLow: (int)trannumData.PercentMortalityLow,
                    mortalityHigh: (int)trannumData.PercentMortalityHigh,
                    age: trannumData.Age,
                    column: ShelterbeltCarbonDataProviderColumns.Tec_Mg_C_km);
            }
            else
            {
                // We need to lookup values by hardiness zone
                totalEcosystemCarbonMegagrams = ShelterbeltHardinessZoneLookupProvider.GetLookupValue(
                    treeSpecies: trannumData.TreeSpecies,
                    hardinessZone: trannumData.HardinessZone,
                    percentMortality: trannumData.PercentMortality,
                    mortalityLow: trannumData.PercentMortalityLow,
                    mortalityHigh: trannumData.PercentMortalityHigh,
                    age: trannumData.Age,
                    column: ShelterbeltCarbonDataProviderColumns.Tec_Mg_C_km);
            }

            var totalEcosystemCarbonKilograms = totalEcosystemCarbonMegagrams * 1000;

            return totalEcosystemCarbonKilograms;
        }

        /// <summary>
        /// Equation 2.1.6-26
        /// </summary>
        /// <returns>The total living biomass carbon (kg C km^-1)</returns>
        private double GetIdealTotalLivingBiomassCarbon(TrannumData trannumData)
        {
            var totalLivingBiomassMegagrams = 0d;

            if (trannumData.CanLookupByEcodistrict)
            {
                // We can lookup by ecodistrict->cluster id mapping
                totalLivingBiomassMegagrams = ShelterbeltCarbonDataProvider.GetLookupValue(
                    treeSpecies: trannumData.TreeSpecies,
                    ecodistrictId: trannumData.EcodistrictId,
                    percentMortality: trannumData.PercentMortality,
                    mortalityLow: (int)trannumData.PercentMortalityLow,
                    mortalityHigh: (int)trannumData.PercentMortalityHigh,
                    age: trannumData.Age,
                    column: ShelterbeltCarbonDataProviderColumns.Biom_Mg_C_km);
            }
            else
            {
                // We need to lookup values by hardiness zone
                totalLivingBiomassMegagrams = ShelterbeltHardinessZoneLookupProvider.GetLookupValue(
                    treeSpecies: trannumData.TreeSpecies,
                    hardinessZone: trannumData.HardinessZone,
                    percentMortality: trannumData.PercentMortality,
                    mortalityLow: trannumData.PercentMortalityLow,
                    mortalityHigh: trannumData.PercentMortalityHigh,
                    age: trannumData.Age,
                    column: ShelterbeltCarbonDataProviderColumns.Biom_Mg_C_km);
            }

            var biomassCarbonKilograms = totalLivingBiomassMegagrams * 1000;

            return biomassCarbonKilograms;
        }

        /// <summary>
        /// Equation 2.1.6-10
        ///
        /// (kg C km^-1)
        /// </summary>
        public double CalculateTotalShelterbeltBiomassCarbon(IEnumerable<TrannumData> trannumData)
        {
            return trannumData.Sum(x => x.EstimatedBiomassCarbonBasedOnRealGrowth);
        }

        /// <summary>
        /// Equation 2.1.6-26
        ///
        /// (kg C km^-1)
        /// </summary>
        private double CalculateTotalDeadOrganicMatter(List<TrannumData> trannumData)
        {
            return trannumData.Sum(x => x.EstimatedDeadOrganicMatterBasedOnRealGrowth);
        }

        /// <summary>
        /// (Mg C km^-1)
        /// </summary>
        private double CalculateTotalEcosystemCarbon(
            double biomassCarbon,
            double deadOrganicMatterCarbon)
        {
            return biomassCarbon + deadOrganicMatterCarbon;
        }

        #endregion
    }
}