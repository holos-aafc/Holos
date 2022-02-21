using H.Core.Enumerations;

namespace H.Core.Providers.Plants
{
    public class LumCMaxAndkValuesForPerennialCroppingChangeData
    {
        #region Properties

        public Ecozone Ecozone { get; set; }
        public SoilTexture SoilTexture { get; set; }
        public PerennialCroppingChangeType PerennialCroppingChangeType { get; set; }
        public double LumCMax { get; set; }
        public double kValue { get; set; }

        #endregion

        #region Methods

        public override string ToString()
        {
            return $"{nameof(Ecozone)}: {Ecozone}, {nameof(SoilTexture)}: {SoilTexture}, {nameof(PerennialCroppingChangeType)}: {PerennialCroppingChangeType}, {nameof(LumCMax)}: {LumCMax}, {nameof(kValue)}: {kValue}";
        }

        #endregion
    }
}
