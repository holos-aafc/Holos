﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Content;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Providers.Animals.Table_69
{
    public class
        Table_61_Volatilization_Fractions_From_Land_Applied_Dairy_Manure_Provider :
        IVolatilizationFractionsFromLandAppliedManureProvider
    {
        #region Constructors

        public Table_61_Volatilization_Fractions_From_Land_Applied_Dairy_Manure_Provider()
        {
            _data = new MultiKeyDictionary<int, Province, VolatilizationFractionsFromLandAppliedManureData>();

            _validProvinces = new List<Province>
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
                Province.PrinceEdwardIsland
            };

            ReadFile(CsvResourceNames.DairyFractionOfNAmmoniaLandAppliedManure);
        }

        #endregion

        #region Public Methods

        public virtual VolatilizationFractionsFromLandAppliedManureData GetData(AnimalType animalType,
            Province province, int year)
        {
            var notFound = new VolatilizationFractionsFromLandAppliedManureData();

            if (animalType.IsDairyCattleType() == false)
            {
                Trace.TraceError(
                    $"{nameof(Table_61_Volatilization_Fractions_From_Land_Applied_Dairy_Manure_Provider)}.{nameof(GetData)}" +
                    $" can only provide data for {AnimalType.Dairy.GetDescription()} animals.");

                return notFound;
            }

            if (_validProvinces.Contains(province) == false)
            {
                Trace.TraceError(
                    $"{nameof(Table_61_Volatilization_Fractions_From_Land_Applied_Dairy_Manure_Provider)}.{nameof(GetData)}" +
                    $" unable to find province {province} in the available data.");

                return notFound;
            }

            var closestYear = MathHelpers.Closest(_data.Select(x => x.Key).ToArray(), year);

            return _data[closestYear][province];
        }

        #endregion

        #region Fields

        protected readonly MultiKeyDictionary<int, Province, VolatilizationFractionsFromLandAppliedManureData> _data;
        protected readonly List<Province> _validProvinces;

        #endregion

        #region Private Methods

        protected void ReadLines(List<string[]> lines)
        {
            foreach (var line in lines.Skip(4))
            {
                if (string.IsNullOrWhiteSpace(line[0])) continue;

                var year = int.Parse(line[0]);
                var column = 1;

                foreach (var province in _validProvinces)
                {
                    _data[year][province] = new VolatilizationFractionsFromLandAppliedManureData
                    {
                        Year = year,
                        Province = province,
                        ImpliedEmissionFactor = double.Parse(line[column], InfrastructureConstants.EnglishCultureInfo)
                    };

                    column++;
                }
            }
        }

        protected void ReadFile(CsvResourceNames resourceName)
        {
            var fileLines = CsvResourceReader.GetFileLines(resourceName).ToList();

            ReadLines(fileLines);
        }

        #endregion
    }
}