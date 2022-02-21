namespace H.Core.Providers.Soil
{
    public class DefaultYieldTableData
    {
        #region Properties
        public int PrId { get; set; }
        public int CarId { get; set; }
        /// <summary>
        /// Code for Province [PR] and Small Area Data [SAD] Region from Stats Canada
        /// </summary>
        public int PrSad { get; set; }
        public int Year { get; set; }
        public Enumerations.CropType CropType { get; set; }
        /// <summary>
        /// Area Seeded in Acres (ac)
        /// </summary>
        public double Seeded { get; set; }
        /// <summary>
        /// Area Seeded in Hectares (ha)
        /// </summary>
        public double ESeed { get; set; }
        /// <summary>
        /// Harvested Area Acres (ac)
        /// </summary>
        public double Harv { get; set; }
        /// <summary>
        /// Harvested Area Hectares (ha)
        /// </summary>
        public double EHarv { get; set; }
        /// <summary>
        /// % Harvested - derived from HARV/SEEDED
        /// </summary>
        public double PerHarv { get; set; }
        /// <summary>
        /// Yield Bu/Ac - provincial average provided in table, not calculated (bu/ac)
        /// </summary>
        public double Yield { get; set; }
        /// <summary>
        /// Yield in lbs/Ac - provincial average provided in table, not calculated (lbs/ac)
        /// </summary>
        public double YldLbs { get; set; }
        /// <summary>
        /// Yield Kg/Ha - provincial average provided in table, not calculated (kg/ha)
        /// </summary>
        public double EYield { get; set; }
        /// <summary>
        /// Total Production in SAD in '000s of Bu (bu)
        /// </summary>
        public double? ProdN { get; set; }
        /// <summary>
        /// Total Production for SAD in '000s of Pounds (lbs)
        /// </summary>
        public double? PrdLbs { get; set; }
        /// <summary>
        /// Total Production for SAD in Metric Tonnes (kg)
        /// </summary>
        public double EProdN { get; set; }
        /// <summary>
        /// Actual Yield Bu/Ac - derived from PRODN/HARV - calculated (bu/ac)
        /// </summary>
        public double? NYield { get; set; }
        /// <summary>
        /// Actual Yield in lbs/Ac - derived from PRDLBS/HARV- calculated (lbs/ac)
        /// </summary>
        public double? NYldLbs { get; set; }
        /// <summary>
        /// Actual Yield in Kg/Ha - derived from EPRODN/EHARV- calculated (kg/ha)
        /// </summary>
        public double NEYield { get; set; }
        /// <summary>
        /// % provincial yield - derived from NEYIELD/EYIELD
        /// </summary>
        public double PPYield { get; set; }
        public string CSad { get; set; }
        #endregion

        #region Methods

        public override string ToString()
        {
            return $"{nameof(Year)}: {Year}, {nameof(CropType)}: {CropType}, {nameof(EYield)}: {EYield}";
        }

        #endregion
    }
}
