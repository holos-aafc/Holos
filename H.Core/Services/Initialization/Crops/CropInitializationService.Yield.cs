using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models;
using H.Core.Providers.Soil;
using H.Core.Services.LandManagement;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Drawing.Design;
using System.Linq;
using H.Infrastructure;

namespace H.Core.Services.Initialization.Crops
{
    public partial class CropInitializationService
    {
        #region Public Methods

        /// <summary>
        /// Assigns a yield to one view item for a field
        /// </summary>
        public void InitializeYieldForYear(
            Farm farm,
            CropViewItem viewItem,
            FieldSystemComponent fieldSystemComponent)
        {
            var yieldAssignmentMethod = farm.UseFieldLevelYieldAssignement ? fieldSystemComponent.YieldAssignmentMethod : farm.YieldAssignmentMethod;
            if (viewItem.CropType == CropType.NotSelected || viewItem.Year == 0)
            {
                Trace.TraceError($"{nameof(FieldResultsService)}.{nameof(InitializeYieldForYear)}: bad crop type or bad year for view item '{viewItem}'");

                viewItem.Yield = 0;
            }

            /*
             * The user will enter (or has entered) yields for each year manually, do not overwrite the value
             */

            if (yieldAssignmentMethod == YieldAssignmentMethod.Custom)
            {
                return;
            }

            /*
             * Use an average of the crops
             */

            if (yieldAssignmentMethod == YieldAssignmentMethod.Average)
            {
                double average = 0;

                // In CLI mode, the fields won't have CropViewItems, need to get average from DetailViewItems instead
                if (farm.IsCommandLineMode)
                {
                    var itemsByField = farm.GetDetailViewItemsForField(fieldSystemComponent);
                    average = itemsByField.Average(x => x.Yield);
                }
                else
                {
                    // Create an average from the crops that define the rotation
                    average = fieldSystemComponent.CropViewItems.Average(cropViewItem => cropViewItem.Yield);
                }

                viewItem.Yield = average;

                return;
            }

            if (yieldAssignmentMethod == YieldAssignmentMethod.SmallAreaData ||
                yieldAssignmentMethod == YieldAssignmentMethod.CARValue)
            {
                // If the cropviewitem is of a silage crop, we assign defaults using a different method.
                if (viewItem.CropType.IsSilageCropWithoutDefaults())
                {
                    this.InitializeSilageCropYield(viewItem, farm);
                }
                else
                {
                    this.InitializeYield(viewItem, farm);
                }

                return;
            }

            /*
             * Use values from a custom yield file whose path has been specified by the user
             */

            if (yieldAssignmentMethod == YieldAssignmentMethod.InputFile)
            {
                var results = new List<CustomUserYieldData>();

                foreach (var customYieldItem in farm.GeographicData.CustomYieldData)
                {
                    var yearMatch = customYieldItem.Year == viewItem.Year;
                    var fieldNameMatch = fieldSystemComponent.Name.IndexOf(customYieldItem.FieldName.Trim(), StringComparison.InvariantCultureIgnoreCase) >= 0;
                    var farmNameMatch = farm.Name.IndexOf(customYieldItem.RotationName.Trim(), StringComparison.InvariantCultureIgnoreCase) >= 0;


                    // Don't assign main year yields to a cover crop yield (for now)
                    if (yearMatch && fieldNameMatch && farmNameMatch && viewItem.IsSecondaryCrop == false)
                    {
                        results.Add(customYieldItem);
                    }
                }

                CustomUserYieldData yieldDataRow = null;
                if (results.Count > 1)
                {
                    yieldDataRow = results.FirstOrDefault(x => x.FieldName.Trim().Equals(fieldSystemComponent.Name.Trim(), StringComparison.InvariantCultureIgnoreCase));
                }
                else if (results.Count == 1)
                {
                    yieldDataRow = results.Single();
                }

                if (yieldDataRow != null)
                {
                    viewItem.Yield = yieldDataRow.Yield;
                }
                else
                {
                    Trace.TraceError($"{nameof(InitializationService)}.{nameof(InitializeYieldForYear)}: no custom yield data for {viewItem.Year} and {fieldSystemComponent.Name} was found in custom yield file. Attempting to assign a default yield for this year from the default yield provider.");

                    // With the Tier 2 model, we need to have yields for the run-in years. If the user loads a custom yield file, they might not have yields for this period. In this case,
                    // we check if we can get yields for these years by checking the small area data table.
                    this.InitializeYield(
                        viewItem: viewItem,
                        farm: farm);

                    if (viewItem.Yield == 0)
                    {
                        Trace.TraceError($"{nameof(InitializationService)}.{nameof(InitializeYieldForYear)}: no yield data for {viewItem.Year} and {fieldSystemComponent.Name} was found.");
                    }
                }

                return;
            }

            throw new Exception("Yield assignment method not accounted for");
        }

        public void InitializeYieldForAllYears(
            IEnumerable<CropViewItem> cropViewItems, 
            Farm farm,
            FieldSystemComponent fieldSystemComponent)
        {
            foreach (var viewItem in cropViewItems)
            {
                this.InitializeYieldForYear(farm, viewItem, fieldSystemComponent);
            }
        }

