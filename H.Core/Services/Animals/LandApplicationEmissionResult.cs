using H.Core.Models.LandManagement.Fields;

namespace H.Core.Services.Animals
{
    public class LandApplicationEmissionResult
    {
        public CropViewItem CropViewItem { get; set; }

        /// <summary>
        /// (kg N2O)
        /// </summary>
        public double TotalIndirectN2OEmissions { get; set; }

        /// <summary>
        /// (kg N2O)
        /// </summary>
        public double TotalIndirectN2ONEmissions { get; set; }
    }
}