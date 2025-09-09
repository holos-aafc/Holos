using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Animals;

namespace H.Core.Models
{
    public partial class Farm
    {
        #region Fields

        private ObservableCollection<ManureExportViewItem> _manureExportViewItems =
            new ObservableCollection<ManureExportViewItem>();

        #endregion

        #region Properties

        public ObservableCollection<ManureExportViewItem> ManureExportViewItems
        {
            get => _manureExportViewItems;
            set => SetProperty(ref _manureExportViewItems, value);
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     When a farm is initialized, defaults are assigned to the farm. The user can then change these values if they wish.
        ///     This data is therefore held here in the farm object since it is specific
        ///     to this farm instance and lookups should be made here and not from the provider class since this method will
        ///     persist changes. Returns data associated with table 9.
        /// </summary>
        public DefaultManureCompositionData GetManureCompositionData(
            ManureStateType manureStateType,
            AnimalType animalType)
        {
            var defaultValue = new DefaultManureCompositionData();

            AnimalType animalLookupType;
            if (animalType.IsBeefCattleType())
                animalLookupType = AnimalType.Beef;
            else if (animalType.IsDairyCattleType())
                animalLookupType = AnimalType.Dairy;
            else if (animalType.IsSheepType())
                animalLookupType = AnimalType.Sheep;
            else if (animalType.IsSwineType())
                animalLookupType = AnimalType.Swine;
            else if (animalType.IsPoultryType())
                animalLookupType = AnimalType.Poultry;
            else
                // Other animals have a value for animal group (Horses, Goats, etc.)
                animalLookupType = animalType;

            var manureCompositionData = DefaultManureCompositionData.SingleOrDefault(x =>
                x.ManureStateType == manureStateType &&
                x.AnimalType == animalLookupType);

            if (manureCompositionData == null)
            {
                Trace.TraceError($"{nameof(Farm)}.{nameof(GetManureCompositionData)}" +
                                 $" unable to get data for manure animal source type: {animalType}, and manure state type: {manureStateType}." +
                                 $" Returning default value of {defaultValue}.");

                return defaultValue;
            }

            return manureCompositionData;
        }

        #endregion
    }
}