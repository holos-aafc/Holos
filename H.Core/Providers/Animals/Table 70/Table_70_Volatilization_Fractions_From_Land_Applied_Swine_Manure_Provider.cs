using H.Content;
using H.Core.Enumerations;
using H.Core.Providers.Animals.Table_69;

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
            return base.GetData(animalType, province, year);
        }

        #endregion
    }
}