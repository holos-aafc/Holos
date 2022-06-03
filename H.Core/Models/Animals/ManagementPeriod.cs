#region Imports

using AutoMapper;
using H.Core.Converters;
using H.Core.CustomAttributes;
using H.Core.Enumerations;
using H.Core.Providers.Feed;
using H.Infrastructure;
using System;
using System.ComponentModel;

#endregion


namespace H.Core.Models.Animals
{
    /// <summary>
    /// </summary>
    public class ManagementPeriod : ModelBase, ITimePeriodItem
    {
        #region Fields

        private Diet _selectedDiet;
        private Diet _displayDiet;

        private DateTime _start;
        private DateTime _end;

        private TimeSpan _duration;

        private DietAdditiveType _dietAdditive;
        private ManureDetails _manureDetails;
        private HousingDetails _housingDetails;
        private AnimalType _animalType;
        private AnimalType _dietGroupType;
        private ProductionStages _productionStage;

        private int _numberOfAnimals;
        private int _numberOfYoungAnimals;
        private int _numberOfDays;
        
        private string _groupName;

        private double _energyRequiredForMilk;
        private double _energyRequiredForWool;
        private double _woolProduction;
        private double _gainCoefficient;
        private double _startWeight;
        private double _endWeight;
        private double _periodDailyGain;
        private double _reproductiveRate;
        private double _gainCoefficientA;
        private double _gainCoefficientB;
        private double _nitrogenRequiredForGain;
        private double _milkProduction;
        private double _milkProteinContentAsPercentage;
        private double _milkFatContent;
        private double _feedIntakeAmount;
        private double _fertilityRateOfBirthingAnimals;
        private double _liveWeightChangeOfPregnantAnimal;
        private double _liveWeightOfYoungAtWeaningAge;
        private double _liveWeightOfYoungAtBirth;

        private bool _useCustomMilkProductionValue;

        private bool _animalsAreMilkFedOnly;

        #endregion

        #region Constructors

        public ManagementPeriod()
        {
            this.Start = new DateTime(DateTime.Now.Year, 1, 1);

            this.ManureDetails = new ManureDetails();

            this.HousingDetails = new HousingDetails();

            this.NumberOfAnimals = 20;
            this.MilkProduction = 8;
            this.MilkFatContent = 4;
            this.MilkProteinContentAsPercentage = 3.5;
        }

        #endregion

        #region Properties

        public ManureDetails ManureDetails
        {
            get { return _manureDetails; }
            set { this.SetProperty(ref _manureDetails, value); }
        }

        public HousingDetails HousingDetails
        {
            get { return _housingDetails; }
            set { this.SetProperty(ref _housingDetails, value); }
        }

        /// <summary>
        /// Energy required to produce 1 kg of milk.
        /// 
        /// MJ kg^-1
        /// </summary>
        [Units(MetricUnitsOfMeasurement.MegaJoulesPerKilogram)]
        public double EnergyRequiredForMilk
        {
            get { return _energyRequiredForMilk; }
            set { this.SetProperty(ref _energyRequiredForMilk, value); }
        }

        /// <summary>
        /// MJ kg^-1
        /// </summary>
        [Units(MetricUnitsOfMeasurement.MegaJoulesPerKilogram)]
        public double EnergyRequiredForWool
        {
            get { return _energyRequiredForWool; }
            set { this.SetProperty(ref _energyRequiredForWool, value); }
        }

        /// <summary>
        /// Start weight of animals (kg)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.Kilograms)]
        public double StartWeight
        {
            get { return _startWeight; }
            set { this.SetProperty(ref _startWeight, value); }
        }

        /// <summary>
        /// End weight of animals (kg)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.Kilograms)]
        public double EndWeight
        {
            get { return _endWeight; }
            set { this.SetProperty(ref _endWeight, value); }
        }

        /// <summary>
        /// (kg)
        /// </summary>
        public double AverageWeight
        {
            get
            {
                return (this.EndWeight - this.StartWeight) / 2;
            }
        }

