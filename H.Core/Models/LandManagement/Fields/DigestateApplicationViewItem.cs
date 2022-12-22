﻿using System;
using H.Core.CustomAttributes;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Models.LandManagement.Fields
{
    public class DigestateApplicationViewItem : ModelBase
    {
        #region Fields

        private DigestateState _digestateState;

        private double _amountAppliedPerHectare;

        #endregion

        #region Constructors

        public DigestateApplicationViewItem()
        {
            base.DateCreated = DateTime.Now;
            this.DigestateState = DigestateState.Raw;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Amount of digestate applied
        ///
        /// (kg ha^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramsPerHectare)]
        public double AmountAppliedPerHectare
        {
            get => _amountAppliedPerHectare;
            set => SetProperty(ref _amountAppliedPerHectare, value);
        }

        public DigestateState DigestateState
        {
            get => _digestateState;
            set => SetProperty(ref _digestateState, value);
        }

        #endregion
    }
}