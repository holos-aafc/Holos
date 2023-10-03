using Prism.Commands;
using Prism.Regions;

namespace H.Avalonia.ViewModels.ResultViewModels
{
    public class ResultsViewModelBase : ViewModelBase
    {
        private bool _processing;
        
        protected ResultsViewModelBase() { }

        protected ResultsViewModelBase(IRegionManager regionManager, Storage storage) : base(regionManager, storage)
        {
        }
        
        /// <summary>
        /// A command that triggers when a user clicks the back button on the page.
        /// </summary>
        public DelegateCommand GoBackCommand { get; set; }

        /// <summary>
        /// A command that triggers when a user clicks the export to csv button on the page.
        /// </summary>
        public DelegateCommand<object> ExportToCsvCommand { get; set; }
        
        /// <summary>
        /// A bool that checks if data extraction is currently processing or not. Returns true if data is still processing, return false otherwise.
        /// </summary>
        public bool IsProcessingData
        {
            get => _processing;
            set => SetProperty(ref _processing, value);
        }
    }
}
