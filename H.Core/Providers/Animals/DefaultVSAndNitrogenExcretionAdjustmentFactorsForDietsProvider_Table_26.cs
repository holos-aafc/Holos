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
    /// Table 26: Volatile solid and nitrogen excretion adjustment factors, by diet.
    /// </summary>
    public class DefaultVSAndNitrogenExcretionAdjustmentFactorsForDietsProvider_Table_26
    {
        private readonly Dictionary<string, DefaultVSAndNitrogenExcretionAdjustmentFactorsForDietsData> _cache = new Dictionary<string, DefaultVSAndNitrogenExcretionAdjustmentFactorsForDietsData>();

        public DefaultVSAndNitrogenExcretionAdjustmentFactorsForDietsProvider_Table_26()
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

                Trace.TraceError($"{nameof(DefaultVSAndNitrogenExcretionAdjustmentFactorsForDietsProvider_Table_26)}.{nameof(GetByDietType)}: unknown diet type, returning default value {defaultValue}");

                return defaultValue;
            }
        }
    }
}
