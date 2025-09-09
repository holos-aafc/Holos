﻿using System;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models.Results;
using H.Core.Properties;

namespace H.Core.Services.LandManagement
{
    public partial class FieldResultsService
    {
        /// <summary>
        ///     Equation 11.4.4-1
        /// </summary>
        public double CalculateHarvest(CropViewItem viewItem)
        {
            // If a perennial is grown, the yield will contain the summation of all harvested view items (biomass) entered by the user for that year (# bales x bale weight)
            if (viewItem.CropType.IsPerennial())
            {
                if (viewItem.HasHarvestViewItems) return viewItem.HarvestViewItems.Sum(x => x.AboveGroundBiomass);

                return viewItem.Yield;
            }

            // If an annual is grown, the yield must be multiplied by the area to get the total harvest since there is no harvest table (yet) for non-perennials
            return viewItem.Yield * viewItem.Area;
        }

        public EconomicsResultsViewItem CalculateEstimateOfProduction(CropViewItem viewItem,
            FieldSystemComponent fieldSystemComponent)
        {
            var result = new EconomicsResultsViewItem();
            result.Name = viewItem.Name + " (" + viewItem.Year + ") " + " (" + viewItem.CropTypeString + ")";
            result.ComponentName = fieldSystemComponent.Name;
            result.Year = viewItem.Year;
            result.GroupingString = Resources.TitleCrops;

            var harvest = CalculateHarvest(viewItem);

            result.Harvest = Math.Round(harvest, 0);

            return result;
        }
    }
}