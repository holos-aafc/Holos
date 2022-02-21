using H.Core.Enumerations;
using H.Core.Tools;
using System.Diagnostics;
using H.Infrastructure;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Default ammonia (NH3) emission factors for beef and dairy cattle.
    /// </summary>
    public class DefaultAmmoniaEmissionProvider_Table_39
    {
        #region Constructors
        
        public DefaultAmmoniaEmissionProvider_Table_39()
        {
            HTraceListener.AddTraceListener();
        } 

        #endregion

        #region Methods

        /// <summary>
        /// Provides the default ammonia emission factor (kg NH3-N kg^-1 TAN) for the given housing type
        /// </summary>
        /// <param name="housingType">The housing type</param>
        /// <returns>The ammonia emission factor for the particular housing type</returns>
        public double GetEmissionFactorByHousing(HousingType housingType)
        {
            if (housingType.IsFeedlot())
            {
                return 0.78;
            }
            else if (housingType == HousingType.HousedInBarnSolid)
            {
                return 0.21;
            }
            else if (housingType == HousingType.HousedInBarnSlurry)
            {
                return 0.32;
            }
            else if (housingType.IsPasture())
            {
                return 0.10;
            }
            else if (housingType == HousingType.FreeStallBarnSolidLitter)
            {
                return 0.21;
            }
            else if (housingType == HousingType.FreeStallBarnSlurryScraping)
            {
                return 0.33;
            }
            else if (housingType == HousingType.FreeStallBarnFlushing)
            {
                return 0.28;
            }
            else if (housingType == HousingType.FreeStallBarnMilkParlourSlurryFlushing)
            {
                return 0.28;
            }
            else if (housingType == HousingType.TieStallSolidLitter)
            {
                return 0.17;
            }
            else if (housingType == HousingType.TieStallSlurry)
            {
                return 0.22;
            }
            else if (housingType == HousingType.DryLot)
            {
                return 0.4;
            }
            else
            {
                Trace.TraceError($"{nameof(DefaultAmmoniaEmissionProvider_Table_39)}.{nameof(DefaultAmmoniaEmissionProvider_Table_39.GetEmissionFactorByHousing)}" +
                                 $" unable to get data for housing type: {housingType}.");

                return 0;
            }
        }

        /// <summary>
        /// Provides the default ammonia emission factor (kg NH3-N kg^-1 TAN) for the given manure storage type
        /// </summary>
        /// <param name="storageType">The manure storage type</param>
        /// <returns>The ammonia emission factor for the particular storage type (kg NH3-N kg^-1 TAN)</returns>
        public double GetByManureStorageType(ManureStateType storageType)
        {
            if (storageType.IsLiquidManure())
            {
                return 0.13;
            }
            else if (storageType.IsCompost())
            {
                return 0.7;
            }
            else if (storageType == ManureStateType.SolidStorage ||
                     storageType == ManureStateType.DeepBedding)
            {
                return 0.35;
            }
            else 
            {
                Trace.TraceError($"{nameof(DefaultAmmoniaEmissionProvider_Table_39)}.{nameof(DefaultAmmoniaEmissionProvider_Table_39.GetByManureStorageType)}" +
                                 $" unable to get data for storage type: {storageType}.");
                return 0;
            }
        }

        /// <summary>
        /// For liquid manure types, the emission factor will depend on the type of manure application method being used.
        /// </summary>
        /// <param name="manureApplicationType">The manure application method</param>
        /// <returns>The ammonia emission factor for the particular application method (kg NH3-N kg^-1 TAN)</returns>
        public double GetAmmoniaEmissionFactorForLiquidAppliedManure(ManureApplicationTypes manureApplicationType)
        {
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
                    Trace.TraceError($"{nameof(DefaultAmmoniaEmissionProvider_Table_39)}.{nameof(DefaultAmmoniaEmissionProvider_Table_39.GetAmmoniaEmissionFactorForLiquidAppliedManure)}" +
                                     $" unable to get data for spreading method: {manureApplicationType.GetDescription()}.");
                    return 0;
            }
        }

        /// <summary>
        /// For solid manure types, the emission factor will depend on the tillage being used when manure is applied to the land.
        /// </summary>
        /// <param name="tillageType">The <see cref="TillageType"/> of the crop.</param>
        /// <returns>The ammonia emission factor for land applied manure (NH3-N (kg TAN)^-1)</returns>
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
                    Trace.TraceError($"{nameof(DefaultAmmoniaEmissionProvider_Table_39)}.{nameof(DefaultAmmoniaEmissionProvider_Table_39.GetAmmoniaEmissionFactorForSolidAppliedManure)}" +
                        $" unable to get data for land application type: {tillageType}.");
                    return 0;
            }
        }

        #endregion
    }
}
