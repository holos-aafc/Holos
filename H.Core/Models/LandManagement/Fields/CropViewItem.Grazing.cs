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

        #endregion
    }
}