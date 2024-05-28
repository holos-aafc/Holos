using System.Security.Permissions;
using H.Core.Enumerations;
using H.Core.Models.Infrastructure;
using H.Core.Models.LandManagement.Fields;
using H.Core.Services.Animals;

namespace H.Core.Models
{
    /// <summary>
    /// A storage tank for digestate resulting from anaerobic digestion
    /// </summary>
    public class DigestateTank : StorageTankBase
    {
        #region Fields

        private DigestateState _digestateState;

        // Amount of digestate available after all field applications
        private double _totalRawDigestateAvailable;
        private double _totalSolidDigestateAvailable;
        private double _totalLiquidDigestateAvailable;

        // Amount of digestate produced by system (not reduced by field applications)
        private double _totalRawDigestateProduced;
        private double _totalSolidDigestateProduced;
        private double _totalLiquidDigestateProduced;

        /*
         * Nitrogen available for land application
         */

        /// <summary>
        /// Amount of N in tank
        /// 
        /// (kg N)
        /// </summary>
        public double NitrogenFromRawDigestate { get; set; }

        /// <summary>
        /// Amount in tank not considering any field applications
        /// 
        /// (kg N)
        /// </summary>
        public double NitrogenFromRawDigestateNotConsideringFieldApplicationAmounts { get; set; }

        /// <summary>
        /// Amount of N in tank
        /// 
        /// (kg N)
        /// </summary>
        public double NitrogenFromSolidDigestate { get; set; }

        /// <summary>
        /// Amount in tank not considering any field applications
        /// 
        /// (kg N)
        /// </summary>
        public double NitrogenFromSolidDigestateNotConsideringFieldApplicationAmounts { get; set; }

        /// <summary>
        /// Amount remaining in tank after field applications
        /// 
        /// (kg N)
        /// </summary>
        public double NitrogenFromLiquidDigestate  { get; set; }

        /// <summary>
        /// Amount in tank not considering any field applications
        /// 
        /// (kg N)
        /// </summary>
        public double NitrogenFromLiquidDigestateNotConsideringFieldApplicationAmounts { get; set; }

        /*
         * Carbon available for land application
         */

        /// <summary>
        /// (kg C)
        /// </summary>
        public double CarbonFromRawDigestate { get; set; }

        /// <summary>
        /// Amount in tank not considering any field applications
        /// 
        /// (kg C)
        /// </summary>
        public double CarbonFromRawDigestateNotConsideringFieldApplicationAmounts { get; set; }

        /// <summary>
        /// Amount remaining in tank after field applications
        /// 
        /// (kg C)
        /// </summary>
        public double CarbonFromSolidDigestate { get; set; }

        /// <summary>
        /// Amount in tank not considering any field applications
        /// 
        /// (kg C)
        /// </summary>
        public double CarbonFromSolidDigestateNotConsideringFieldApplicationAmounts { get; set; }


        /// <summary>
        /// Amount remaining in tank after field applications
        /// 
        /// (kg C)
        /// </summary>
        public double CarbonFromLiquidDigestate { get; set; }

