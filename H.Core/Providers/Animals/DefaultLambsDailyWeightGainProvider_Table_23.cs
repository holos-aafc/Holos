using H.Core.Tools;
using System;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 23
    ///
    /// Lamb daily weight gain
    /// </summary>
    public class DefaultLambsDailyWeightGainProvider_Table_23
    {
        public DefaultLambsDailyWeightGainProvider_Table_23()
        {
            HTraceListener.AddTraceListener();
        }

        /// <summary>
        /// Returns the daily weight gain of lambs for a given lamb ratio
        /// </summary>
        /// <returns>The daily weight gain (kg day^-1)</returns>
        public double GetDailyWeightGain(double lambRatio)
        {
            if (lambRatio == 0)
            {
                return 0;
            }

           if (lambRatio <= 1)
            {
                return 0.233;
            }

            if (lambRatio > 1 && lambRatio <= 2)
            {
                return 0.233 * (2 - lambRatio) + 0.388 * (lambRatio - 1);
            }

            else if (lambRatio > 2 && lambRatio <= 3)
            {
                return 0.388 * (3 - lambRatio) + 0.582 * (lambRatio - 2);
            }

            else
            {
                System.Diagnostics.Trace.TraceError($"{nameof(DefaultLambsDailyWeightGainProvider_Table_23)}.{nameof(DefaultLambsDailyWeightGainProvider_Table_23.GetDailyWeightGain)}" +
                    $" unable to get data for lamb ratio: {lambRatio}." +
                    $" Returning default value of {0}.");
                return 0;
            }

        }

    }
}