        /// <summary>
        /// Equation 2.1.2-13
        /// 
        /// Calculates the default yield for a silage crop using information from its grain crop equivalent.
        /// </summary>
        /// <param name="grainCropViewItem">The <see cref="H.Core.Models.LandManagement.Fields.CropViewItem"/> for the grain crop.</param>
        /// <returns>The estimated yield (dry matter) for a silage crop</returns>
        public double CalculateSilageCropYield(CropViewItem grainCropViewItem)
        {
            if (grainCropViewItem.BiomassCoefficientProduct == 0)
            {
                return 0;
            }

            var term1 = grainCropViewItem.Yield + grainCropViewItem.Yield * (grainCropViewItem.BiomassCoefficientStraw / grainCropViewItem.BiomassCoefficientProduct);
            var term2 = (1 + (grainCropViewItem.PercentageOfProductYieldReturnedToSoil / 100.0));
            var result = term1 * term2;

            return result;
        }

        /// <summary>
        /// Calculates the yield of silage crop using information from the grain crop equivalent to that silage crop e.g. if silage crop is Barley Silage, its grain equivalent will be
        /// Barley.
        /// </summary>
        /// <param name="silageCropViewItem">The <see cref="H.Core.Models.LandManagement.Fields.CropViewItem"/> representing the silage crop. </param>
        /// <param name="farm">The current farm of the user.</param>
        public void InitializeSilageCropYield(CropViewItem silageCropViewItem, Farm farm)
        {
            // Find the grain crop equivalent of the silage crop.
            var grainCrop = silageCropViewItem.CropType.GetGrainCropEquivalentOfSilageCrop();

            // Create a new CropViewItem that will represent this grain crop. It gets assigned the same year as the silage crop.
            var grainCropViewItem = new CropViewItem
            {
                Year = silageCropViewItem.Year,
                CropType = grainCrop,
            };

            // We call AssignSystemDefaults with the CropViewItem representing the grain crop to get its default values.
            this.InitializePercentageReturns(farm, grainCropViewItem);
            this.InitializeBiomassCoefficients(grainCropViewItem, farm);

            // We specifically find the PlantCarbonInAgriculturalProduct of the grain crop as that is needed in the yield calculation.
            grainCropViewItem.PlantCarbonInAgriculturalProduct = _icbmCarbonInputCalculator.CalculatePlantCarbonInAgriculturalProduct(previousYearViewItem: null, currentYearViewItem: grainCropViewItem, farm: farm);

            // We then calculate the wet and dry yield of the crop.
            silageCropViewItem.DryYield = CalculateSilageCropYield(grainCropViewItem: grainCropViewItem);
            silageCropViewItem.CalculateWetWeightYield();

            // No defaults for any grass silages so we use SAD data
            if (silageCropViewItem.CropType == CropType.GrassSilage)
            {
                this.InitializeYieldUsingSmallAreaData(silageCropViewItem, farm);
                silageCropViewItem.CalculateWetWeightYield();
            }
        }

        public void InitializeYield(Farm farm)
        {
            foreach (var viewItem in farm.GetAllCropViewItems())
            {
                this.InitializeYield(viewItem, farm);
            }
        }


        public void InitializeYield(CropViewItem viewItem, Farm farm)
        {
            if (viewItem.CropType.IsSilageCropWithoutDefaults())
            {
                this.InitializeSilageCropYield(viewItem, farm);
            }
            else
            {
                this.InitializeYieldUsingSmallAreaData(viewItem, farm);
            }
        }

        #endregion

        #region Private Methods

        private void InitializeYieldUsingSmallAreaData(CropViewItem viewItem, Farm farm)
        {
            var province = farm.Province;

            // No small area data exists for years > 2018, take average of last 10 years as placeholder values when considering these years
            const int NoDataYear = 2018;
            const int NumberOfYearsInAverage = 10;
            if (viewItem.Year > NoDataYear)
            {
                var startYear = NoDataYear - NumberOfYearsInAverage;
                var yields = new List<double>();
                for (int year = startYear; year <= NoDataYear; year++)
                {
                    var smallAreaYieldData = _smallAreaYieldProvider.GetData(
                        year: year,
                        polygon: farm.PolygonId,
                        cropType: viewItem.CropType,
                        province: province);

                    if (smallAreaYieldData != null)
                    {
                        yields.Add(smallAreaYieldData.Yield);
                    }
                }

                if (yields.Any())
                {
                    viewItem.Yield = Math.Round(yields.Average(), 1);
                    viewItem.DefaultYield = viewItem.Yield;
                }
                else
                {
                    Trace.TraceWarning($"No default yield found for {viewItem.CropType.GetDescription()} in {viewItem.Year}");
                }

                viewItem.CalculateDryYield();

                return;
            }

            var smallAreaYield = _smallAreaYieldProvider.GetData(
                year: viewItem.Year,
                polygon: farm.PolygonId,
                cropType: viewItem.CropType,
                province: province);

            if (smallAreaYield != null)
            {
                viewItem.Yield = smallAreaYield.Yield;
                viewItem.CalculateDryYield();
            }
            else
            {
                Trace.TraceWarning($"No default yield found for {viewItem.CropType.GetDescription()} in {viewItem.Year}");
            }
        }

        #endregion
    }
}