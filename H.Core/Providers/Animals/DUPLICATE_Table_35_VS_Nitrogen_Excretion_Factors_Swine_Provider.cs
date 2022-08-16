using System.Collections.Generic;
using System.Linq;
using H.Content;
using H.Infrastructure;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 35 Duplicate
    /// This is a duplicate entry of Table_35_Swine_VS_Nitrogen_Excretion_Factors_Provider
    /// The other file has hard coded values and is being used in other files.
    /// </summary>
    public class DUPLICATE_Table_35_VS_Nitrogen_Excretion_Factors_Swine_Provider
    {
        #region Fields
        private readonly List<DUPLICATE_Table_35_VS_Nitrogen_Excretion_Factors_Swine_Data> _cache;
        #endregion

        #region Constructors
        public DUPLICATE_Table_35_VS_Nitrogen_Excretion_Factors_Swine_Provider()
        {
            _cache = BuildCache();
        }
        #endregion

        #region Public Methods
        public List<DUPLICATE_Table_35_VS_Nitrogen_Excretion_Factors_Swine_Data> GetSwineVolatileAndSoilNitrogenExcretionAdjustmentFactors()
        {
            return _cache;
        }
        #endregion

        #region Private Methods
        private List<DUPLICATE_Table_35_VS_Nitrogen_Excretion_Factors_Swine_Data> BuildCache()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var filename = CsvResourceNames.SwineVolatileAndSoilNitrogenExcretionAdjustmentFactors;
            var filelines = CsvResourceReader.GetFileLines(filename);
            var result = new List<DUPLICATE_Table_35_VS_Nitrogen_Excretion_Factors_Swine_Data>();
            foreach (var line in filelines.Skip(1))
            {
                var entry = new DUPLICATE_Table_35_VS_Nitrogen_Excretion_Factors_Swine_Data();
                var volatileSolidExcretion = double.Parse(line[1], cultureInfo);
                var nitrogenExcretionAdjustmentFactor = double.Parse(line[2], cultureInfo);
                entry.VolatileSolidExcretion = volatileSolidExcretion;
                entry.NitrogenExcretionAdjustmentFactor = nitrogenExcretionAdjustmentFactor;
                result.Add(entry);
            }

            return result;
        }
        #endregion
    }
}