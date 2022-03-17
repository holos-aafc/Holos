using Prism.Mvvm;

namespace H.Core.Models
{
    /// <summary>
    /// The class used to store the visibility of the columns related to the FieldSystemsDetailView when the user has selected the ICBM carbon modelling approach
    /// </summary>
    public class FieldSystemDetailsColumnsVisibility : ColumnVisibilityBase
    {
        #region fields

        private bool _fieldName;
        private bool _timePeriod;
        private bool _year;
        private bool _cropType;
        private bool _tillageType;
        private bool _yield;
        private bool _aboveGroundCarbonInput;
        private bool _belowGroundCarbonInput;
        private bool _manureCarbonInput;
        private bool _climateParameter;
        private bool _tillageFactor;
        private bool _managementFactor;
        private bool _edit;
        private bool _percentageOfProductReturned;
        private bool _percentageOfStrawReturned;
        private bool _percentageOfRootsReturned;
        private bool _description;

        #endregion

        public FieldSystemDetailsColumnsVisibility()
        {
            this.DefaultVisibility();
        }

        public void DefaultVisibility()
        {
            //clear current selection
            base.SetAllColumnsInvisible();

            //the default columns for the grid
            this.FieldName = true;
            this.TimePeriod = true;
            this.Year = true;
            this.CropType = true;
            this.Yield = true;
            this.PercentageOfProductReturned = false;
            this.PercentageOfStrawReturned = false;
            this.PercentageOfRootsReturned = false;
            this.AboveGroundCarbonInput = false;
            this.BelowGroundCarbonInput = false;
            this.Description = true;
        }

        public bool FieldName
        {
            get { return _fieldName; }
            set { SetProperty(ref _fieldName, value); }
        }
        public bool TimePeriod
        {
            get { return _timePeriod; }
            set { SetProperty(ref _timePeriod, value); }
        }
        public bool Year
        {
            get { return _year; }
            set { SetProperty(ref _year, value); }
        }
        public bool CropType
        {
            get { return _cropType; }
            set { SetProperty(ref _cropType, value); }
        }
        public bool TillageType
        {
            get { return _tillageType; }
            set { SetProperty(ref _tillageType, value); }
        }
        public bool Yield
        {
            get { return _yield; }
            set { SetProperty(ref _yield, value); }
        }
        public bool AboveGroundCarbonInput
        {
            get { return _aboveGroundCarbonInput; }
            set { SetProperty(ref _aboveGroundCarbonInput, value); }
        }
        public bool BelowGroundCarbonInput
        {
            get { return _belowGroundCarbonInput; }
            set { SetProperty(ref _belowGroundCarbonInput, value); }
        }
        public bool ManureCarbonInput
        {
            get { return _manureCarbonInput; }
            set { SetProperty(ref _manureCarbonInput, value); }
        }
        public bool ClimateParameter
        {
            get { return _climateParameter; }
            set { SetProperty(ref _climateParameter, value); }
        }
        public bool TillageFactor
        {
            get { return _tillageFactor; }
            set { SetProperty(ref _tillageFactor, value); }
        }
        public bool ManagementFactor
        {
            get { return _managementFactor; }
            set { SetProperty(ref _managementFactor, value); }
        }
        public bool Edit
        {
            get { return _edit; }
            set { SetProperty(ref _edit, value); }
        }

        public bool PercentageOfProductReturned
        {
            get => _percentageOfProductReturned;
            set => SetProperty(ref _percentageOfProductReturned, value);
        }

        public bool PercentageOfStrawReturned
        {
            get => _percentageOfStrawReturned;
            set => SetProperty(ref _percentageOfStrawReturned, value);
        }

        public bool PercentageOfRootsReturned
        {
            get => _percentageOfRootsReturned;
            set => SetProperty(ref _percentageOfRootsReturned, value);
        }

        public bool Description
        {
            get => _description;
            set => SetProperty(ref  _description, value);
        }
    }
}
