using H.Core.Enumerations;
using H.Core.Models.Infrastructure;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Models
{
    /// <summary>
    ///     A storage tank for digestate resulting from anaerobic digestion
    /// </summary>
    public class DigestateTank : StorageTankBase
    {
        #region Fields

        private DigestateState _digestateState;

        private double _totalRawDigestateAvailable;
        private double _totalSolidDigestateAvailable;
        private double _totalLiquidDigestateAvailable;

        private double _totalRawDigestateProduced;
        private double _totalSolidDigestateProduced;
        private double _totalLiquidDigestateProduced;

        private double _nitrogenFromRawDigestate;
        private double _nitrogenFromRawDigestateNotConsideringFieldApplicationAmounts;

        private double _nitrogenFromSolidDigestate;
        private double _nitrogenFromSolidDigestateNotConsideringFieldApplicationAmounts;

        private double _nitrogenFromLiquidDigestate;
        private double _nitrogenFromLiquidDigestateNotConsideringFieldApplicationAmounts;

        private double _carbonFromRawDigestate;
        private double _carbonFromRawDigestateNotConsideringFieldApplicationAmounts;

        private double _carbonFromSolidDigestate;
        private double _carbonFromSolidDigestateNotConsideringFieldApplicationAmounts;

        private double _carbonFromLiquidDigestate;
        private double _carbonFromLiquidDigestateNotConsideringFieldApplicationAmounts;

        #endregion

        #region Properties

        /*
         * Nitrogen available for land application
         */

        /// <summary>
        ///     Amount of N in tank after any land applications have been considered
        ///     (kg N)
        /// </summary>
        public double NitrogenFromRawDigestate
        {
            get => _nitrogenFromRawDigestate;
            set => SetProperty(ref _nitrogenFromRawDigestate, value);
        }

        /// <summary>
        ///     Amount in tank not considering any field applications
        ///     (kg N)
        /// </summary>
        public double NitrogenFromRawDigestateNotConsideringFieldApplicationAmounts
        {
            get => _nitrogenFromRawDigestateNotConsideringFieldApplicationAmounts;
            set => SetProperty(ref _nitrogenFromRawDigestateNotConsideringFieldApplicationAmounts, value);
        }

        /// <summary>
        ///     Amount of N in tank after any land applications have been considered
        ///     (kg N)
        /// </summary>
        public double NitrogenFromSolidDigestate
        {
            get => _nitrogenFromSolidDigestate;
            set => SetProperty(ref _nitrogenFromSolidDigestate, value);
        }

        /// <summary>
        ///     Amount in tank not considering any field applications
        ///     (kg N)
        /// </summary>
        public double NitrogenFromSolidDigestateNotConsideringFieldApplicationAmounts
        {
            get => _nitrogenFromSolidDigestateNotConsideringFieldApplicationAmounts;
            set => SetProperty(ref _nitrogenFromSolidDigestateNotConsideringFieldApplicationAmounts, value);
        }

        /// <summary>
        ///     Amount of N in tank after any land applications have been considered
        ///     (kg N)
        /// </summary>
        public double NitrogenFromLiquidDigestate
        {
            get => _nitrogenFromLiquidDigestate;
            set => SetProperty(ref _nitrogenFromLiquidDigestate, value);
        }

        /// <summary>
        ///     Amount of N in tank not considering any field applications
        ///     (kg N)
        /// </summary>
        public double NitrogenFromLiquidDigestateNotConsideringFieldApplicationAmounts
        {
            get => _nitrogenFromLiquidDigestateNotConsideringFieldApplicationAmounts;
            set => SetProperty(ref _nitrogenFromLiquidDigestateNotConsideringFieldApplicationAmounts, value);
        }

        /*
         * Carbon available for land application
         */

        /// <summary>
        ///     Amount of C in tank after any land applications have been considered
        ///     (kg C)
        /// </summary>
        public double CarbonFromRawDigestate
        {
            get => _carbonFromRawDigestate;
            set => SetProperty(ref _carbonFromRawDigestate, value);
        }

        /// <summary>
        ///     Amount of C in tank not considering any field applications
        ///     (kg C)
        /// </summary>
        public double CarbonFromRawDigestateNotConsideringFieldApplicationAmounts
        {
            get => _carbonFromRawDigestateNotConsideringFieldApplicationAmounts;
            set => SetProperty(ref _carbonFromRawDigestateNotConsideringFieldApplicationAmounts, value);
        }

        /// <summary>
        ///     Amount of C in tank after any land applications have been considered
        ///     (kg C)
        /// </summary>
        public double CarbonFromSolidDigestate
        {
            get => _carbonFromSolidDigestate;
            set => SetProperty(ref _carbonFromSolidDigestate, value);
        }

        /// <summary>
        ///     Amount of C in tank not considering any field applications
        ///     (kg C)
        /// </summary>
        public double CarbonFromSolidDigestateNotConsideringFieldApplicationAmounts
        {
            get => _carbonFromSolidDigestateNotConsideringFieldApplicationAmounts;
            set => SetProperty(ref _carbonFromSolidDigestateNotConsideringFieldApplicationAmounts, value);
        }


        /// <summary>
        ///     Amount of C in tank after any land applications have been considered
        ///     (kg C)
        /// </summary>
        public double CarbonFromLiquidDigestate
        {
            get => _carbonFromLiquidDigestate;
            set => SetProperty(ref _carbonFromLiquidDigestate, value);
        }

        /// <summary>
        ///     Amount of C in tank not considering any field applications
        ///     (kg C)
        /// </summary>
        public double CarbonFromLiquidDigestateNotConsideringFieldApplicationAmounts
        {
            get => _carbonFromLiquidDigestateNotConsideringFieldApplicationAmounts;
            set => SetProperty(ref _carbonFromLiquidDigestateNotConsideringFieldApplicationAmounts, value);
        }

        /// <summary>
        ///     A tank of digestate can store whole (raw) digestate or separated digestate
        /// </summary>
        public DigestateState DigestateState
        {
            get => _digestateState;
            set => SetProperty(ref _digestateState, value);
        }

        /// <summary>
        ///     Total amount of raw digestate available after all field applications have been considered
        ///     (kg)
        /// </summary>
        public double TotalRawDigestateAvailable
        {
            get => _totalRawDigestateAvailable;
            set => SetProperty(ref _totalRawDigestateAvailable, value);
        }

        /// <summary>
        ///     Amount of raw digestate in tank not considering any field applications
        ///     (kg)
        /// </summary>
        public double TotalRawDigestateProduced
        {
            get => _totalRawDigestateProduced;
            set => SetProperty(ref _totalRawDigestateProduced, value);
        }

        /// <summary>
        ///     Total amount of solid digestate available after all field applications have been considered
        ///     (kg)
        /// </summary>
        public double TotalSolidDigestateAvailable
        {
            get => _totalSolidDigestateAvailable;
            set => SetProperty(ref _totalSolidDigestateAvailable, value);
        }

        /// <summary>
        ///     Amount of solid digestate in tank not considering any field applications
        ///     (kg)
        /// </summary>
        public double TotalSolidDigestateProduced
        {
            get => _totalSolidDigestateProduced;
            set => SetProperty(ref _totalSolidDigestateProduced, value);
        }

        /// <summary>
        ///     Total amount of solid digestate available after all field applications have been considered
        ///     (kg)
        /// </summary>
        public double TotalLiquidDigestateAvailable
        {
            get => _totalLiquidDigestateAvailable;
            set => SetProperty(ref _totalLiquidDigestateAvailable, value);
        }

        /// <summary>
        ///     Amount of liquid digestate in tank not considering any field applications
        ///     (kg)
        /// </summary>
        public double TotalLiquidDigestateProduced
        {
            get => _totalLiquidDigestateProduced;
            set => SetProperty(ref _totalLiquidDigestateProduced, value);
        }

        #endregion

        #region Public Methods

        public double GetFractionUsed(DigestateApplicationViewItem viewItem, AnaerobicDigestionComponent component,
            CropViewItem cropViewItem)
        {
            if (cropViewItem == null) return 0;

            var totalAmountCreated = GetTotalDigestateCreated(component, viewItem.DigestateState);
            var totalAmountApplied = viewItem.AmountAppliedPerHectare * cropViewItem.Area;

            var result = 0d;
            if (totalAmountCreated > 0) result = totalAmountApplied / totalAmountCreated;

            return result;
        }

        /// <summary>
        ///     (kg N)
        /// </summary>
        public double GetTotalNitrogenCreatedBySystem(AnaerobicDigestionComponent component)
        {
            if (component.IsLiquidSolidSeparated) return NitrogenFromLiquidDigestate + NitrogenFromSolidDigestate;

            return NitrogenFromRawDigestate;
        }

        /// <summary>
        ///     (kg N)
        /// </summary>
        public double GetTotalNitrogenCreatedBySystemNotIncludingFieldApplicationRemovals(
            AnaerobicDigestionComponent component)
        {
            if (component.IsLiquidSolidSeparated)
                return NitrogenFromLiquidDigestateNotConsideringFieldApplicationAmounts +
                       NitrogenFromSolidDigestateNotConsideringFieldApplicationAmounts;

            return NitrogenFromRawDigestateNotConsideringFieldApplicationAmounts;
        }

        public double GetTotalNitrogenCreatedBySystemNotIncludingFieldApplicationRemovals(DigestateState state)
        {
            switch (state)
            {
                case DigestateState.LiquidPhase:
                    return NitrogenFromLiquidDigestateNotConsideringFieldApplicationAmounts;
                case DigestateState.SolidPhase:
                    return NitrogenFromSolidDigestateNotConsideringFieldApplicationAmounts;
                default:
                    return NitrogenFromRawDigestateNotConsideringFieldApplicationAmounts;
            }
        }

        /// <summary>
        ///     (kg C)
        /// </summary>
        public double GetTotalCarbonRemainingAtEndOfYearAfterDigestateApplications(
            AnaerobicDigestionComponent component)
        {
            if (component.IsLiquidSolidSeparated) return CarbonFromLiquidDigestate + CarbonFromSolidDigestate;

            return CarbonFromRawDigestate;
        }

        /// <summary>
        ///     (kg C)
        /// </summary>
        public double GetTotalCarbonCreatedBySystemNotIncludingFieldApplicationRemovals(
            AnaerobicDigestionComponent component)
        {
            if (component.IsLiquidSolidSeparated)
                return CarbonFromLiquidDigestateNotConsideringFieldApplicationAmounts +
                       CarbonFromSolidDigestateNotConsideringFieldApplicationAmounts;

            return CarbonFromRawDigestateNotConsideringFieldApplicationAmounts;
        }

        /// <summary>
        ///     (kg C)
        /// </summary>
        public double GetTotalCarbonCreatedBySystemNotIncludingFieldApplicationRemovals(DigestateState state)
        {
            switch (state)
            {
                case DigestateState.LiquidPhase:
                    return CarbonFromLiquidDigestateNotConsideringFieldApplicationAmounts;
                case DigestateState.SolidPhase:
                    return CarbonFromSolidDigestateNotConsideringFieldApplicationAmounts;
                default:
                    return CarbonFromRawDigestateNotConsideringFieldApplicationAmounts;
            }
        }

        /// <summary>
        ///     This will return the total amount of digestate produced (both liquid and solid amounts if there is separation
        ///     performed)
        ///     (kg digestate)
        /// </summary>
        public double GetTotalDigestateCreated(AnaerobicDigestionComponent component, DigestateState state)
        {
            if (component == null) return 0;

            switch (state)
            {
                case DigestateState.LiquidPhase:
                    return TotalLiquidDigestateProduced;
                case DigestateState.SolidPhase:
                    return TotalSolidDigestateProduced;
                default:
                    return TotalRawDigestateProduced;
            }
        }

        #endregion
    }
}