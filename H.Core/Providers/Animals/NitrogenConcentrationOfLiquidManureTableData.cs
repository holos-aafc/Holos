using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    public class NitrogenConcentrationOfLiquidManureTableData
    {
        #region Properties

        public AnimalType AnimalType { get; set; }
        public double NitrogenConcentration { get; set; }

        #endregion

        #region Methods

        public override string ToString()
        {
            return $"{nameof(AnimalType)}: {AnimalType}, {nameof(NitrogenConcentration)}: {NitrogenConcentration}";
        }

        #endregion
    }
}