using H.Core.Enumerations;

namespace H.Core.Providers.Soil
{
    public class CanadianAgriculturalRegionIdToSlcIdData
    {
        public int PrId { get; set; }
        public Enumerations.Province PrName{get; set;}
        public int CarId { get; set; }
        public string CarName { get; set; }
        /// <summary>
        /// Split polys: 1 - majority of SLC poly, 0 - minor portion of SLC poly
        /// </summary>
        public int? SplitPolys { get; set; }
        public int PolygonId { get; set; }
        public int EcodistrictId { get; set; }
    }
}
