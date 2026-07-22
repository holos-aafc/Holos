#region Imports

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
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

        /// <summary>
        /// <see cref="GlobalSettings.ActiveFarm"/> is not written to file - only its guid is - so after loading it must
        /// be re-pointed at the corresponding farm in <see cref="Farms"/>. Without this the active farm would be a
        /// separate object from the one the user edits.
        /// </summary>
        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            if (this.GlobalSettings == null || this.Farms == null || this.Farms.Any() == false)
            {
                return;
            }

            // Files written before ActiveFarmGuid existed still carry the farm itself, so fall back to its guid.
            var guid = this.GlobalSettings.GetPersistedActiveFarmGuid();

            var farm = this.Farms.FirstOrDefault(x => x.Guid == guid);
            if (farm == null)
            {
                farm = this.Farms.First();
            }

            this.GlobalSettings.RestoreActiveFarm(farm);

            // The comparison selection is stored the same way - as guids into Farms.
            var comparisonGuids = this.GlobalSettings.GetPersistedFarmsForComparisonGuids();
            var farmsForComparison = comparisonGuids
                .Select(x => this.Farms.FirstOrDefault(y => y.Guid == x))
                .Where(x => x != null)
                .ToList();

            this.GlobalSettings.RestoreFarmsForComparison(farmsForComparison);
        }

        #endregion

        #region Private Methods

        #endregion

        #region Event Handlers

        #endregion
    }
}