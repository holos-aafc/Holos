using System.Collections.Generic;
using System.Linq;
using H.Content;
using H.Infrastructure;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Duplicate provider. Not used
    /// 
    /// Table 34. Volatile solid excretion for performance standard diets for each pig group, by province.
    /// </summary>
    public class Duplicate_Table_34_Volatile_Solid_Excretion_Swine_Provider : ProviderBase
    {
        #region Fields
        private List<Duplicate_Table_34_Volatile_Solid_Excretion_Swine_Data> _cache;
        #endregion
        #region Constructors
        public Duplicate_Table_34_Volatile_Solid_Excretion_Swine_Provider()
        {
            _cache = BuildCache();
        }
        #endregion

        #region Public Methods
        public List<Duplicate_Table_34_Volatile_Solid_Excretion_Swine_Data> GetSwineVolatileExcretion()
        {
            return _cache;
        }
        #endregion

        #region Private Methods
        private List<Duplicate_Table_34_Volatile_Solid_Excretion_Swine_Data> BuildCache()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var filename = CsvResourceNames.SwineVolatileExcretion;
            var filelines = CsvResourceReader.GetFileLines(filename);
            var result = new List<Duplicate_Table_34_Volatile_Solid_Excretion_Swine_Data>();

            foreach (var line in filelines.Skip(1))
            {
                var entry = new Duplicate_Table_34_Volatile_Solid_Excretion_Swine_Data();
                var animalType = animalTypeStringConverter.Convert(line[0]);
                var province = provinceStringConverter.Convert(line[1]);
                var volatileSolidExcretion = double.Parse(line[2], cultureInfo);

                entry.AnimalType = animalType;
                entry.Province = province;
                entry.VolatileSolidExcretion = volatileSolidExcretion;

                result.Add(entry);
            }

            return result;
        }

        #endregion
    }
}