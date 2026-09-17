using H.Core.Enumerations;

namespace H.Core.Providers.Nitrogen
{
    public class NitrogenFixationResult
    {
        #region Properties

        public CropType CropType { get; set; }

        /// <summary>
        /// The share of the crop's nitrogen that comes from biological N2 fixation - %NDFA expressed as a fraction,
        /// so 0.55 means 55%. Not an amount; the table held kg N ha^-1 yr^-1 at one time and no longer does.
        /// </summary>
        public double Fixation { get; set; }

        #endregion
    }
}