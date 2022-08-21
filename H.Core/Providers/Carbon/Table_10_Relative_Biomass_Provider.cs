#region Imports

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Content;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Core.Tools;
using H.Infrastructure;

#endregion

namespace H.Core.Providers.Carbon
{
    /// <summary>
    /// This implements multiple tables.
    /// <para>Table 10a: Relative biomass allocation coefficients for different crops in the Holos model.</para>
    /// <para>Table 10b: Relative biomass lignin and nitrogen contents</para>
    /// </summary>
    public class Table_10_Relative_Biomass_Provider : IResidueDataProvider
    {
        #region Fields

        private List<Table_10_Relative_Biomass_Data> _data;

        #endregion

        #region Constructors

        public Table_10_Relative_Biomass_Provider()
        {
            HTraceListener.AddTraceListener();
            _data = this.GetData().ToList();
        }

        #endregion

        #region Properties

        #endregion

        #region Public Methods

        /// <summary>
        /// Get residue values for a specified set of crop parameters
        /// </summary>
        public Table_10_Relative_Biomass_Data GetResidueData(IrrigationType irrigationType,
                                          double irrigationAmount,
                                          CropType cropType,
                                          SoilFunctionalCategory soilFunctionalCategory,
                                          Province province)
        {
            if (cropType == CropType.NotSelected || cropType.IsFallow())
            {
                return new Table_10_Relative_Biomass_Data();
            }

            if (cropType.IsGrassland())
            {
                // Only have values for grassland (native). If type is grassland (broken) or grassland (seeded), return values for grassland (native)
                cropType = CropType.RangelandNative;
            }

            var byCropType = _data.Where(x => x.CropType == cropType).ToList();
            if (byCropType.Any() == false)
            {
                Trace.TraceError($"{nameof(Table_10_Relative_Biomass_Provider)}.{nameof(this.GetResidueData)}: unknown crop type: '{cropType.GetDescription()}'. Returning default values.");

                return new Table_10_Relative_Biomass_Data();
            }

            if (byCropType.Count() == 1)
            {
                return byCropType.First();
            }

            var firstItem = byCropType.First();
            if (firstItem.IrrigationUpperRangeLimit > 0)
            {
                return byCropType.Single(x => irrigationAmount >= x.IrrigationLowerRangeLimit && irrigationAmount < x.IrrigationUpperRangeLimit);
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

        public IEnumerable<Table_10_Relative_Biomass_Data> GetData()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.ResidueDataFile);
            var cropTypeStringConverter = new CropTypeStringConverter();
            var tillageTypeConverter = new TillageTypeStringConverter();
            var provinceStringConverter = new ProvinceStringConverter();

            foreach (var line in fileLines.Skip(15))
            {
                var residueData = new Table_10_Relative_Biomass_Data();

                var cropTypeColumn = line[1];
                if (string.IsNullOrWhiteSpace(cropTypeColumn))
                {
                    continue;
                }

                residueData.CropType = cropTypeStringConverter.Convert(cropTypeColumn);

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

                var carbonInProductColumn = line[8];
                if (string.IsNullOrWhiteSpace(carbonInProductColumn) == false)
                {
                    residueData.RelativeBiomassProduct = double.Parse(carbonInProductColumn, cultureInfo);
                }

                var carbonInStrawColumn = line[9];
                if (string.IsNullOrWhiteSpace(carbonInStrawColumn) == false)
                {
                    residueData.RelativeBiomassStraw = double.Parse(carbonInStrawColumn, cultureInfo);
                }

                var carbonInRootsColumn = line[10];
                if (string.IsNullOrWhiteSpace(carbonInRootsColumn) == false)
                {
                    residueData.RelativeBiomassRoot = double.Parse(carbonInRootsColumn, cultureInfo);
                }

                var carbonInExtrarootsColumn = line[11];
                if (string.IsNullOrWhiteSpace(carbonInExtrarootsColumn) == false)
                {
                    residueData.RelativeBiomassExtraroot = double.Parse(carbonInExtrarootsColumn, cultureInfo);
                }

                #endregion

                #region Nitrogen residue

                var nitrogenInProductColumn = line[14];
                if (string.IsNullOrWhiteSpace(nitrogenInProductColumn) == false)
                {
                    residueData.NitrogenContentProduct = double.Parse(nitrogenInProductColumn, cultureInfo);
                }

                var nitrogenInStrawColumn = line[15];
                if (string.IsNullOrWhiteSpace(nitrogenInStrawColumn) == false)
                {
                    residueData.NitrogenContentStraw = double.Parse(nitrogenInStrawColumn, cultureInfo);
                }

                var nitrogenInRootsColumn = line[16];
                if (string.IsNullOrWhiteSpace(nitrogenInRootsColumn) == false)
                {
                    residueData.NitrogenContentRoot = double.Parse(nitrogenInRootsColumn, cultureInfo);

                    // TODO: As of 6/17/20 we don't have values for extraroot. Current fix (Roland) is to use root values for extraroot too
                    residueData.NitrogenContentExtraroot = double.Parse(nitrogenInRootsColumn, cultureInfo);
                }


                //var nitrogenInExtrarootsColumn = line[17];
                //if (string.IsNullOrWhiteSpace(nitrogenInExtrarootsColumn) == false)
                //{
                //    residueData.NitrogenContentExtraroot = double.Parse(nitrogenInExtrarootsColumn, cultureInfo);
                //} 

                #endregion

                #region Nitrogen fertilizer parsing
                residueData.NitrogenFertilizerRateTable.Add(Province.Alberta, new Dictionary<SoilFunctionalCategory, double>());

                var albertaBrownNitrogenRate = line[21].Replace("-", string.Empty);
                if (string.IsNullOrWhiteSpace(albertaBrownNitrogenRate) == false)
                {
                    residueData.NitrogenFertilizerRateTable[Province.Alberta].Add(SoilFunctionalCategory.Brown, double.Parse(albertaBrownNitrogenRate, cultureInfo));
                }

                var albertaDarkBrownNitrogenRate = line[22].Replace("-", string.Empty);
                if (string.IsNullOrWhiteSpace(albertaDarkBrownNitrogenRate) == false)
                {
                    residueData.NitrogenFertilizerRateTable[Province.Alberta].Add(SoilFunctionalCategory.DarkBrown, double.Parse(albertaDarkBrownNitrogenRate, cultureInfo));
                }

                var albertaBlackNitrogenRate = line[23].Replace("-", string.Empty);
                if (string.IsNullOrWhiteSpace(albertaBlackNitrogenRate) == false)
                {
                    residueData.NitrogenFertilizerRateTable[Province.Alberta].Add(SoilFunctionalCategory.Black, double.Parse(albertaBlackNitrogenRate, cultureInfo));
                }

                var albertaGrayNitrogenRate = line[24].Replace("-", string.Empty);
                if (string.IsNullOrWhiteSpace(albertaGrayNitrogenRate) == false)
                {
                    residueData.NitrogenFertilizerRateTable[Province.Alberta].Add(SoilFunctionalCategory.BlackGrayChernozem, double.Parse(albertaGrayNitrogenRate, cultureInfo));
                }

                var nonSpecificProvinceNitrogenRate = line[25].Replace("-", string.Empty);
                if (string.IsNullOrWhiteSpace(nonSpecificProvinceNitrogenRate) == false)
                {
                    residueData.NitrogenFertilizerRate = double.Parse(nonSpecificProvinceNitrogenRate, cultureInfo);
                }

                residueData.NitrogenFertilizerRateTable.Add(Province.Saskatchewan, new Dictionary<SoilFunctionalCategory, double>());

                var saskatchewanBrownNitrogenRate = line[26].Replace("-", string.Empty);
                if (string.IsNullOrWhiteSpace(saskatchewanBrownNitrogenRate) == false)
                {
                    residueData.NitrogenFertilizerRateTable[Province.Saskatchewan].Add(SoilFunctionalCategory.Brown, double.Parse(saskatchewanBrownNitrogenRate, cultureInfo));
                }

                var saskatchewanDarkBrownNitrogenRate = line[27].Replace("-", string.Empty);
                if (string.IsNullOrWhiteSpace(saskatchewanDarkBrownNitrogenRate) == false)
                {
                    residueData.NitrogenFertilizerRateTable[Province.Saskatchewan].Add(SoilFunctionalCategory.DarkBrown, double.Parse(saskatchewanDarkBrownNitrogenRate, cultureInfo));
                }

                var saskatchewanBlackNitrogenRate = line[28].Replace("-", string.Empty);
                if (string.IsNullOrWhiteSpace(saskatchewanBlackNitrogenRate) == false)
                {
                    residueData.NitrogenFertilizerRateTable[Province.Saskatchewan].Add(SoilFunctionalCategory.Black, double.Parse(saskatchewanBlackNitrogenRate, cultureInfo));
                }

                residueData.NitrogenFertilizerRateTable.Add(Province.Manitoba, new Dictionary<SoilFunctionalCategory, double>());

                var manitobaNitrogenRate = line[29].Replace("-", string.Empty);
                if (string.IsNullOrWhiteSpace(manitobaNitrogenRate) == false)
                {
                    residueData.NitrogenFertilizerRateTable[Province.Manitoba].Add(SoilFunctionalCategory.All, double.Parse(manitobaNitrogenRate, cultureInfo));
                }
                #endregion

                #region Phosphorus fertilizer parsing
                residueData.PhosphorusFertilizerRateTable.Add(Province.Alberta, new Dictionary<SoilFunctionalCategory, double>());

                var albertaBrownPhosphorusRate = line[38].Replace("-", string.Empty);
                if (string.IsNullOrWhiteSpace(albertaBrownPhosphorusRate) == false)
                {
                    residueData.PhosphorusFertilizerRateTable[Province.Alberta].Add(SoilFunctionalCategory.Brown, double.Parse(albertaBrownPhosphorusRate, cultureInfo));
                }

                var albertaDarkBrownPhosphorusRate = line[39].Replace("-", string.Empty);
                if (string.IsNullOrWhiteSpace(albertaDarkBrownPhosphorusRate) == false)
                {
                    residueData.PhosphorusFertilizerRateTable[Province.Alberta].Add(SoilFunctionalCategory.DarkBrown, double.Parse(albertaDarkBrownPhosphorusRate, cultureInfo));
                }

                var albertaBlackPhosphorusRate = line[40].Replace("-", string.Empty);
                if (string.IsNullOrWhiteSpace(albertaBlackPhosphorusRate) == false)
                {
                    residueData.PhosphorusFertilizerRateTable[Province.Alberta].Add(SoilFunctionalCategory.Black, double.Parse(albertaBlackPhosphorusRate, cultureInfo));
                }

                var albertaGrayPhosphorusRate = line[41].Replace("-", string.Empty);
                if (string.IsNullOrWhiteSpace(albertaGrayPhosphorusRate) == false)
                {
                    residueData.PhosphorusFertilizerRateTable[Province.Alberta].Add(SoilFunctionalCategory.BlackGrayChernozem, double.Parse(albertaGrayPhosphorusRate, cultureInfo));
                }

                var nonSpecificProvincePhosphorusRate = line[42].Replace("-", string.Empty);
                if (string.IsNullOrWhiteSpace(nonSpecificProvincePhosphorusRate) == false)
                {
                    residueData.PhosphorusFertilizerRate = double.Parse(nonSpecificProvincePhosphorusRate, cultureInfo);
                }

                residueData.PhosphorusFertilizerRateTable.Add(Province.Saskatchewan, new Dictionary<SoilFunctionalCategory, double>());

                var saskatchewanBrownPhosphorusRate = line[43].Replace("-", string.Empty);
                if (string.IsNullOrWhiteSpace(saskatchewanBrownPhosphorusRate) == false)
                {
                    residueData.PhosphorusFertilizerRateTable[Province.Saskatchewan].Add(SoilFunctionalCategory.Brown, double.Parse(saskatchewanBrownPhosphorusRate, cultureInfo));
                }

                var saskatchewanDarkBrownPhosphorusRate = line[44].Replace("-", string.Empty);
                if (string.IsNullOrWhiteSpace(saskatchewanDarkBrownPhosphorusRate) == false)
                {
                    residueData.PhosphorusFertilizerRateTable[Province.Saskatchewan].Add(SoilFunctionalCategory.DarkBrown, double.Parse(saskatchewanDarkBrownPhosphorusRate, cultureInfo));
                }

                var saskatchewanBlackPhosphorusRate = line[45].Replace("-", string.Empty);
                if (string.IsNullOrWhiteSpace(saskatchewanBlackPhosphorusRate) == false)
                {
                    residueData.PhosphorusFertilizerRateTable[Province.Saskatchewan].Add(SoilFunctionalCategory.Black, double.Parse(saskatchewanBlackPhosphorusRate, cultureInfo));
                }

                residueData.PhosphorusFertilizerRateTable.Add(Province.Manitoba, new Dictionary<SoilFunctionalCategory, double>());

                var manitobaPhosphorusRate = line[46].Replace("-", string.Empty);
                if (string.IsNullOrWhiteSpace(manitobaPhosphorusRate) == false)
                {
                    residueData.PhosphorusFertilizerRateTable[Province.Manitoba].Add(SoilFunctionalCategory.All, double.Parse(manitobaPhosphorusRate, cultureInfo));
                }

                #endregion

                #region Tillage type parsing

                var bcTillageTypeColumn = line[54].Replace("-", String.Empty);
                if (string.IsNullOrWhiteSpace(bcTillageTypeColumn) == false)
                {
                    var tillageType = tillageTypeConverter.Convert(bcTillageTypeColumn);
                    residueData.TillageTypeTable.Add(Province.BritishColumbia, tillageType);
                }

                var abTillageTypeColumn = line[55].Replace("-", String.Empty); ;
                if (string.IsNullOrWhiteSpace(abTillageTypeColumn) == false)
                {
                    var tillageType = tillageTypeConverter.Convert(abTillageTypeColumn);
                    residueData.TillageTypeTable.Add(Province.Alberta, tillageType);
                }

                var skTillageTypeColumn = line[56].Replace("-", String.Empty); ;
                if (string.IsNullOrWhiteSpace(skTillageTypeColumn) == false)
                {
                    var tillageType = tillageTypeConverter.Convert(skTillageTypeColumn);
                    residueData.TillageTypeTable.Add(Province.Saskatchewan, tillageType);
                }

                var mbTillageTypeColumn = line[57].Replace("-", String.Empty); ;
                if (string.IsNullOrWhiteSpace(mbTillageTypeColumn) == false)
                {
                    var tillageType = tillageTypeConverter.Convert(mbTillageTypeColumn);
                    residueData.TillageTypeTable.Add(Province.Manitoba, tillageType);
                }

                var onTillageTypeColumn = line[58].Replace("-", String.Empty); ;
                if (string.IsNullOrWhiteSpace(onTillageTypeColumn) == false)
                {
                    var tillageType = tillageTypeConverter.Convert(onTillageTypeColumn);
                    residueData.TillageTypeTable.Add(Province.Ontario, tillageType);
                }

                var qcTillageTypeColumn = line[59].Replace("-", String.Empty); ;
                if (string.IsNullOrWhiteSpace(qcTillageTypeColumn) == false)
                {
                    var tillageType = tillageTypeConverter.Convert(qcTillageTypeColumn);
                    residueData.TillageTypeTable.Add(Province.Quebec, tillageType);
                }

                var nbTillageTypeColumn = line[60].Replace("-", String.Empty); ;
                if (string.IsNullOrWhiteSpace(nbTillageTypeColumn) == false)
                {
                    var tillageType = tillageTypeConverter.Convert(nbTillageTypeColumn);
                    residueData.TillageTypeTable.Add(Province.NewBrunswick, tillageType);
                }

                var peiTillageTypeColumn = line[61].Replace("-", String.Empty); ;
                if (string.IsNullOrWhiteSpace(peiTillageTypeColumn) == false)
                {
                    var tillageType = tillageTypeConverter.Convert(peiTillageTypeColumn);
                    residueData.TillageTypeTable.Add(Province.PrinceEdwardIsland, tillageType);
                }

                var nsTillageTypeColumn = line[62].Replace("-", String.Empty); ;
                if (string.IsNullOrWhiteSpace(nsTillageTypeColumn) == false)
                {
                    var tillageType = tillageTypeConverter.Convert(nsTillageTypeColumn);
                    residueData.TillageTypeTable.Add(Province.NovaScotia, tillageType);
                }

                var nflTillageTypeColumn = line[63].Replace("-", String.Empty); ;
                if (string.IsNullOrWhiteSpace(nflTillageTypeColumn) == false)
                {
                    var tillageType = tillageTypeConverter.Convert(nflTillageTypeColumn);
                    residueData.TillageTypeTable.Add(Province.Newfoundland, tillageType);
                }

                #endregion

                #region Irrigation parsing
                #endregion

                #region Pesticide parsing
                #endregion

                #region Lignin content parsing

                string ligninContentColumn = line[86];
                if (!string.IsNullOrWhiteSpace(ligninContentColumn))
                {
                    residueData.LigninContent = double.Parse(ligninContentColumn, cultureInfo);
                }


                #endregion

                yield return residueData;
            }
        }

        #endregion      
    }
}