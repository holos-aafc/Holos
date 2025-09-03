﻿using H.Core.Enumerations;
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
        /// It is assumed that 30% of the root biomass is turned over (input) annually before harvest, and 100% on harvest for perennials
        /// </summary>
        public void AssignPerennialRootsReturned(IEnumerable<CropViewItem> list)
        {
            var perennialStands = list.Where(x => x.CropType.IsPerennial()).GroupBy(x => x.PerennialStandGroupId);
            foreach (var currentPerennialStand in perennialStands)
            {
                for (var index = 0; index < currentPerennialStand.OrderBy(x => x.Year).Count(); index++)
                {
                    var cropViewItem = currentPerennialStand.ElementAt(index);

                    if (cropViewItem.YearInPerennialStand == cropViewItem.PerennialStandLength)
                    {
                        if (cropViewItem.CropType == CropType.RangelandNative)
                        {
                            // Range lands are never harvested and so we continue with 30% root turnover
                            cropViewItem.PercentageOfRootsReturnedToSoil = 30;
                        }
                        else
                        {
                            // Last year of stand is when 100% of roots are returned
                            cropViewItem.PercentageOfRootsReturnedToSoil = 100;
                        }
                    }
                    else
                    {
                        // In years leading up to the last, only 30% of roots are returned
                        cropViewItem.PercentageOfRootsReturnedToSoil = 30;
                    }
                }
            }
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