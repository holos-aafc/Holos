#region Imports

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Prism.Mvvm;

#endregion

namespace H.Core.Models
{
    /// <summary>
    /// </summary>
    public class ApplicationData : BindableBase
    {
        #region Fields

        private GlobalSettings _globalSettings;
        private DisplayUnitStrings _displayDisplayUnitStrings;
        private ObservableCollection<Farm> _farms;

        #endregion

        #region Constructors

        public ApplicationData()
        {
            this.GlobalSettings = new GlobalSettings();
            this.Farms = new ObservableCollection<Farm>();
            this.DisplayUnitStrings = new DisplayUnitStrings();
        }

        #endregion

        #region Properties

        public GlobalSettings GlobalSettings
        {
            get { return _globalSettings; }
            set { this.SetProperty(ref _globalSettings, value); }
        }

        public ObservableCollection<Farm> Farms
        {
            get { return _farms; }
            set { this.SetProperty(ref _farms, value); }
        }

        public DisplayUnitStrings DisplayUnitStrings
        {
            get { return _displayDisplayUnitStrings; }
            set { SetProperty(ref _displayDisplayUnitStrings, value); }
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion

        #region Event Handlers

        #endregion
    }
}