        /// <summary>
        /// The daily gain of the animals (kg head-1 day-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.Kilograms)]
        public double PeriodDailyGain
        {
            get { return _periodDailyGain; }
            set { this.SetProperty(ref _periodDailyGain, value); }
        }

        public string GroupName
        {
            get { return _groupName; }
            set { SetProperty(ref _groupName, value); }
        }

        public Guid AnimalGroupGuid { get; set; }

        /// <summary>
        /// The total number of animals
        /// </summary>
        public int NumberOfAnimals
        {
            get { return _numberOfAnimals; }
            set { SetProperty(ref _numberOfAnimals, value); }
        }

        public AnimalType AnimalType
        {
            get { return _animalType; }
            set { SetProperty(ref _animalType, value); }
        }

        public DietAdditiveType DietAdditive
        {
            get { return _dietAdditive; }
            set { SetProperty(ref _dietAdditive, value); }
        }

        public AnimalType DietGroupType
        {
            get { return _dietGroupType; }
            set { SetProperty(ref _dietGroupType, value); }
        }

        public double ReproductiveRate
        {
            get { return _reproductiveRate; }
            set { this.SetProperty(ref _reproductiveRate, value); }
        }

        /// <summary>
        /// Milk produced per day (kg head⁻¹ day⁻¹)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.Kilograms)]
        public double MilkProduction
        {
            get { return _milkProduction; }
            set { SetProperty(ref _milkProduction, value); }
        }

        /// <summary>
        /// Fat content of milk (%)
        /// </summary>
        public double MilkFatContent
        {
            get { return _milkFatContent; }
            set { SetProperty(ref _milkFatContent, value); }
        }

        /// <summary>
        /// Protein content of milk (%)
        /// </summary>
        public double MilkProteinContentAsPercentage
        {
            get { return _milkProteinContentAsPercentage; }
            set { SetProperty(ref _milkProteinContentAsPercentage, value); }
        }

        /// <summary>
        /// Protein content of milk (kg kg⁻¹)
        /// </summary>
        public double MilkProteinContent
        {
            get { return this.MilkProteinContentAsPercentage / 100; }
        }

        /// <summary>
        /// Wool produced per year (kg year^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.Kilograms)]
        public double WoolProduction
        {
            get { return _woolProduction; }
            set { SetProperty(ref _woolProduction, value); }
        }

        /// <summary>
        /// C_d
        /// </summary>
        public double GainCoefficient
        {
            get { return _gainCoefficient; }
            set { SetProperty(ref _gainCoefficient, value); }
        }

        /// <summary>
        /// (kg head^-1 day^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.Kilograms)]
        public double FeedIntakeAmount
        {
            get { return _feedIntakeAmount; }
            set { SetProperty(ref _feedIntakeAmount, value); }
        }

        /// <summary>
        /// MJ kg^-1
        /// </summary>
        [Units(MetricUnitsOfMeasurement.MegaJoulesPerKilogram)]
        public double GainCoefficientA
        {
            get { return _gainCoefficientA; }
            set { SetProperty(ref _gainCoefficientA, value); }
        }

        /// <summary>
        /// MJ kg^-2
        /// </summary>
        [Units(MetricUnitsOfMeasurement.MegaJoulesPerKilogramSquared)]
        public double GainCoefficientB
        {
            get { return _gainCoefficientB; }
            set { SetProperty(ref _gainCoefficientB, value); }
        }

        /// <summary>
        /// The selected diet.
        /// </summary>
        public Diet SelectedDiet
        {
            get { return _selectedDiet; }
            set { SetProperty(ref _selectedDiet, value); }
        }

        /// <summary>
        /// Note used.
        /// </summary>
        [Obsolete]
        public Diet DisplayDiet
        {
            get { return _displayDiet; }
            set { SetProperty(ref _displayDiet, value); }
        }

