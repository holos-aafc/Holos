using System.Diagnostics;
using H.Core.Enumerations;
using H.Core.Tools;
using H.Infrastructure;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Duplicate provider. Not used
    /// 
    /// Table 46. Default emission factors (kg NH3-N kg-1 TAN) for housing storage and land application of beef
    /// and dairy cattle manure at the reference temperature of 15 °C (Chai et al., 2014,2016).
    /// </summary>
    public class Duplicate_Table_46_Beef_Dairy_Default_Emission_Factors_Provider
    {
        public Duplicate_Table_46_Beef_Dairy_Default_Emission_Factors_Provider()
        {
            HTraceListener.AddTraceListener();
        }
        public double GetEmissionFactorByHousingType(HousingType housingType)
        {
            switch (housingType)
            {
                case HousingType.HousedInBarn:
                    return 0.78;

                case HousingType.HousedInBarnSolid:
                    return 0.21;

                case HousingType.HousedInBarnSlurry:
                    return 0.32;

                case HousingType.EnclosedPasture:
                    return 0.1;

                case HousingType.OpenRangeOrHills:
                    return 0.2;                    

                default:
                    Trace.TraceError($"{nameof(Duplicate_Table_46_Beef_Dairy_Default_Emission_Factors_Provider)}.{nameof(GetEmissionFactorByHousingType)} uknown housing type {housingType.GetDescription()}. Returning 0.");
                    return 0;
            }
        }

        public double GetEmissionFactorByStorageType(ManureStateType stateType)
        {
            switch (stateType)
            {
                case ManureStateType.CompostIntensive:
                    return 0.7;

                case ManureStateType.CompostPassive:
                    return 0.7;

                case ManureStateType.Solid:
                case ManureStateType.SolidStorage:
                    return 0.35;

                case ManureStateType.DeepBedding:
                    return 0.35;

                default:
                    Trace.TraceError($"{nameof(Duplicate_Table_46_Beef_Dairy_Default_Emission_Factors_Provider)}.{nameof(GetEmissionFactorByStorageType)} uknown storage type {stateType.GetDescription()}. Returning 0.");
                    return 0;
            }
        }

        public double GetEmissionFactorForLandApplication(TillageType tillageType)
        {
            switch (tillageType)
            {
                case TillageType.Intensive:
                    return 0.69;

                case TillageType.NoTill:
                case TillageType.Reduced:
                    return 0.79;

                default:
                    Trace.TraceError($"{nameof(Duplicate_Table_46_Beef_Dairy_Default_Emission_Factors_Provider)}.{nameof(GetEmissionFactorForLandApplication)} unknown tillage type {tillageType.GetDescription()}. Returning 0.");
                    return 0;
            }
        }
    }
}