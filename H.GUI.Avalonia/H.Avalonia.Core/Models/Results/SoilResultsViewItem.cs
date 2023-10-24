using H.Core.Enumerations;

namespace H.Avalonia.Core.Models.Results
{
    /// <summary>
    /// Contains properties that are tied to the Grid shown for the soil results page.
    /// </summary>
    public class SoilResultsViewItem : ModelBase
    {
        private Province _province;
        private SoilGreatGroupType _soilGreatGroup;
        private SoilTexture _soilTexture;
        private double _percentClayInSoil;
        private double _percentOrganicMatterInSoil;
        private double _soilPh;

        /// <summary>
        /// The province for which the soil data is extracted.
        /// </summary>
        public Province Province
        {
            get => _province;
            set => SetProperty(ref _province, value);
        }

        /// <summary>
        /// The great group type of a particular soil.
        /// </summary>
        public SoilGreatGroupType SoilGreatGroup
        {
            get => _soilGreatGroup;
            set => SetProperty(ref _soilGreatGroup, value);
        }

        /// <summary>
        /// The texture type of a particular soil.
        /// </summary>
        public SoilTexture SoilTexture
        {
            get => _soilTexture;
            set => SetProperty(ref _soilTexture, value);
        }

        /// <summary>
        /// The percent clay in a particular soil.
        /// </summary>
        public double PercentClayInSoil
        {
            get => _percentClayInSoil;
            set => SetProperty(ref _percentClayInSoil, value);
        }

        /// <summary>
        /// The acidity/potential of hydrogen amount of the soil.
        /// </summary>
        public double SoilPh
        {
            get => _soilPh;
            set => SetProperty(ref _soilPh, value);
        }

        /// <summary>
        /// The percent of organic matter in the soil.
        /// </summary>
        public double PercentOrganicMatterInSoil
        {
            get => _percentOrganicMatterInSoil;
            set => SetProperty(ref _percentOrganicMatterInSoil, value);
        }

        public SoilResultsViewItem()
        {

        }
    }
}