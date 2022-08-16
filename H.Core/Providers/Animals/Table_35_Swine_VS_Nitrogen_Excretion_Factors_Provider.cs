using System;
using H.Core.Enumerations;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Infrastructure;
using H.Core.Tools;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 35. Volatile solid and nitrogen excretion adjustment factors for swine, by diet.
    /// <para>Greenhouse Gas System Pork Protocol 2006</para>
    /// </summary>
    public class Table_35_Swine_VS_Nitrogen_Excretion_Factors_Provider
    {
        private readonly Dictionary<string, DefaultVSAndNitrogenExcretionAdjustmentFactorsForDietsData> _cache = new Dictionary<string, DefaultVSAndNitrogenExcretionAdjustmentFactorsForDietsData>();

        public Table_35_Swine_VS_Nitrogen_Excretion_Factors_Provider()
        {
            HTraceListener.AddTraceListener();

            _cache.Add(DietType.Standard.GetDescription().ToUpperInvariant(), new DefaultVSAndNitrogenExcretionAdjustmentFactorsForDietsData()
            {
                Name = DietType.Standard.GetDescription(),
                VSAdjustment = 1,
                NitrogenExcretedAdjustment = 1

            });

            _cache.Add(DietType.ReducedProtein.GetDescription().ToUpperInvariant(), new DefaultVSAndNitrogenExcretionAdjustmentFactorsForDietsData()
            {
                Name = DietType.ReducedProtein.GetDescription(),
                VSAdjustment = 0.99,
                NitrogenExcretedAdjustment = 0.7
            });

            _cache.Add(DietType.HighlyDigestibleFeed.GetDescription().ToUpperInvariant(), new DefaultVSAndNitrogenExcretionAdjustmentFactorsForDietsData()
            {
                Name = DietType.HighlyDigestibleFeed.GetDescription(),
                VSAdjustment = 0.95,
                NitrogenExcretedAdjustment = 0.95
            });
        }

        public DefaultVSAndNitrogenExcretionAdjustmentFactorsForDietsData GetByDietType(string dietName)
        {
            if (_cache.ContainsKey(dietName.ToUpperInvariant()))
            {
                return _cache[dietName.ToUpperInvariant()];
            }
            else
            {
                var defaultValue = _cache.First().Value;

                Trace.TraceError($"{nameof(Table_35_Swine_VS_Nitrogen_Excretion_Factors_Provider)}.{nameof(GetByDietType)}: unknown diet type, returning default value {defaultValue}");

                return defaultValue;
            }
        }
    }
}
