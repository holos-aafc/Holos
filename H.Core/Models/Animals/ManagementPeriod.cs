#region Imports

using System;
using H.Core.CustomAttributes;
using H.Core.Enumerations;
using H.Core.Providers.Feed;
using H.Infrastructure;

#endregion


namespace H.Core.Models.Animals
{
    /// <summary>
    /// </summary>
    public class ManagementPeriod : ModelBase, ITimePeriodItem
    {
        #region Constructors

        public ManagementPeriod()
        {
            Start = new DateTime(DateTime.Now.Year, 1, 1);

            ManureDetails = new ManureDetails();
            HousingDetails = new HousingDetails();

            NumberOfAnimals = 20;
            MilkFatContent = 4;
            MilkProteinContentAsPercentage = 3.5;

            SelectedDiet = new Diet();
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return $"{nameof(Start)}: {Start}, {nameof(End)}: {End}";
        }

        #endregion

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

        #region Properties

        public ManureDetails ManureDetails
        {
            get => _manureDetails;
            set => SetProperty(ref _manureDetails, value);
        }

        public HousingDetails HousingDetails
        {
            get => _housingDetails;
            set => SetProperty(ref _housingDetails, value);
        }

        /// <summary>
        ///     Energy required to produce 1 kg of milk.
        ///     MJ kg^-1
        /// </summary>
        [Units(MetricUnitsOfMeasurement.MegaJoulesPerKilogram)]
        public double EnergyRequiredForMilk
        {
            get => _energyRequiredForMilk;
            set => SetProperty(ref _energyRequiredForMilk, value);
        }

        /// <summary>
        ///     MJ kg^-1
        /// </summary>
        [Units(MetricUnitsOfMeasurement.MegaJoulesPerKilogram)]
        public double EnergyRequiredForWool
        {
            get => _energyRequiredForWool;
            set => SetProperty(ref _energyRequiredForWool, value);
        }

        /// <summary>
        ///     Start weight of animals (kg)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.Kilograms)]
        public double StartWeight
        {
            get => _startWeight;
            set => SetProperty(ref _startWeight, value);
        }

        /// <summary>
        ///     End weight of animals (kg)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.Kilograms)]
        public double EndWeight
        {
            get => _endWeight;
            set => SetProperty(ref _endWeight, value);
        }

        /// <summary>
        ///     (kg)
        /// </summary>
        public double AverageWeight => (EndWeight - StartWeight) / 2;

        /// <summary>
        ///     The daily gain of the animals (kg head-1 day-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.Kilograms)]
        public double PeriodDailyGain
        {
            get => _periodDailyGain;
            set => SetProperty(ref _periodDailyGain, value);
        }

        public string GroupName
        {
            get => _groupName;
            set => SetProperty(ref _groupName, value);
        }

        public Guid AnimalGroupGuid { get; set; }

        /// <summary>
        ///     The total number of animals
        /// </summary>
        public int NumberOfAnimals
        {
            get => _numberOfAnimals;
            set => SetProperty(ref _numberOfAnimals, value);
        }

        public AnimalType AnimalType
        {
            get => _animalType;
            set => SetProperty(ref _animalType, value);
        }

        public DietAdditiveType DietAdditive
        {
            get => _dietAdditive;
            set => SetProperty(ref _dietAdditive, value);
        }

        public AnimalType DietGroupType
        {
            get => _dietGroupType;
            set => SetProperty(ref _dietGroupType, value);
        }

        public double ReproductiveRate
        {
            get => _reproductiveRate;
            set => SetProperty(ref _reproductiveRate, value);
        }

        /// <summary>
        ///     Milk produced per day (kg head⁻¹ day⁻¹)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.Kilograms)]
        public double MilkProduction
        {
            get => _milkProduction;
            set => SetProperty(ref _milkProduction, value);
        }

        /// <summary>
        ///     Fat content of milk (%)
        /// </summary>
        public double MilkFatContent
        {
            get => _milkFatContent;
            set => SetProperty(ref _milkFatContent, value);
        }

        /// <summary>
        ///     Protein content of milk (%)
        /// </summary>
        public double MilkProteinContentAsPercentage
        {
            get => _milkProteinContentAsPercentage;
            set => SetProperty(ref _milkProteinContentAsPercentage, value);
        }

        /// <summary>
        ///     Protein content of milk (kg kg⁻¹)
        /// </summary>
        public double MilkProteinContent => MilkProteinContentAsPercentage / 100;

        /// <summary>
        ///     Wool produced per year (kg year^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.Kilograms)]
        public double WoolProduction
        {
            get => _woolProduction;
            set => SetProperty(ref _woolProduction, value);
        }

