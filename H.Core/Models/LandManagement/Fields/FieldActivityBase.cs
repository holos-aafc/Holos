﻿using System;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Models.LandManagement.Fields
{
    /// <summary>
    ///     A base class for activities that can take place on a field (i.e. a harvest, a grazing period, and any other types
    ///     of activities)
    /// </summary>
    public class FieldActivityBase : ModelBase
    {
        #region Fields

        private ForageActivities _forageActivity;
        private DateTime _start;
        private DateTime _end;
        private double _aboveGroundBiomass;
        private double _aboveGroundBiomassDryWeight;
        private double _moistureContentAsPercentage;
        private ForageGrowthStages _forageGrowthStage;
        private BaleTypes _baleType;
        private double _utilization;

        private DateTime _minimumAllowableDateOfApplication;
        private DateTime _maximumAllowableDateOfApplication;

        #endregion

        #region Properties

        public DateTime MinimumAllowableDateOfApplication
        {
            get => _minimumAllowableDateOfApplication;
            set => SetProperty(ref _minimumAllowableDateOfApplication, value);
        }

        public DateTime MaximumAllowableDateOfApplication
        {
            get => _maximumAllowableDateOfApplication;
            set => SetProperty(ref _maximumAllowableDateOfApplication, value);
        }

        public ForageActivities ForageActivity
        {
            get => _forageActivity;
            set => SetProperty(ref _forageActivity, value);
        }

        /// <summary>
        ///     The start date of the activity
        /// </summary>
        public DateTime Start
        {
            get => _start;
            set => SetProperty(ref _start, value);
        }

        /// <summary>
        ///     The end date of the activity
        /// </summary>
        public DateTime End
        {
            get => _end;
            set => SetProperty(ref _end, value);
        }

        /// <summary>
        ///     The total wet weight of harvested/imported bales
        ///     (kg)
        /// </summary>
        public double AboveGroundBiomass
        {
            get => _aboveGroundBiomass;
            set => SetProperty(ref _aboveGroundBiomass, value);
        }

        /// <summary>
        ///     The total dry matter weight of harvested bales.
        ///     (kg)
        /// </summary>
        public double AboveGroundBiomassDryWeight
        {
            get => _aboveGroundBiomassDryWeight;
            set => SetProperty(ref _aboveGroundBiomassDryWeight, value);
        }

        /// <summary>
        ///     (%)
        /// </summary>
        public double MoistureContentAsPercentage
        {
            get => _moistureContentAsPercentage;
            set => SetProperty(ref _moistureContentAsPercentage, value);
        }

        /// <summary>
        ///     The stage of growth of the forage at the time of the activity
        /// </summary>
        public ForageGrowthStages ForageGrowthStage
        {
            get => _forageGrowthStage;
            set => SetProperty(ref _forageGrowthStage, value);
        }

        /// <summary>
        ///     The proportion of pasture consumed by animals or the amount gathered from harvest (after considering harvest loss)
        ///     (%)
        /// </summary>
        public double Utilization
        {
            get => _utilization;
            set => SetProperty(ref _utilization, value);
        }

        public BaleTypes BaleType
        {
            get => _baleType;
            set => SetProperty(ref _baleType, value);
        }

        #endregion
    }
}