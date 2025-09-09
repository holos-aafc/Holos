﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using H.Core.CustomAttributes;
using H.Core.Enumerations;
using H.Core.Providers.Animals;
using H.Infrastructure;

namespace H.Core.Models.LandManagement.Fields
{
    /// <summary>
    ///     A class to specify the details of one application of manure to a field.
    /// </summary>
    public class ManureApplicationViewItem : ManureItemBase
    {
        #region Constructors

        public ManureApplicationViewItem()
        {
            DateOfApplication = DateTime.Now;
            ManureLocationSourceType = ManureLocationSourceType.Livestock;
            DefaultManureCompositionData = new DefaultManureCompositionData();

            AvailableManureApplicationTypes = new ObservableCollection<ManureApplicationTypes>();

            PropertyChanged += OnPropertyChanged;
        }

        #endregion

        #region Methods

        public bool IsImportedManure()
        {
            return ManureLocationSourceType == ManureLocationSourceType.Imported ||
                   AnimalType == AnimalType.NotSelected;
        }

        #endregion

        #region Event Handlers

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        #endregion

        #region Fields

        private DateTime _dateOfApplication;

        private double _amount;
        private double _amountOfNitrogenInManureApplied;
        private double _amountOfManureAppliedPerHectare;

        private ManureAnimalSourceTypes _manureAnimalSourceType;
        private ManureApplicationTypes _manureApplicationTypes;

        private ObservableCollection<ManureApplicationTypes> _availableManureApplicationTypes;

        #endregion

        #region Properties

        public DateTime DateOfApplication
        {
            get => _dateOfApplication;
            set => SetProperty(ref _dateOfApplication, value);
        }

        public ObservableCollection<ManureApplicationTypes> AvailableManureApplicationTypes
        {
            get => _availableManureApplicationTypes;
            set => SetProperty(ref _availableManureApplicationTypes, value);
        }

        /// <summary>
        ///     Indicates if the manure was sourced from beef cattle, dairy cattle, etc.
        ///     This must be entered from the user so we can differentiate the N, C, and P fraction used by different handling
        ///     systems (i.e. beef and dairy both have 'solid storage (stockpiled)' but
        ///     different fractions for N, C, and P fractions
        /// </summary>
        public ManureAnimalSourceTypes ManureAnimalSourceType
        {
            get => _manureAnimalSourceType;
            set => SetProperty(ref _manureAnimalSourceType, value);
        }

        public string ManureApplicationTypeString => ManureApplicationMethod.GetDescription();

        /// <summary>
        ///     Amount of manure applied
        ///     (kg)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.Kilograms)]
        public double Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        /// <summary>
        ///     Amount of nitrogen in the amount of manure applied
        ///     (kg N)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramsNitrogen)]
        public double AmountOfNitrogenInManureApplied
        {
            get => _amountOfNitrogenInManureApplied;
            set => SetProperty(ref _amountOfNitrogenInManureApplied, value);
        }

        /// <summary>
        ///     Amount (volume) of manure applied per hectare (wet weight)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramsPerHectare)]
        public double AmountOfManureAppliedPerHectare
        {
            get => _amountOfManureAppliedPerHectare;
            set => SetProperty(ref _amountOfManureAppliedPerHectare, value);
        }

        /// <summary>
        ///     Indicates the method that was used to apply the manure to the land
        /// </summary>
        public ManureApplicationTypes ManureApplicationMethod
        {
            get => _manureApplicationTypes;
            set => SetProperty(ref _manureApplicationTypes, value);
        }

        #endregion
    }
}