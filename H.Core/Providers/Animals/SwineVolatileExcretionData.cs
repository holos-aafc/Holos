using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    public class SwineVolatileExcretionData
    {
        public AnimalType AnimalType { get; set; }
        public double VolatileSolidExcretion { get; set; }
        public Province Province { get; set; }
    }
}