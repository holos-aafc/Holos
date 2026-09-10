using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using System;
using System.Collections.Generic;
using System.Linq;

namespace H.Core.Services.Initialization.Crops
{
    public partial class CropInitializationService
    {
        #region Public Methods

        public void InitializePerennialDefaults(Farm farm)
        {
            foreach (var viewItem in farm.GetAllCropViewItems())
            {
                this.InitializePerennialDefaults(viewItem, farm);
            }
        }

        public void InitializePerennialDefaults(CropViewItem viewItem, Farm farm)
        {
            if (farm != null && viewItem != null && viewItem.CropType.IsPerennial())
            {
                viewItem.TillageType = TillageType.NoTill;
                viewItem.PastTillageType = TillageType.NoTill;
                viewItem.FertilizerApplicationMethodology = FertilizerApplicationMethodologies.Broadcast;
                viewItem.ForageUtilizationRate = _utilizationRatesForLivestockGrazingProvider.GetUtilizationRate(viewItem.CropType);
                viewItem.TotalBiomassHarvest = viewItem.DefaultYield;
                viewItem.IsNativeGrassland = viewItem.CropType == CropType.RangelandNative;
            }
        }

        /// <summary>
        /// The share of a perennial's root biomass turned over, and so returned to the soil, in a year the stand
        /// continues. The algorithm document gives Sr as 30% in every year but the termination year.
        /// </summary>
        private const double AnnualRootTurnoverPercentage = 30;

        /// <summary>
        /// It is assumed that 30% of the root biomass is turned over (input) annually before harvest, and 100% on harvest for perennials
        /// </summary>
        public void AssignPerennialRootsReturned(IEnumerable<CropViewItem> list)
        {
            var viewItemsForField = list.ToList();

            var perennialStands = viewItemsForField.Where(x => x.CropType.IsPerennial()).GroupBy(x => x.PerennialStandGroupId);
            foreach (var currentPerennialStand in perennialStands)
            {
                foreach (var cropViewItem in currentPerennialStand)
                {
                    // If the user has manually overridden the residue return values, skip this crop and preserve their custom values.
                    // This allows users to specify their own percentages for product, straw, roots, and extraroots returned to soil
                    // instead of using the default calculations. The user can set this override via the "Override residue return values"
                    // radio button on the Residue tab in the UI.
                    if (cropViewItem.OverrideResidueReturnedToSoilDefaults == true)
                    {
                        continue;
                    }

                    cropViewItem.PercentageOfRootsReturnedToSoil = IsStandTerminated(cropViewItem, viewItemsForField)
                        ? 100
                        : AnnualRootTurnoverPercentage;
                }
            }
        }

        /// <summary>
        /// The whole root mass is returned only when the stand is actually ploughed under and a different crop follows
        /// it. Every other year - and every year of a stand that is never terminated - keeps the annual turnover.
        ///
        /// The "a different crop follows it" half of that used to be decided in
        /// <see cref="H.Core.Services.LandManagement.FieldResultsService.PostProcessPerennials"/>, which runs after the
        /// carbon inputs derived from this percentage have already been calculated, so the correction never reached
        /// them: a stand running to the end of the simulation had its root carbon worked out at 100% while the details
        /// screen displayed 30%. Deciding it here settles the value before anything reads it.
        /// </summary>
        private static bool IsStandTerminated(CropViewItem cropViewItem, List<CropViewItem> viewItemsForField)
        {
            if (cropViewItem.IsFinalYearInPerennialStand() == false)
            {
                return false;
            }

            // Range lands are never harvested and so continue with the annual root turnover
            if (cropViewItem.CropType == CropType.RangelandNative)
            {
                return false;
            }

            // A stand that lasts to the end of the simulation is not ploughed under either. The algorithm document
            // states this directly: a continuous perennial crop lasting until the end of the simulation period returns
            // the annual turnover each year, "incl. the final year of the simulation".
            return viewItemsForField.Any(x => x.Year == cropViewItem.Year + 1 && x.IsSecondaryCrop == false);
        }

        public void AssignPerennialViewItemsDescription(IEnumerable<CropViewItem> viewItems)
        {
            foreach (var cropViewItem in viewItems)
            {
                if (cropViewItem.CropType.IsPerennial())
                {
                    cropViewItem.Description = string.Format(H.Core.Properties.Resources.LabelYearXInYYearPerennialStand, cropViewItem.YearInPerennialStand, cropViewItem.PerennialStandLength);
                }
            }
        }

