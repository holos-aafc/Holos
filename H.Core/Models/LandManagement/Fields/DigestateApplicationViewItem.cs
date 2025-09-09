﻿using System;
using H.Core.CustomAttributes;
using H.Core.Enumerations;

namespace H.Core.Models.LandManagement.Fields
{
    public class DigestateApplicationViewItem : ManureItemBase
    {
        #region Constructors

        public DigestateApplicationViewItem()
        {
            DateCreated = DateTime.Now;
            DigestateState = DigestateState.Raw;
        }

        #endregion

        #region Fields

        private DigestateState _digestateState;

        private double _amountAppliedPerHectare;
        private double _amountOfCarbonAppliedPerHectare;

        private bool _attemptToGoOverMaximum;

        #endregion

        #region Properties

        public DigestateState DigestateState
        {
            get => _digestateState;
            set => SetProperty(ref _digestateState, value);
        }

        /// <summary>
        ///     Amount of digestate applied
        ///     (kg ha^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramsPerHectare)]
        public double AmountAppliedPerHectare
        {
            get => _amountAppliedPerHectare;
            set => SetProperty(ref _amountAppliedPerHectare, value);
        }

        /// <summary>
        ///     Amount of C applied
        ///     (kg C ha^-1)
        /// </summary>
        public double AmountOfCarbonAppliedPerHectare
        {
            get => _amountOfCarbonAppliedPerHectare;
            set => SetProperty(ref _amountOfCarbonAppliedPerHectare, value);
        }

        public bool AttemptedToGoOverMaximum
        {
            get => _attemptToGoOverMaximum;
            set => SetProperty(ref _attemptToGoOverMaximum, value);
        }

        #endregion
    }
}