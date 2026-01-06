using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Content;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Providers.Climate
{
    public class Table_63_Indoor_Temperature_Provider : ProviderBase, IIndoorTemperatureProvider
    {
        #region Fields

        private readonly ProvinceStringConverter _provincesConverter = new ProvinceStringConverter();
        private readonly List<IndoorTemperatureData> _data;

        #endregion

        #region Constructors

        public Table_63_Indoor_Temperature_Provider()
        {
            _data = this.ReadFile();
        }

        #endregion

        #region Public Methods

        public double GetIndoorTemperatureForMonth(Province province, Months months)
        {
            var temperatureForProvince = this.GetIndoorTemperature(province);

            return temperatureForProvince.GetValueByMonth(months);
        }

        public IndoorTemperatureData GetIndoorTemperature(Province province)
        {
            var result = _data.SingleOrDefault(x => x.Province == province);
            if (result != null)
            {
                return result;
            }
            else
            {
                Trace.TraceError($"{nameof(Table_63_Indoor_Temperature_Provider)}.{nameof(Table_63_Indoor_Temperature_Provider.GetIndoorTemperature)}" +
                                 $" unknown province: '{province.GetDescription()}', returning 0");

                return new IndoorTemperatureData();
            }
        }

        public List<IndoorTemperatureData> ReadFile()
        {
            var list = new List<IndoorTemperatureData>();

            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.IndoorBarnTemperatures).ToList();

            var dataRows = fileLines.Skip(2).Take(10);
            foreach (var line in dataRows)
            {
                var province = _provincesConverter.Convert(line[0]);
                var monthlyValues = new List<double>();
                for (int i = 2; i < line.Length; i++)
                {
                    var valueForMonth = base.ParseDouble(line[i]);
                    monthlyValues.Add(valueForMonth);
                }

                var indoorTemperatureForProvince = new IndoorTemperatureData();
                indoorTemperatureForProvince.Province = province;
                indoorTemperatureForProvince.AssignMonthlyValues(monthlyValues);

                list.Add(indoorTemperatureForProvince);
            }

            return list;
        }

        public double GetTemperature(DateTime dateTime)
        {
            var month = (Months)dateTime.Date.Month;

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

        #endregion
    }
}