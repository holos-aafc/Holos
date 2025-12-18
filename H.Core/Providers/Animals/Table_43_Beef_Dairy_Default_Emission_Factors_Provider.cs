using System.Diagnostics;
using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using H.Core.Tools;
using H.Infrastructure;

namespace H.Core.Providers.Animals
{
    /// <summary>
    ///     Table 43. Default emission factors (kg NH3-N kg-1 TAN) for housing, storage and land application of beef
    ///     and dairy cattle manure at the reference temperature of 15 °C (Chai et al., 2014,2016).
    /// </summary>
    public class Table_43_Beef_Dairy_Default_Emission_Factors_Provider : IAnimalAmmoniaEmissionFactorProvider
    {
        #region Constructors

        public Table_43_Beef_Dairy_Default_Emission_Factors_Provider()
        {
            HTraceListener.AddTraceListener();
        }

        #endregion

        #region Methods

        public double GetEmissionFactorByHousing(HousingType housingType)
        {
            if (housingType.IsFeedlot()) return 0.78;

            if (housingType == HousingType.HousedInBarnSolid) return 0.21;

            if (housingType.IsPasture()) return 0.10;

            if (housingType == HousingType.FreeStallBarnSolidLitter) return 0.22;

            if (housingType == HousingType.FreeStallBarnSlurryScraping) return 0.33;

            if (housingType == HousingType.FreeStallBarnFlushing) return 0.28;

            if (housingType == HousingType.FreeStallBarnMilkParlourSlurryFlushing) return 0.28;

            if (housingType == HousingType.TieStallSolidLitter) return 0.17;

            if (housingType == HousingType.TieStallSlurry) return 0.22;

            if (housingType == HousingType.DryLot) return 0.4;

            Trace.TraceError(
                $"{nameof(Table_43_Beef_Dairy_Default_Emission_Factors_Provider)}.{nameof(GetEmissionFactorByHousing)}" +
                $" unable to get data for housing type: {housingType}.");

            return 0;
        }

        public double GetByManureStorageType(ManureStateType storageType)
        {
            // Footnote 1: Read for data reference information.

            if (storageType.IsLiquidManure() || storageType == ManureStateType.DeepPit) return 0.13;

            if (storageType.IsCompost()) return 0.7;

            if (storageType == ManureStateType.SolidStorage || storageType == ManureStateType.DeepBedding) return 0.35;

            Trace.TraceError(
                $"{nameof(Table_43_Beef_Dairy_Default_Emission_Factors_Provider)}.{nameof(GetByManureStorageType)}" +
                $" unable to get data for storage type: {storageType}.");
            return 0;
        }

        public double GetEmissionFactorForLandAppliedManure(ManureApplicationViewItem manureApplicationViewItem,
            CropViewItem viewItem)
        {
            if (manureApplicationViewItem.ManureStateType.IsLiquidManure())
                return GetAmmoniaEmissionFactorForLiquidAppliedManure(manureApplicationViewItem
                    .ManureApplicationMethod);

            return GetAmmoniaEmissionFactorForSolidAppliedManure(viewItem.TillageType);
        }

        public double GetAmmoniaEmissionFactorForLiquidAppliedManure(ManureApplicationTypes manureApplicationType)
        {
            // Footnote 2: Read for data reference information.
            switch (manureApplicationType)
            {
                case ManureApplicationTypes.TilledLandSolidSpread:
                    return 0.69;
                case ManureApplicationTypes.UntilledLandSolidSpread:
                    return 0.79;
                case ManureApplicationTypes.SlurryBroadcasting:
                    return 0.58;
                case ManureApplicationTypes.DropHoseBanding:
                    return 0.29;
                case ManureApplicationTypes.ShallowInjection:
                    return 0.16;
                case ManureApplicationTypes.DeepInjection:
                    return 0.02;

                default:
                    Trace.TraceError(
                        $"{nameof(Table_43_Beef_Dairy_Default_Emission_Factors_Provider)}.{nameof(GetAmmoniaEmissionFactorForLiquidAppliedManure)}" +
                        $" unable to get data for spreading method: {manureApplicationType.GetDescription()}.");
                    return 0;
            }
        }

        public double GetAmmoniaEmissionFactorForSolidAppliedManure(TillageType tillageType)
        {
            switch (tillageType)
            {
                case TillageType.Intensive:
                case TillageType.Reduced:
                    return 0.69;

                case TillageType.NoTill:
                    return 0.79;

                default:
                    Trace.TraceError(
                        $"{nameof(Table_43_Beef_Dairy_Default_Emission_Factors_Provider)}.{nameof(GetAmmoniaEmissionFactorForSolidAppliedManure)}" +
                        $" unable to get data for land application type: {tillageType}.");
                    return 0;
            }
        }

        #region Footnotes

        /*
         * Footnote 1 : Compost: manure heap is turned multiple times to accelerate decomposition of organic compounds
             and stockpile/pack: manure placed in a heap without turning, minimizing oxygen supply and the rate of decomposition.
         * Footnote 2 : Land application EFs are adjusted for time delay before incorporation on tilled land
            (median 1 d in summer, 2 d in spring and fall) and probability of rain (> 2mm rain within a day after spreading).

         */

        #endregion

        #endregion
    }
}