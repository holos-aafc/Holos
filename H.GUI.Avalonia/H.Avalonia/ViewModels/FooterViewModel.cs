using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Avalonia.ViewModels
{
    public class FooterViewModel : ViewModelBase
    {
        public FooterViewModel()
        {

        }

        public FooterViewModel(IRegionManager regionManager) : base(regionManager) { }
    }
}
