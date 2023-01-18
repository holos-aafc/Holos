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

        private double _totalDigestateAfterAllApplication;
        private double _totalSolidDigestateAfterAllApplications;
        private double _totalLiquidDigestateAfterAllApplications;
        private double _volumeOfAllDigestateApplications;

        private double _totalDigestateProducedBySystem;

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

        public double TotalDigestateAfterAllApplication
        {
            get => _totalDigestateAfterAllApplication;
            set => SetProperty(ref _totalDigestateAfterAllApplication, value);
        }

        public double TotalSolidDigestateAfterAllApplications
        {
            get => _totalSolidDigestateAfterAllApplications;
            set => SetProperty(ref _totalSolidDigestateAfterAllApplications, value);
        }

        public double TotalLiquidDigestateAfterAllApplications
        {
            get => _totalLiquidDigestateAfterAllApplications;
            set => SetProperty(ref _totalLiquidDigestateAfterAllApplications, value);
        }

        public double VolumeOfAllDigestateApplications
        {
            get => _volumeOfAllDigestateApplications;
            set => SetProperty(ref _volumeOfAllDigestateApplications, value);
        }

        public double TotalDigestateProducedBySystem
        {
            get => _totalDigestateProducedBySystem;
            set => SetProperty(ref  _totalDigestateProducedBySystem, value);
        }

        #endregion

        #region Public Methods

        public override void ResetTank()
        {
            base.ResetTank();

            this.TotalDigestateAfterAllApplication = 0;
            this.TotalLiquidDigestateAfterAllApplications = 0;
            this.TotalSolidDigestateAfterAllApplications = 0;
        }

        #endregion
    }
}