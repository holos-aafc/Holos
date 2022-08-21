using H.Core.Tools;
using System;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 27. Lamb daily weight gain.
    /// <para>This is based on a pre-weaning weight gain for single lambs of 0.233 kg day-1, a combined weight gain of 0.388 kg day-1 for twins
    /// (Dimsoski et al. 1999) and a combined weight gain of  0.582 for triplets (derived from from twin weight values).</para>
    /// </summary>
    public class Table_27_Lamb_Daily_Weight_Gain_Provider
    {
        public Table_27_Lamb_Daily_Weight_Gain_Provider()
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
                System.Diagnostics.Trace.TraceError($"{nameof(Table_27_Lamb_Daily_Weight_Gain_Provider)}.{nameof(Table_27_Lamb_Daily_Weight_Gain_Provider.GetDailyWeightGain)}" +
                    $" unable to get data for lamb ratio: {lambRatio}." +
                    $" Returning default value of {0}.");
                return 0;
            }

        }

    }
}
