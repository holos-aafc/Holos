using H.Content;
using H.Core.Enumerations;
using H.Core.Providers.Animals.Table_69;
using H.Infrastructure;
using System.Diagnostics;
using System.Linq;

namespace H.Core.Providers.Animals.Table_70
{
    public class Table_70_Volatilization_Fractions_From_Land_Applied_Swine_Manure_Provider : Table_69_Volatilization_Fractions_From_Land_Applied_Dairy_Manure_Provider
    {
        #region Constructors

        public Table_70_Volatilization_Fractions_From_Land_Applied_Swine_Manure_Provider() : base()
        {
            base.ReadFile(CsvResourceNames.SwineFractionOfNAmmoniaLandAppliedManure);
        }

        #endregion

        #region Public Methods

        public override VolatilizationFractionsFromLandAppliedManureData GetData(AnimalType animalType, Province province, int year)
        {
            var notFound = new VolatilizationFractionsFromLandAppliedManureData();

            if (animalType.IsSwineType() == false)
            {
                Trace.TraceError($"{nameof(Table_70_Volatilization_Fractions_From_Land_Applied_Swine_Manure_Provider)}.{nameof(GetData)}" +
                                 $" can only provide data for {AnimalType.Dairy.GetDescription()} animals.");

                return notFound;
            }

            if (_validProvinces.Contains(province) == false)
            {
                Trace.TraceError($"{nameof(Table_70_Volatilization_Fractions_From_Land_Applied_Swine_Manure_Provider)}.{nameof(GetData)}" +
                                 $" unable to find province {province} in the available data.");

                return notFound;
            }

            var closestYear = MathHelpers.Closest(_data.Select(x => x.Key).ToArray(), year);

            return _data[closestYear][province];
        }

        #endregion
    }
}