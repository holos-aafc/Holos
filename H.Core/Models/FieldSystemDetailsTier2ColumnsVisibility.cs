namespace H.Core.Models
{
    public class FieldSystemDetailsTier2ColumnsVisibility : ColumnVisibilityBase
    {
        #region Methods

        public void DefaultVisibility()
        {
            // Clear current selection
            SetAllColumnsInvisible();

            // The default columns for the grid
            FieldName = true;
            TimePeriod = true;
            Year = true;
            CropType = true;
            TillageType = true;
            Yield = true;
            AboveGroundCarbonInput = true;
            BelowGroundCarbonInput = true;
        }

        #endregion

        #region Fields

        private bool _fieldName;
        private bool _timePeriod;
        private bool _year;
        private bool _cropType;
        private bool _tillageType;
        private bool _yield;
        private bool _aboveGroundCarbonInput;
        private bool _belowGroundCarbonInput;

        #endregion

        #region Properties

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

        #endregion
    }
}