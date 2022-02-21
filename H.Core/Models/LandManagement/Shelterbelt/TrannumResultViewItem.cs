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
        /// Biom_kgCkm
        ///
        /// The accumulated biomass carbon for the shelterbelt at a particular year
        ///
        /// (kg C km^-1)
        /// </summary>
        public double TotalShelterbeltBiomassCarbon { get; set; }

        /// <summary>
        /// Biom_kgCkmyr
        ///
        /// The difference in accumulated biomass carbon for the shelterbelt between two years
        ///
        /// (kg C km^-1 year^-1)
        /// </summary>
        public double TotalShelterbeltBiomassCarbonDelta { get; set; }

        /// <summary>
        /// DOM_kgCkm
        ///
        /// The accumulated dead organic matter for the shelterbelt at a particular year
        ///
        /// (kg C km^-1)
        /// </summary>
        public double TotalDeadOrganicMatter { get; set; }

        /// <summary>
        /// DOM_kgCkmyr
        ///
        /// The difference in accumulated dead organic matter for the shelterbelt between two years
        ///
        /// (kg C km^-1 year^-1)
        /// </summary>
        public double TotalDeadOrganicMatterDelta { get; set; }

        /// <summary>
        /// TEC_kgCkm
        ///
        /// The accumulated total equivalent carbon for the shelterbelt at a particular year
        /// 
        /// (kg C km^-1)
        /// </summary>
        public double TotalEquivalentCarbon { get; set; }

        /// <summary>
        /// TEC_kgCkmyr
        ///
        /// The difference in accumulated total equivalent carbon for the shelterbelt between two years
        /// 
        /// (kg C km^-1 year^-1)
        /// </summary>
        public double TotalEquivalentCarbonDelta { get; set; }

        /// <summary>
        /// The age of the shelterbelt
        /// </summary>
        public int Age { get; set; }

        #endregion

        #region Public Methods



        #endregion

        #region Private Methods



        #endregion

        #region Event Handlers



        #endregion
    }
}