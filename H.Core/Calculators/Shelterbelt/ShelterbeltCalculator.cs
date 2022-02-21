using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly List<TrannumData> _trannums = new List<TrannumData>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the A value for a species that is present in Beyhan Amichev's paper: Carbon sequestration and growth of six common tree and shrub shelterbelts in Saskatchewan, Canada
        /// </summary>
        public double GetA(TreeSpecies species)
        {
            var entry = ShelterbeltAllometricTableProvider.GetShelterbeltAllometricTable()
                .Find(x => x.TreeSpecies == species);
            if (entry == null)
                throw new Exception(
                    "GetA received a species which does not exist in table (Must not average the coefficients, calculate carbon first.) : " +
                    species.GetDescription());
            return entry.A;
        }

        /// <summary>
        /// Gets the B value for a species that is present in Beyhan Amichev's paper: Carbon sequestration and growth of six common tree and shrub shelterbelts in Saskatchewan, Canada
        /// </summary>
        public double GetB(TreeSpecies species)
        {
            var entry = ShelterbeltAllometricTableProvider.GetShelterbeltAllometricTable()
                .Find(x => x.TreeSpecies == species);
            if (entry == null)
                throw new Exception(
                    "GetA received a species which does not exist in table (Must not average the coefficients, calculate carbon first.) : " +
                    species.GetDescription());
            return entry.B;
        }

        public double AverageTwo(double one, double two)
        {
            return (one + two) / 2.0;
        }

        public void CalculateInitialResults(ShelterbeltComponent component)
        {
            component.BuildTrannums();

            this.CalculateInitialResults(component.TrannumData);
        }

        public void CalculateInitialResults(
            IEnumerable<TrannumData> trannumData)
        {
            _trannums.Clear();
            _trannums.AddRange(trannumData);

            // Group trannums by treetype since we could have two groups of trees with the same year of observation in the row
            var groups = _trannums.GroupBy(x => x.TreeGroupGuid);
            foreach (var treeGroup in groups)
            {
                /*
                 * User has entered the circumference of the trees at the year of observation (year of measurement). We can only calculate biomass/carbon
                 * for this one year. Once we have these calculated biomass/carbon values, we compare these against the lookup values for TEC, BIOM, DOM, etc. and
                 * get a ratio of actual growth to ideal (lookup) growth.
                 *
                 * With this ratio we then lookup values for TEC, BIOM, DOM for all other years and multiply by the calculated ratio to get estimated growth in past/future years.
                 */

                var yearOfObservationTrannum = treeGroup.Single(x => Math.Abs(x.Year - x.YearOfObservation) < double.Epsilon);

                // Calculate results for the year of observation
                this.CalculateBiomassPerTree(yearOfObservationTrannum);
                yearOfObservationTrannum.RealGrowthRatio = this.CalculateRealGrowthRatio(yearOfObservationTrannum);

                // Now that we have calculated the real grown ratio, we assign this to all other years
                foreach (var trannum in trannumData)
                {
                    trannum.RealGrowthRatio = yearOfObservationTrannum.RealGrowthRatio;
                }

                // Calculate growth based on real growth ratio for all other years (i.e. from plant date to the cut date)
                foreach (var data in trannumData)
                {
                    this.CalculateEstimatesBasedOnRealGrowth(data);
                }
            }
        }

        public void CalculateBiomassPerTree(TrannumData trannumData)
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
                trannumData.TotalBiomassPerTree = this.CalculateBiomassOfNonAverageTree(trannumData);
            }

            // Biomass_treetype
            trannumData.BiomassPerTreeType = this.CalculateBiomassPerTreeType(
                biomassPerTree: trannumData.TotalBiomassPerTree,
                treeCount: trannumData.TreeCount);

            // TLB_treetype
            trannumData.TotalLivingBiomassPerTreeType = this.CalculateTotalLivingBiomassPerStandardLength(
                biomassOfAllTrees: trannumData.BiomassPerTreeType,
                rowLength: trannumData.RowLength);
        }

        public double CalculateRealGrowthRatio(TrannumData trannumData)
        {
            // TLC_treetype
            trannumData.TotalLivingCarbonPerTreeType = this.CalculateTotalLivingCarbonPerTreeType(trannumData.TotalLivingBiomassPerTreeType);

            var biomasCarbonPerKilometer = ShelterbeltCarbonDataProvider.GetInterpolatedValue(
                treeSpecies: trannumData.TreeSpecies,
                hardinessZone: trannumData.HardinessZone,
                ecodistrictId: trannumData.EcodistrictId,
                percentMortality: trannumData.PercentMortality,
                mortalityLow: (int)trannumData.PercentMortalityLow,
                mortalityHigh: (int)trannumData.PercentMortalityHigh,
                age: trannumData.Age,
                column: ShelterbeltCarbonDataProvider.Columns.Biom_Mg_C_km,
                year: (int)trannumData.Year);

            // Convert to kilograms
            trannumData.BiomasCarbonPerKilometerInKilograms = biomasCarbonPerKilometer * 1000;

            // Calculate the real growth ratio
            var result = this.CalculateRealGrowthRatio(
                calculatedTotalLivingCarbon: trannumData.TotalLivingCarbonPerTreeType,
                lookupTotalLivingCarbon: trannumData.BiomasCarbonPerKilometerInKilograms);

            return result;
        }

        /// <summary>
        /// For all years except the year of observation/measurement, we need to estimate growth based on the real growth.
        /// </summary>
        public void CalculateEstimatesBasedOnRealGrowth(TrannumData trannumData)
        {
            var tec = 0d;
            var dom = 0d;

            // Check if tree is average type
            if (trannumData.TreeSpecies == TreeSpecies.AverageConifer)
            {
                trannumData.TreeSpecies = TreeSpecies.WhiteSpruce;

                var whiteSpruceTec = this.GetTecFarm(trannumData);
                var whiteSpruceDom = this.GetDomFarm(trannumData);

                trannumData.TreeSpecies = TreeSpecies.WhiteSpruce;

                var scotsPineTec = this.GetTecFarm(trannumData);
                var scotsPineDom = this.GetDomFarm(trannumData);

                // Change back to original species
                trannumData.TreeSpecies = TreeSpecies.AverageConifer;

                tec = AverageTwo(whiteSpruceTec, scotsPineTec);
                dom = AverageTwo(whiteSpruceDom, scotsPineDom);

            }
            else if (trannumData.TreeSpecies == TreeSpecies.AverageDeciduous)
            {
                trannumData.TreeSpecies = TreeSpecies.WhiteSpruce;

                var manitobaMapleTec = this.GetTecFarm(trannumData);
                var manitobaMapleDom = this.GetDomFarm(trannumData);

                trannumData.TreeSpecies = TreeSpecies.WhiteSpruce;

                var greenAshTec = this.GetTecFarm(trannumData);
                var greenAshDom = this.GetDomFarm(trannumData);

                // Change back to original species
                trannumData.TreeSpecies = TreeSpecies.AverageDeciduous;

                tec = AverageTwo(greenAshTec, greenAshDom);
                dom = AverageTwo(manitobaMapleTec, manitobaMapleDom);
            }
            else
            {
                tec = this.GetTecFarm(trannumData);
                dom = this.GetDomFarm(trannumData);
            }

            var biom = tec - dom;

            // Use ratio of actual tree 
            var domRatio = dom * trannumData.RealGrowthRatio;
            var biomRatio = biom * trannumData.RealGrowthRatio;

            // Calculate the estimated biomass carbon based on the real growth ratio
            trannumData.EstimatedBiomassCarbonBasedOnRealGrowth = biomRatio;

            // Calculate the estimated dead organic matter based on the real growth ratio
            trannumData.EstimatedDeadOrganicMatterBasedOnRealGrowth = domRatio;
        }

        /// <summary>
        /// Once the yearly values have been calculated, we can then total up the biomass/carbon/CO2 values for each shelterbelt.
        /// </summary>
        public List<TrannumResultViewItem> CalculateFinalResults(
            IEnumerable<ShelterbeltComponent> shelterbeltComponents)
        {
            var results = new List<TrannumResultViewItem>();

            foreach (var component in shelterbeltComponents)
            {
                if (component.StageStateSet == false)
                {
                    this.CalculateInitialResults(component); 
                }

                var resultsForComponent = new List<TrannumResultViewItem>();

                var distinctYears = component.TrannumData.Select(x => x.Year).Distinct();
                foreach (var year in distinctYears)
                {
                    var resultViewItem = new TrannumResultViewItem();

                    var viewItemsForYear = component.TrannumData.Where(x => Math.Abs(x.Year - year) < double.Epsilon).ToList();

                    resultViewItem.ShelterbeltComponent = component;
                    resultViewItem.Year = (int)year;

                    resultViewItem.Age = (int) viewItemsForYear.First().Age; // All items in the same year in the same shelterbelt will have the same age

                    resultViewItem.TotalShelterbeltBiomassCarbon = this.CalculateTotalShelterbeltBiomassCarbon(viewItemsForYear);
                    resultViewItem.TotalDeadOrganicMatter = this.CalculateTotalDeadOrganicMatter(viewItemsForYear);
                    resultViewItem.TotalEquivalentCarbon = this.CalculateTotalEquivalentCarbon(
                        biomassCarbon: resultViewItem.TotalShelterbeltBiomassCarbon,
                        deadOrganicMatterCarbon: resultViewItem.TotalDeadOrganicMatter);

                    resultsForComponent.Add(resultViewItem);
                }

                results.AddRange(resultsForComponent);

                for (int i = 0; i < resultsForComponent.Count; i++)
                {
                    if (i == 0)
                    {
                        var resultViewItem = resultsForComponent.ElementAt(0);
                        resultViewItem.TotalDeadOrganicMatterDelta = 0;
                        resultViewItem.TotalEquivalentCarbonDelta = 0;
                        resultViewItem.TotalShelterbeltBiomassCarbonDelta = 0;

                        continue;
                    }

                    var previousYearViewItem = resultsForComponent.ElementAt(i - 1);
                    var currentYearViewItem = resultsForComponent.ElementAt(i);

                    /*
                     * Calculate the changes (delta) from year to year
                     */
                    currentYearViewItem.TotalDeadOrganicMatterDelta = currentYearViewItem.TotalDeadOrganicMatter - previousYearViewItem.TotalDeadOrganicMatter;
                    currentYearViewItem.TotalEquivalentCarbonDelta = currentYearViewItem.TotalEquivalentCarbon - previousYearViewItem.TotalEquivalentCarbon;
                    currentYearViewItem.TotalShelterbeltBiomassCarbonDelta = currentYearViewItem.TotalShelterbeltBiomassCarbon - previousYearViewItem.TotalShelterbeltBiomassCarbon;
                }
            }

            return results;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// An 'average' coniferous tree is the average of white spruce and scots pine
        /// </summary>
        private double CalculateAverageConiferBiomass(TrannumData trannumData)
        {
            trannumData.TreeSpecies = TreeSpecies.WhiteSpruce;

            var whiteSpruceBiomass = this.CalculateBiomassOfNonAverageTree(trannumData);

            trannumData.TreeSpecies = TreeSpecies.ScotsPine;

            var scotsPineBiomass = this.CalculateBiomassOfNonAverageTree(trannumData);

            var result = this.AverageTwo(scotsPineBiomass, whiteSpruceBiomass);

            trannumData.TreeSpecies = TreeSpecies.AverageConifer;

            return result;
        }

        /// <summary>
        /// An 'average' deciduous tree is the average of green ash and manitoba maple
        /// </summary>
        private double CalculateAverageDeciduousBiomass(TrannumData trannumData)
        {
            trannumData.TreeSpecies = TreeSpecies.ManitobaMaple;

            var manitobaMapleBiomass = this.CalculateBiomassOfNonAverageTree(trannumData);

            trannumData.TreeSpecies = TreeSpecies.GreenAsh;

            var greenAshBiomass = this.CalculateBiomassOfNonAverageTree(trannumData);

            var result = this.AverageTwo(greenAshBiomass, manitobaMapleBiomass);

            trannumData.TreeSpecies = TreeSpecies.AverageDeciduous;

            return result;
        }

        private double CalculateBiomassOfNonAverageTree(TrannumData trannumData)
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
        /// Calculates the ratio of user specified over average (ideal) tree growth.
        /// </summary>
        public double CalculateRealGrowthRatio(
            double calculatedTotalLivingCarbon,
            double lookupTotalLivingCarbon)
        {
            if (lookupTotalLivingCarbon == 0)
            {
                // Assume value is not available and return 1 to indicate we are assuming an ideal tree for this year
                return 1;
            }

            return calculatedTotalLivingCarbon / lookupTotalLivingCarbon;
        }

        /// <summary>
        /// Equation 2.1.6-26 
        ///
        /// Calculates the total carbon gain in the current year
        /// </summary>
        public double CalculateTotalEquivalentCarbon(
            double domFarm,
            double tecTreeyTypeCurrentYear,
            double tecTreeTypePreviousYear,
            double rowLength,
            double standardLength)
        {
            var a = (domFarm + (tecTreeyTypeCurrentYear - tecTreeTypePreviousYear));
            var b = rowLength / standardLength;

            var result = a / b;

            return result;
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
        public double CalculateEstimatedCarbonInLivingTreeBiomass(
            double biomassLookup,
            double ratio)
        {
            return biomassLookup * ratio;
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
        /// <returns>The total dead organic matter (kg C km^-1)</returns>
        private double GetDomFarm(TrannumData trannumData)
        {
            var deadOrganicMatterMegagrams = ShelterbeltCarbonDataProvider.GetInterpolatedValue(
                treeSpecies: trannumData.TreeSpecies,
                hardinessZone: trannumData.HardinessZone,
                ecodistrictId: trannumData.EcodistrictId,
                percentMortality: trannumData.PercentMortality,
                mortalityLow: (int)trannumData.PercentMortalityLow,
                mortalityHigh: (int)trannumData.PercentMortalityHigh,
                age: trannumData.Age,
                column: ShelterbeltCarbonDataProvider.Columns.Dom_Mg_C_km,
                year: (int)trannumData.Year);

            var deadOrganicMatterKilograms = deadOrganicMatterMegagrams * 1000;

            return deadOrganicMatterKilograms;
        }

        /// <summary>
        /// Equation 2.1.6-26
        /// </summary>
        /// <returns>The total dead organic matter (kg C km^-1)</returns>
        private double GetTecFarm(TrannumData trannumData)
        {
            var deadOrganicMatterMegagrams = ShelterbeltCarbonDataProvider.GetInterpolatedValue(
                treeSpecies: trannumData.TreeSpecies,
                hardinessZone: trannumData.HardinessZone,
                ecodistrictId: trannumData.EcodistrictId,
                percentMortality: trannumData.PercentMortality,
                mortalityLow: (int)trannumData.PercentMortalityLow,
                mortalityHigh: (int)trannumData.PercentMortalityHigh,
                age: trannumData.Age,
                column: ShelterbeltCarbonDataProvider.Columns.Tec_Mg_C_km,
                year: (int)trannumData.Year);

            var deadOrganicMatterKilograms = deadOrganicMatterMegagrams * 1000;

            return deadOrganicMatterKilograms;
        }

        /// <summary>
        /// Equation 2.1.6-26
        /// </summary>
        /// <returns>The total living biomass carbon (kg C km^-1)</returns>
        private double GetBiomFarm(TrannumData trannumData)
        {
            var deadOrganicMatterMegagrams = ShelterbeltCarbonDataProvider.GetInterpolatedValue(
                treeSpecies: trannumData.TreeSpecies,
                hardinessZone: trannumData.HardinessZone,
                ecodistrictId: trannumData.EcodistrictId,
                percentMortality: trannumData.PercentMortality,
                mortalityLow: (int)trannumData.PercentMortalityLow,
                mortalityHigh: (int)trannumData.PercentMortalityHigh,
                age: trannumData.Age,
                column: ShelterbeltCarbonDataProvider.Columns.Biom_Mg_C_km,
                year: (int)trannumData.Year);

            var biomassCarbonKilograms = deadOrganicMatterMegagrams * 1000;

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
        /// (kg C km^-1)
        /// </summary>
        private double CalculateTotalEquivalentCarbon(
            double biomassCarbon,
            double deadOrganicMatterCarbon)
        {
            return biomassCarbon + deadOrganicMatterCarbon;
        }

        #endregion
    }
}