using System;
using System.Collections.Generic;
using System.ComponentModel;
using H.Core.Calculators.Shelterbelt;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Models.LandManagement.Shelterbelt
{
    /// <summary>
    ///     This class exists for saving information relevant to the shelterbelt details screen.
    /// </summary>
    public class TrannumData : ModelBase
    {
        #region Event Handlers

        private void OnCircumferenceDataPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(CircumferenceData));
        }

        #endregion

        #region Fields

        private readonly ShelterbeltCalculator _shelterbeltCalculator = new ShelterbeltCalculator();

        private double _rowLength;
        private double _treeSpacing;
        private double _treeCount;
        private TreeSpecies _treeSpecies;
        private CircumferenceData _circumferenceData;
        private double _year;
        private double _aboveGroundCarbonKgPerTree;
        private double _belowGroundCarbonKgPerTree;
        private string _shelterbeltName;
        private string _rowName;
        private string _treeGroupName;
        private Guid _shelterbeltGuid;
        private Guid _rowGuid;
        private int _age;

        private double _percentMortality;
        private double _percentMortalityHigh;
        private double _percentMortalityLow;
        private double _totalBiomassPerTree;
        private double _totalLivingCarbonPerTreeTypePerStandardLength;
        private double _totalLivingBiomassForAllTreesOfSameType;
        private double _totalLivingBiomassPerTreeTypePerStandardLength;

        private Guid _treeGroupGuid;

        #endregion

        #region Constructors

        /// <summary>
        ///     This is NOT the way to construct TrannumDatas. Pass in a shelterbeltcomponent, rowData and treegroupdata, and year
        ///     instead.
        /// </summary>
        public TrannumData()
        {
            RowLength = 100;
            TreeCount = 1;
            TreeSpecies = TreeSpecies.Caragana;
            CircumferenceData = new CircumferenceData();
            Year = DateTime.Now.Year;
            AboveGroundCarbonStocksPerTree = 0.0; //need to decide when this gets generated
            BelowGroundCarbonKgPerTree = 0.0; //need to decide when this gets generated
            ShelterbeltName = "Shelterbelt";
            RowName = "Row";
            TreeGroupName = "Treetype";
            //There are no Guids to connect, but could end up needing to be able to check that later?
            ShelterbeltGuid = Guid.NewGuid();
            RowGuid = Guid.NewGuid();
            TreeGroupGuid = Guid.NewGuid();
            SharedConstruction();
        }

        /// <summary>
        ///     Generate a Trannum with all of its properties filled in from a corresponding set of components, along with the year
        ///     of the trannum.
        /// </summary>
        /// <param name="shelterbeltComponent">The shelterbelt this trannum represents.</param>
        /// <param name="row">The row this trannum represents.</param>
        /// <param name="treeGroup">The treegroup this trannum represents.</param>
        /// <param name="year">The year this trannum represents.</param>
        public TrannumData(ShelterbeltComponent shelterbeltComponent, RowData row, TreeGroupData treeGroup, double year)
        {
            if (shelterbeltComponent == null || row == null || treeGroup == null)
                throw new Exception(nameof(TrannumData) + "'s constructor cannot be passed null values.");

            if (year < treeGroup.PlantYear || year > treeGroup.CutYear)
                throw new Exception(nameof(TrannumData) +
                                    " the year passed to the constructor was outside the lifespan of the treegroup.");

            TreeGroupData = treeGroup;

            //Simple copying
            YearOfObservation = shelterbeltComponent.YearOfObservation;
            TreeSpecies = treeGroup.TreeSpecies;
            CircumferenceData =
                new CircumferenceData(treeGroup.CircumferenceData); //Cannot be a reference to the same copy.
            Year = year;
            if (shelterbeltComponent.NameIsFromUser)
                ShelterbeltName = shelterbeltComponent.Name;
            else
                ShelterbeltName = shelterbeltComponent.GetCorrectName();
            if (row.NameIsFromUser)
                RowName = row.Name;
            else
                RowName = row.GetCorrectName();

            if (treeGroup.NameIsFromUser)
                TreeGroupName = treeGroup.Name;
            else
                TreeGroupName = treeGroup.GetCorrectName();
            ShelterbeltGuid = shelterbeltComponent.Guid;
            RowGuid = row.Guid;
            TreeGroupGuid = treeGroup.Guid;

            var livetrees = new List<double>();
            var plantedtrees = new List<double>();
            foreach (var treegroup in row.TreeGroupData)
            {
                livetrees.Add(treegroup.LiveTreeCount);
                plantedtrees.Add(treegroup.PlantedTreeCount);
            }

            PercentMortality =
                _shelterbeltCalculator.CalculatePercentMortalityOfALinearPlanting(plantedtrees, livetrees);

            var mortalityLow = _shelterbeltCalculator.CalculateMortalityLow(PercentMortality);
            var mortalityHigh = _shelterbeltCalculator.CalculateMortalityHigh(mortalityLow);

            RowLength = row.Length;

            PercentMortalityLow = mortalityLow;
            PercentMortalityHigh = mortalityHigh;
            HardinessZone = shelterbeltComponent.HardinessZone;
            EcodistrictId = shelterbeltComponent.EcoDistrictId;
            Age = (int)(Year - treeGroup.PlantYear) + 1;

            // Assume all trees died in first year
            if (Age == 1)
                TreeCount = treeGroup.PlantedTreeCount;
            else
                // For all other years, we use the number of trees that have lived past the first year
                TreeCount = treeGroup.PlantedTreeCount * (100 - PercentMortality) / 100.0;

            TreeSpacing = _shelterbeltCalculator.CalculateTreeSpacing(RowLength, TreeCount);

            SharedConstruction();
        }

        private void SharedConstruction()
        {
            Name = "Tree Annum";
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Will be used to lookup growth data for trees located in Saskatchewan. Trees outside of Saskatchewan will use the
        ///     hardiness zone
        ///     to lookup growth data.
        /// </summary>
        public int EcodistrictId { get; set; }

        /// <summary>
        ///     Will be used to lookup growth data for trees located outside of Saskatchewan. Trees located in Saskatchewan will
        ///     use a ecodistrict to
        ///     cluster id mapping to lookup growth data.
        /// </summary>
        public HardinessZone HardinessZone { get; set; }

        public TreeGroupData TreeGroupData { get; set; }

        public double RowLength
        {
            get => _rowLength;
            set => SetProperty(ref _rowLength, value);
        }

        public double TreeSpacing
        {
            get => _treeSpacing;
            set => SetProperty(ref _treeSpacing, value);
        }

        public double TreeCount
        {
            get => _treeCount;
            set => SetProperty(ref _treeCount, value);
        }

        public TreeSpecies TreeSpecies
        {
            get => _treeSpecies;
            set => SetProperty(ref _treeSpecies, value);
        }

        public CircumferenceData CircumferenceData
        {
            get => _circumferenceData;
            set
            {
                if (_circumferenceData != null)
                    _circumferenceData.PropertyChanged -= OnCircumferenceDataPropertyChanged;

                SetProperty(ref _circumferenceData, value);
                if (_circumferenceData != null)
                    _circumferenceData.PropertyChanged += OnCircumferenceDataPropertyChanged;
            }
        }

        public double Year
        {
            get => _year;
            set => SetProperty(ref _year, value);
        }

        public double AboveGroundCarbonStocksPerTree
        {
            get => _aboveGroundCarbonKgPerTree;
            set => SetProperty(ref _aboveGroundCarbonKgPerTree, value);
        }

        public double BelowGroundCarbonKgPerTree
        {
            get => _belowGroundCarbonKgPerTree;
            set => SetProperty(ref _belowGroundCarbonKgPerTree, value);
        }

        public string ShelterbeltName
        {
            get => _shelterbeltName;
            set => SetProperty(ref _shelterbeltName, value);
        }

        public string RowName
        {
            get => _rowName;
            set => SetProperty(ref _rowName, value);
        }

        public string TreeGroupName
        {
            get => _treeGroupName;
            set => SetProperty(ref _treeGroupName, value);
        }

        public Guid ShelterbeltGuid
        {
            get => _shelterbeltGuid;
            set => SetProperty(ref _shelterbeltGuid, value);
        }

        public Guid RowGuid
        {
            get => _rowGuid;
            set => SetProperty(ref _rowGuid, value);
        }

        public Guid TreeGroupGuid
        {
            get => _treeGroupGuid;
            set => SetProperty(ref _treeGroupGuid, value);
        }

        /// <summary>
        ///     Percent mortality of an entire row of trees
        ///     (%)
        /// </summary>
        public double PercentMortality
        {
            get => _percentMortality;
            set => SetProperty(ref _percentMortality, value);
        }

        public double PercentMortalityHigh
        {
            get => _percentMortalityHigh;
            set => SetProperty(ref _percentMortalityHigh, value);
        }

        public double PercentMortalityLow
        {
            get => _percentMortalityLow;
            set => SetProperty(ref _percentMortalityLow, value);
        }

        public int Age
        {
            get => _age;
            set => SetProperty(ref _age, value);
        }

        /// <summary>
        ///     The total biomass of a single tree (includes aboveground and belowground)
        ///     (kg)
        /// </summary>
        public double TotalBiomassPerTree
        {
            get => _totalBiomassPerTree;
            set => SetProperty(ref _totalBiomassPerTree, value);
        }

        /// <summary>
        ///     Total carbon per tree (includes aboveground and belowground) per standard length
        ///     (kg C km^-1)
        /// </summary>
        public double TotalLivingCarbonPerTreeTypePerStandardLength
        {
            get => _totalLivingCarbonPerTreeTypePerStandardLength;
            set => SetProperty(ref _totalLivingCarbonPerTreeTypePerStandardLength, value);
        }

        /// <summary>
        ///     Total biomass of all trees in the same row of the same species
        ///     (kg)
        /// </summary>
        public double TotalLivingBiomassForAllTreesOfSameType
        {
            get => _totalLivingBiomassForAllTreesOfSameType;
            set => SetProperty(ref _totalLivingBiomassForAllTreesOfSameType, value);
        }

        /// <summary>
        ///     Total biomass of all trees in the same row of the same species per standard planting (i.e. 1 km)
        ///     (kg km^-1)
        /// </summary>
        public double TotalLivingBiomassPerTreeTypePerStandardLength
        {
            get => _totalLivingBiomassPerTreeTypePerStandardLength;
            set => SetProperty(ref _totalLivingBiomassPerTreeTypePerStandardLength, value);
        }

        /// <summary>
        ///     (kg C km^-1 year^-1)
        /// </summary>
        public double EstimatedDeadOrganicMatterBasedOnRealGrowth { get; set; }

        public double RealGrowthRatio { get; set; }

        /// <summary>
        ///     (kg C km^-1)
        /// </summary>
        public double EstimatedTotalLivingBiomassCarbonBasedOnRealGrowth { get; set; }

        /// <summary>
        ///     Used to determine if biomass/carbon values from lookup tables will use the hardiness zone or ecodistrict.
        /// </summary>
        public bool CanLookupByEcodistrict { get; set; }

        #endregion

        #region Public Methods

        public string GenerateAutonym()
        {
            return TreeGroupName + " - " + Year;
        }

        public string GetCorrectName()
        {
            if (NameIsFromUser) return Name;

            return GenerateAutonym();
        }

        #endregion
    }
}