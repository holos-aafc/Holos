#region Imports

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Content;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Core.Providers.AnaerobicDigestion;
using H.Core.Tools;
using H.Infrastructure;

#endregion

namespace H.Core.Providers.Carbon
{
    /// <summary>
    /// This implements multiple tables.
    /// <para>Table 7a: Relative biomass allocation coefficients for different crops in the Holos model.</para>
    /// <para>Table 7b: Relative biomass lignin and nitrogen contents</para>
    /// </summary>
    public class Table_7_Relative_Biomass_Information_Provider : IResidueDataProvider
    {
        #region Fields

        private List<Table_7_Relative_Biomass_Information_Data> _data;

        #endregion

        #region Constructors

        public Table_7_Relative_Biomass_Information_Provider()
        {
            HTraceListener.AddTraceListener();
            _data = this.GetData().ToList();
        }

        #endregion

        #region Properties

        #endregion

        #region Public Methods

        public Table_46_Biogas_Methane_Production_CropResidue_Data GetBiomethaneData(CropType cropType)
        {
            var result = new Table_46_Biogas_Methane_Production_CropResidue_Data();

            var data = _data.FirstOrDefault(x => x.CropType == cropType);
            if (data != null)
            {
                result = data.BiomethaneData;
            }

            return result;
        }

        /// <summary>
        /// Get residue values for a specified set of crop parameters
        /// </summary>
        /// <param name="irrigationType">Irrigation</param>
        /// <param name="totalWaterInputs">The total water from irrigation and precipitation (mm)</param>
        /// <param name="cropType"></param>
        /// <param name="soilFunctionalCategory"></param>
        /// <param name="province"></param>
        /// <returns></returns>
        public Table_7_Relative_Biomass_Information_Data GetResidueData(
            IrrigationType irrigationType, 
            double totalWaterInputs, 
            CropType cropType, 
            SoilFunctionalCategory soilFunctionalCategory, 
            Province province)
        {
            if (cropType == CropType.NotSelected || cropType.IsFallow())
            {
                return new Table_7_Relative_Biomass_Information_Data();
            }

            if (cropType.IsGrassland())
            {
                // Only have values for grassland (native). If type is grassland (broken) or grassland (seeded), return values for grassland (native)
                cropType = CropType.RangelandNative;
            }

            var byCropType = _data.Where(x => x.CropType == cropType).ToList();
            if (byCropType.Any() == false)
            {
                Trace.TraceError($"{nameof(Table_7_Relative_Biomass_Information_Provider)}.{nameof(this.GetResidueData)}: unknown crop type: '{cropType.GetDescription()}'. Returning default values.");

                return new Table_7_Relative_Biomass_Information_Data();
            }

            if (byCropType.Count() == 1)
            {
                return byCropType.First();
            }

            var firstItem = byCropType.First();
            if (firstItem.IrrigationUpperRangeLimit > 0)
            {
                return byCropType.Single(x => totalWaterInputs >= x.IrrigationLowerRangeLimit && totalWaterInputs < x.IrrigationUpperRangeLimit);
            }

            if (firstItem.IrrigationType != null)
            {
                return byCropType.Single(x => x.IrrigationType == irrigationType);
            }

            // Potato is a special case
            var byProvince = byCropType.SingleOrDefault(x => x.Province != null && x.Province == province);
            if (byProvince != null)
            {
                return byProvince;
            }

            // Return the 'Canada' entry
            return byCropType.SingleOrDefault(residueData => residueData.Province == null);
        }

        public IEnumerable<Table_7_Relative_Biomass_Information_Data> GetData()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.ResidueDataFile);
            var cropTypeStringConverter = new CropTypeStringConverter();
            var tillageTypeConverter = new TillageTypeStringConverter();
            var provinceStringConverter = new ProvinceStringConverter();

