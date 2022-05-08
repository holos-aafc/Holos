using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models.Results;
using H.Infrastructure;

namespace H.Core.Services.LandManagement
{
    public partial class FieldResultsService
    {
        /// <summary>
        /// Equation 9.4-5
        /// 
        /// Returns a list of harvest view items for a field if in multiyear mode or a list with a single item when in single year mode.
        /// </summary>
        public List<EstimatesOfProductionResultsViewItem> CalculateHarvestForField(FieldSystemComponent fieldSystemComponent, Farm farm)
        {
            var results = new List<EstimatesOfProductionResultsViewItem>();

            if (farm.EnableCarbonModelling == false)
            {
                // Use single year item only
                var singleYearViewItem = fieldSystemComponent.GetSingleYearViewItem();
                if (singleYearViewItem != null)
                {
                    // User must specify at least one harvest view item for a harvest to be calculated
                    if (singleYearViewItem.CropType.IsPerennial() && singleYearViewItem.HasHarvestViewItems == false)
                    {
                        return results;
                    }

                    var name = fieldSystemComponent.Name + " (" + singleYearViewItem.CropTypeString + ")";

                    var harvest = this.CalculateHarvest(singleYearViewItem);
                    results.Add(new EconomicsResultsViewItem()
                    {
                        ComponentName = fieldSystemComponent.Name,
                        GroupingString = Properties.Resources.TitleCrops,
                        Name = name,
                        Harvest = Math.Round(harvest, 0),
                    });
                }
            }
            else
            {
                if (!farm.StageStates.Any())
                {
                    this.GetStageState(farm);
                }

                // Create a view item for each year and crop
                var detailViewItems = farm.GetDetailViewItemsForField(fieldSystemComponent);
                foreach (var cropViewItem in detailViewItems)
                {
                    var harvest = this.CalculateHarvest(cropViewItem);

                    results.Add(new EconomicsResultsViewItem()
                    {
                        Year = cropViewItem.Year,
                        ComponentName = fieldSystemComponent.Name,
                        GroupingString = Properties.Resources.TitleCrops,
                        Name = fieldSystemComponent.Name + " (" + cropViewItem.Year + ") " + " " + cropViewItem.CropTypeString,
                        Harvest = Math.Round(harvest, 0),
                    });
                }
            }

            return results;
        }

        public double CalculateHarvest(CropViewItem viewItem)
        {
            // If a perennial is grown, the yield will contain the summation of all harvested view items (biomasses) entered by the user for that year (# bales x bale weight)
            if (viewItem.CropType.IsPerennial())
            {
                if (viewItem.HasHarvestViewItems)
                {
                    return viewItem.TotalBiomassHarvest;
                }
                return viewItem.Yield;
            }
            else
            {
                // If an annual is grown, the yield must be multiplied by the area to get the total harvest since there is no harvest table (yet) for non-perennials
                return viewItem.Yield * viewItem.Area;
            }                                    
        }
    }
}