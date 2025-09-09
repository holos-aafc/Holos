namespace H.Core.Models
{
    /// <summary>
    ///     The class used to store the visibility of the columns related to the FieldSystemsDetailView when the user has
    ///     selected the ICBM carbon modelling approach
    /// </summary>
    public class FieldSystemDetailsColumnsVisibility : ColumnVisibilityBase
    {
        public FieldSystemDetailsColumnsVisibility()
        {
            DefaultVisibility();
        }

        public bool FieldName
        {
            get => _fieldName;
            set => SetProperty(ref _fieldName, value);
        }

        public bool TimePeriod
        {
            get => _timePeriod;
            set => SetProperty(ref _timePeriod, value);
        }

        public bool Year
        {
            get => _year;
            set => SetProperty(ref _year, value);
        }

        public bool CropType
        {
            get => _cropType;
            set => SetProperty(ref _cropType, value);
        }

        public bool TillageType
        {
            get => _tillageType;
            set => SetProperty(ref _tillageType, value);
        }

        public bool Yield
        {
            get => _yield;
            set => SetProperty(ref _yield, value);
        }

        public bool AboveGroundCarbonInput
        {
            get => _aboveGroundCarbonInput;
            set => SetProperty(ref _aboveGroundCarbonInput, value);
        }

        public bool BelowGroundCarbonInput
        {
            get => _belowGroundCarbonInput;
            set => SetProperty(ref _belowGroundCarbonInput, value);
        }

        public bool ManureCarbonInput
        {
            get => _manureCarbonInput;
            set => SetProperty(ref _manureCarbonInput, value);
        }

        public bool ClimateParameter
        {
            get => _climateParameter;
            set => SetProperty(ref _climateParameter, value);
        }

        public bool TillageFactor
        {
            get => _tillageFactor;
            set => SetProperty(ref _tillageFactor, value);
        }

        public bool ManagementFactor
        {
            get => _managementFactor;
            set => SetProperty(ref _managementFactor, value);
        }

        public bool Edit
        {
            get => _edit;
            set => SetProperty(ref _edit, value);
        }

        public bool PlantCarbonInAgriculturalProduct
        {
            get => _plantCarbonInAgriculturalProduct;
            set => SetProperty(ref _plantCarbonInAgriculturalProduct, value);
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
            set => SetProperty(ref _description, value);
        }

        public bool DigestateCarbonInput
        {
            get => _digestateCarbonInput;
            set => SetProperty(ref _digestateCarbonInput, value);
        }

        public void DefaultVisibility()
        {
            //clear current selection
            SetAllColumnsInvisible();

            //the default columns for the grid
            FieldName = true;
            TimePeriod = true;
            Year = true;
            CropType = true;
            Yield = true;
            PercentageOfProductReturned = false;
            PercentageOfStrawReturned = false;
            PercentageOfRootsReturned = false;
            AboveGroundCarbonInput = false;
            BelowGroundCarbonInput = false;
            Description = true;
            PlantCarbonInAgriculturalProduct = false;
        }

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
        private bool _digestateCarbonInput;
        private bool _climateParameter;
        private bool _tillageFactor;
        private bool _managementFactor;
        private bool _edit;
        private bool _percentageOfProductReturned;
        private bool _percentageOfStrawReturned;
        private bool _percentageOfRootsReturned;
        private bool _description;
        private bool _plantCarbonInAgriculturalProduct;

        #endregion
    }
}