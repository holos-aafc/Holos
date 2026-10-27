using System.Collections.ObjectModel;
using System.Linq;

namespace H.Core.Models.LandManagement.Fields
{
    public partial class CropViewItem
    {
        #region Fields

        private double _totalCarbonLossesFromGrazingAnimals;

        private ObservableCollection<GrazingViewItem> _grazingViewItems;

        #endregion

        #region Properties

        public ObservableCollection<GrazingViewItem> GrazingViewItems
        {
            get => _grazingViewItems;
            set => SetProperty(ref _grazingViewItems, value);
        }

        /// <summary>
        /// (kg C)
        /// </summary>
        public double TotalCarbonLossesByGrazingAnimals
        {
            get => _totalCarbonLossesFromGrazingAnimals;
            set => SetProperty(ref _totalCarbonLossesFromGrazingAnimals, value);
        }

        /// <summary>
        /// (kg C ha^-1)
        /// </summary>
        public double TotalCarbonInputFromManureFromAnimalsGrazingOnPasture { get; set; }

        /// <summary>
        /// (kg N ha^-1)
        /// </summary>
        public double TotalNitrogenInputFromManureFromAnimalsGrazingOnPasture { get; set; }

        public double TotalCarbonUptakeByAnimals { get; set; }

        #endregion

        #region Public Methods

        public double GetAverageUtilizationFromGrazingAnimals()
        {
            if (this.HasGrazingViewItems)
            {
                return this.GrazingViewItems.Average(x => x.Utilization);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Returns true if any <see cref="GrazingViewItem"/> in the collection has a start date that falls within the same year as this <see cref="CropViewItem"/>.
        /// </summary>
        /// <returns>True if at least one grazing item exists for the current year, false otherwise.</returns>
        public bool HasGrazingItemsForTheCurrentYear()
        {
            return this.GrazingViewItems.Any(x => x.Start.Year == this.Year);
        }

        /// <summary>
        /// The moisture content the yield is held at (fraction): in a grazed year, the average over the grazing periods,
        /// otherwise <see cref="MoistureContentOfCrop"/>. Used wherever the yield is converted to dry matter (Eq. 2.1.1-1,
        /// Eq. 2.1.2-1) so those equations work from the same dry matter.
        /// </summary>
        public double GetMoistureContentOfYield()
        {
            if (this.HasGrazingItemsForTheCurrentYear())
            {
                return this.GrazingViewItems.Average(x => x.MoistureContentAsPercentage) / 100.0;
            }

            return this.MoistureContentOfCrop;
        }

        #endregion
    }
}