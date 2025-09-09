using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Enumerations;
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
        ///     Gets the A value for aboveground biomass estimation.
        /// </summary>
        public double GetA(TreeSpecies species)
        {
            var entry = ShelterbeltAllometricTableProvider.GetShelterbeltAllometricTable()
                .Find(x => x.TreeSpecies == species);
            if (entry == null) throw new Exception("Species does not exist in table: " + species.GetDescription());

            return entry.A;
        }

        /// <summary>
        ///     Gets the B value for aboveground biomass estimation.
        /// </summary>
        public double GetB(TreeSpecies species)
        {
            var entry = ShelterbeltAllometricTableProvider.GetShelterbeltAllometricTable()
                .Find(x => x.TreeSpecies == species);
            if (entry == null) throw new Exception("Species does not exist in table: " + species.GetDescription());

            return entry.B;
        }

        public double AverageTwo(double one, double two)
        {
            return (one + two) / 2.0;
        }

        /// <summary>
        ///     Builds <see cref="TrannumData" />s for the <see cref="ShelterbeltComponent" /> and calculates biomass carbon for
        ///     each group of tree in each row of the shelterbelt.
        /// </summary>
        public void CalculateInitialResults(ShelterbeltComponent component)
        {
            component.BuildTrannums();

            CalculateInitialResults(component.TrannumData);
        }

        /// <summary>
        ///     Calculate the biomass carbon for each group of tree in each row of the shelterbelt.
        /// </summary>
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
                 * With this ratio we then lookup values for TEC, BIOM, DOM for all other years and multiply by the calculated ratio to get the estimated growth in past/future years.
                 */

                var yearOfObservationTrannum =
                    treeGroup.SingleOrDefault(x => Math.Abs(x.Year - x.YearOfObservation) < double.Epsilon);
                if (yearOfObservationTrannum == null)
                {
                    // If year is out of range, fallback to maximum year

                    var maxYear = treeGroup.Select(x => x.Year).Max();
                    yearOfObservationTrannum = treeGroup.Single(x => Math.Abs(x.Year - maxYear) < double.Epsilon);
                }

                // Calculate biomass for the year of observation
                CalculateBiomassPerTreeAtYearOfObservation(yearOfObservationTrannum);

                /* We now check if the farm location is inside of Saskatchewan or not - this will determine which lookup table we use for biomass/carbon values. Currently,
                 * only ecodistricts in Saskatchewan (and some close neightbouring ecodistricts can be mapped to a cluster id).
                 */

                yearOfObservationTrannum.CanLookupByEcodistrict =
                    ShelterbeltEcodistrictToClusterLookupProvider.CanLookupByEcodistrict(yearOfObservationTrannum
                        .EcodistrictId);

                // Now we calculate the ratio of real growth compared to ideal growth for the trees at this age

                // Lookups don't have entries for average species use averages here if this is the case
                var originalSpecies = yearOfObservationTrannum.TreeSpecies;
                var realGrowthRatio = 0d;
                if (originalSpecies == TreeSpecies.AverageConifer)
                    realGrowthRatio = CalculateRealGrowthRatioAverageConiferous(yearOfObservationTrannum);

                else if (originalSpecies == TreeSpecies.AverageDeciduous)
                    realGrowthRatio = CalculateRealGrowthRatioAverageDeciduous(yearOfObservationTrannum);
                else
                    realGrowthRatio = CalculateRealGrowthRatioComparedToIdealGrowth(yearOfObservationTrannum);

                yearOfObservationTrannum.RealGrowthRatio = realGrowthRatio;

                // Now that we have calculated the real growth ratio, we assign this to all other years within same group and indicate the table lookup method we will use
                foreach (var trannum in treeGroup)
                {
                    trannum.RealGrowthRatio = yearOfObservationTrannum.RealGrowthRatio;
                    trannum.CanLookupByEcodistrict = yearOfObservationTrannum.CanLookupByEcodistrict;
                }

                // Finally, calculate the estimated growth based on real growth ratio for all other years within same group (i.e. from plant date to the cut date)
                foreach (var data in treeGroup) CalculateEstimatedGrowth(data);
            }
        }

        private double CalculateRealGrowthRatioAverageConiferous(TrannumData trannumData)
        {
            trannumData.TreeSpecies = TreeSpecies.WhiteSpruce;

            var whiteSpruceRealGrowth = CalculateRealGrowthRatioComparedToIdealGrowth(trannumData);

            trannumData.TreeSpecies = TreeSpecies.ScotsPine;

            var scotsPineRealGrowth = CalculateRealGrowthRatioComparedToIdealGrowth(trannumData);

            var result = AverageTwo(scotsPineRealGrowth, whiteSpruceRealGrowth);

            trannumData.TreeSpecies = TreeSpecies.AverageConifer;

            return result;
        }

        private double CalculateRealGrowthRatioAverageDeciduous(TrannumData trannumData)
        {
            trannumData.TreeSpecies = TreeSpecies.ManitobaMaple;

            var manitobaMapleRealGrowth = CalculateRealGrowthRatioComparedToIdealGrowth(trannumData);

            trannumData.TreeSpecies = TreeSpecies.GreenAsh;

            var greenAshRealGrowth = CalculateRealGrowthRatioComparedToIdealGrowth(trannumData);

            var result = AverageTwo(greenAshRealGrowth, manitobaMapleRealGrowth);

            trannumData.TreeSpecies = TreeSpecies.AverageDeciduous;

            return result;
        }

        /// <summary>
        ///     Calculates the actual biomass of trees at the year of observation. Performs calucation of biomass only - not
        ///     carbon.
        /// </summary>
        public void CalculateBiomassPerTreeAtYearOfObservation(TrannumData trannumData)
        {
            // Calculate biomass per tree first
            if (trannumData.TreeSpecies == TreeSpecies.AverageConifer)
                trannumData.TotalBiomassPerTree = CalculateAverageConiferBiomass(trannumData);
            else if (trannumData.TreeSpecies == TreeSpecies.AverageDeciduous)
                trannumData.TotalBiomassPerTree = CalculateAverageDeciduousBiomass(trannumData);
            else
                trannumData.TotalBiomassPerTree = CalculateBiomassOfTrees(trannumData);

            // Calculate living biomass of all trees that are of the same species
            trannumData.TotalLivingBiomassForAllTreesOfSameType = CalculateTotalLivingBiomassPerTreeType(
                trannumData.TotalBiomassPerTree,
                trannumData.TreeCount);

            // Calculate living biomass of all trees that are of the same species per standard length (i.e. 1 km)
            trannumData.TotalLivingBiomassPerTreeTypePerStandardLength = CalculateTotalLivingBiomassPerStandardLength(
                trannumData.TotalLivingBiomassForAllTreesOfSameType,
                trannumData.RowLength);
        }

        /// <summary>
        ///     Compares the actual growth with that of the ideal growth for trees of the same age and species.
        /// </summary>
        public double CalculateRealGrowthRatioComparedToIdealGrowth(TrannumData trannumData)
        {
            // Convert total living biomass to total living carbon per standard length since lookup tables have values that are all measured in units of carbon
            trannumData.TotalLivingCarbonPerTreeTypePerStandardLength =
                CalculateTotalLivingCarbonPerTreeType(trannumData.TotalLivingBiomassPerTreeTypePerStandardLength);

            /*
             * Lookup tables do not have total living biomass for first three years (in some situations). If user has a year of observation that is in
             * these first few years of growth (i.e. age = 1, 2, 3) it will not be possible to calculate the real growth ratio. Go forward in time (and increment the age)
             * until we get a non-zero value and use that for comparison.
             */

            var age = trannumData.Age;
            var totalLivingBiomassCarbonOfIdealTree = 0d;
            if (trannumData.CanLookupByEcodistrict)
                do
                {
                    // Get total living biomass carbon of an ideal tree
                    totalLivingBiomassCarbonOfIdealTree = ShelterbeltCarbonDataProvider.GetLookupValue(
                        trannumData.TreeSpecies,
                        trannumData.EcodistrictId,
                        trannumData.PercentMortality,
                        trannumData.PercentMortalityLow,
                        trannumData.PercentMortalityHigh,
                        age,
                        ShelterbeltCarbonDataProviderColumns.Biom_Mg_C_km);

                    age++;
                } while (totalLivingBiomassCarbonOfIdealTree == 0 &&
                         age < CoreConstants.ShelterbeltCarbonTablesMaximumAge);
            else
                /*
                 * If we are outside of Saskatchewan, we won't have access to the cluster id that is needed to lookup live biomass values, instead we lookup values by
                 * hardiness zone instead.
                 */
                do
                {
                    // Get total living biomass carbon of an ideal tree
                    totalLivingBiomassCarbonOfIdealTree =
                        Table_12_Shelterbelt_Hardiness_Zone_Lookup_Provider.GetLookupValue(
                            trannumData.TreeSpecies,
                            trannumData.HardinessZone,
                            trannumData.PercentMortality,
                            trannumData.PercentMortalityLow,
                            trannumData.PercentMortalityHigh,
                            age,
                            ShelterbeltCarbonDataProviderColumns.Biom_Mg_C_km);

                    age++;
                } while (totalLivingBiomassCarbonOfIdealTree == 0 &&
                         age < CoreConstants.ShelterbeltCarbonTablesMaximumAge);

            var result = CalculateRealGrowthRatio(
                trannumData.TotalLivingCarbonPerTreeTypePerStandardLength,
                totalLivingBiomassCarbonOfIdealTree);

            return result;
        }

        /// <summary>
        ///     For all years except the year of observation/measurement, we need to estimate growth based on the calculated real
        ///     growth ratio. Calculate the estimated
        ///     growth (living biomass carbon) for the particular species at the given year.
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

                var whiteSpruceTotalEcosystemCarbon = GetIdealTotalEcosystemCarbon(trannumData);
                var whiteSpruceDeadOrganicMatterCarbon = GetIdealDeadOrganicMatter(trannumData);

                trannumData.TreeSpecies = TreeSpecies.ScotsPine;

                var scotsPineTotalEcosystemCarbon = GetIdealTotalEcosystemCarbon(trannumData);
                var scotsPineDeadOrganicMatterCarbon = GetIdealDeadOrganicMatter(trannumData);

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

                var manitobaMapleTotalEcoystemCarbon = GetIdealTotalEcosystemCarbon(trannumData);
                var manitobaMapleDeadOrganicMatterCarbon = GetIdealDeadOrganicMatter(trannumData);

                trannumData.TreeSpecies = TreeSpecies.GreenAsh;

                var greenAshTotalEcosystemCarbon = GetIdealTotalEcosystemCarbon(trannumData);
                var greenAshDeadOrganicMatterCarbon = GetIdealDeadOrganicMatter(trannumData);

                // Change back to original species
                trannumData.TreeSpecies = TreeSpecies.AverageDeciduous;

                totalEcosystemCarbon = AverageTwo(greenAshTotalEcosystemCarbon, manitobaMapleTotalEcoystemCarbon);
                deadOrganicMatter = AverageTwo(greenAshDeadOrganicMatterCarbon, manitobaMapleDeadOrganicMatterCarbon);
            }
            else
            {
                totalEcosystemCarbon = GetIdealTotalEcosystemCarbon(trannumData);
                deadOrganicMatter = GetIdealDeadOrganicMatter(trannumData);
            }

            var livingBiomass = totalEcosystemCarbon - deadOrganicMatter;

            // Equation 2.3.3-6
            var livingBiomassFraction = livingBiomass * trannumData.RealGrowthRatio;

            // Equation 2.3.4-2
            var deadOrganicMatterFraction = deadOrganicMatter * trannumData.RealGrowthRatio;

            // Calculate the estimated biomass carbon based on the real growth ratio
            trannumData.EstimatedTotalLivingBiomassCarbonBasedOnRealGrowth = livingBiomassFraction;

            // Calculate the estimated dead organic matter based on the real growth ratio
            trannumData.EstimatedDeadOrganicMatterBasedOnRealGrowth = deadOrganicMatterFraction;
        }

        /// <summary>
        ///     Once the yearly values have been calculated, we can then total up the values for each shelterbelt during each year.
        /// </summary>
        public List<TrannumResultViewItem> TotalResultsForEachYear(
            IEnumerable<ShelterbeltComponent> shelterbeltComponents)
        {
            var results = new List<TrannumResultViewItem>();

            foreach (var component in shelterbeltComponents)
            {
                var resultsForComponent = new List<TrannumResultViewItem>();

                // There may be several rows each with multiple groups all in the same year. We need to get a list of distinct years so we can group items by year.
                var distinctYears = component.TrannumData.Select(x => x.Year).Distinct();
                foreach (var year in distinctYears)
                {
                    var resultViewItem = new TrannumResultViewItem();

                    var viewItemsForYear = component.TrannumData.Where(x => Math.Abs(x.Year - year) < double.Epsilon)
                        .ToList();

                    resultViewItem.ShelterbeltComponent = component;
                    resultViewItem.Year = (int)year;
                    // Equation 2.3.3-7
                    resultViewItem.TotalLivingBiomassCarbon =
                        CalculateTotalShelterbeltBiomassCarbon(viewItemsForYear) / 1000; // Convert to Mg
                    resultViewItem.TotalDeadOrganicMatterCarbon =
                        CalculateTotalDeadOrganicMatter(viewItemsForYear) / 1000; // Convert to Mg
                    resultViewItem.TotalEcosystemCarbon = CalculateTotalEcosystemCarbon(
                        resultViewItem.TotalLivingBiomassCarbon,
                        resultViewItem.TotalDeadOrganicMatterCarbon);

                    resultsForComponent.Add(resultViewItem);
                }

                results.AddRange(resultsForComponent);

                // Next we calculate the changes in the total ecosystem carbon, living biomass carbon, and dead organic matter carbon from year to year
                for (var i = 0; i < resultsForComponent.Count; i++)
                {
                    if (i == 0)
                    {
                        // In the first year, there are no changes from the previous year
                        var resultViewItem = resultsForComponent.ElementAt(0);
                        resultViewItem.TotalDeadOrganicMatterChange = 0;
                        resultViewItem.TotalEcosystemCarbonChange = 0;
                        resultViewItem.TotalLivingBiomassCarbonChange = 0;

                        continue;
                    }

                    var previousYearViewItem = resultsForComponent.ElementAt(i - 1);
                    var currentYearViewItem = resultsForComponent.ElementAt(i);

                    currentYearViewItem.TotalDeadOrganicMatterChange =
                        currentYearViewItem.TotalDeadOrganicMatterCarbon -
                        previousYearViewItem.TotalDeadOrganicMatterCarbon;
                    currentYearViewItem.TotalEcosystemCarbonChange = currentYearViewItem.TotalEcosystemCarbon -
                                                                     previousYearViewItem.TotalEcosystemCarbon;
                    currentYearViewItem.TotalLivingBiomassCarbonChange = currentYearViewItem.TotalLivingBiomassCarbon -
                                                                         previousYearViewItem.TotalLivingBiomassCarbon;
                }
            }

            return results;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     An 'average' coniferous tree is the average biomass carbon of white spruce and scots pine at the same age
        /// </summary>
        private double CalculateAverageConiferBiomass(TrannumData trannumData)
        {
            trannumData.TreeSpecies = TreeSpecies.WhiteSpruce;

            var whiteSpruceBiomass = CalculateBiomassOfTrees(trannumData);

            trannumData.TreeSpecies = TreeSpecies.ScotsPine;

            var scotsPineBiomass = CalculateBiomassOfTrees(trannumData);

            var result = AverageTwo(scotsPineBiomass, whiteSpruceBiomass);

            trannumData.TreeSpecies = TreeSpecies.AverageConifer;

            return result;
        }

        /// <summary>
        ///     An 'average' deciduous tree is the average biomass carbon of green ash and manitoba maple trees at the same age
        /// </summary>
        private double CalculateAverageDeciduousBiomass(TrannumData trannumData)
        {
            trannumData.TreeSpecies = TreeSpecies.ManitobaMaple;

            var manitobaMapleBiomass = CalculateBiomassOfTrees(trannumData);

            trannumData.TreeSpecies = TreeSpecies.GreenAsh;

            var greenAshBiomass = CalculateBiomassOfTrees(trannumData);

            var result = AverageTwo(greenAshBiomass, manitobaMapleBiomass);

            trannumData.TreeSpecies = TreeSpecies.AverageDeciduous;

            return result;
        }

        private double CalculateBiomassOfTrees(TrannumData trannumData)
        {
            // Get the two lookup values (a, b) that will be used to calculate the above ground biomass of the tree
            var a = GetA(trannumData.TreeSpecies);
            var b = GetB(trannumData.TreeSpecies);

            // At this point, each trannum will have had a default circumference based on ideal trees. The user can now override the circumference values
            // for each individual year and the view model will recalculate the biomass/carbon values.
            var circumferencePerTree = trannumData.CircumferenceData.UserCircumference;

            var agtRatio = _shelterbeltAgTRatioProvider.GetAboveGroundBiomassTotalTreeBiomassRatio(
                trannumData.TreeSpecies,
                trannumData.Age);

            // AboveGroundBiomass_tree
            var aboveGroundBiomassPerTree = CalculateAboveGroundBiomassPerTree(
                circumferencePerTree,
                a,
                b);

            // Total biomass per tree (aboveground and belowground)
            var totalBiomassPerTree = CalculateTotalBiomassOfTree(
                aboveGroundBiomassPerTree,
                agtRatio);

            return totalBiomassPerTree;
        }

        #endregion

        #region Algorithms

        /// <summary>
        ///     Equation 2.3.1-1
        ///     Calculates the mortality of a row of trees given the total live count and the total planted count.
        /// </summary>
        /// <param name="plantedTreeCountAllSpecies">Number of trees originally planted into the linear planting </param>
        /// <param name="liveTreeCountAllSpecies">Number of trees alive for all species in a linear planting </param>
        /// <returns>Percent mortality of an entire linear planting (i.e. row). </returns>
        public double CalculatePercentMortalityOfALinearPlanting(
            List<double> plantedTreeCountAllSpecies,
            List<double> liveTreeCountAllSpecies)
        {
            var plantedTreesOfSameSpeciesSum = plantedTreeCountAllSpecies.Sum();
            var liveTreeCountAllSpeciesSum = liveTreeCountAllSpecies.Sum();

            var ratio = (plantedTreesOfSameSpeciesSum - liveTreeCountAllSpeciesSum) / plantedTreesOfSameSpeciesSum;

            var result = ratio * 100;

            return result;
        }

        /// <summary>
        ///     Equation 2.3.1-2
        ///     There are only 3 lookup values in the table for mortality. Convert user defined mortality to one of these lookup
        ///     values.
        /// </summary>
        /// <param name="percentMortalityOfALinearPlanting">Percent mortality of an entire linear planting (i.e. row). </param>
        /// <returns>
        ///     Percent mortality used for looking up values in Table 12. Represents the lesser value used in linear
        ///     interpolation.
        /// </returns>
        public int CalculateMortalityLow(double percentMortalityOfALinearPlanting)
        {
            if (percentMortalityOfALinearPlanting >= 30.0)
                return 30;
            if (percentMortalityOfALinearPlanting >= 15.0)
                return 15;
            return 0;
        }

        /// <summary>
        ///     Equation 2.3.1-3
        ///     There are only 3 lookup values in the table for mortality. Use one level up from the mortality low value.
        /// </summary>
        /// <param name="aboveMortalityLow">Above level of mortality low value.</param>
        /// <returns>
        ///     Percent mortality used for looking up values in Table 12. Represents the greater value used in linear
        ///     interpolation.
        /// </returns>
        /// <exception cref="Exception"></exception>
        public int CalculateMortalityHigh(int aboveMortalityLow)
        {
            switch (aboveMortalityLow)
            {
                case 0:
                    return 15;
                case 15:
                    return 30;
                case 30:
                    return 50;
                default:
                    throw new Exception(nameof(ShelterbeltCalculator) + "." + nameof(CalculateMortalityHigh) + "(int " +
                                        nameof(aboveMortalityLow) +
                                        ") must only receive the following values: 0, 15 or 30");
            }
        }

        /// <summary>
        ///     Equation 2.3.2-1
        /// </summary>
        /// <param name="circumference">
        ///     Average of the following property over one or more trees: cumulative tree stem
        ///     circumference (cm) at 1.3 m height along the individual stem (breast height) (outside bark)
        /// </param>
        /// <param name="coefficientA">Coefficient a from Table 11</param>
        /// <param name="coefficientB">Coefficient b from Table 11</param>
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
        ///     Equation 2.3.2-2
        /// </summary>
        /// <param name="diameter">Diameter of a circle (cm)</param>
        /// <returns>Circumference of a circle (cm)</returns>
        public double CalculateCircumferenceFromDiameter(double diameter)
        {
            return 3.14159 * diameter;
        }

        /// <summary>
        ///     Equation 2.3.2-3
        /// </summary>
        /// <param name="circumferences">Circumferences at breast height of an individual stem (cm)</param>
        /// <returns>
        ///     Cumulative tree stem circumference (cm) at 1.3m height along the individual stem (breast height) (outside
        ///     bark).
        /// </returns>
        public double CalculateTreeCircumference(List<double> circumferences)
        {
            var treeCircumference = 0.0;
            foreach (var circumference in circumferences) treeCircumference += circumference * circumference;

            treeCircumference = Math.Sqrt(treeCircumference);
            return treeCircumference;
        }

        /// <summary>
        ///     Equation 2.3.2-4
        /// </summary>
        /// <param name="treeCircumferences">List of all the tree circumferences. Helps us calculate number of trees sampled</param>
        /// <returns>Average circumference per tree.</returns>
        public double CalculateAverageCircumference(List<double> treeCircumferences)
        {
            var averageCircumference = 0.0;
            foreach (var circumference in treeCircumferences) averageCircumference += circumference;

            averageCircumference /= treeCircumferences.Count;

            return averageCircumference;
        }

        /// <summary>
        ///     Equation 2.3.2-5
        /// </summary>
        /// <param name="aboveGroundBiomassOfTree">Above ground biomass of tree (kg tree^-1)</param>
        /// <param name="aboveGroundBiomassRatio">
        ///     Fraction of aboveground over total biomass (total biomass = aboveground +
        ///     belowground), derived from Table 12, specific to the user-provided age and mortality of the shelterbelt (%/100)
        /// </param>
        /// <returns>Total tree biomass (aboveground + belowground) (kg tree^-1)</returns>
        public double CalculateTotalBiomassOfTree(
            double aboveGroundBiomassOfTree,
            double aboveGroundBiomassRatio)
        {
            return aboveGroundBiomassOfTree / aboveGroundBiomassRatio;
        }

        /// <summary>
        ///     Equation 2.1.6-3 (b)
        /// </summary>
        public double CalculateDiameterFromCircumference(double circumference)
        {
            return circumference / 3.14159;
        }

        /// <summary>
        ///     Equation 2.3.3-1
        /// </summary>
        /// <param name="rowLength">Length of a given linear planting.</param>
        /// <param name="treeSpacing">Space between one tree of a given kind and the next within a given linear planting.</param>
        /// <param name="percentMortality">Percent dead trees over planted trees</param>
        /// <returns>Number of live trees of a particular species/taxon within a given linear planting.</returns>
        public double CalculateTreeCount(
            double rowLength,
            double treeSpacing,
            double percentMortality)
        {
            return rowLength / treeSpacing * ((100 - percentMortality) / 100.0);
        }

        /// <summary>
        ///     Equation 2.3.3-2
        /// </summary>
        /// <param name="biomassPerTree">Total tree biomass (aboveground + belowground) (kg tree^-1)</param>
        /// <param name="treeCount">Total number of trees (of same type) within the row</param>
        /// <returns>Biomass of trees of a particular species within a row (kg row^-1)</returns>
        public double CalculateTotalLivingBiomassPerTreeType(
            double biomassPerTree,
            double treeCount)
        {
            return biomassPerTree * treeCount;
        }

        /// <summary>
        ///     Equation 2.3.3-3
        /// </summary>
        /// <param name="biomassOfAllTrees">Biomass of trees of the same species within a linear planting (kg planting-1)</param>
        /// <param name="rowLength">Length of the </param>
        /// <returns>Total tree biomass per standard length linear planting (kg km^-1)</returns>
        public double CalculateTotalLivingBiomassPerStandardLength(
            double biomassOfAllTrees,
            double rowLength)
        {
            return biomassOfAllTrees * (rowLength / 1000.0);
        }

        /// <summary>
        ///     Equation 2.3.3-4
        /// </summary>
        /// <param name="biomassPerKilometer">Biomass (kg km^-1)</param>
        /// <returns>Total carbon in the living biomass per standard length linear planting (kg C km^-1)</returns>
        public double CalculateTotalLivingCarbonPerTreeType(
            double biomassPerKilometer)
        {
            return biomassPerKilometer * CoreConstants.CarbonConcentrationOfTrees;
        }

        /// <summary>
        ///     Equation 2.3.3-5
        ///     Calculates the ratio of user specified growth (biomass carbon) over ideal tree growth (biomass carbon). User
        ///     specified growth is based on measure circumference/diameter and compared
        ///     to lookup tables of ideal tree biomass carbon values.
        /// </summary>
        /// <param name="calculatedTotalLivingCarbonKilogramPerStandardLength">
        ///     Total C stocks in the living biomass per standard
        ///     length linear planting (kg C km-1)
        /// </param>
        /// <param name="lookupTotalLivingCarbonMegagramsPerStandardLength">
        ///     Total C stocks per average (ideal) tree recorded for an area of similar geographical location (Saskatchewan)
        ///     or ecological condition (plant hardiness zone outside SK) (kg C km-1)
        /// </param>
        /// <returns>Ratio of user specified over average (ideal) tree growth</returns>
        public double CalculateRealGrowthRatio(
            double calculatedTotalLivingCarbonKilogramPerStandardLength,
            double lookupTotalLivingCarbonMegagramsPerStandardLength)
        {
            if (lookupTotalLivingCarbonMegagramsPerStandardLength == 0)
                // Assume value is not available and return 1 to indicate we are assuming an ideal tree for this year
                return 1;

            var result = calculatedTotalLivingCarbonKilogramPerStandardLength /
                         (lookupTotalLivingCarbonMegagramsPerStandardLength * 1000);

            return result;
        }

        /// <summary>
        /// </summary>
        public double CalculateTreeSpacing(
            double rowLength,
            double treeCount)
        {
            return rowLength / treeCount;
        }

        /// <summary>
        ///     Calculates the total above ground carbon for a number of trees of the same type
        /// </summary>
        public double CalculateCarbonForTreetype(
            double aboveGroundCarbonStocksPerTree,
            double treeCount)
        {
            return aboveGroundCarbonStocksPerTree * treeCount;
        }

        /// <summary>
        /// </summary>
        public double CalculateCarbonForLinearPlanting(List<double> aboveGroundCarbonForTreetypesInPlanting)
        {
            var aboveGroundCarbonForLinearPlanting = 0.0;
            foreach (var carbon in aboveGroundCarbonForTreetypesInPlanting)
                aboveGroundCarbonForLinearPlanting += carbon;

            return aboveGroundCarbonForLinearPlanting;
        }

        /// <summary>
        ///     Equation 2.3.3-1
        /// </summary>
        public double CalculateCarbonForShelterbelt(List<double> aboveGroundCarbonForLinearPlantingsInShelterbelt)
        {
            var aboveGroundCarbonForShelterbelt = 0.0;
            foreach (var carbon in aboveGroundCarbonForLinearPlantingsInShelterbelt)
                aboveGroundCarbonForShelterbelt += carbon;

            return aboveGroundCarbonForShelterbelt;
        }

        /// <summary>
        ///     Equation 2.3.5-1
        /// </summary>
        /// <param name="totalAboveGroundCarbonForShelterbelt">
        ///     Annual C accumulation in tree plantings/shelterbelt (kg C
        ///     shelterbelt-1 yr-1)
        /// </param>
        /// <returns>Annual CO2 sequestration from tree plantings/shelterbelt (kg CO2 shelterbelt-1 yr-1)</returns>
        public double CalculateCarbonDioxideSequesteredInShelterbelt(double totalAboveGroundCarbonForShelterbelt)
        {
            return totalAboveGroundCarbonForShelterbelt * CoreConstants.ConvertFromCToCO2 * -1.0;
        }

        /// <summary>
        ///     Equation 2.1.6-26
        /// </summary>
        /// <returns>The total dead organic matter carbon  (kg C km^-1)</returns>
        private double GetIdealDeadOrganicMatter(TrannumData trannumData)
        {
            var deadOrganicMatterMegagrams = 0d;
            if (trannumData.CanLookupByEcodistrict)
                // We can lookup by ecodistrict->cluster id mapping
                deadOrganicMatterMegagrams = ShelterbeltCarbonDataProvider.GetLookupValue(
                    trannumData.TreeSpecies,
                    trannumData.EcodistrictId,
                    trannumData.PercentMortality,
                    (int)trannumData.PercentMortalityLow,
                    (int)trannumData.PercentMortalityHigh,
                    trannumData.Age,
                    ShelterbeltCarbonDataProviderColumns.Dom_Mg_C_km);
            else
                // We need to lookup values by hardiness zone
                deadOrganicMatterMegagrams = Table_12_Shelterbelt_Hardiness_Zone_Lookup_Provider.GetLookupValue(
                    trannumData.TreeSpecies,
                    trannumData.HardinessZone,
                    trannumData.PercentMortality,
                    trannumData.PercentMortalityLow,
                    trannumData.PercentMortalityHigh,
                    trannumData.Age,
                    ShelterbeltCarbonDataProviderColumns.Dom_Mg_C_km);

            var deadOrganicMatterKilograms = deadOrganicMatterMegagrams * 1000;

            return deadOrganicMatterKilograms;
        }

        /// <summary>
        ///     Equation
        /// </summary>
        /// <returns>The total ecosystem carbon (kg C km^-1)</returns>
        private double GetIdealTotalEcosystemCarbon(TrannumData trannumData)
        {
            var totalEcosystemCarbonMegagrams = 0d;
            if (trannumData.CanLookupByEcodistrict)
                // We can lookup by ecodistrict->cluster id mapping
                totalEcosystemCarbonMegagrams = ShelterbeltCarbonDataProvider.GetLookupValue(
                    trannumData.TreeSpecies,
                    trannumData.EcodistrictId,
                    trannumData.PercentMortality,
                    (int)trannumData.PercentMortalityLow,
                    (int)trannumData.PercentMortalityHigh,
                    trannumData.Age,
                    ShelterbeltCarbonDataProviderColumns.Tec_Mg_C_km);
            else
                // We need to lookup values by hardiness zone
                totalEcosystemCarbonMegagrams = Table_12_Shelterbelt_Hardiness_Zone_Lookup_Provider.GetLookupValue(
                    trannumData.TreeSpecies,
                    trannumData.HardinessZone,
                    trannumData.PercentMortality,
                    trannumData.PercentMortalityLow,
                    trannumData.PercentMortalityHigh,
                    trannumData.Age,
                    ShelterbeltCarbonDataProviderColumns.Tec_Mg_C_km);

            var totalEcosystemCarbonKilograms = totalEcosystemCarbonMegagrams * 1000;

            return totalEcosystemCarbonKilograms;
        }

        /// <summary>
        ///     Equation 2.1.6-26
        /// </summary>
        /// <returns>The total living biomass carbon (kg C km^-1)</returns>
        private double GetIdealTotalLivingBiomassCarbon(TrannumData trannumData)
        {
            var totalLivingBiomassMegagrams = 0d;

            if (trannumData.CanLookupByEcodistrict)
                // We can lookup by ecodistrict->cluster id mapping
                totalLivingBiomassMegagrams = ShelterbeltCarbonDataProvider.GetLookupValue(
                    trannumData.TreeSpecies,
                    trannumData.EcodistrictId,
                    trannumData.PercentMortality,
                    (int)trannumData.PercentMortalityLow,
                    (int)trannumData.PercentMortalityHigh,
                    trannumData.Age,
                    ShelterbeltCarbonDataProviderColumns.Biom_Mg_C_km);
            else
                // We need to lookup values by hardiness zone
                totalLivingBiomassMegagrams = Table_12_Shelterbelt_Hardiness_Zone_Lookup_Provider.GetLookupValue(
                    trannumData.TreeSpecies,
                    trannumData.HardinessZone,
                    trannumData.PercentMortality,
                    trannumData.PercentMortalityLow,
                    trannumData.PercentMortalityHigh,
                    trannumData.Age,
                    ShelterbeltCarbonDataProviderColumns.Biom_Mg_C_km);

            var biomassCarbonKilograms = totalLivingBiomassMegagrams * 1000;

            return biomassCarbonKilograms;
        }

        /// <summary>
        ///     Equation 2.1.6-10
        ///     (kg C km^-1)
        /// </summary>
        public double CalculateTotalShelterbeltBiomassCarbon(IEnumerable<TrannumData> trannumData)
        {
            return trannumData.Sum(x => x.EstimatedTotalLivingBiomassCarbonBasedOnRealGrowth);
        }

        /// <summary>
        ///     Equation 2.1.6-26
        ///     (kg C km^-1)
        /// </summary>
        private double CalculateTotalDeadOrganicMatter(List<TrannumData> trannumData)
        {
            return trannumData.Sum(x => x.EstimatedDeadOrganicMatterBasedOnRealGrowth);
        }

        /// <summary>
        ///     (Mg C km^-1)
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