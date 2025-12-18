using System.Diagnostics;
using H.Core.Enumerations;

namespace H.Core.Providers.AnaerobicDigestion
{
    // Provider that calculates the Biodegraded fraction during storage.
    public class BiodegradedFractionDuringStorageProvider
    {
        #region Public Methods

        /// <summary>
        ///     Method takes in a <see cref="DigestateParameters" /> and a <see cref="DigestateState" /> and returns an instance of
        ///     <see cref="BiodegradedFractionDuringStorageData" />.
        ///     The returning instance contains a "No Cover" and "Covered" fraction representing the Biodegraded fraction during
        ///     storage.
        /// </summary>
        /// <param name="parameter">The digestate parameter for which biodegraded fraction is needed.</param>
        /// <param name="state">The digestate state, which is either Raw, Liquid or Solid.</param>
        /// <returns>Returns an instance of BiodegradedFractionDuringStorageData. If nothing found returns an empty instance.</returns>
        public BiodegradedFractionDuringStorageData GetBiodegradedFractionData(DigestateParameters parameter,
            DigestateState state)
        {
            if (parameter == DigestateParameters.TotalSolids)
            {
                if (state == DigestateState.Raw)
                    return new BiodegradedFractionDuringStorageData
                    {
                        NoCover = 0.05,
                        Cover = 0.05
                    };

                if (state == DigestateState.LiquidPhase)
                    return new BiodegradedFractionDuringStorageData
                    {
                        NoCover = 0.08,
                        Cover = 0.08
                    };

                return new BiodegradedFractionDuringStorageData
                {
                    NoCover = 0.22,
                    Cover = 0.30
                };
            }

            if (parameter == DigestateParameters.VolatileSolids)
            {
                if (state == DigestateState.Raw)
                    return new BiodegradedFractionDuringStorageData
                    {
                        NoCover = 0.07,
                        Cover = 0.07
                    };

                if (state == DigestateState.LiquidPhase)
                    return new BiodegradedFractionDuringStorageData
                    {
                        NoCover = 0.13,
                        Cover = 0.13
                    };

                return new BiodegradedFractionDuringStorageData
                {
                    NoCover = 0.30,
                    Cover = 0.30
                };
            }

            if (parameter == DigestateParameters.TotalCarbon)
            {
                if (state == DigestateState.Raw)
                    return new BiodegradedFractionDuringStorageData
                    {
                        NoCover = 0.08,
                        Cover = 0.08
                    };

                if (state == DigestateState.LiquidPhase)
                    return new BiodegradedFractionDuringStorageData
                    {
                        NoCover = 0.15,
                        Cover = 0.15
                    };

                return new BiodegradedFractionDuringStorageData
                {
                    NoCover = 0.34,
                    Cover = 0.34
                };
            }

            if (parameter == DigestateParameters.OrganicNitrogen)
            {
                if (state == DigestateState.Raw)
                    return new BiodegradedFractionDuringStorageData
                    {
                        NoCover = 0,
                        Cover = 0
                    };

                if (state == DigestateState.LiquidPhase)
                    return new BiodegradedFractionDuringStorageData
                    {
                        NoCover = 0,
                        Cover = 0
                    };

                return new BiodegradedFractionDuringStorageData
                {
                    NoCover = 0.09,
                    Cover = 0.09
                };
            }

            if (parameter == DigestateParameters.TotalAmmoniaNitrogen)
            {
                if (state == DigestateState.Raw)
                    return new BiodegradedFractionDuringStorageData
                    {
                        NoCover = 0.36,
                        Cover = 0.07
                    };

                if (state == DigestateState.LiquidPhase)
                    return new BiodegradedFractionDuringStorageData
                    {
                        NoCover = 0.44,
                        Cover = 0.09
                    };

                return new BiodegradedFractionDuringStorageData
                {
                    NoCover = 0.51,
                    Cover = 0.51
                };
            }

            Trace.TraceError(
                $"{nameof(BiodegradedFractionDuringStorageProvider)}.{nameof(GetBiodegradedFractionData)} " +
                $"could not find DigestateParameters: {parameter}. Returning an empty instance of BiodegradedFractionDuringStorageData");
            return new BiodegradedFractionDuringStorageData();
        }

        #endregion
    }
}