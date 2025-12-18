using H.Core.Enumerations;

namespace H.Core.Providers.Nitrogen
{
    public class NitrogenFixationResult
    {
        #region Properties

        public CropType CropType { get; set; }

        /// <summary>
        /// Biological N2 fixation (kg N ha^-1 yr^-1)
        /// </summary>
        public double Fixation { get; set; }

        #endregion
    }
}