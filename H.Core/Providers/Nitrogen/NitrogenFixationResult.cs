using H.Core.Enumerations;

namespace H.Core.Providers.Nitrogen
{
    public class NitrogenFixationResult
    {
        #region Properties

        public CropType CropType { get; set; }

        /// <summary>
        /// The share of the crop's nitrogen that comes from biological N2 fixation - %NDFA expressed as a fraction,
        /// so 0.55 means 55%. A proportion, not an amount.
        /// </summary>
        public double Fixation { get; set; }

        #endregion
    }
}