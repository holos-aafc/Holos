using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    public class DefaultMethaneAndN2OEmissionRatesForPoultryData
    {
        public AnimalType PoultryGroup { get; set; }
        public double MethaneEntericRate { get; set; }
        public double MethaneManureRate { get; set; }
        public double NitrogenExcretionRate { get; set; }
        public double EFDirect { get; set; }
    }
}
