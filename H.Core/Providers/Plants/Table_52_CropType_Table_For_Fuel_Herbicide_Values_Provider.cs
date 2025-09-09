﻿using System.Collections.Generic;
using H.Core.Enumerations;

namespace H.Core.Providers.Plants
{
    /// <summary>
    ///     Table 52. Crop type table for Eastern Canada, used to determine Efuel and Eherbicide value.
    /// </summary>
    public class Table_52_CropType_Table_For_Fuel_Herbicide_Values_Provider
    {
        private readonly List<CropTypeToDetermineEFuelAndEHerbicideForEasternCanadaTableData> _data;

        public Table_52_CropType_Table_For_Fuel_Herbicide_Values_Provider()
        {
            _data = new List<CropTypeToDetermineEFuelAndEHerbicideForEasternCanadaTableData>();

            _data.Add(new CropTypeToDetermineEFuelAndEHerbicideForEasternCanadaTableData
            {
                CropType = CropType.Barley,
                CropTypeValue = 3
            });

            _data.Add(new CropTypeToDetermineEFuelAndEHerbicideForEasternCanadaTableData
            {
                CropType = CropType.Buckwheat,
                CropTypeValue = 3
            });

            _data.Add(new CropTypeToDetermineEFuelAndEHerbicideForEasternCanadaTableData
            {
                CropType = CropType.CanarySeed,
                CropTypeValue = 3
            });

            _data.Add(new CropTypeToDetermineEFuelAndEHerbicideForEasternCanadaTableData
            {
                CropType = CropType.Canola,
                CropTypeValue = 3
            });

            _data.Add(new CropTypeToDetermineEFuelAndEHerbicideForEasternCanadaTableData
            {
                CropType = CropType.Chickpeas,
                CropTypeValue = 2
            });

            _data.Add(new CropTypeToDetermineEFuelAndEHerbicideForEasternCanadaTableData
            {
                CropType = CropType.ColouredWhiteFabaBeans,
                CropTypeValue = 2
            });

            _data.Add(new CropTypeToDetermineEFuelAndEHerbicideForEasternCanadaTableData
            {
                CropType = CropType.DryPeas,
                CropTypeValue = 2
            });

            _data.Add(new CropTypeToDetermineEFuelAndEHerbicideForEasternCanadaTableData
            {
                CropType = CropType.FlaxSeed,
                CropTypeValue = 3
            });

            _data.Add(new CropTypeToDetermineEFuelAndEHerbicideForEasternCanadaTableData
            {
                CropType = CropType.FodderCorn,
                CropTypeValue = 1
            });

            _data.Add(new CropTypeToDetermineEFuelAndEHerbicideForEasternCanadaTableData
            {
                CropType = CropType.GrainCorn,
                CropTypeValue = 1
            });

            _data.Add(new CropTypeToDetermineEFuelAndEHerbicideForEasternCanadaTableData
            {
                CropType = CropType.HayAndForageSeed,
                CropTypeValue = 4
            });

            _data.Add(new CropTypeToDetermineEFuelAndEHerbicideForEasternCanadaTableData
            {
                CropType = CropType.TameGrass,
                CropTypeValue = 4
            });

            _data.Add(new CropTypeToDetermineEFuelAndEHerbicideForEasternCanadaTableData
            {
                CropType = CropType.TameLegume,
                CropTypeValue = 4
            });

            _data.Add(new CropTypeToDetermineEFuelAndEHerbicideForEasternCanadaTableData
            {
                CropType = CropType.TameMixed,
                CropTypeValue = 4
            });

            _data.Add(new CropTypeToDetermineEFuelAndEHerbicideForEasternCanadaTableData
            {
                CropType = CropType.Lentils,
                CropTypeValue = 2
            });

            _data.Add(new CropTypeToDetermineEFuelAndEHerbicideForEasternCanadaTableData
            {
                CropType = CropType.MixedGrains,
                CropTypeValue = 3
            });

            _data.Add(new CropTypeToDetermineEFuelAndEHerbicideForEasternCanadaTableData
            {
                CropType = CropType.MustardSeed,
                CropTypeValue = 3
            });

            _data.Add(new CropTypeToDetermineEFuelAndEHerbicideForEasternCanadaTableData
            {
                CropType = CropType.Oats,
                CropTypeValue = 3
            });

            _data.Add(new CropTypeToDetermineEFuelAndEHerbicideForEasternCanadaTableData
            {
                CropType = CropType.Potatoes,
                CropTypeValue = 1
            });

            _data.Add(new CropTypeToDetermineEFuelAndEHerbicideForEasternCanadaTableData
            {
                CropType = CropType.Rye,
                CropTypeValue = 3
            });

            _data.Add(new CropTypeToDetermineEFuelAndEHerbicideForEasternCanadaTableData
            {
                CropType = CropType.Safflower,
                CropTypeValue = 3
            });

            _data.Add(new CropTypeToDetermineEFuelAndEHerbicideForEasternCanadaTableData
            {
                CropType = CropType.Soybeans,
                CropTypeValue = 2
            });

            _data.Add(new CropTypeToDetermineEFuelAndEHerbicideForEasternCanadaTableData
            {
                CropType = CropType.SpringWheat,
                CropTypeValue = 3
            });

            _data.Add(new CropTypeToDetermineEFuelAndEHerbicideForEasternCanadaTableData
            {
                CropType = CropType.Durum,
                CropTypeValue = 3
            });

            _data.Add(new CropTypeToDetermineEFuelAndEHerbicideForEasternCanadaTableData
            {
                CropType = CropType.SunflowerSeed,
                CropTypeValue = 3
            });

            _data.Add(new CropTypeToDetermineEFuelAndEHerbicideForEasternCanadaTableData
            {
                CropType = CropType.Triticale,
                CropTypeValue = 3
            });

            _data.Add(new CropTypeToDetermineEFuelAndEHerbicideForEasternCanadaTableData
            {
                CropType = CropType.WinterWheat,
                CropTypeValue = 3
            });
        }
    }
}