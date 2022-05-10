using System.Collections.ObjectModel;

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
        /// Equation 12.3.2-4
        ///
        /// (kg C)
        /// </summary>
        public double TotalCarbonLossesByGrazingAnimals
        {
            get => _totalCarbonLossesFromGrazingAnimals;
            set => SetProperty(ref _totalCarbonLossesFromGrazingAnimals, value);
        }

        public double TotalCarbonInputFromManureFromAnimalsGrazingOnPasture { get; set; }

        #endregion
    }
}