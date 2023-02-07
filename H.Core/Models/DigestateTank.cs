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

        private double _totalDigestateAfterAllApplications;
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

        public double TotalDigestateAfterAllApplications
        {
            get => _totalDigestateAfterAllApplications;
            set => SetProperty(ref _totalDigestateAfterAllApplications, value);
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

        public double TotalDigestateAvailablePerHectare(double area)
        {
            return this.TotalDigestateAfterAllApplications / area;
        }

        public override void ResetTank()
        {
            base.ResetTank();

            this.TotalDigestateAfterAllApplications = 0;
        }

        #endregion
    }
}