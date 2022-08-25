using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Content;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Providers.Plants
{
    /// <summary>
    /// This provider has been split and implemented as the Fuel and Herbicide providers.
    /// </summary>
    public class Duplicate_EnergyRequirementsForCropsProvider
    {
        private List<Duplicate_EnergyRequirementsForCropsData> _data;
        private readonly CropTypeStringConverter _cropTypeStringConverter;

        public Duplicate_EnergyRequirementsForCropsProvider()
        {            
            _cropTypeStringConverter = new CropTypeStringConverter();

            _data = this.ReadFile();
        }

        private List<Duplicate_EnergyRequirementsForCropsData> ReadFile()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var filename = CsvResourceNames.EnergyRequirementsForCrops;
            var filelines = CsvResourceReader.GetFileLines(filename).ToList();
            var result = new List<Duplicate_EnergyRequirementsForCropsData>();

            const int startingRowIndex = 4;
            for (int row = startingRowIndex; row < filelines.Count; row++)
            {
                var line = filelines.ElementAt(row);
                if (string.IsNullOrWhiteSpace(line[0]) || line[0].Equals("Total tree fruits & nuts", StringComparison.CurrentCultureIgnoreCase))
                {
                    continue;
                }

                var cropType = _cropTypeStringConverter.Convert(line[0]);

                // Create multiple entries for the crop type at this row, one for different provinces, soil, etc
                var westernProvinces = new List<Province>() {Province.Alberta, Province.Saskatchewan, Province.Manitoba};                
                foreach (var province in westernProvinces)
                {
                    // Black soil type
                    result.Add(new Duplicate_EnergyRequirementsForCropsData()
                    {
                        CropType = cropType,
                        Province = province,
                        SoilFunctionalCategory = SoilFunctionalCategory.Black,
                        TillageType = TillageType.Intensive,
                        EnergyForFuel = string.IsNullOrWhiteSpace(line[1]) ? 0 : double.Parse(line[1], cultureInfo),
                        EnergyForHerbicide = string.IsNullOrWhiteSpace(line[19]) ? 0 : double.Parse(line[19], cultureInfo)
                    });

                    result.Add(new Duplicate_EnergyRequirementsForCropsData()
                    {
                        CropType = cropType,
                        Province = province,
                        SoilFunctionalCategory = SoilFunctionalCategory.Black,
                        TillageType = TillageType.Reduced,
                        EnergyForFuel = string.IsNullOrWhiteSpace(line[2]) ? 0 : double.Parse(line[2], cultureInfo),
                        EnergyForHerbicide = string.IsNullOrWhiteSpace(line[20]) ? 0 : double.Parse(line[20], cultureInfo)
                    });

                    result.Add(new Duplicate_EnergyRequirementsForCropsData()
                    {
                        CropType = cropType,
                        Province = province,
                        SoilFunctionalCategory = SoilFunctionalCategory.Black,
                        TillageType = TillageType.NoTill,
                        EnergyForFuel = string.IsNullOrWhiteSpace(line[3]) ? 0 : double.Parse(line[3], cultureInfo),
                        EnergyForHerbicide = string.IsNullOrWhiteSpace(line[21]) ? 0 : double.Parse(line[21], cultureInfo)
                    });

                    // Brown soil type
                    result.Add(new Duplicate_EnergyRequirementsForCropsData()
                    {
                        CropType = cropType,
                        Province = province,
                        SoilFunctionalCategory = SoilFunctionalCategory.Brown,
                        TillageType = TillageType.Intensive,
                        EnergyForFuel = string.IsNullOrWhiteSpace(line[4]) ? 0 : double.Parse(line[4], cultureInfo),
                        EnergyForHerbicide = string.IsNullOrWhiteSpace(line[22]) ? 0 : double.Parse(line[22], cultureInfo)
                    });

                    result.Add(new Duplicate_EnergyRequirementsForCropsData()
                    {
                        CropType = cropType,
                        Province = province,
                        SoilFunctionalCategory = SoilFunctionalCategory.Brown,
                        TillageType = TillageType.Reduced,
                        EnergyForFuel = string.IsNullOrWhiteSpace(line[5]) ? 0 : double.Parse(line[5], cultureInfo),
                        EnergyForHerbicide = string.IsNullOrWhiteSpace(line[23]) ? 0 : double.Parse(line[23], cultureInfo)
                    });

                    result.Add(new Duplicate_EnergyRequirementsForCropsData()
                    {
                        CropType = cropType,
                        Province = province,
                        SoilFunctionalCategory = SoilFunctionalCategory.Brown,
                        TillageType = TillageType.NoTill,
                        EnergyForFuel = string.IsNullOrWhiteSpace(line[6]) ? 0 : double.Parse(line[6], cultureInfo),
                        EnergyForHerbicide = string.IsNullOrWhiteSpace(line[24]) ? 0 : double.Parse(line[24], cultureInfo)
                    });
                }       
                
                var easternProvinces = new List<Province>() {Province.PrinceEdwardIsland, Province.NovaScotia, Province.NewBrunswick, Province.Newfoundland, Province.Ontario, Province.Quebec};
                foreach (var province in easternProvinces)
                {
                    // Easter soil type
                    result.Add(new Duplicate_EnergyRequirementsForCropsData()
                    {
                        CropType = cropType,
                        Province = province,
                        SoilFunctionalCategory = SoilFunctionalCategory.EasternCanada,
                        TillageType = TillageType.Intensive,
                        EnergyForFuel = string.IsNullOrWhiteSpace(line[7]) ? 0 : double.Parse(line[7], cultureInfo),
                        EnergyForHerbicide = string.IsNullOrWhiteSpace(line[25]) ? 0 : double.Parse(line[25], cultureInfo)
                    });

                    result.Add(new Duplicate_EnergyRequirementsForCropsData()
                    {
                        CropType = cropType,
                        Province = province,
                        SoilFunctionalCategory = SoilFunctionalCategory.EasternCanada,
                        TillageType = TillageType.Reduced,
                        EnergyForFuel = string.IsNullOrWhiteSpace(line[8]) ? 0 : double.Parse(line[8], cultureInfo),
                        EnergyForHerbicide = string.IsNullOrWhiteSpace(line[26]) ? 0 : double.Parse(line[26], cultureInfo)
                    });

                    result.Add(new Duplicate_EnergyRequirementsForCropsData()
                    {
                        CropType = cropType,
                        Province = province,
                        SoilFunctionalCategory = SoilFunctionalCategory.EasternCanada,
                        TillageType = TillageType.NoTill,
                        EnergyForFuel = string.IsNullOrWhiteSpace(line[9]) ? 0 : double.Parse(line[9], cultureInfo),
                        EnergyForHerbicide = string.IsNullOrWhiteSpace(line[27]) ? 0 : double.Parse(line[27], cultureInfo)
                    });                   
                }

                var bc = new List<Province>() {Province.BritishColumbia};
                foreach (var province in bc)
                {
                    // Black soil type
                    result.Add(new Duplicate_EnergyRequirementsForCropsData()
                    {
                        CropType = cropType,
                        Province = province,
                        SoilFunctionalCategory = SoilFunctionalCategory.Black,
                        TillageType = TillageType.Intensive,
                        EnergyForFuel = string.IsNullOrWhiteSpace(line[10]) ? 0 : double.Parse(line[10], cultureInfo),
                        EnergyForHerbicide = string.IsNullOrWhiteSpace(line[28]) ? 0 : double.Parse(line[28], cultureInfo)
                    });

                    result.Add(new Duplicate_EnergyRequirementsForCropsData()
                    {
                        CropType = cropType,
                        Province = province,
                        SoilFunctionalCategory = SoilFunctionalCategory.Black,
                        TillageType = TillageType.Reduced,
                        EnergyForFuel = string.IsNullOrWhiteSpace(line[11]) ? 0 : double.Parse(line[11], cultureInfo),
                        EnergyForHerbicide = string.IsNullOrWhiteSpace(line[29]) ? 0 : double.Parse(line[29], cultureInfo)
                    });

                    result.Add(new Duplicate_EnergyRequirementsForCropsData()
                    {
                        CropType = cropType,
                        Province = province,
                        SoilFunctionalCategory = SoilFunctionalCategory.Black,
                        TillageType = TillageType.NoTill,
                        EnergyForFuel = string.IsNullOrWhiteSpace(line[12]) ? 0 : double.Parse(line[12], cultureInfo),
                        EnergyForHerbicide = string.IsNullOrWhiteSpace(line[30]) ? 0 : double.Parse(line[30], cultureInfo)
                    });

                    // Brown soil type
                    result.Add(new Duplicate_EnergyRequirementsForCropsData()
                    {
                        CropType = cropType,
                        Province = province,
                        SoilFunctionalCategory = SoilFunctionalCategory.Brown,
                        TillageType = TillageType.Intensive,
                        EnergyForFuel = string.IsNullOrWhiteSpace(line[13]) ? 0 : double.Parse(line[13], cultureInfo),
                        EnergyForHerbicide = string.IsNullOrWhiteSpace(line[31]) ? 0 : double.Parse(line[31], cultureInfo)
                    });

                    result.Add(new Duplicate_EnergyRequirementsForCropsData()
                    {
                        CropType = cropType,
                        Province = province,
                        SoilFunctionalCategory = SoilFunctionalCategory.Brown,
                        TillageType = TillageType.Reduced,
                        EnergyForFuel = string.IsNullOrWhiteSpace(line[14]) ? 0 : double.Parse(line[14], cultureInfo),
                        EnergyForHerbicide = string.IsNullOrWhiteSpace(line[32]) ? 0 : double.Parse(line[32], cultureInfo)
                    });

                    result.Add(new Duplicate_EnergyRequirementsForCropsData()
                    {
                        CropType = cropType,
                        Province = province,
                        SoilFunctionalCategory = SoilFunctionalCategory.Brown,
                        TillageType = TillageType.NoTill,
                        EnergyForFuel = string.IsNullOrWhiteSpace(line[15]) ? 0 : double.Parse(line[15], cultureInfo),
                        EnergyForHerbicide = string.IsNullOrWhiteSpace(line[33]) ? 0 : double.Parse(line[33], cultureInfo)
                    });
                }
            }

            return result;
        }

        public Duplicate_EnergyRequirementsForCropsData GetEnergyData(Province province,
                                                            SoilFunctionalCategory soilFunctionalCategory,
                                                            TillageType tillageType, 
                                                            CropType cropType)
        {
            // Translate to a value we have in the table
            if (soilFunctionalCategory.IsBlack())
            {
                soilFunctionalCategory = SoilFunctionalCategory.Black;
            }
            else if (soilFunctionalCategory.IsBrown())
            {
                soilFunctionalCategory = SoilFunctionalCategory.Brown;
            }

            var result = this._data.FirstOrDefault(x =>
                x.Province == province && x.SoilFunctionalCategory == soilFunctionalCategory &&
                x.TillageType == tillageType && x.CropType == cropType);

            if (result == null)
            {
                Trace.TraceError($"Energy data not found for {province.GetDescription()}, {soilFunctionalCategory.GetDescription()}, {tillageType.GetDescription()}, {cropType.GetDescription()}");

                return new Duplicate_EnergyRequirementsForCropsData()
                {
                    EnergyForHerbicide = 1,
                    EnergyForFuel = 1,
                };
            }
            else
            {
                return result;
            }
        }
    }
}