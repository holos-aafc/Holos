using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Avalonia.ViewModels
{
    public class ViewModelBase : BindableBase, INavigationAware
    {
        private Storage? _storage;

        protected ViewModelBase()
        {
        }
        protected ViewModelBase(Storage storage)
        {
            Storage = storage;
        }

        protected ViewModelBase(IRegionManager regionManager)
        {

        }

        protected ViewModelBase(IRegionManager regionManager, Storage storage)
        {
            Storage = storage;
        }

        /// <summary>
        /// A storage file that contains various data items that are shored between viewmodels are passed around the system. This storage
        /// item is instantiated using Prism and through Dependency Injection, is passed within the system.
        /// </summary>
        public Storage? Storage
        {
            get => _storage;
            set => SetProperty(ref _storage, value);
        }
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        /// <summary>Navigation validation checker.</summary>
        /// <remarks>Override for Prism 7.2's IsNavigationTarget.</remarks>
        /// <param name="navigationContext">The navigation context.</param>
        /// <returns><see langword="true"/> if this instance accepts the navigation request; otherwise, <see langword="false"/>.</returns>
        public virtual bool OnNavigatingTo(NavigationContext navigationContext)
        {
            return true;
        }
    }
}
