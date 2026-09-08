using System;
using H.Core.CustomAttributes;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Models.LandManagement.Fields
{
    public class DigestateApplicationViewItem : ManureItemBase, IRepeatableFieldActivity
    {
        #region Fields

        private bool _repeatsInEveryYear;

        private DigestateState _digestateState;

        private double _amountAppliedPerHectare;
        private double _amountOfCarbonAppliedPerHectare;

        private bool _attemptToGoOverMaximum;

        #endregion

        #region Constructors

        public DigestateApplicationViewItem()
        {
            this.RepeatsInEveryYear = true;

            base.DateCreated = DateTime.Now;
            this.DigestateState = DigestateState.Raw;
        }

        #endregion

        #region Properties

        /// <summary>
        /// See <see cref="IRepeatableFieldActivity.RepeatsInEveryYear"/>. Defaults to true: the algorithm document
        /// builds the historical period from the management the user specifies once, so repeating is the norm and a
        /// entry happening in a single year is the exception. Items saved before this property existed deserialize without it and keep
        /// that default deliberately.
        /// </summary>
        public bool RepeatsInEveryYear
        {
            get => _repeatsInEveryYear;
            set => SetProperty(ref _repeatsInEveryYear, value);
        }

        public DigestateState DigestateState
        {
            get => _digestateState;
            set => SetProperty(ref _digestateState, value);
        }

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

        /// <summary>
        /// Amount of C applied
        ///
        /// (kg C ha^-1)
        /// </summary>
        public double AmountOfCarbonAppliedPerHectare
        {
            get => _amountOfCarbonAppliedPerHectare;
            set
            {
                SetProperty(ref _amountOfCarbonAppliedPerHectare, value);
            } 
        }

        public bool AttemptedToGoOverMaximum
        {
            get => _attemptToGoOverMaximum;
            set => SetProperty(ref _attemptToGoOverMaximum, value);
        }

        #endregion

        #region Public Method

        #endregion
    }
}