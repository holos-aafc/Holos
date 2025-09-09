﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Content;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Core.Providers.AnaerobicDigestion;
using H.Infrastructure;

namespace H.Core.Providers.Plants
{
    /// <summary>
    ///     Table 9. Default values for nitrogen and lignin contents in crops for the IPCC (2019) Tier 2 steady-state method
    ///     (iterated)
    /// </summary>
    public class Table_9_Nitrogen_Lignin_Content_In_Crops_Provider : ProviderBase
    {
        #region Fields

        private readonly CropTypeStringConverter _cropTypeStringConverter;

        #endregion

        #region Constructors

        /// <summary>
        ///     Creates a crop string converter and reads the csv file.
        /// </summary>
        public Table_9_Nitrogen_Lignin_Content_In_Crops_Provider()
        {
            _cropTypeStringConverter = new CropTypeStringConverter();

            Data = ReadFile();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     List contains instances where each entry corresponds to data from a line in the csv file.
        /// </summary>
        private List<Table_9_Nitrogen_Lignin_Content_In_Crops_Data> Data { get; }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Reads the csv file and returns a list of instances for every crop in that file.
        /// </summary>
        /// <returns>
        ///     A list containing NitrogenLinginInCropsForSteadyStateMethodData instances. Each instance corresponds to a crop
        ///     and a line in the csv.
        /// </returns>
        private List<Table_9_Nitrogen_Lignin_Content_In_Crops_Data> ReadFile()
        {
            var cropInstances = new List<Table_9_Nitrogen_Lignin_Content_In_Crops_Data>();
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;

            var fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.NitrogenLinginContentsInSteadyStateMethods);

            foreach (var line in fileLines.Skip(1).Take(58))
            {
                if (line.All(string.IsNullOrWhiteSpace)) continue;

                var row = new Table_9_Nitrogen_Lignin_Content_In_Crops_Data();

                var cropType = _cropTypeStringConverter.Convert(line[1]);
                var intercept = double.Parse(line[2].ParseUntilOrDefault(), cultureInfo);
                var slope = double.Parse(line[3].ParseUntilOrDefault(), cultureInfo);
                var rst = double.Parse(line[4], cultureInfo);
                var nitrogenContent = double.Parse(line[5], cultureInfo);
                var ligninContent = double.Parse(line[6], cultureInfo);
                var moistureContent = double.Parse(line[7], cultureInfo);

                // Check if the line contains biomethane data
                if (line.Length == 13)
                {
                    var data = new Table_46_Biogas_Methane_Production_CropResidue_Data
                    {
                        CropType = cropType,
                        BioMethanePotential = ParseDouble(line[8]),
                        MethaneFraction = ParseDouble(line[9]),
                        VolatileSolids = ParseDouble(line[10]),
                        TotalSolids = ParseDouble(line[11]),
                        TotalNitrogen = ParseDouble(line[12])
                    };

                    row.BiomethaneData = data;
                }

                row.CropType = cropType;
                row.InterceptValue = intercept;
                row.SlopeValue = slope;
                row.RSTRatio = rst;
                row.NitrogenContentResidues = nitrogenContent;
                row.LigninContentResidues = ligninContent;
                row.MoistureContent = moistureContent;

                cropInstances.Add(row);
            }

            return cropInstances;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Takes a croptype and returns an instance containing data related to that crop.
        /// </summary>
        /// <param name="cropType">The type of crop for which we need the required data.</param>
        /// <returns>
        ///     An instance for Table_9_Nitrogen_Lignin_Content_In_Crops_Data containing various values including nitrogen and
        ///     lingin content values. Returns null if nothing found.
        ///     For each data instance Lignin and Nitrogen content = Proportion of Carbon content. Moisture content = %
        /// </returns>
        public Table_9_Nitrogen_Lignin_Content_In_Crops_Data GetDataByCropType(CropType cropType)
        {
            if (cropType == CropType.Fallow) return new Table_9_Nitrogen_Lignin_Content_In_Crops_Data();

            var lookupType = cropType;

            if (cropType == CropType.Wheat) lookupType = CropType.Durum;

            if (cropType == CropType.RyeSecaleCerealeWinterRyeCerealRye) lookupType = CropType.Rye;

            var data = Data.Find(x => x.CropType == lookupType);

            if (data != null) return data;

            Trace.TraceError(
                $"{nameof(Table_9_Nitrogen_Lignin_Content_In_Crops_Provider)}.{nameof(GetDataByCropType)}" +
                $" could not find Crop Type: {cropType} in the available crop data. Returning 0.");

            return new Table_9_Nitrogen_Lignin_Content_In_Crops_Data();
        }

        public List<Table_9_Nitrogen_Lignin_Content_In_Crops_Data> GetAllCrops()
        {
            return Data;
        }

        #endregion
    }
}