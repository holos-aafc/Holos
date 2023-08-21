using Avalonia.Controls.Chrome;
using H.Avalonia.Views;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Avalonia.ViewModels
{
    public class SidebarViewModel : ViewModelBase
    {
        private string? _title;
        private readonly IRegionManager _regionManager = null!;

        public SidebarViewModel()
        {
            Title = Core.Properties.Resources.ApplicationTitle;
        }

        public SidebarViewModel(IRegionManager regionManager) : base(regionManager)
        {
            Title = Core.Properties.Resources.ApplicationTitle;
            _regionManager = regionManager;
            SwitchToClimateViewCommand = new DelegateCommand(OnClickSwitchToClimateView);
            SwitchToSoilViewCommand = new DelegateCommand(OnClickSwitchToSoilView);
            SwitchToAboutViewCommand = new DelegateCommand(OnClickSwitchToAboutView);
        }

        /// <summary>
        /// The title to be displayed in the sidebar
        /// </summary>
        public string? Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        /// <summary>
        /// A command that helps switch the view to <see cref="ClimateDataView"/>
        /// </summary>
        public DelegateCommand SwitchToClimateViewCommand { get; set; } = null!;

        /// <summary>
        /// A command that helps switch view to <see cref="SoilDataView"/>
        /// </summary>
        public DelegateCommand SwitchToSoilViewCommand { get; set; } = null!;

        /// <summary>
        /// A command that helps switch view to <see cref="AboutPageView"/>
        /// </summary>
        public DelegateCommand SwitchToAboutViewCommand { get; set; } = null!;

        private void OnClickSwitchToClimateView()
        {
            _regionManager.RequestNavigate(UiRegions.ContentRegion, nameof(ClimateDataView));
        }

        private void OnClickSwitchToSoilView()
        {
            _regionManager.RequestNavigate(UiRegions.ContentRegion, nameof(SoilDataView));
        }

        private void OnClickSwitchToAboutView()
        {
            _regionManager.RequestNavigate(UiRegions.ContentRegion, nameof(AboutPageView));
        }

    }
}