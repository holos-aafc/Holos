using System.Diagnostics;
using System.Linq;
using H.Content;
using H.Core.Enumerations;
using H.Core.Providers.Animals.Table_69;
using H.Infrastructure;

namespace H.Core.Providers.Animals.Table_70
{
    public class
        Table_62_Volatilization_Fractions_From_Land_Applied_Swine_Manure_Provider :
        Table_61_Volatilization_Fractions_From_Land_Applied_Dairy_Manure_Provider
    {
        #region Constructors

        public Table_62_Volatilization_Fractions_From_Land_Applied_Swine_Manure_Provider()
        {
            ReadFile(CsvResourceNames.SwineFractionOfNAmmoniaLandAppliedManure);
        }

        #endregion

        #region Public Methods

        public override VolatilizationFractionsFromLandAppliedManureData GetData(AnimalType animalType,
            Province province, int year)
        {
            var notFound = new VolatilizationFractionsFromLandAppliedManureData();

            if (animalType.IsSwineType() == false)
            {
                Trace.TraceError(
                    $"{nameof(Table_62_Volatilization_Fractions_From_Land_Applied_Swine_Manure_Provider)}.{nameof(GetData)}" +
                    $" can only provide data for {AnimalType.Dairy.GetDescription()} animals.");

                return notFound;
            }

            if (_validProvinces.Contains(province) == false)
            {
                Trace.TraceError(
                    $"{nameof(Table_62_Volatilization_Fractions_From_Land_Applied_Swine_Manure_Provider)}.{nameof(GetData)}" +
                    $" unable to find province {province} in the available data.");

                return notFound;
            }

            var closestYear = MathHelpers.Closest(_data.Select(x => x.Key).ToArray(), year);

            return _data[closestYear][province];
        }

        #endregion
    }
}