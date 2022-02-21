using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Infrastructure;
using H.Content;

namespace H.Core.Providers.Plants
{
    public class NitrogenLigninInCropsForSteadyStateMethodProvider_Table_12
    {
        #region Fields

        private readonly CropTypeStringConverter _cropTypeStringConverter; 
        
        #endregion

        #region Constructors

        /// <summary>
        /// Creates a crop string converter and reads the csv file.
        /// </summary>
        public NitrogenLigninInCropsForSteadyStateMethodProvider_Table_12()
        {
            _cropTypeStringConverter = new CropTypeStringConverter();

            this.Data = this.ReadFile();
        }

        #endregion

        #region Properties

        /// <summary>
        /// List contains instances where each entry corresponds to data from a line in the csv file.
        /// </summary>
        List<NitrogenLigninInCropsForSteadyStateMethodData> Data { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Takes a croptype and returns an instance containing data related to that crop.
        /// </summary>
        /// <param name="cropType">The type of crop for which we need the required data.</param>
        /// <returns>An instance for NitrogenLigninInCropsForSteadyStateMethodData containing various values including nitrogen and lingin content values. Returns null if nothing found.
        /// For each data instance Lignin and Nitrogen content = Proportion of Carbon content. Moisture content = %</returns>
        public NitrogenLigninInCropsForSteadyStateMethodData GetDataByCropType(CropType cropType)
        {
            if (cropType == CropType.Fallow)
            {
                return new NitrogenLigninInCropsForSteadyStateMethodData();
            }

            NitrogenLigninInCropsForSteadyStateMethodData data = this.Data.Find(x => x.CropType == cropType);

            if (data != null)
            {
                return data;
            }
            
            else
            {
                Trace.TraceError($"{nameof(NitrogenLigninInCropsForSteadyStateMethodProvider_Table_12)}.{nameof(NitrogenLigninInCropsForSteadyStateMethodProvider_Table_12.GetDataByCropType)}" +
                    $" could not find Crop Type: {cropType} in the available crop data. Returning 0.");

                return new NitrogenLigninInCropsForSteadyStateMethodData();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Reads the csv file and returns a list of instances for every crop in that file.
        /// </summary>
        /// <returns>A list containing NitrogenLinginInCropsForSteadyStateMethodData instances. Each instance corresponds to a crop and a line in the csv.</returns>
        private List<NitrogenLigninInCropsForSteadyStateMethodData> ReadFile()
        {
            var cropInstances = new List<NitrogenLigninInCropsForSteadyStateMethodData>();
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;

            IEnumerable<string[]> fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.NitrogenLinginContentsInSteadyStateMethods);

            foreach (string[] line in fileLines.Skip(1))
            {
                CropType cropType = _cropTypeStringConverter.Convert(line[0]);
                var intercept = double.Parse(line[1], cultureInfo);
                var slope = double.Parse(line[2], cultureInfo);
                var rst = double.Parse(line[3], cultureInfo);
                var nitrogenContent = double.Parse(line[4], cultureInfo);
                var ligninContent = double.Parse(line[5], cultureInfo);
                var moistureContent = double.Parse(line[6], cultureInfo);

                cropInstances.Add(new NitrogenLigninInCropsForSteadyStateMethodData 
                {
                    CropType = cropType,
                    InterceptValue = intercept,
                    SlopeValue = slope,
                    RSTRatio = rst,
                    NitrogenContentResidues = nitrogenContent,
                    LigninContentResidues = ligninContent,
                    MoistureContent = moistureContent,
                });
            }

            return cropInstances;
        }

        #endregion
    }
}
