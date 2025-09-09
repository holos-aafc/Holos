﻿using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Infrastructure;

namespace H.Core.Services.Initialization.Crops
{
    public partial class CropInitializationService
    {
        #region Public Methods

        /// <summary>
        ///     Determines the amount of N fertilizer required for the specified crop type and yield
        /// </summary>
        public double CalculateRequiredNitrogenFertilizer(
            Farm farm,
            CropViewItem viewItem,
            FertilizerApplicationViewItem fertilizerApplicationViewItem)
        {
            var plantCarbonInAgriculturalProduct = _icbmCarbonInputCalculator.CalculatePlantCarbonInAgriculturalProduct(
                null,
                viewItem,
                farm);

            var nitrogenContentOfGrainReturnedToSoil = _icbmNitrogenInputCalculator.CalculateGrainNitrogenTotal(
                plantCarbonInAgriculturalProduct,
                viewItem.NitrogenContentInProduct);

            var isLeguminousCrop = viewItem.CropType.IsLeguminousCoverCrop() || viewItem.CropType.IsPulseCrop();

            var syntheticFertilizerApplied = _icbmNitrogenInputCalculator.CalculateSyntheticFertilizerApplied(
                nitrogenContentOfGrainReturnedToSoil,
                fertilizerApplicationViewItem.FertilizerEfficiencyFraction,
                viewItem.SoilTestNitrogen,
                isLeguminousCrop,
                viewItem.NitrogenFixation,
                viewItem.NitrogenDepositionAmount);

            return syntheticFertilizerApplied;
        }

        public void InitializeFertilizerBlendData(Farm farm)
        {
            foreach (var cropViewItem in farm.GetAllCropViewItems())
            foreach (var fertilizerApplicationViewItem in cropViewItem.FertilizerApplicationViewItems)
                InitializeFertilizerBlendData(fertilizerApplicationViewItem);
        }

        public void InitializeFertilizerBlendData(FertilizerApplicationViewItem fertilizerApplicationViewItem)
        {
            var data = _carbonFootprintForFertilizerBlendsProvider.GetData(fertilizerApplicationViewItem
                .FertilizerBlendData.FertilizerBlend);
            if (data != null)
            {
                /*
                 * Don't reassign the FertilizerBlendData property to the object returned from the provider since the view model will have attached event handlers
                 * that will get lost of the object is assigned, instead copy individual properties
                 */

                fertilizerApplicationViewItem.FertilizerBlendData.PercentageNitrogen = data.PercentageNitrogen;
                fertilizerApplicationViewItem.FertilizerBlendData.PercentagePhosphorus = data.PercentagePhosphorus;
                fertilizerApplicationViewItem.FertilizerBlendData.PercentagePotassium = data.PercentagePotassium;
                fertilizerApplicationViewItem.FertilizerBlendData.PercentageSulphur = data.PercentageSulphur;
                fertilizerApplicationViewItem.FertilizerBlendData.ApplicationEmissions = data.ApplicationEmissions;
                fertilizerApplicationViewItem.FertilizerBlendData.CarbonDioxideEmissionsAtTheGate =
                    data.CarbonDioxideEmissionsAtTheGate;
            }
        }

        /// <summary>
        ///     When not using a blended P fertilizer approach, use this to assign a P rate directly to the crop
        /// </summary>
        public void InitializePhosphorusFertilizerRate(Farm farm)
        {
            foreach (var cropViewItem in farm.GetAllCropViewItems())
                InitializePhosphorusFertilizerRate(cropViewItem, farm);
        }

        /// <summary>
        ///     When not using a blended P fertilizer approach, use this to assign a P rate directly to the crop
        /// </summary>
        public void InitializePhosphorusFertilizerRate(CropViewItem viewItem, Farm farm)
        {
            var soilData = farm.GetPreferredSoilData(viewItem);
            var province = soilData.Province;

            // Start with a default then get lookup value if one is available
            viewItem.PhosphorusFertilizerRate = 25;

            var residueData = GetResidueData(farm, viewItem);
            if (residueData != null)
                if (residueData.PhosphorusFertilizerRateTable.ContainsKey(province))
                {
                    var phosphorusFertilizerTable = residueData.PhosphorusFertilizerRateTable[province];
                    if (phosphorusFertilizerTable.ContainsKey(soilData.SoilFunctionalCategory))
                    {
                        var rate = phosphorusFertilizerTable[soilData.SoilFunctionalCategory];
                        viewItem.PhosphorusFertilizerRate = rate;
                    }
                }
        }

        /// <summary>
        ///     Equation 2.5.5-7
        ///     Calculates the amount of the fertilizer blend needed to support the yield that was input.This considers the amount
        ///     of nitrogen uptake by the plant and then
        ///     converts that value into an amount of fertilizer blend/product
        /// </summary>
        /// <returns>The amount of product required (kg product ha^-1)</returns>
        public double CalculateAmountOfProductRequired(
            Farm farm,
            CropViewItem viewItem,
            FertilizerApplicationViewItem fertilizerApplicationViewItem)
        {
            var requiredNitrogen = CalculateRequiredNitrogenFertilizer(
                farm,
                viewItem,
                fertilizerApplicationViewItem);

            // If blend is custom, default N value will be zero and so we can't calculate the amount of product required
            if (fertilizerApplicationViewItem.FertilizerBlendData.PercentageNitrogen == 0) return 0;

            // Need to convert to amount of fertilizer product from required nitrogen
            var requiredAmountOfProduct = requiredNitrogen /
                                          (fertilizerApplicationViewItem.FertilizerBlendData.PercentageNitrogen / 100);

            return requiredAmountOfProduct;
        }

