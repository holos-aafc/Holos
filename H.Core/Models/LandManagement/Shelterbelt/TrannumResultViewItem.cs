#region Imports

using H.Infrastructure;

#endregion

namespace H.Core.Models.LandManagement.Shelterbelt
{
    /// <summary>
    /// 
    /// </summary>
    public class TrannumResultViewItem : ModelBase
    {
        #region Fields



        #endregion

        #region Constructors

        public TrannumResultViewItem()
        {

        }

        #endregion

        #region Properties

        public ShelterbeltComponent ShelterbeltComponent { get; set; }

        public TrannumData TrannumData { get; set; }

        public int Year { get; set; }

        /// <summary>
        /// The accumulated biomass carbon for the shelterbelt at a particular year
        ///
        /// (Mg C km^-1)
        /// </summary>
        public double TotalLivingBiomassCarbon { get; set; }

        /// <summary>
        /// The difference in accumulated biomass carbon for the shelterbelt between two years
        ///
        /// (Mg C km^-1 year^-1)
        /// </summary>
        public double TotalLivingBiomassCarbonChange { get; set; }

        /// <summary>
        /// The accumulated dead organic matter for the shelterbelt at a particular year
        ///
        /// (Mg C km^-1)
        /// </summary>
        public double TotalDeadOrganicMatterCarbon { get; set; }

        /// <summary>
        /// The difference in accumulated dead organic matter for the shelterbelt between two years
        ///
        /// (Mg C km^-1 year^-1)
        /// </summary>
        public double TotalDeadOrganicMatterChange { get; set; }

        /// <summary>
        /// The accumulated total ecosystem carbon for the shelterbelt at a particular year
        /// 
        /// (Mg C km^-1)
        /// </summary>
        public double TotalEcosystemCarbon { get; set; }

        /// <summary>
        /// The difference in accumulated total ecosystem carbon for the shelterbelt between two years
        /// 
        /// (Mg C km^-1 year^-1)
        /// </summary>
        public double TotalEcosystemCarbonChange { get; set; }

        #endregion

        #region Public Methods



        #endregion

        #region Private Methods



        #endregion

        #region Event Handlers



        #endregion
    }
}