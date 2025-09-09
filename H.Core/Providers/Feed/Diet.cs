using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using AutoMapper;
using H.Core.Converters;
using H.Core.CustomAttributes;
using H.Core.Enumerations;
using H.Core.Properties;
using H.Infrastructure;
using Microsoft.Extensions.Logging.Abstractions;

namespace H.Core.Providers.Feed
{
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
            _dietMapperConfiguration = new MapperConfiguration(
                x =>
                {
                    x.CreateMap<Diet, Diet>().ForMember(property => property.Ingredients, options => options.Ignore());
                }, new NullLoggerFactory());

            _ingredientMapperConfiguration = new MapperConfiguration(x =>
            {
                x.CreateMap<FeedIngredient, FeedIngredient>()
                    .ForMember(property => property.Guid, options => options.Ignore());
            }, new NullLoggerFactory());

            _dietMapper = _dietMapperConfiguration.CreateMapper();

            _ingredientMapper = _ingredientMapperConfiguration.CreateMapper();
        }

        public Diet()
        {
            VolatileSolidsAdjustmentFactorForDiet = 1;
            NitrogenExcretionAdjustFactorForDiet = 1;

            Ingredients = new ObservableCollection<FeedIngredient>();

            Ingredients.CollectionChanged -= FeedIngredientsOnCollectionChanged;
            Ingredients.CollectionChanged += FeedIngredientsOnCollectionChanged;

            PropertyChanged -= OnPropertyChanged;
            PropertyChanged += OnPropertyChanged;
        }

        #endregion

        #region Properties

        public string SystemName => Name + " - (" + AnimalTypeString + ")";

        public DietType DietType { get; set; }

        /// <summary>
        ///     The type of animal this diet is for. This could also be a catch-all since a cow diet is given to cows and calves
        /// </summary>
        public AnimalType AnimalType
        {
            get => _animalType;
            set
            {
                _animalType = value;
                RaisePropertyChanged(nameof(AnimalTypeString));
            }
        }

        public string AnimalTypeString => AnimalType.GetDescription();

        public ObservableCollection<FeedIngredient> Ingredients
        {
            get => _ingredients;
            set => SetProperty(ref _ingredients, value, UpdateTotals);
        }

        public string Comments { get; set; }

        /// <summary>
        ///     TDN
        ///     (%)
        /// </summary>
        public double TotalDigestibleNutrient
        {
            get => _totalDigestibleNutrient;
            set => SetProperty(ref _totalDigestibleNutrient, value);
        }

        /// <summary>
        ///     Crude protein content
        ///     CP
        ///     (% DM)
        /// </summary>
        public double CrudeProtein
        {
            get => _crudeProtein;
            set => SetProperty(ref _crudeProtein, value);
        }

        /// <summary>
        ///     Crude protein content
        ///     CP
        ///     (fraction [kg kg^-1])
        /// </summary>
        public double CrudeProteinContent => CrudeProtein / 100;

        /// <summary>
        ///     Methane conversion factor for diet
        ///     Y_m
        ///     (kg CH4 kg CH4^-1)
        /// </summary>
        public double MethaneConversionFactor
        {
            get => _methaneConversionFactor;
            set => SetProperty(ref _methaneConversionFactor, value);
        }

        /// <summary>
        ///     YM Adjustment (%)
        /// </summary>
        public double MethaneConversionFactorAdjustment
        {
            get => _methaneConversionFactorAdjustment;
            set => SetProperty(ref _methaneConversionFactorAdjustment, value);
        }

        /// <summary>
        ///     (% DM)
        /// </summary>
        public double Forage
        {
            get => _forage;
            set => SetProperty(ref _forage, value);
        }

        /// <summary>
        ///     (% DM)
        /// </summary>
        public double Starch
        {
            get => _starch;
            set => SetProperty(ref _starch, value);
        }

        /// <summary>
        ///     (fraction)
        /// </summary>
        public double StarchContent => Starch / 100.0;

        /// <summary>
        ///     (% DM)
        /// </summary>
        public double Fat
        {
            get => _fat;
            set => SetProperty(ref _fat, value);
        }

        /// <summary>
        ///     Fat content
        ///     (fraction)
        /// </summary>
        public double FatContent => Fat / 100.0;

        /// <summary>
        ///     Neutral detergent fiber (NDF)
        ///     (% DM)
        /// </summary>
        public double Ndf
        {
            get => _ndf;
            set => SetProperty(ref _ndf, value);
        }

        /// <summary>
        ///     Neutral detergent fiber (NDF)
        ///     (fraction)
        /// </summary>
        public double NdfContent => Ndf / 100.0;

        /// <summary>
        ///     Acid detergent fiber
        ///     (% DM)
        /// </summary>
        public double Adf
        {
            get => _adf;
            set => SetProperty(ref _adf, value);
        }

        /// <summary>
        ///     Acid detergent fiber
        ///     (fraction)
        /// </summary>
        public double AdfContent => Adf / 100;

        [Units(MetricUnitsOfMeasurement.MegaCaloriePerKilogram)]
        public double MetabolizableEnergy
        {
            get => _metabolizableEnergy;
            set => SetProperty(ref _metabolizableEnergy, value);
        }

        /// <summary>
        ///     N Excr. Adj.
        /// </summary>
        public double NitrogenExcretionAdjustFactorForDiet
        {
            get => _nitrogenExcretionAdjustFactorForDiet;
            set => SetProperty(ref _nitrogenExcretionAdjustFactorForDiet, value);
        }

        /// <summary>
        ///     (kg kg^-1)
        /// </summary>
        public double VolatileSolidsAdjustmentFactorForDiet
        {
            get => _volatileSolidsAdjustmentFactorForDiet;
            set => SetProperty(ref _volatileSolidsAdjustmentFactorForDiet, value);
        }

        public bool IsDefaultDiet
        {
            get => _isDefaultDiet;
            set => SetProperty(ref _isDefaultDiet, value);
        }

        /// <summary>
        ///     Some animal groups will not have a diet (poultry, other livestock, suckling pigs, etc.). In these cases, a non-null
        ///     diet must still be set. This flag indicates if the diet is related to that situation.
        /// </summary>
        public bool IsCustomPlaceholderDiet
        {
            get => _isCustomPlaceholderDiet;
            set => SetProperty(ref _isCustomPlaceholderDiet, value);
        }

        [Units(MetricUnitsOfMeasurement.KiloCaloriePerKilogram)]
        public double NetEnergy
        {
            get => _netEnergy;
            set => SetProperty(ref _netEnergy, value);
        }

        /// <summary>
        ///     The fat content of a dairy diet.
        ///     (% DM)
        /// </summary>
        public double Ee
        {
            get => _ee;
            set => SetProperty(ref _ee, value);
        }

        [Units(MetricUnitsOfMeasurement.MegaCaloriePerKilogram)]
        public double De1X
        {
            get => _de1x;
            set => SetProperty(ref _de1x, value);
        }

        [Units(MetricUnitsOfMeasurement.MegaCaloriePerKilogram)]
        public double Nel3X
        {
            get => _nel3x;
            set => SetProperty(ref _nel3x, value);
        }

        public double DryMatter
        {
            get => _dryMatter;
            set => SetProperty(ref _dryMatter, value);
        }

        public double CrudeFiber
        {
            get => _crudeFiber;
            set => SetProperty(ref _crudeFiber, value);
        }

        public double Aee
        {
            get => _aee;
            set => SetProperty(ref _aee, value);
        }

        /// <summary>
        ///     (% DM)
        /// </summary>
        public double Ash
        {
            get => _ash;
            set => SetProperty(ref _ash, value);
        }

        public EntericMethanEmissionMethodologies SelectedMethaneEmissionMethodology
        {
            get => _selectedMethaneEmissionMethodology;
            set => SetProperty(ref _selectedMethaneEmissionMethodology, value);
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
        ///     (DMI, kg head^-1 day^-1)
        /// </summary>
        public double DailyDryMatterFeedIntakeOfFeed
        {
            get => _dailyDryMatterFeedIntakeOfFeed;
            set => SetProperty(ref _dailyDryMatterFeedIntakeOfFeed, value);
        }

        /// <summary>
        ///     Used in the calculation of DMI for beef calves
        ///     NE_mf
        ///     (MJ (kg DM)^-1)
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
            TotalDigestibleNutrient = Ingredients.Sum(x => x.PercentageInDiet / 100 * x.TotalDigestibleNutrient);
            CrudeProtein = Ingredients.Sum(x => x.PercentageInDiet / 100 * x.CrudeProtein);
            Forage = Ingredients.Sum(x => x.PercentageInDiet / 100 * x.Forage);
            if (DietType == DietType.MediumEnergyAndProtein)
            {
                var animal = AnimalType;
            }

            Starch = Ingredients.Sum(x => x.PercentageInDiet / 100 * x.Starch);
            Fat = Ingredients.Sum(x => x.PercentageInDiet / 100 * x.Fat);

            MetabolizableEnergy = Ingredients.Sum(x => x.PercentageInDiet / 100 * x.ME);
            Ndf = Ingredients.Sum(x => x.PercentageInDiet / 100 * x.NDF);
            Adf = Ingredients.Sum(x => x.PercentageInDiet / 100 * x.ADF);
            if (AnimalType.IsSwineType())
                De1X = Ingredients.Sum(x => x.PercentageInDiet / 100 * x.DeSwine);
            else
                De1X = Ingredients.Sum(x => x.PercentageInDiet / 100 * x.DE);
            NetEnergy = Ingredients.Sum(x => x.PercentageInDiet / 100 * x.NE);
            Nel3X = Ingredients.Sum(x => x.PercentageInDiet / 100 * x.NEL_ThreeX);
            Ee = Ingredients.Sum(x => x.PercentageInDiet / 100 * x.EE);

            DryMatter = Ingredients.Sum(x => x.PercentageInDiet / 100 * x.DryMatter);
            Aee = Ingredients.Sum(x => x.PercentageInDiet / 100 * x.AcidEtherExtract);
            Ash = Ingredients.Sum(x => x.PercentageInDiet / 100 * x.Ash);
            CrudeFiber = Ingredients.Sum(x => x.PercentageInDiet / 100 * x.CrudeFiber);
            P = Ingredients.Sum(x => x.PercentageInDiet / 100 * x.P);

            CalculateMCF();
        }

        public double GetTotalPercentageOfDietItems()
        {
            return Ingredients.Sum(x => x.PercentageInDiet);
        }

        /// <summary>
        ///     Copy a diet
        /// </summary>
        /// <param name="dietToCopy">the diet to copy</param>
        /// <returns>a copied diet</returns>
        public static Diet CopyDiet(Diet dietToCopy)
        {
            if (dietToCopy == null) return new Diet();

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
        ///     Used in System -> Binding transfer: system to Gui unit conversion
        ///     When we initialize a binding in the GUI we need to read in the the data saved on the system that is metric and
        ///     return either metric or imperial
        /// </summary>
        /// <param name="dietToConvert">a diet</param>
        /// <returns></returns>
        public static Diet GetBindingDietFromSystem(Diet dietToConvert)
        {
            var convertedDiet = CopyDiet(dietToConvert);

            if (convertedDiet.IsConverted) return convertedDiet;

            foreach (var ingredient in convertedDiet.Ingredients) ConvertToBindingIngredientFromSystem(ingredient);

            return convertedDiet;
        }

        /// <summary>
        ///     Used in Binding -> System transfer Gui to System unit conversion
        ///     When the binding changes we call this method to create a metric version and set that to the system instance
        /// </summary>
        /// <param name="dietToConvert">a diet</param>
        /// <returns>a metric diet</returns>
        public static Diet GetSystemDietFromBinding(Diet dietToConvert)
        {
            var convertedDiet = CopyDiet(dietToConvert);

            //change each ingredient to metric
            foreach (var feedIngredient in convertedDiet.Ingredients)
                //the system is always metric
                ConvertToSystemIngredientFromBinding(feedIngredient);

            return convertedDiet;
        }


        /// <summary>
        ///     converts feed ingredient to metric for the system
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
        ///     converts feed ingredient to metric or imperial for the GUI bindings
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
            return Ingredients.WeightedAverage(x => x.Nemf, x => x.PercentageInDiet);
        }

        public double CalculateMCF(AnimalType animalType, double totalDigestibleNutrient)
        {
            // Assign a default ym so that if there are no cases that cover the diet below, there will be a value assigned
            var result = 0.4;

            if (animalType.IsDairyCattleType())
            {
                if (totalDigestibleNutrient >= 65)
                    result = 0.063;
                else if (55 <= totalDigestibleNutrient && totalDigestibleNutrient < 65)
                    result = 0.065;
                else
                    result = 0.07;
            }

            if (animalType.IsBeefCattleType())
            {
                if (totalDigestibleNutrient >= 65)
                    result = 0.065;
                else if (totalDigestibleNutrient >= 55 && totalDigestibleNutrient < 65)
                    result = 0.07;
                else
                    result = 0.08;
            }

            if (animalType == AnimalType.BeefFinisher)
                if (string.IsNullOrWhiteSpace(Name) == false)
                {
                    if (Name.Equals(Resources.LabelCornGrainBasedDiet)) result = 0.03;

                    if (Name.Equals(Resources.LabelBarleyGrainBasedDiet)) result = 0.04;
                }


            return result;
        }

        public void CalculateMCF()
        {
            MethaneConversionFactor = CalculateMCF(AnimalType, TotalDigestibleNutrient);
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
                    feedIngredient.PropertyChanged += FeedIngredientOnPropertyChanged;
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems[0] is FeedIngredient feedIngredient)
                    feedIngredient.PropertyChanged -= FeedIngredientOnPropertyChanged;
            }

            // Need to update diet totals when an ingredient is removed/added
            UpdateTotals();
        }

        private void FeedIngredientOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            // Need to update diet totals when an ingredient is modified
            UpdateTotals();
        }

        #endregion
    }
}