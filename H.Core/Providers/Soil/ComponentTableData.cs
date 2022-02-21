namespace H.Core.Providers.Soil
{
    /// <summary>
    /// http://sis.agr.gc.ca/cansis/nsdb/slc/v3.2/cmp/index.html
    /// </summary>
    internal class ComponentTableData
    {
        private string _soilNameIdentifier;


        public int PolygonId { get; set; }
        public int ComponentNumber { get; set; }
        public int PercentageOfPolygonOccupiedByComponent { get; set; }
        public string SlopeGradient { get; set; }
        public string Stone { get; set; }
        public string LocalSurfaceForm { get; set; }
        public string ProvinceCode { get; set; }
        public string SoilCode { get; set; }
        public string SoilCodeModifier { get; set; }
        public string TypeOfSoilProfile { get; set; }

        /// <summary>
        /// Component table uses 'NF' to indicate Newfoundland but soil layer and soil name tables use 'NL'.
        /// </summary>
        public string SoilNameIdentifier
        {
            get
            {
                if (this._soilNameIdentifier.StartsWith("NF"))
                {                    
                    return this._soilNameIdentifier.Replace("NF", "NL");
                }
                
                return _soilNameIdentifier;
            }
            set { _soilNameIdentifier = value; }
        }

        public int PolygonComponentId { get; set; }
    }
}