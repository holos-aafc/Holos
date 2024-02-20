using System.Collections.ObjectModel;
using System.ComponentModel;
using H.Core.CustomAttributes;
using H.Core.Enumerations;
using H.Core.Providers.Animals;
using H.Infrastructure;

namespace H.Core.Models.LandManagement.Fields
{
    public abstract class ManureItemBase : ModelBase
    {
        #region Fields
        
        private AnimalType _animalType;
        private ManureStateType _manureStateType;
        private ObservableCollection<ManureStateType> _validManureStateTypesForSelectedTypeOfAnimalManure;
        private ManureLocationSourceType manureLocationSourceType;
        private DefaultManureCompositionData _defaultManureCompositionData;
        protected double _amountOfNitrogenAppliedPerHectare;

        #endregion

        #region Constructors

        protected ManureItemBase()
        {
            this.ValidManureStateTypesForSelectedTypeOfAnimalManure = new ObservableCollection<ManureStateType>()
            {
                ManureStateType.NotSelected,
            };
        }

        #endregion

        #region Properties

        /// <summary>
        /// Animal type is required for lookups into default manure composition table since 'other animals' requires the specific animal group for lookups
        /// </summary>
        public AnimalType AnimalType
        {
            get => _animalType;
            set => SetProperty(ref _animalType, value);
        }

        /// <summary>
        /// This must be entered by user so we can look up N, C, and P fractions for the applied manure
        /// </summary>
        public ManureStateType ManureStateType
        {
            get => _manureStateType;
            set => SetProperty(ref _manureStateType, value);
        }

        /// <summary>
        /// Each view item must have its own collection of valid state types so the table rows presented to the user will have their own distinct collection
        /// </summary>
        public ObservableCollection<ManureStateType> ValidManureStateTypesForSelectedTypeOfAnimalManure
        {
            get => _validManureStateTypesForSelectedTypeOfAnimalManure;
            set => SetProperty(ref _validManureStateTypesForSelectedTypeOfAnimalManure, value);
        }

        /// <summary>
        /// Indicates if the manure was from livestock on farm or imported from off-farm.
        /// </summary>
        public ManureLocationSourceType ManureLocationSourceType 
        { 
            get => manureLocationSourceType; 
            set => SetProperty(ref manureLocationSourceType, value); 
        }

        public DefaultManureCompositionData DefaultManureCompositionData
        {
            get => _defaultManureCompositionData;
            set => SetProperty(ref _defaultManureCompositionData, value, () =>
            {
                if (this.DefaultManureCompositionData != null)
                {
                    this.DefaultManureCompositionData.PropertyChanged -= DefaultManureCompositionDataOnPropertyChanged;
                    this.DefaultManureCompositionData.PropertyChanged += DefaultManureCompositionDataOnPropertyChanged;
                }
            });
        }

        /// <summary>
        /// Amount of N applied
        ///
        /// (kg N ha^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare)]
        public double AmountOfNitrogenAppliedPerHectare
        {
            get => _amountOfNitrogenAppliedPerHectare;
            set
            {
                SetProperty(ref _amountOfNitrogenAppliedPerHectare, value);
            }
        }

        #endregion

        #region Event Handlers
        
        private void DefaultManureCompositionDataOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        } 

        #endregion
    }
}