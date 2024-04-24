using H.Core.Enumerations;

namespace H.Core.Models
{
    /// <summary>
    /// A storage tank for digestate resulting from anaerobic digestion
    /// </summary>
    public class DigestateTank : StorageTankBase
    {
        #region Fields

        private DigestateState _digestateState;

        // Amount of digestate available after all field applications
        private double _totalRawDigestateAvailable;
        private double _totalSolidDigestateAvailable;
        private double _totalLiquidDigestateAvailable;

        // Amount of digestate produced by system (not reduced by field applications)
        private double _totalRawDigestateProduced;
        private double _totalSolidDigestateProduced;
        private double _totalLiquidDigestateProduced;

        /*
         * Nitrogen available for land application
         */

        /// <summary>
        /// (kg N)
        /// </summary>
        public double NitrogenFromRawDigestate { get; set; }

        /// <summary>
        /// (kg N)
        /// </summary>
        public double NitrogenFromSolidDigestate { get; set; }

        /// <summary>
        /// (kg N)
        /// </summary>
        public double NitrogenFromLiquidDigestate  { get; set; }

        /*
         * Carbon available for land application
         */

        /// <summary>
        /// (kg C)
        /// </summary>
        public double CarbonFromRawDigestate { get; set; }

        /// <summary>
        /// (kg C)
        /// </summary>
        public double CarbonFromSolidDigestate { get; set; }

        /// <summary>
        /// (kg C)
        /// </summary>
        public double CarbonFromLiquidDigestate { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// A tank of digestate can store whole (raw) digestate or separated digestate
        /// </summary>
        public DigestateState DigestateState
        {
            get => _digestateState;
            set => SetProperty(ref _digestateState, value);
        }

        /// <summary>
        /// Total amount of raw digestate available after all field applications have been considered
        ///  
        /// (kg)
        /// </summary>
        public double TotalRawDigestateAvailable
        {
            get => _totalRawDigestateAvailable;
            set => SetProperty(ref _totalRawDigestateAvailable, value);
        }

        /// <summary>
        /// (kg)
        /// </summary>
        public double TotalSolidDigestateAvailable
        {
            get => _totalSolidDigestateAvailable;
            set => SetProperty(ref _totalSolidDigestateAvailable, value);
        }

        /// <summary>
        /// (kg)
        /// </summary>
        public double TotalLiquidDigestateAvailable
        {
            get => _totalLiquidDigestateAvailable;
            set => SetProperty(ref _totalLiquidDigestateAvailable, value);
        }

        public double TotalRawDigestateProduced
        {
            get => _totalRawDigestateProduced;
            set => SetProperty(ref _totalRawDigestateProduced, value);
        }

        public double TotalSolidDigestateProduced
        {
            get => _totalSolidDigestateProduced;
            set => SetProperty(ref _totalSolidDigestateProduced, value);
        }

        public double TotalLiquidDigestateProduced
        {
            get => _totalLiquidDigestateProduced;
            set => SetProperty(ref _totalLiquidDigestateProduced, value);
        }

        #endregion

        #region Public Methods

        #endregion
    }
}