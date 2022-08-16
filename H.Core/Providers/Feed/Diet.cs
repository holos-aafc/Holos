using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using AutoMapper;
using H.Core.Converters;
using H.Core.CustomAttributes;
using H.Core.Enumerations;
using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Providers.Feed
{
    /// <summary>
    /// Do not hold a reference to a Diet object in a ManagementPeriod object since it causes too much complication in the GUI
    /// </summary>
    public class Diet : ModelBase
    {
        #region Fields                

        private double _totalDigestibleNutrient;
        private double _crudeProtein;
        private double _forage;
        private double _starch;
        private double _fat;
        private double _metabolizableEnergy;
        private double _ndf;
        private double _netEnergy;
        private double _methaneConversionFactor;
        private double _methaneConversionFactorAdjustment;
        private double _nitrogenExcretionAdjustFactorForDiet;
        private double _volatileSolidsAdjustmentFactorForDiet;
        private double _adf;
        private double _ee;
        private double _de1x;
        private double _nel3x;
        private double _dryMatter;
        private double _dailyDryMatterFeedIntakeOfFeed;
        private double _aee;
        private double _ash;
        private double _crudeFiber;
        private double _p;
        private double _dietaryNetEnergyConcentration;

        private bool _isConverted;
        private bool _isDefaultDiet;
        private bool _isCustomPlaceholderDiet;

        private AnimalType _animalType;
        private EntericMethanEmissionMethodologies _selectedMethaneEmissionMethodology;

        private ObservableCollection<FeedIngredient> _ingredients;
        private static MapperConfiguration _dietMapperConfiguration;
        private static MapperConfiguration _ingredientMapperConfiguration;
        private static IMapper _dietMapper;
        private static IMapper _ingredientMapper;

        #endregion

        #region Constructors

        static Diet()
        {
            _dietMapperConfiguration = new MapperConfiguration(x =>
            {
                x.CreateMap<Diet, Diet>().ForMember(property => property.Ingredients, options => options.Ignore());
            });

            _ingredientMapperConfiguration = new MapperConfiguration(x =>
            {
                x.CreateMap<FeedIngredient, FeedIngredient>()
                    .ForMember(property => property.Guid, options => options.Ignore());
            });

            _dietMapper = _dietMapperConfiguration.CreateMapper();

            _ingredientMapper = _ingredientMapperConfiguration.CreateMapper();
        }

        public Diet()
        {
            this.VolatileSolidsAdjustmentFactorForDiet = 1;
            this.NitrogenExcretionAdjustFactorForDiet = 1;

            this.Ingredients = new ObservableCollection<FeedIngredient>();

            this.Ingredients.CollectionChanged -= this.FeedIngredientsOnCollectionChanged;
            this.Ingredients.CollectionChanged += this.FeedIngredientsOnCollectionChanged;

            this.PropertyChanged -= OnPropertyChanged;
            this.PropertyChanged += OnPropertyChanged;
        }

        #endregion

        #region Properties

        public DietType DietType { get; set; }

        /// <summary>
        /// The type of animal this diet is for. This could also be a catch-all since a cow diet is given to cows and calves
        /// </summary>
        public AnimalType AnimalType
        {
            get { return _animalType; }
            set
            {
                _animalType = value;
                this.RaisePropertyChanged(nameof(this.AnimalTypeString));
            }
        }

        public string AnimalTypeString
        {
            get { return this.AnimalType.GetDescription(); }
        }

        public ObservableCollection<FeedIngredient> Ingredients
        {
            get { return _ingredients; }
            set { SetProperty(ref _ingredients, value, this.UpdateTotals); }
        }

        public string Comments { get; set; }

        /// <summary>
        /// TDN
        ///
        /// (%)
        /// </summary>
        public double TotalDigestibleNutrient
        {
            get { return _totalDigestibleNutrient; }
            set { this.SetProperty(ref _totalDigestibleNutrient, value); }
        }

        /// <summary>
        /// Crude protein content
        ///
        /// CP
        /// 
        /// (% DM)
        /// </summary>
        public double CrudeProtein
        {
            get { return _crudeProtein; }
            set { this.SetProperty(ref _crudeProtein, value); }
        }

        /// <summary>
        /// Crude protein content
        ///
        /// CP
        /// 
        /// (fraction [kg kg^-1])
        /// </summary>
        public double CrudeProteinContent
        {
            get { return this.CrudeProtein / 100; }
        }

        /// <summary>
        /// Methane conversion factor for diet 
        /// 
        /// Y_m
        ///
        /// (kg CH4 kg CH4^-1)
        /// </summary>
        public double MethaneConversionFactor
        {
            get { return _methaneConversionFactor; }
            set { this.SetProperty(ref _methaneConversionFactor, value); }
        }

        /// <summary>
        /// YM Adjustment (%)
        /// </summary>
        public double MethaneConversionFactorAdjustment
        {
            get { return _methaneConversionFactorAdjustment; }
            set { this.SetProperty(ref _methaneConversionFactorAdjustment, value); }
        }

        /// <summary>
        /// (% DM)
        /// </summary>
        public double Forage
        {
            get { return _forage; }
            set { this.SetProperty(ref _forage, value); }
        }

        /// <summary>
        /// (% DM)
        /// </summary>
        public double Starch
        {
            get { return _starch; }
            set { this.SetProperty(ref _starch, value); }
        }

        /// <summary>
        /// (fraction)
        /// </summary>
        public double StarchContent
        {
            get { return this.Starch / 100.0; }
        }

        /// <summary>
        /// (% DM)
        /// </summary>
        public double Fat
        {
            get { return _fat; }
            set { this.SetProperty(ref _fat, value); }
        }

        /// <summary>
        /// Fat content
        /// 
        /// (fraction)
        /// </summary>
        public double FatContent
        {
            get
            {
                return this.Fat / 100.0;
            }
        }

        /// <summary>
        /// Neutral detergent fiber (NDF)
        ///
        /// (% DM)
        /// </summary>
        public double Ndf
        {
            get { return _ndf; }
            set { this.SetProperty(ref _ndf, value); }
        }

        /// <summary>
        /// Neutral detergent fiber (NDF)
        ///
        /// (fraction)
        /// </summary>
        public double NdfContent
        {
            get
            {
                return this.Ndf / 100.0;
            }
        }

        /// <summary>
        /// Acid detergent fiber
        /// 
        /// (% DM)
        /// </summary>
        public double Adf
        {
            get { return _adf; }
            set { SetProperty(ref _adf, value); }
        }

        /// <summary>
        /// Acid detergent fiber
        /// 
        /// (fraction)
        /// </summary>
        public double AdfContent
        {
            get
            {
                return this.Adf / 100;
            }
        }

        [Units(MetricUnitsOfMeasurement.MegaCaloriePerKilogram)]
        public double MetabolizableEnergy
        {
            get { return _metabolizableEnergy; }
            set { this.SetProperty(ref _metabolizableEnergy, value); }
        }

        /// <summary>
        /// N Excr. Adj.
        /// </summary>
        public double NitrogenExcretionAdjustFactorForDiet
        {
            get { return _nitrogenExcretionAdjustFactorForDiet; }
            set { this.SetProperty(ref _nitrogenExcretionAdjustFactorForDiet, value); }
        }

        /// <summary>
        /// (kg kg^-1)
        /// </summary>
        public double VolatileSolidsAdjustmentFactorForDiet
        {
            get { return _volatileSolidsAdjustmentFactorForDiet; }
            set { this.SetProperty(ref _volatileSolidsAdjustmentFactorForDiet, value); }
        }

        public bool IsDefaultDiet
        {
            get { return _isDefaultDiet; }
            set { this.SetProperty(ref _isDefaultDiet, value); }
        }

        /// <summary>
        /// Some animal groups will not have a diet (poultry, other livestock, suckling pigs, etc.). In these cases, a non-null diet must still be set. This flag indicates if the diet is related to that situation.
        /// </summary>
        public bool IsCustomPlaceholderDiet
        {
            get { return _isCustomPlaceholderDiet; }
            set { this.SetProperty(ref _isCustomPlaceholderDiet, value); }
        }

        [Units(MetricUnitsOfMeasurement.KiloCaloriePerKilogram)]
        public double NetEnergy
        {
            get { return _netEnergy; }
            set { SetProperty(ref _netEnergy, value); }
        }

        /// <summary>
        /// The fat content of a dairy diet.
        ///
        /// (% DM)
        /// </summary>
        public double Ee
        {
            get { return _ee; }
            set { SetProperty(ref _ee, value); }
        }

        [Units(MetricUnitsOfMeasurement.MegaCaloriePerKilogram)]
        public double De1X
        {
            get { return _de1x; }
            set { SetProperty(ref _de1x, value); }
        }

        [Units(MetricUnitsOfMeasurement.MegaCaloriePerKilogram)]
        public double Nel3X
        {
            get { return _nel3x; }
            set { SetProperty(ref _nel3x, value); }
        }
        public double DryMatter
        {
            get { return _dryMatter; }
            set { SetProperty(ref _dryMatter, value); }
        }
        public double CrudeFiber
        {
            get { return _crudeFiber; }
            set { SetProperty(ref _crudeFiber, value); }
        }
        public double Aee
        {
            get { return _aee; }
            set { SetProperty(ref _aee, value); }
        }

        /// <summary>
        /// (% DM)
        /// </summary>
        public double Ash
        {
            get { return _ash; }
            set { SetProperty(ref _ash, value); }
        }
        public EntericMethanEmissionMethodologies SelectedMethaneEmissionMethodology
        {
            get { return _selectedMethaneEmissionMethodology; }
            set { SetProperty(ref _selectedMethaneEmissionMethodology, value); }
        }

        public double P
        {
            get => _p;
            set => SetProperty(ref _p, value);
        }

        //so we can keep track if we converted to imperial
        public bool IsConverted
        {
            get => _isConverted;
            set => SetProperty(ref _isConverted, value);
        }

        /// <summary>
        /// (DMI, kg head^-1 day^-1)
        /// </summary>
        public double DailyDryMatterFeedIntakeOfFeed
        {
            get => _dailyDryMatterFeedIntakeOfFeed;
            set => SetProperty(ref _dailyDryMatterFeedIntakeOfFeed, value);
        }

        /// <summary>
        /// Used in the calculation of DMI for beef calves
        ///
        /// NE_mf
        ///
        /// (MJ (kg DM)^-1)
        /// </summary>
        public double DietaryNetEnergyConcentration
        {
            get => _dietaryNetEnergyConcentration;
            set => SetProperty(ref _dietaryNetEnergyConcentration, value);
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, ({nameof(AnimalTypeString)}: {AnimalTypeString})";
        }

        public void UpdateTotals()
        {
            this.TotalDigestibleNutrient = this.Ingredients.Sum(x => x.PercentageInDiet / 100 * x.TotalDigestibleNutrient);
            this.CrudeProtein = this.Ingredients.Sum(x => x.PercentageInDiet / 100 * x.CrudeProtein);
            this.Forage = this.Ingredients.Sum(x => x.PercentageInDiet / 100 * x.Forage);
            if (this.DietType == DietType.MediumEnergyAndProtein)
            {
                var animal = this.AnimalType;
            }
            this.Starch = this.Ingredients.Sum(x => x.PercentageInDiet / 100 * x.Starch);
            this.Fat = this.Ingredients.Sum(x => x.PercentageInDiet / 100 * x.Fat);

            this.MetabolizableEnergy = this.Ingredients.Sum(x => x.PercentageInDiet / 100 * x.ME);
            this.Ndf = this.Ingredients.Sum(x => x.PercentageInDiet / 100 * x.NDF);
            this.Adf = this.Ingredients.Sum(x => x.PercentageInDiet / 100 * x.ADF);
            if (this.AnimalType.IsSwineType())
            {
                this.De1X = this.Ingredients.Sum(x => x.PercentageInDiet / 100 * x.DeSwine);
            }
            else
            {
                this.De1X = this.Ingredients.Sum(x => x.PercentageInDiet / 100 * x.DE);
            }
            this.NetEnergy = this.Ingredients.Sum(x => x.PercentageInDiet / 100 * x.NE);
            this.Nel3X = this.Ingredients.Sum(x => x.PercentageInDiet / 100 * x.NEL_ThreeX);
            this.Ee = this.Ingredients.Sum(x => x.PercentageInDiet / 100 * x.EE);

            this.DryMatter = this.Ingredients.Sum(x => x.PercentageInDiet / 100 * x.DryMatter);
            this.Aee = this.Ingredients.Sum(x => x.PercentageInDiet / 100 * x.AcidEtherExtract);
            this.Ash = this.Ingredients.Sum(x => x.PercentageInDiet / 100 * x.Ash);
            this.CrudeFiber = this.Ingredients.Sum(x => x.PercentageInDiet / 100 * x.CrudeFiber);
            this.P = this.Ingredients.Sum(x => x.PercentageInDiet / 100 * x.P);

            this.AssignYmValue();
        }

        public double GetTotalPercentageOfDietItems()
        {
            return this.Ingredients.Sum(x => x.PercentageInDiet);
        }      

        /// <summary>
        /// Copy a diet
        /// </summary>
        /// <param name="dietToCopy">the diet to copy</param>
        /// <returns>a copied diet</returns>
        public static Diet CopyDiet(Diet dietToCopy)
        {
            if (dietToCopy == null)
            {
                return new Diet();
            }

            var copiedDiet = new Diet();
            

            // This needs to be new'd up here, for some reason setting this property to be ignored in the mapper configuration doesn't actually ignore it. Instead, auto mapper
            // will just use the same reference to the Ingredients property and the result will be that ingredients get duplicated back into the dietToCopy variable. This is not
            // what is wanted as we just want to copy ingredients to the copiedDiet property.
            copiedDiet.Ingredients = new ObservableCollection<FeedIngredient>();

            _dietMapper.Map(dietToCopy, copiedDiet);            

            // Copy ingredients
            var ingredientsToBeCopied = dietToCopy.Ingredients.ToList();

            foreach (var feedIngredient in ingredientsToBeCopied)
            {
                var copiedIngredient = _ingredientMapper.Map<FeedIngredient>(feedIngredient);

                copiedDiet.Ingredients.Add(copiedIngredient);
            }

            return copiedDiet;
        }

        /// <summary>
        /// Used in System -> Binding transfer: system to Gui unit conversion
        ///
        /// When we initialize a binding in the GUI we need to read in the the data saved on the system that is metric and return either metric or imperial
        /// </summary>
        /// <param name="dietToConvert">a diet</param>
        /// <returns></returns>
        public static Diet GetBindingDietFromSystem(Diet dietToConvert)
        {
            var convertedDiet = CopyDiet(dietToConvert);

            if (convertedDiet.IsConverted) return convertedDiet;

            foreach (var ingredient in convertedDiet.Ingredients)
            {
                ConvertToBindingIngredientFromSystem(ingredient);
            }

            return convertedDiet;
        }
        
        /// <summary>
        /// Used in Binding -> System transfer Gui to System unit conversion
        ///
        /// When the binding changes we call this method to create a metric version and set that to the system instance
        /// </summary>
        /// <param name="dietToConvert">a diet</param>
        /// <returns>a metric diet</returns>
        public static Diet GetSystemDietFromBinding(Diet dietToConvert)
        {
            var convertedDiet = CopyDiet(dietToConvert);

            //change each ingredient to metric
            foreach (var feedIngredient in convertedDiet.Ingredients)
            {
                //the system is always metric
                ConvertToSystemIngredientFromBinding(feedIngredient);
            }

            return convertedDiet;
        }


        /// <summary>
        /// converts feed ingredient to metric for the system
        /// </summary>
        private static void ConvertToSystemIngredientFromBinding(FeedIngredient feedIngredient)
        {
            //property converter for this exact ingredient
            var ingredientPropertyConverter = new PropertyConverter<FeedIngredient>(feedIngredient);

            //all the attributed properties
            var attrProps = ingredientPropertyConverter.PropertyInfos;

            //convert the ingredients properties using reflection
            foreach (var prop in attrProps)
            {
                //this is the converted value of the original ingredient
                var convertedValue = ingredientPropertyConverter.GetSystemValueFromBinding(prop);

                //set the converted value for the  ingredient
                prop.SetValue(feedIngredient, convertedValue);
            }
        }

        /// <summary>
        /// converts feed ingredient to metric or imperial for the GUI bindings
        /// </summary>
        private static void ConvertToBindingIngredientFromSystem(FeedIngredient feedIngredient)
        {
            //property converter for this exact ingredient
            var ingredientPropertyConverter = new PropertyConverter<FeedIngredient>(feedIngredient);

            //all the attributed properties
            var attrProps = ingredientPropertyConverter.PropertyInfos;

            //convert the ingredients properties using reflection
            foreach (var prop in attrProps)
            {
                //this is the converted value of the original ingredient
                var convertedValue = ingredientPropertyConverter.GetBindingValueFromSystem(prop);

                //set the converted value for the  ingredient
                prop.SetValue(feedIngredient, convertedValue);
            }
        }

        public double CalculateNemf()
        {
            return this.Ingredients.WeightedAverage(x => x.Nemf, x => x.PercentageInDiet);
        }

        #endregion

        #region Private Methods

        private void AssignYmValue()
        {
            // Assign a default ym so that if there are no cases that cover the diet below, there will be a value assigned
            this.MethaneConversionFactor = 0.04;


            if (this.AnimalType == AnimalType.DairyLactatingCow ||
                this.AnimalType == AnimalType.DairyDryCow ||
                this.AnimalType == AnimalType.DairyHeifers)
            {
                if (this.TotalDigestibleNutrient >= 65)
                {
                    this.MethaneConversionFactor = 0.063;
                }
                else if (55 <= this.TotalDigestibleNutrient && this.TotalDigestibleNutrient < 65)
                {
                    this.MethaneConversionFactor = 0.065;
                }
                else
                {
                    this.MethaneConversionFactor = 0.07;
                }
            }

            if (this.AnimalType == AnimalType.BeefCow ||
                this.AnimalType == AnimalType.BeefReplacementHeifers ||
                this.AnimalType == AnimalType.Stockers ||
                this.AnimalType == AnimalType.BeefBackgrounder ||
                this.AnimalType == AnimalType.BeefBulls)
            {
                if (this.TotalDigestibleNutrient >= 65)
                {
                    this.MethaneConversionFactor = 0.065;
                }
                else if (this.TotalDigestibleNutrient >= 55 && this.TotalDigestibleNutrient < 65)
                {
                    this.MethaneConversionFactor = 0.07;
                }
                else
                {
                    this.MethaneConversionFactor = 0.08;
                }
            }

            if (this.AnimalType == AnimalType.BeefFinisher)
            {
                if (string.IsNullOrWhiteSpace(this.Name) == false)
                {
                    if (this.Name.Equals(Resources.LabelCornGrainBasedDiet))
                    {
                        this.MethaneConversionFactor = 0.03;
                    }

                    if (this.Name.Equals(Resources.LabelBarleyGrainBasedDiet))
                    {
                        this.MethaneConversionFactor = 0.04;
                    }
                }
            }
        }

        #endregion

        #region Event Handlers

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is Diet diet)
            {
            }
        }

        private void FeedIngredientsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems[0] is FeedIngredient feedIngredient)
                {
                    feedIngredient.PropertyChanged += this.FeedIngredientOnPropertyChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems[0] is FeedIngredient feedIngredient)
                {
                    feedIngredient.PropertyChanged -= this.FeedIngredientOnPropertyChanged;
                }
            }

            // Need to update diet totals when an ingredient is removed/added
            this.UpdateTotals();
        }

        private void FeedIngredientOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            // Need to update diet totals when an ingredient is modified
            this.UpdateTotals();
        }

        #endregion
    }
}