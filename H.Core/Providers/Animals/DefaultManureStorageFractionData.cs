using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    public class DefaultManureStorageFractionData
    {
        public ManureStateType StorageType { get; set; }
        public double FractionMineralizedAsTan { get; set; }
        public double FractionImmobilized { get; set; }
        public double FractionNitrified { get; set; }
    }
}
