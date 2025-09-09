#region Imports

using System.Collections.ObjectModel;
using Prism.Mvvm;

#endregion

namespace H.Core.Models
{
    /// <summary>
    /// </summary>
    public class ApplicationData : BindableBase
    {
        #region Constructors

        public ApplicationData()
        {
            GlobalSettings = new GlobalSettings();
            Farms = new ObservableCollection<Farm>();
            DisplayUnitStrings = new DisplayUnitStrings();
        }

        #endregion

        #region Fields

        private GlobalSettings _globalSettings;
        private DisplayUnitStrings _displayDisplayUnitStrings;
        private ObservableCollection<Farm> _farms;

        #endregion

        #region Properties

        public GlobalSettings GlobalSettings
        {
            get => _globalSettings;
            set => SetProperty(ref _globalSettings, value);
        }

        public ObservableCollection<Farm> Farms
        {
            get => _farms;
            set => SetProperty(ref _farms, value);
        }

        public DisplayUnitStrings DisplayUnitStrings
        {
            get => _displayDisplayUnitStrings;
            set => SetProperty(ref _displayDisplayUnitStrings, value);
        }

        #endregion
    }
}