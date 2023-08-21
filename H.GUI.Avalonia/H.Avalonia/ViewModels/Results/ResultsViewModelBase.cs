using H.Core.Providers;
using Prism.Regions;
using SharpKml.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Avalonia.ViewModels.Results
{
    public class ResultsViewModelBase : ViewModelBase
    {
        protected ResultsViewModelBase() { }

        protected ResultsViewModelBase(IRegionManager regionManager, Storage storage) : base(regionManager, storage)
        {
        }
    }
}