            foreach (var line in fileLines.Skip(4))
            {
                var residueData = new Table_7_Relative_Biomass_Information_Data();

                var cropTypeColumn = line[1];
                if (string.IsNullOrWhiteSpace(cropTypeColumn))
                {
                    continue;
                }

                residueData.CropType = cropTypeStringConverter.Convert(cropTypeColumn);
                if (residueData.CropType == CropType.NotSelected)
                {
                    continue;
                }

                var irrigationTypeColumn = line[2].Trim();
                if (irrigationTypeColumn.Equals("rainfed", StringComparison.InvariantCultureIgnoreCase))
                {
                    residueData.IrrigationType = IrrigationType.RainFed;
                }

                if (irrigationTypeColumn.Equals("irrigated", StringComparison.InvariantCultureIgnoreCase))
                {
                    residueData.IrrigationType = IrrigationType.Irrigated;
                }

                if (irrigationTypeColumn.Contains("<"))
                {
                    // Lower range
                    var irrigationString = irrigationTypeColumn.Replace("<", string.Empty).Replace("mm", String.Empty).Trim();
                    var upperRangeLimit = double.Parse(irrigationString, cultureInfo);

                    residueData.IrrigationLowerRangeLimit = 0;
                    residueData.IrrigationUpperRangeLimit = upperRangeLimit;
                }

                if (irrigationTypeColumn.Contains(">"))
                {
                    // Upper range
                    var irrigationString = irrigationTypeColumn.Replace(">", string.Empty).Replace("mm", String.Empty).Trim();
                    var lowerRangeLimit = double.Parse(irrigationString, cultureInfo);

                    residueData.IrrigationLowerRangeLimit = lowerRangeLimit;
                    residueData.IrrigationUpperRangeLimit = double.MaxValue;
                }

                if (irrigationTypeColumn.Contains("-"))
                {
                    // Irrigation is a range
                    var tokens = irrigationTypeColumn.Replace("mm", string.Empty).Split(new[] { '-' }).Select(x => x.Trim()).ToArray();
                    var lowerRange = double.Parse(tokens[0], cultureInfo);
                    var upperRange = double.Parse(tokens[1], cultureInfo);

                    residueData.IrrigationLowerRangeLimit = lowerRange;
                    residueData.IrrigationUpperRangeLimit = upperRange;
                }

                if (irrigationTypeColumn.Equals("canada", StringComparison.InvariantCultureIgnoreCase))
                {
                    // This is needed to deal with special case of potatoes
                    residueData.Province = null;
                }
                else
                {
                    if (irrigationTypeColumn.Equals("rainfed", StringComparison.InvariantCultureIgnoreCase) ||
                        irrigationTypeColumn.Equals("irrigated", StringComparison.InvariantCultureIgnoreCase) ||
                        irrigationTypeColumn.Contains(">") ||
                        irrigationTypeColumn.Contains("<") ||
                        string.IsNullOrWhiteSpace(irrigationTypeColumn) ||
                        irrigationTypeColumn.Contains("-"))
                    {
                        residueData.Province = null;
                    }
                    else
                    {
                        var province = provinceStringConverter.Convert(irrigationTypeColumn);
                        residueData.Province = province;
                    }
                }

                var moistureContentColumn = line[3];
                if (string.IsNullOrWhiteSpace(moistureContentColumn) == false)
                {
                    var moistureContent = double.Parse(line[3], cultureInfo);
                    residueData.MoistureContentOfProduct = moistureContent;
                }

                #region Carbon residue

                var carbonInProductColumn = line[5];
                if (string.IsNullOrWhiteSpace(carbonInProductColumn) == false)
                {
                    residueData.RelativeBiomassProduct = double.Parse(carbonInProductColumn, cultureInfo);
                }

                var carbonInStrawColumn = line[6];
                if (string.IsNullOrWhiteSpace(carbonInStrawColumn) == false)
                {
                    residueData.RelativeBiomassStraw = double.Parse(carbonInStrawColumn, cultureInfo);
                }

                var carbonInRootsColumn = line[7];
                if (string.IsNullOrWhiteSpace(carbonInRootsColumn) == false)
                {
                    residueData.RelativeBiomassRoot = double.Parse(carbonInRootsColumn, cultureInfo);
                }

                var carbonInExudateColumn = line[8];
                if (string.IsNullOrWhiteSpace(carbonInExudateColumn) == false)
                {
                    residueData.RelativeBiomassExtraroot = double.Parse(carbonInExudateColumn, cultureInfo);
                }

                #endregion

                #region Nitrogen residue

                var nitrogenInProductColumn = line[11];
                if (string.IsNullOrWhiteSpace(nitrogenInProductColumn) == false)
                {
                    residueData.NitrogenContentProduct = double.Parse(nitrogenInProductColumn, cultureInfo);
                }

                var nitrogenInStrawColumn = line[12];
                if (string.IsNullOrWhiteSpace(nitrogenInStrawColumn) == false)
                {
                    residueData.NitrogenContentStraw = double.Parse(nitrogenInStrawColumn, cultureInfo);
                }

                var nitrogenInRootsColumn = line[13];
                if (string.IsNullOrWhiteSpace(nitrogenInRootsColumn) == false)
                {
                    residueData.NitrogenContentRoot = double.Parse(nitrogenInRootsColumn, cultureInfo);
                    residueData.NitrogenContentExtraroot = residueData.NitrogenContentRoot;
                }

                #endregion

                #region Lignin content parsing

                string ligninContentColumn = line[16];
                if (!string.IsNullOrWhiteSpace(ligninContentColumn))
                {
                    residueData.LigninContent = double.Parse(ligninContentColumn, cultureInfo);
                }

                #endregion

                #region Biomethane content parsing

                residueData.BiomethaneData.CropType = residueData.CropType;

                var biomethanePotential = line[17];
                if (!string.IsNullOrWhiteSpace(biomethanePotential))
                {
                    residueData.BiomethaneData.BioMethanePotential = double.Parse(biomethanePotential, cultureInfo);
                }

                var methaneFraction = line[18];
                if (!string.IsNullOrWhiteSpace(methaneFraction))
                {
                    residueData.BiomethaneData.MethaneFraction = double.Parse(methaneFraction, cultureInfo);
                }

                var volatileSolids = line[19];
                if (!string.IsNullOrWhiteSpace(volatileSolids))
                {
                    residueData.BiomethaneData.VolatileSolids = double.Parse(volatileSolids, cultureInfo);
                }

                var totalSolids = line[20];
                if (!string.IsNullOrWhiteSpace(totalSolids))
                {
                    residueData.BiomethaneData.TotalSolids = double.Parse(totalSolids, cultureInfo);
                }

                var totalNitrogen = line[21];
                if (!string.IsNullOrWhiteSpace(totalNitrogen))
                {
                    residueData.BiomethaneData.TotalNitrogen = double.Parse(totalNitrogen, cultureInfo);
                }

                #endregion

                yield return residueData;
            }
        }

        #endregion      
    }
}