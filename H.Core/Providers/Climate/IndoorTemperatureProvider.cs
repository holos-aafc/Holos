using System;
using H.Core.Enumerations;

namespace H.Core.Providers.Climate
{
    public class IndoorTemperatureProvider : IIndoorTemperatureProvider
    {
        public double GetTemperature(DateTime dateTime)
        {
            var month = (Months) dateTime.Date.Month;

            switch (month)
            {
                case Months.January:
                {
                    return 15;
                }

                case Months.February:
                {
                    return 17;
                }

                case Months.March:
                {
                    return 19;
                }

                case Months.April:
                {
                    return 21;
                }

                case Months.May:
                {
                    return 23;
                }

                case Months.June:
                {
                    return 25;
                }

                case Months.July:
                {
                    return 25;
                }

                case Months.August:
                {
                    return 23;
                }

                case Months.September:
                {
                    return 21;
                }

                case Months.October:
                {
                    return 19;
                }

                case Months.November:
                {
                    return 17;
                }

                case Months.December:
                {
                    return 15;
                }

                default:
                {
                    return 15;
                }
            }
        }
    }
}