        public void AssignTillageToFinalYearOfPerennialStands(IEnumerable<CropViewItem> viewItems)
        {
            foreach (var cropViewItem in viewItems)
            {
                if (cropViewItem.CropType.IsPerennial())
                {
                    if (cropViewItem.YearInPerennialStand == cropViewItem.PerennialStandLength)
                    {
                        if (cropViewItem.CropType == CropType.RangelandNative)
                        {
                            // Rangelands are never tilled
                            cropViewItem.TillageType = TillageType.NoTill;
                        }
                        else
                        {
                            cropViewItem.TillageType = TillageType.Reduced;
                        }
                    }
                    else
                    {
                        cropViewItem.TillageType = TillageType.NoTill;
                    }
                }
            }
        }

        public void AssignPerennialStandPositionalYears(
            IEnumerable<CropViewItem> viewItems, 
            FieldSystemComponent fieldSystemComponent)
        {
            var groups = viewItems.Where(x => x.CropType.IsPerennial()).GroupBy(x => x.PerennialStandGroupId);
            foreach (var group in groups)
            {
                var distinctYears = group.Select(x => x.Year).Distinct();
                var totalLength = distinctYears.Count();

                for (int i = 0; i < distinctYears.Count(); i++)
                {
                    var position = i + 1;
                    var year = distinctYears.ElementAt(i);
                    var itemsByYear = group.Where(x => x.Year == year);
                    var mainCropForYear = itemsByYear.SingleOrDefault(x => x.IsSecondaryCrop == false);
                    var secondaryCropForYear = itemsByYear.SingleOrDefault(x => x.IsSecondaryCrop);

                    if (i == 0)
                    {
                        if (mainCropForYear != null)
                        {
                            mainCropForYear.YearInPerennialStand = position;
                            mainCropForYear.PerennialStandLength = totalLength;

                            continue;
                        }

                        // If year == 1, consider secondary crop as first year in stand        
                        if (secondaryCropForYear != null)
                        {
                            secondaryCropForYear.YearInPerennialStand = position;
                            secondaryCropForYear.PerennialStandLength = totalLength;

                            continue;
                        }
                    }
                    else
                    {
                        if (mainCropForYear != null && mainCropForYear.CropType.IsPerennial())
                        {
                            mainCropForYear.YearInPerennialStand = position;
                            mainCropForYear.PerennialStandLength = totalLength;
                        }
                    }
                }
            }
        }

        public IEnumerable<IGrouping<Guid, CropViewItem>> AssignPerennialStandIds(
            IEnumerable<CropViewItem> viewItems, 
            FieldSystemComponent fieldSystemComponent)
        {
            var distinctYears = viewItems.Select(y => y.Year).Distinct().OrderBy(x => x);

            var currentPerennialId = Guid.NewGuid();
            var lastCropType = CropType.None;

            foreach (var year in distinctYears)
            {
                var mainCrop = viewItems.Single(x => x.Year == year && x.IsSecondaryCrop == false);
                var secondaryCrop = viewItems.SingleOrDefault(x => x.Year == year && x.IsSecondaryCrop == true);

                if (secondaryCrop == null)
                {
                    // Consider case where only one crop grown in the year (no undersown or cover crop)

                    if (mainCrop.CropType.IsPerennial())
                    {
                        if (mainCrop.CropType != lastCropType)
                        {
                            currentPerennialId = Guid.NewGuid();
                        }

                        mainCrop.PerennialStandGroupId = currentPerennialId;
                        lastCropType = mainCrop.CropType;
                    }
                    else
                    {
                        currentPerennialId = Guid.NewGuid();
                    }

                    continue;
                }
                else
                {
                    if (mainCrop.CropType.IsPerennial())
                    {
                        if (mainCrop.CropType != lastCropType)
                        {
                            currentPerennialId = Guid.NewGuid();
                        }

                        mainCrop.PerennialStandGroupId = currentPerennialId;
                        lastCropType = mainCrop.CropType;
                    }
                    else
                    {
                        currentPerennialId = Guid.NewGuid();
                    }

                    if (secondaryCrop.CropType.IsPerennial())
                    {
                        if (secondaryCrop.CropType != lastCropType)
                        {
                            currentPerennialId = Guid.NewGuid();
                        }

                        secondaryCrop.PerennialStandGroupId = currentPerennialId;
                        lastCropType = secondaryCrop.CropType;
                    }
                    else
                    {
                        currentPerennialId = Guid.NewGuid();
                    }
                }
            }

            var groupsOfStands = viewItems.Where(x => x.PerennialStandGroupId != Guid.Empty).GroupBy(x => x.PerennialStandGroupId);

            return groupsOfStands;
        }


        #endregion
    }
}