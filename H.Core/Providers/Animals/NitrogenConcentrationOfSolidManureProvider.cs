using System.Collections.Generic;
using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 41
    /// </summary>
    public class NitrogenConcentrationOfSolidManureProvider
    {
        private List<NitrogenConcentrationOfSolidManureTableData> _data;

        public NitrogenConcentrationOfSolidManureProvider()
        {
            _data = new List<NitrogenConcentrationOfSolidManureTableData>();

            _data.Add(new NitrogenConcentrationOfSolidManureTableData()
            {
                AnimalType = AnimalType.Swine,
                NitrogenConcentration = 8.0
            });

            _data.Add(new NitrogenConcentrationOfSolidManureTableData()
            {
                AnimalType = AnimalType.DairyCalves,
                NitrogenConcentration = 5.0
            });

            _data.Add(new NitrogenConcentrationOfSolidManureTableData()
            {
                AnimalType = AnimalType.Poultry,
                NitrogenConcentration = 24.1
            });

            _data.Add(new NitrogenConcentrationOfSolidManureTableData()
            {
                AnimalType = AnimalType.Beef,
                NitrogenConcentration = 10.0
            });

            _data.Add(new NitrogenConcentrationOfSolidManureTableData()
            {
                AnimalType = AnimalType.Sheep,
                NitrogenConcentration = 10.0
            });
        }

        public List<NitrogenConcentrationOfSolidManureTableData> GetData()
        {
            return _data;
        }
    }
}
