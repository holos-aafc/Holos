using System;
using System.Collections.Generic;
using System.Diagnostics;
using H.Content;
using System.Globalization;
using System.Linq;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Providers.Animals.Table_69
{
    public class Table_69_Provider : ITable_69_Provider
    {
        #region Fields

        private readonly MultiKeyDictionary<int, Province, Table_69_Data> _data;
        private List<Province> _validProvinces;

        #endregion

        #region Constructors

        public Table_69_Provider()
        {
            _data = new MultiKeyDictionary<int, Province, Table_69_Data>();

            _validProvinces = new List<Province>()
            {
                Province.BritishColumbia,
                Province.Alberta,
                Province.Saskatchewan,
                Province.Manitoba,
                Province.Ontario,
                Province.Quebec,
                Province.NovaScotia,
                Province.NewBrunswick,
                Province.Newfoundland,
                Province.PrinceEdwardIsland,
            };

            this.ReadFile();
        }

        #endregion

        #region Public Methods

        public Table_69_Data GetData(AnimalType animalType, Province province, int year)
        {
            var notFound = new Table_69_Data();

            if (animalType.IsDairyCattleType() == false)
            {
                Trace.TraceError($"{nameof(Table_69_Provider)}.{nameof(Table_69_Provider.GetData)}" +
                                 $" can only provide data for {AnimalType.Dairy.GetDescription()} animals.");

                return notFound;
            }

            if (_validProvinces.Contains(province) == false)
            {
                Trace.TraceError($"{nameof(Table_69_Provider)}.{nameof(Table_69_Provider.GetData)}" +
                                 $" unable to find province {province} in the available data.");

                return notFound;
            }

            return _data[year][province];
        }

        #endregion

        #region Private Methods

        private void ReadFile()
        {
            var fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.DairyFractionOfNAmmoniaLandAppliedManure).ToList();

            foreach (var line in fileLines.Skip(4))
            {
                if (string.IsNullOrWhiteSpace(line[0]))
                {
                    continue;
                }

                var year = int.Parse(line[0]);
                var column = 1;

                foreach (var province in _validProvinces)
                {
                    _data[year][province] = new Table_69_Data()
                    {
                        Year = year,
                        Province = province,
                        ImpliedEmissionFactor = double.Parse(line[column], InfrastructureConstants.EnglishCultureInfo)
                    };

                    column++;
                }
            }
        }

        #endregion
    }
}