        /// <summary>
        /// The start date of this <see cref="ManagementPeriod"/>.
        /// </summary>
        public DateTime Start
        {
            get { return _start; }
            set
            {
                // Don't allow the start date to start after the end date
                if (value > this.End && this.End.Equals(default(DateTime)) == false)
                {
                    return;
                }

                SetProperty(ref _start, value);
            }
        }

        /// <summary>
        /// The end date of this <see cref="ManagementPeriod"/>.
        /// </summary>
        public DateTime End
        {
            get { return _end; }
            set
            {
                // Don't allow the end date to begin before the start date
                if (value < this.Start && this.Start.Equals(default(DateTime)) == false)
                {
                    return;
                }

                SetProperty(ref _end, value);
            }
        }

        public TimeSpan Duration
        {
            get { return _duration; }
            set { SetProperty(ref _duration, value); }
        }

        /// <summary>
        /// The total number of days for this <see cref="ManagementPeriod"/>.
        /// </summary>
        public int NumberOfDays
        {
            get { return _numberOfDays; }
            set { SetProperty(ref _numberOfDays, value); }
        }

        /// <summary>
        /// Indicates if the animals have grown from the start of the management period to the end
        /// </summary>
        public bool HasGrowingAnimals
        {
            get { return Math.Abs(this.StartWeight - this.EndWeight) > double.Epsilon; }
        }

        public int NumberOfYoungAnimals
        {
            get => _numberOfYoungAnimals;
            set => SetProperty(ref _numberOfYoungAnimals, value);
        }

        /// <summary>
        /// (litter year^-1)
        /// </summary>
        public double FertilityRateOfBirthingAnimals
        {
            get => _fertilityRateOfBirthingAnimals;
            set => SetProperty(ref _fertilityRateOfBirthingAnimals, value);
        }

        /// <summary>
        /// (kg head^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.Kilograms)]
        public double LiveWeightChangeOfPregnantAnimal
        {
            get => _liveWeightChangeOfPregnantAnimal;
            set => SetProperty(ref _liveWeightChangeOfPregnantAnimal, value);
        }

        /// <summary>
        /// (kg head^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.Kilograms)]
        public double LiveWeightOfYoungAtWeaningAge
        {
            get => _liveWeightOfYoungAtWeaningAge;
            set => SetProperty(ref _liveWeightOfYoungAtWeaningAge, value);
        }

        /// <summary>
        /// (kg head ^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.Kilograms)]
        public double LiveWeightOfYoungAtBirth
        {
            get => _liveWeightOfYoungAtBirth;
            set => SetProperty(ref _liveWeightOfYoungAtBirth, value);
        }

        /// <summary>
        /// (kg N kg BW^-1)
        /// </summary>
        public double NitrogenRequiredForGain
        {
            get => _nitrogenRequiredForGain;
            set => SetProperty(ref _nitrogenRequiredForGain, value);
        }

        public ProductionStages ProductionStage
        {
            get => _productionStage;
            set => SetProperty(ref _productionStage, value);
        }

        /// <summary>
        /// Used to indicate when animals are not consuming forage but only milk (distinction needed for calculate enteric methane for beef calves)
        /// </summary>
        public bool AnimalsAreMilkFedOnly
        {
            get => _animalsAreMilkFedOnly;
            set => SetProperty(ref _animalsAreMilkFedOnly, value);
        }

        public bool UseCustomMilkProductionValue
        {
            get => _useCustomMilkProductionValue;
            set => SetProperty(ref _useCustomMilkProductionValue, value);
        }

        /// <summary>
        /// Indicates if the manure emissions should be calculated by the anaerobic digester algorithms
        /// </summary>
        public bool IsProcessedByAnaerobicDigester { get; set; }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return $"{nameof(this.Start)}: {this.Start}, {nameof(this.End)}: {this.End}";
        }

        #endregion

        #region Private Methods

        #endregion

        #region Event Handlers

        #endregion
    }
}