        public void InitializeManureApplicationMethod(Farm farm)
        {
            var validManureAppliciationTypes = _manureService.GetValidManureApplicationTypes();

            foreach (var viewItem in farm.GetAllCropViewItems())
            foreach (var manureApplicationViewItem in viewItem.ManureApplicationViewItems)
                InitializeManureApplicationMethod(viewItem, manureApplicationViewItem, validManureAppliciationTypes);
        }

        public void InitializeManureApplicationMethod(CropViewItem viewItem,
            ManureApplicationViewItem manureApplicationViewItem,
            List<ManureApplicationTypes> validManureApplicationTypes)
        {
            if (manureApplicationViewItem.AnimalType.IsBeefCattleType())
            {
                var validBeefCattleApplicationMethods = new List<ManureApplicationTypes>
                {
                    ManureApplicationTypes.UntilledLandSolidSpread,
                    ManureApplicationTypes.TilledLandSolidSpread
                };

                manureApplicationViewItem.AvailableManureApplicationTypes
                    .UpdateItems(validBeefCattleApplicationMethods);

                // Update the selected item based on the tillage type of the field
                if (viewItem.TillageType == TillageType.NoTill)
                    manureApplicationViewItem.ManureApplicationMethod = ManureApplicationTypes.UntilledLandSolidSpread;
                else
                    manureApplicationViewItem.ManureApplicationMethod = ManureApplicationTypes.TilledLandSolidSpread;
            }
            else if (manureApplicationViewItem.AnimalType.IsDairyCattleType())
            {
                var validDairyCattleApplicationMethods = new List<ManureApplicationTypes>
                {
                    ManureApplicationTypes.UntilledLandSolidSpread,
                    ManureApplicationTypes.TilledLandSolidSpread,
                    ManureApplicationTypes.SlurryBroadcasting,
                    ManureApplicationTypes.DropHoseBanding,
                    ManureApplicationTypes.ShallowInjection,
                    ManureApplicationTypes.DeepInjection
                };

                // Can't use ObservableCollection.Clear(),
                manureApplicationViewItem.AvailableManureApplicationTypes.UpdateItems(
                    validDairyCattleApplicationMethods);

                if (manureApplicationViewItem.ManureStateType.IsSolidManure())
                {
                    if (viewItem.TillageType == TillageType.NoTill)
                        manureApplicationViewItem.ManureApplicationMethod =
                            ManureApplicationTypes.UntilledLandSolidSpread;
                    else
                        manureApplicationViewItem.ManureApplicationMethod =
                            ManureApplicationTypes.TilledLandSolidSpread;
                }
                else
                {
                    manureApplicationViewItem.ManureApplicationMethod = ManureApplicationTypes.DeepInjection;
                }
            }
            else
            {
                manureApplicationViewItem.AvailableManureApplicationTypes.UpdateItems(validManureApplicationTypes);

                manureApplicationViewItem.ManureApplicationMethod = validManureApplicationTypes.FirstOrDefault();
            }

            manureApplicationViewItem.MinimumAllowableDateOfApplication = new DateTime(viewItem.Year, 1, 1);
            manureApplicationViewItem.MaximumAllowableDateOfApplication = new DateTime(viewItem.Year, 12, 31);
        }

        public void InitializeFertilizerApplicationMethod(Farm farm)
        {
            foreach (var cropViewItem in farm.GetAllCropViewItems())
            foreach (var fertilizerApplicationViewItem in cropViewItem.FertilizerApplicationViewItems)
                InitializeFertilizerApplicationMethod(cropViewItem, fertilizerApplicationViewItem);
        }

        public void InitializeFertilizerApplicationMethod(CropViewItem viewItem,
            FertilizerApplicationViewItem fertilizerApplicationViewItem)
        {
            if (viewItem != null && fertilizerApplicationViewItem != null)
            {
                if (viewItem.CropType.IsPerennial())
                    fertilizerApplicationViewItem.FertilizerApplicationMethodology =
                        FertilizerApplicationMethodologies.Broadcast;
                else
                    fertilizerApplicationViewItem.FertilizerApplicationMethodology =
                        FertilizerApplicationMethodologies.IncorporatedOrPartiallyInjected;
            }
        }

        public void InitializeAmountOfBlendedProduct(Farm farm, CropViewItem viewItem,
            FertilizerApplicationViewItem fertilizerApplicationViewItem)
        {
            var selectedBlend = fertilizerApplicationViewItem.FertilizerBlendData.FertilizerBlend;

            if (selectedBlend == FertilizerBlends.Lime)
            {
                // Leave as 0 for now until methodology developed to calculate amount of lime needed by crop
                fertilizerApplicationViewItem.AmountOfBlendedProductApplied = 0;
            }
            else if (selectedBlend == FertilizerBlends.CustomOrganic)
            {
                // Leave as 0 for now until methodology to calculate default is developed
                fertilizerApplicationViewItem.AmountOfBlendedProductApplied = 0;
            }
            else
            {
                var amountSuggested = CalculateAmountOfProductRequired(farm, viewItem, fertilizerApplicationViewItem);
                fertilizerApplicationViewItem.AmountOfBlendedProductApplied = amountSuggested;
            }
        }

        #endregion
    }
}