        /// <summary>
        ///     C_d
        /// </summary>
        public double GainCoefficient
        {
            get => _gainCoefficient;
            set => SetProperty(ref _gainCoefficient, value);
        }

        /// <summary>
        ///     (kg head^-1 day^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.Kilograms)]
        [Obsolete("Use DailyDryMatterFeedIntakeOfFeed on Diet property")]
        public double FeedIntakeAmount
        {
            get => _feedIntakeAmount;
            set => SetProperty(ref _feedIntakeAmount, value);
        }

        /// <summary>
        ///     MJ kg^-1
        /// </summary>
        [Units(MetricUnitsOfMeasurement.MegaJoulesPerKilogram)]
        public double GainCoefficientA
        {
            get => _gainCoefficientA;
            set => SetProperty(ref _gainCoefficientA, value);
        }

        /// <summary>
        ///     MJ kg^-2
        /// </summary>
        [Units(MetricUnitsOfMeasurement.MegaJoulesPerKilogramSquared)]
        public double GainCoefficientB
        {
            get => _gainCoefficientB;
            set => SetProperty(ref _gainCoefficientB, value);
        }

        /// <summary>
        ///     The selected diet.
        /// </summary>
        public Diet SelectedDiet
        {
            get => _selectedDiet;
            set => SetProperty(ref _selectedDiet, value);
        }

        /// <summary>
        ///     Not used.
        /// </summary>
        [Obsolete]
        public Diet DisplayDiet
        {
            get => _displayDiet;
            set => SetProperty(ref _displayDiet, value);
        }

        /// <summary>
        ///     The start date of this <see cref="ManagementPeriod" />.
        /// </summary>
        public DateTime Start
        {
            get => _start;
            set
            {
                // Don't allow the start date to start after the end date
                if (value > End && End.Equals(default) == false) return;

                SetProperty(ref _start, value);
            }
        }

        /// <summary>
        ///     The end date of this <see cref="ManagementPeriod" />.
        /// </summary>
        public DateTime End
        {
            get => _end;
            set
            {
                // Don't allow the end date to begin before the start date
                if (value < Start && Start.Equals(default) == false) return;

                SetProperty(ref _end, value);
            }
        }

        public TimeSpan Duration
        {
            get => _duration;
            set => SetProperty(ref _duration, value);
        }

        /// <summary>
        ///     The total number of days for this <see cref="ManagementPeriod" />.
        /// </summary>
        public int NumberOfDays
        {
            get => _numberOfDays;
            set => SetProperty(ref _numberOfDays, value);
        }

        /// <summary>
        ///     Indicates if the animals have grown from the start of the management period to the end
        /// </summary>
        public bool HasGrowingAnimals => Math.Abs(StartWeight - EndWeight) > double.Epsilon;

        public int NumberOfYoungAnimals
        {
            get => _numberOfYoungAnimals;
            set => SetProperty(ref _numberOfYoungAnimals, value);
        }

        /// <summary>
        ///     (litter year^-1)
        /// </summary>
        public double FertilityRateOfBirthingAnimals
        {
            get => _fertilityRateOfBirthingAnimals;
            set => SetProperty(ref _fertilityRateOfBirthingAnimals, value);
        }

        /// <summary>
        ///     (kg head^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.Kilograms)]
        public double LiveWeightChangeOfPregnantAnimal
        {
            get => _liveWeightChangeOfPregnantAnimal;
            set => SetProperty(ref _liveWeightChangeOfPregnantAnimal, value);
        }

        /// <summary>
        ///     (kg head^-1)
        /// </summary>
        [Obsolete("Use WeightOfWeanedAnimals property on AnimalGroup instead")]
        [Units(MetricUnitsOfMeasurement.Kilograms)]
        public double LiveWeightOfYoungAtWeaningAge
        {
            get => _liveWeightOfYoungAtWeaningAge;
            set => SetProperty(ref _liveWeightOfYoungAtWeaningAge, value);
        }

        /// <summary>
        ///     (kg head^-1)
        /// </summary>
        [Obsolete("Use WeightOfPigletsAtBirth property on AnimalGroup instead")]
        [Units(MetricUnitsOfMeasurement.Kilograms)]
        public double LiveWeightOfYoungAtBirth
        {
            get => _liveWeightOfYoungAtBirth;
            set => SetProperty(ref _liveWeightOfYoungAtBirth, value);
        }

        /// <summary>
        ///     (kg N kg BW^-1)
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
        ///     Used to indicate when animals are not consuming forage but only milk (distinction needed for calculate enteric
        ///     methane for beef calves)
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
        ///     Indicates if the manure emissions should be calculated by the anaerobic digester algorithms
        /// </summary>
        public bool IsProcessedByAnaerobicDigester { get; set; }

        #endregion
    }
}