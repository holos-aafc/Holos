using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Infrastructure;

namespace H.Core.Services.LandManagement
{
    public partial class FieldResultsService
    {
        protected void ProcessPerennials(IEnumerable<CropViewItem> viewItems, FieldSystemComponent fieldSystemComponent)
        {
            _initializationService.AssignPerennialStandIds(viewItems, fieldSystemComponent);
            _initializationService.AssignPerennialStandPositionalYears(viewItems, fieldSystemComponent);
            _initializationService.AssignPerennialViewItemsDescription(viewItems);
            _initializationService.AssignPerennialRootsReturned(viewItems);
            _initializationService.AssignTillageToFinalYearOfPerennialStands(viewItems);
        }

        public void PostProcessPerennials(FieldSystemComponent fieldSystemComponent, Farm farm)
        {
            var stageState = farm.GetFieldSystemDetailsStageState();
            if (stageState == null)
            {
                return;
            }

            var viewItemsForField = stageState.DetailsScreenViewCropViewItems.Where(x => x.FieldSystemComponentGuid == fieldSystemComponent.Guid).OrderBy(x => x.Year).ToList();

            foreach (var cropViewItem in viewItemsForField)
            {
                var nextYear = cropViewItem.Year + 1;
                var hasCropInNextYear = viewItemsForField.SingleOrDefault(x => x.Year == nextYear && x.IsSecondaryCrop == false) != null;

                if (cropViewItem.CropType.IsPerennial() && cropViewItem.IsFinalYearInPerennialStand() && hasCropInNextYear == false)
                {
                    cropViewItem.PercentageOfRootsReturnedToSoil = 30;
                }
                else
                {
                    // Leave whatever was set for the last year (by default this will be 100% set at initialization)
                }

                if (farm.IsNonSwathingGrazingScenario(cropViewItem))
                {
                    var moistureContent = cropViewItem.GrazingViewItems.Any() ? cropViewItem.GrazingViewItems.Average(x => x.MoistureContentAsPercentage) : 1;
                    cropViewItem.MoistureContentOfCropPercentage = moistureContent;
                }

                // Check if there was a harvest in this year
                var harvestViewItems = cropViewItem.GetHayHarvestsByYear(cropViewItem.Year);
                if (harvestViewItems.Any())
                {
                    var moistureContent = harvestViewItems.Average(x => x.MoistureContentAsPercentage);
                    if (moistureContent == 0)
                    {
                        moistureContent = 1;
                    }

                    cropViewItem.MoistureContentOfCropPercentage = moistureContent;
                }
            }
        }
    }
}