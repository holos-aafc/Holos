using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Core.Models
{
    public class FieldSystemDetailsTier2ColumnsVisibility : ColumnVisibilityBase
    {
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

        #region Constructors

        public FieldSystemDetailsTier2ColumnsVisibility()
        {
            
        }

        #endregion

        #region Properties

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

        #endregion

        #region Methods

        public void DefaultVisibility()
        {
            // Clear current selection
            base.SetAllColumnsInvisible();

            // The default columns for the grid
            this.FieldName = true;
            this.TimePeriod = true;
            this.Year = true;
            this.CropType = true;
            this.TillageType = true;
            this.Yield = true;      
            this.AboveGroundCarbonInput = true; 
            this.BelowGroundCarbonInput = true;
        }

        #endregion
    }
}
