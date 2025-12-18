using System;
using System.ComponentModel;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Models.LandManagement.Shelterbelt
{
    public class TreeGroupData : ModelBase
    {
        #region Constructors

        public TreeGroupData()
        {
            _parentRowDataName = new TimelineGrouper();
            _circumferenceData = new CircumferenceData();
            _circumferenceData.PropertyChanged += OnCircumferenceDataPropertyChanged;
            //Assign Default values
            _cutDate = new DateTime(2050, 12, 31);
            //_plantDate = new DateTime(1, 1, 1); //Fake Plant Data will get changed the first time YearOfObservation is set.
            _plantedTreeCount = 1;
            _liveTreeCount = 1;
            Name = "Tree Type";
        }

        #endregion

        #region Event Handlers

        private void OnCircumferenceDataPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(CircumferenceData));
        }

        #endregion

        #region Fields

        private CircumferenceData _circumferenceData;

        //Basic data
        private double _a;
        private double _b;
        private TreeSpecies _treeSpecies;

        private double _plantedTreeCount;
        private double _liveTreeCount;
        private double _plantedTreeSpacing;
        private double _liveTreeSpacing;

        private bool _plantYearInitialized;
        private DateTime _plantDate;
        private DateTime _cutDate;

        private Guid _parentRowData;
        private Guid _grandparentShelterbeltComponent;

        private TimelineGrouper _parentRowDataName;

        #endregion

        #region Properties

        //See comment in constructor for reason for override.
        public override int YearOfObservation
        {
            get => base.YearOfObservation;
            set
            {
                if (!_plantYearInitialized)
                {
                    _plantYearInitialized = true;
                    PlantYear = value; //deals with lack of knowledge in constructor; can't rearrange code - last week
                }

                SetProperty(ref _yearOfObservation, value);
            }
        }

        public CircumferenceData CircumferenceData
        {
            get => _circumferenceData;
            set => SetProperty(ref _circumferenceData, value);
        }

        //Basic Properties

        public double A
        {
            get => _a;
            set => SetProperty(ref _a, value);
        }

        public double B
        {
            get => _b;
            set => SetProperty(ref _b, value);
        }

        public TreeSpecies TreeSpecies
        {
            get => _treeSpecies;
            set => SetProperty(ref _treeSpecies, value);
        }

        public double PlantedTreeCount
        {
            get => _plantedTreeCount;
            set => SetProperty(ref _plantedTreeCount, value);
        }

        public double LiveTreeCount
        {
            get => _liveTreeCount;
            set => SetProperty(ref _liveTreeCount, value);
        }

        public double PlantedTreeSpacing
        {
            get => _plantedTreeSpacing;
            set => SetProperty(ref _plantedTreeSpacing, value);
        }

        public double LiveTreeSpacing
        {
            get => _liveTreeSpacing;
            set => SetProperty(ref _liveTreeSpacing, value);
        }

        /// <summary>
        ///     The year the tree(s) were planted in the sheleterbelt. This may or may not be the same as the
        ///     <see cref="YearOfObservation" />.
        /// </summary>
        public int PlantYear
        {
            get => _plantDate.Year;
            set
            {
                var plantDate = GeneratePlantDateFromPlantYear(value);
                SetProperty(ref _plantDate, plantDate);
            }
        }

        public int CutYear
        {
            get => _cutDate.Year;
            set
            {
                var cutDate = GenerateCutDateFromCutYear(value);
                SetProperty(ref _cutDate, cutDate);
            }
        }

        //For display on RadTimeline on Timeline Screen
        public DateTime PlantDate => _plantDate;

        public DateTime CutDate => _cutDate;

        public TimeSpan Lifespan => CutDate.Subtract(PlantDate.AddDays(1));

        //Relationship Discovery
        public Guid ParentRowData
        {
            get => _parentRowData;
            set => SetProperty(ref _parentRowData, value);
        }

        public Guid GrandParentShelterbeltComponent
        {
            get => _grandparentShelterbeltComponent;
            set => SetProperty(ref _grandparentShelterbeltComponent, value);
        }

        public TimelineGrouper ParentRowDataName
        {
            get => _parentRowDataName;
            set => SetProperty(ref _parentRowDataName, value);
        }

        #endregion

        #region Public Methods

        public string GenerateAutonym()
        {
            return TreeSpecies.GetDescription();
        }

        public string GetCorrectName()
        {
            if (NameIsFromUser) return Name;

            return GenerateAutonym();
        }

        #endregion

        #region Private Methods

        private DateTime GeneratePlantDateFromPlantYear(int plantYear)
        {
            var year = plantYear;
            var plantDate = new DateTime(year, 1, 1);
            return plantDate;
        }

        private DateTime GenerateCutDateFromCutYear(int cutYear)
        {
            var year = cutYear;
            var cutDate = new DateTime(year, 12, 31);
            return cutDate;
        }

        #endregion
    }
}