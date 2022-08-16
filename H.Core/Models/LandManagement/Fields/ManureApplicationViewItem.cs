using H.Core.Enumerations;
using H.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using H.Core.CustomAttributes;
using H.Core.Providers.Animals;

namespace H.Core.Models.LandManagement.Fields
{
    /// <summary>
    /// A class to specify the details of one application of manure to a field.
    /// </summary>
    public class ManureApplicationViewItem : ModelBase
    {
        #region Fields

        private DateTime _dateOfApplication;

        private double _amount;
        private double _amountOfNitrogenInManureApplied;
        private double _amountOfManureAppliedPerHectare;
        private double _amountOfNitrogenAppliedPerHectare;

        private ManureAnimalSourceTypes _manureAnimalSourceType;
        private ManureLocationSourceType manureLocationSourceType;
        private ManureStateType _manureStateType;
        private DefaultManureCompositionData _defaultManureCompositionData;
        private ManureApplicationTypes _manureApplicationTypes;
        private AnimalType _animalType;

        private ObservableCollection<ManureStateType> _validManureStateTypesForSelectedTypeOfAnimalManure;
        private ObservableCollection<ManureApplicationTypes> _availableManureApplicationTypes;

        #endregion

        #region Constructors

        public ManureApplicationViewItem()
        {
            this.DateOfApplication = DateTime.Now;
            this.DefaultManureCompositionData = new DefaultManureCompositionData();

            this.ValidManureStateTypesForSelectedTypeOfAnimalManure = new ObservableCollection<ManureStateType>()
            {
                ManureStateType.NotSelected,
            };

            this.AvailableManureApplicationTypes = new ObservableCollection<ManureApplicationTypes>();

            this.PropertyChanged += OnPropertyChanged;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Each view item must have its own collection of valid state types so the table rows presented to the user will have their own distinct collection
        /// </summary>
        public ObservableCollection<ManureStateType> ValidManureStateTypesForSelectedTypeOfAnimalManure
        {
            get => _validManureStateTypesForSelectedTypeOfAnimalManure;
            set => SetProperty(ref _validManureStateTypesForSelectedTypeOfAnimalManure, value);
        }

        public ObservableCollection<ManureApplicationTypes> AvailableManureApplicationTypes
        {
            get => _availableManureApplicationTypes;
            set => SetProperty(ref _availableManureApplicationTypes, value);
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
        /// Indicates if the manure was sourced from beef cattle, dairy cattle, etc.
        /// 
        /// This must be entered from the user so we can differentiate the N, C, and P fraction used by different handling systems (i.e. beef and dairy both have 'solid storage (stockpiled)' but
        /// different fractions for N, C, and P fractions
        /// </summary>
        public ManureAnimalSourceTypes ManureAnimalSourceType 
        { 
            get => _manureAnimalSourceType; 
            set => SetProperty(ref  _manureAnimalSourceType, value); 
        }

        public string ManureApplicationTypeString
        {
            get
            {
                return this.ManureApplicationMethod.GetDescription();
            }
        }
        
        /// <summary>
        /// Indicates if the manure was from livestock on farm or imported from off-farm.
        /// </summary>
        public ManureLocationSourceType ManureLocationSourceType 
        { 
            get => manureLocationSourceType; 
            set => SetProperty(ref manureLocationSourceType, value); 
        }

        public DateTime DateOfApplication 
        { 
            get => _dateOfApplication; 
            set => SetProperty(ref _dateOfApplication, value); 
        }

        /// <summary>
        /// Amount of manure applied
        ///
        /// (kg)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.Kilograms)]
        public double Amount 
        { 
            get => _amount; 
            set => SetProperty(ref _amount, value); 
        }

        /// <summary>
        /// Amount of nitrogen in the amount of manure applied
        ///
        /// (kg N)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramsNitrogen)]
        public double AmountOfNitrogenInManureApplied
        {
            get => _amountOfNitrogenInManureApplied;
            set => SetProperty(ref _amountOfNitrogenInManureApplied, value);
        }

        /// <summary>
        /// Amount of manure applied per hectare (wet weight)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramsPerHectare)]
        public double AmountOfManureAppliedPerHectare
        {
            get => _amountOfManureAppliedPerHectare;
            set => SetProperty(ref _amountOfManureAppliedPerHectare, value);
        }

        /// <summary>
        /// Amount of nitrogen applied per hectare from the amount of manure applied
        ///
        /// (kg N ha^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare)]
        public double AmountOfNitrogenAppliedPerHectare
        {
            get => _amountOfNitrogenAppliedPerHectare;
            set => SetProperty(ref _amountOfNitrogenAppliedPerHectare, value);
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
        /// Animal type is required for lookups into default manure composition table since 'other animals' requires the specific animal group for lookups
        /// </summary>
        public AnimalType AnimalType
        {
            get => _animalType;
            set => SetProperty(ref _animalType, value);
        }

        /// <summary>
        /// Indicates the method that was used to apply the manure to the land
        /// </summary>
        public ManureApplicationTypes ManureApplicationMethod
        {
            get => _manureApplicationTypes;
            set => SetProperty(ref _manureApplicationTypes, value);
        }

        #endregion

        #region Methods

        #endregion

        #region Private Methods    

        #endregion

        #region Event Handlers

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {                     
        }

        private void DefaultManureCompositionDataOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {           
        }

        #endregion
    }
}
