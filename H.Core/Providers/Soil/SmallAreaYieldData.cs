using H.Core.Enumerations;

namespace H.Core.Providers.Soil
{
    public class SmallAreaYieldData
    {
        /// <summary>
        /// The line number in the CSV
        /// </summary>
        public int Id { get; set; }

        public int Year { get; set; }
        public int Polygon { get; set; }
        public Province Province { get; set; }
        public CropType CropType { get; set; }
        public int Yield { get; set; }

        public override string ToString()
        {
            return $"{nameof(Year)}: {Year}, {nameof(Polygon)}: {Polygon}, {nameof(CropType)}: {CropType}, {nameof(Yield)}: {Yield}";
        }
    }
}