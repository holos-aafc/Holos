using H.Core.Enumerations;

namespace H.Core.Providers.Plants
{
    public class Duplicate_EnergyRequirementsForCropsData
    {
        public bool IsForFallow { get; set; }

        public bool IsAnyCrop { get; set; }

        public TillageType TillageType { get; set; }
        public Province Province { get; set; }
        public SoilFunctionalCategory SoilFunctionalCategory { get; set; }
        public double EnergyForFuel { get; set; }
        public double EnergyForHerbicide { get; set; }
        public CropType CropType { get; set; }

        public override string ToString()
        {
            return $"{nameof(IsForFallow)}: {IsForFallow}, {nameof(IsAnyCrop)}: {IsAnyCrop}, {nameof(TillageType)}: {TillageType}, {nameof(Province)}: {Province}, {nameof(SoilFunctionalCategory)}: {SoilFunctionalCategory}, {nameof(EnergyForFuel)}: {EnergyForFuel}, {nameof(EnergyForHerbicide)}: {EnergyForHerbicide}, {nameof(CropType)}: {CropType}";
        }
    }
}