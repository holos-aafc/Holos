using H.Core.Tools;
using System;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 33
    /// </summary>
    public class DefaultPregnancyCoefficientsForSheepProvider_Table_33
    {

        public DefaultPregnancyCoefficientsForSheepProvider_Table_33()
        {
            HTraceListener.AddTraceListener(); 
        }
        public double GetPregnancyCoefficient(double lambRatio)
        {
            if (lambRatio <= 1)
            {
                return 0.077;
            }

            if (lambRatio > 1 && lambRatio <= 2)
            {
                return 0.077 * (2 - lambRatio) + 0.126 * (lambRatio - 1);
            }

            else if (lambRatio > 2 && lambRatio <= 3)
            {
                return 0.126 * (3 - lambRatio) + 0.150 * (lambRatio - 2);
            }

            else
            {
                System.Diagnostics.Trace.TraceError($"{nameof(DefaultPregnancyCoefficientsForSheepProvider_Table_33)}.{nameof(DefaultPregnancyCoefficientsForSheepProvider_Table_33.GetPregnancyCoefficient)}" +
                    $" unable to get data for lamb ratio: {lambRatio}." +
                    $" Returning default value of {0}.");
                return 0;
            }
        }

    }
}