        /// <summary>
        /// Amount in tank not considering any field applications
        /// 
        /// (kg C)
        /// </summary>
        public double CarbonFromLiquidDigestateNotConsideringFieldApplicationAmounts { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// A tank of digestate can store whole (raw) digestate or separated digestate
        /// </summary>
        public DigestateState DigestateState
        {
            get => _digestateState;
            set => SetProperty(ref _digestateState, value);
        }

        /// <summary>
        /// Total amount of raw digestate available after all field applications have been considered
        ///  
        /// (kg)
        /// </summary>
        public double TotalRawDigestateAvailable
        {
            get => _totalRawDigestateAvailable;
            set => SetProperty(ref _totalRawDigestateAvailable, value);
        }

        /// <summary>
        /// (kg)
        /// </summary>
        public double TotalSolidDigestateAvailable
        {
            get => _totalSolidDigestateAvailable;
            set => SetProperty(ref _totalSolidDigestateAvailable, value);
        }

        /// <summary>
        /// (kg)
        /// </summary>
        public double TotalLiquidDigestateAvailable
        {
            get => _totalLiquidDigestateAvailable;
            set => SetProperty(ref _totalLiquidDigestateAvailable, value);
        }

        public double TotalRawDigestateProduced
        {
            get => _totalRawDigestateProduced;
            set => SetProperty(ref _totalRawDigestateProduced, value);
        }

        public double TotalSolidDigestateProduced
        {
            get => _totalSolidDigestateProduced;
            set => SetProperty(ref _totalSolidDigestateProduced, value);
        }

        public double TotalLiquidDigestateProduced
        {
            get => _totalLiquidDigestateProduced;
            set => SetProperty(ref _totalLiquidDigestateProduced, value);
        }

        #endregion

        #region Public Methods

        public double GetFractionUsed(DigestateApplicationViewItem viewItem, AnaerobicDigestionComponent component, CropViewItem cropViewItem)
        {
            if (cropViewItem == null)
            {
                return 0;
            }

            var totalAmountCreated = this.GetTotalDigestateCreated(component);
            var totalAmountApplied = viewItem.AmountAppliedPerHectare * cropViewItem.Area;

            var result = 0d;
            if (totalAmountCreated > 0)
            {
                result = totalAmountApplied / totalAmountCreated;
            }

            return result;
        }

        /// <summary>
        /// (kg N)
        /// </summary>
        public double GetTotalNitrogenCreatedBySystem(AnaerobicDigestionComponent component)
        {        
            if (component.IsLiquidSolidSeparated)
            {
                return this.NitrogenFromLiquidDigestate + this.NitrogenFromSolidDigestate;
            }
            else
            {
                return this.NitrogenFromRawDigestate;
            }
        }

        /// <summary>
        /// (kg N)
        /// </summary>
        public double GetTotalNitrogenCreatedBySystemNotIncludingFieldApplicationRemovals(AnaerobicDigestionComponent component)
        {
            if (component.IsLiquidSolidSeparated)
            {
                return this.NitrogenFromLiquidDigestateNotConsideringFieldApplicationAmounts + this.NitrogenFromSolidDigestateNotConsideringFieldApplicationAmounts;
            }
            else
            {
                return this.NitrogenFromRawDigestateNotConsideringFieldApplicationAmounts;
            }
        }

        /// <summary>
        /// (kg C)
        /// </summary>
        public double GetTotalCarbonRemainingAtEndOfYearAfterDigestateApplications(AnaerobicDigestionComponent component)
        {
            if (component.IsLiquidSolidSeparated)
            {
                return this.CarbonFromLiquidDigestate + this.CarbonFromSolidDigestate;
            }
            else
            {
                return this.CarbonFromRawDigestate;
            }
        }

        /// <summary>
        /// (kg C)
        /// </summary>
        public double GetTotalCarbonCreatedBySystemNotIncludingFieldApplicationRemovals(AnaerobicDigestionComponent component)
        {
            if (component.IsLiquidSolidSeparated)
            {
                return this.CarbonFromLiquidDigestateNotConsideringFieldApplicationAmounts + this.CarbonFromSolidDigestateNotConsideringFieldApplicationAmounts;
            }
            else
            {
                return this.CarbonFromRawDigestateNotConsideringFieldApplicationAmounts;
            }
        }

        /// <summary>
        /// This will return the total amount of digestate produced (both liquid and solid amounts if there is separation performed)
        /// 
        /// (kg digestate)
        /// </summary>
        public double GetTotalDigestateCreated(AnaerobicDigestionComponent component)
        {
            if (component == null)
            {
                return 0;
            }

            if (component.IsLiquidSolidSeparated)
            {
                return this.TotalLiquidDigestateProduced + this.TotalSolidDigestateProduced;
            }
            else
            {
                return this.TotalRawDigestateProduced;
            }
        }

        #endregion
    }
}