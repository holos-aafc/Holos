using System;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Models.LandManagement.Fields
{
    /// <summary>
    /// A base class for activities that can take place on a field (i.e. a harvest, a grazing period, and any other types of activities)
    /// </summary>
    public class FieldActivityBase : ModelBase
    {
        #region Fields
        
        private ForageActivities _forageActivity;
        private DateTime _start;
        private DateTime _end;
        private double _aboveGroundBiomass;
        private double _moistureContentAsPercentage;
        private ForageGrowthStages _forageGrowthStage;
        private BaleTypes _baleType;
        private double _utilization;

        #endregion

        #region Properties
        
        public ForageActivities ForageActivity
        {
            get { return _forageActivity; }
            set { this.SetProperty(ref _forageActivity, value); }
        }

        /// <summary>
        /// The start date of the activity
        /// </summary>
        public DateTime Start
        {
            get { return _start; }
            set { SetProperty(ref _start, value); }
        }

        /// <summary>
        /// The end date of the activity
        /// </summary>
        public DateTime End
        {
            get { return _end; }
            set { SetProperty(ref _end, value); }
        }

        /// <summary>
        /// The total wet weight of harvested bales. This value will be added to the total yield for a crop.
        /// 
        /// (kg)
        /// </summary>
        public double AboveGroundBiomass
        {
            get { return _aboveGroundBiomass; }
            set { this.SetProperty(ref _aboveGroundBiomass, value); }
        }

        /// <summary>
        /// (%)
        /// </summary>
        public double MoistureContentAsPercentage
        {
            get { return _moistureContentAsPercentage; }
            set { this.SetProperty(ref _moistureContentAsPercentage, value); }
        }

        /// <summary>
        /// The stage of growth of the forage at the time of the activity
        /// </summary>
        public ForageGrowthStages ForageGrowthStage
        {
            get { return _forageGrowthStage; }
            set { this.SetProperty(ref _forageGrowthStage, value); }
        }

        /// <summary>
        /// The proportion of pasture consumed by animals or the amount gathered from harvest (after considering harvest loss)
        /// </summary>
        public double Utilization
        {
            get { return _utilization; }
            set { this.SetProperty(ref _utilization, value); }
        }

        public BaleTypes BaleType 
        { 
            get => _baleType; 
            set => _baleType = value; 
        }

        #endregion
    }
}