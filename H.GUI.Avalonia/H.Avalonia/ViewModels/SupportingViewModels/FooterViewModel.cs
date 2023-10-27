using Prism.Regions;

namespace H.Avalonia.ViewModels.SupportingViewModels
{
    public class FooterViewModel : ViewModelBase
    {
        public FooterViewModel()
        {

        }

        public FooterViewModel(IRegionManager regionManager) : base(regionManager) { }
    }
}
