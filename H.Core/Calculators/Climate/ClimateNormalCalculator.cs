using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Providers;
using H.Core.Providers.Climate;
using H.Core.Providers.Evapotranspiration;
using H.Core.Providers.Precipitation;
using H.Core.Providers.Temperature;

namespace H.Core.Calculators.Climate
{
    /// <summary>
    /// Calculates monthly climate normals based on daily climate values
    /// </summary>
    public class ClimateNormalCalculator
    {
        #region Inner Classes

        //the various types of climate normals we are calculating
        public enum ClimateType
        {
            Temperature,
            Precipitation,
            Evapotranspiration,
        }
        public class SpecificClimateValueType
        {
            public ClimateType ClimateNormalType { get; set; }
            public int Year { get; set; }
            public int JulianDay { get; set; }
            public double ClimateValue { get; set; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get evapotranspiration for custom timeframe
        /// </summary>
        /// <param name="customClimateData">the daily climate data needed to calc the Evapotranspiration data</param>
        /// <param name="customTimeFrameStart">beginning of custom time frame</param>
        /// <param name="customTimeFrameEnd">end of custom time frame</param>
        /// <returns>EvapotranspirationData</returns>
        public EvapotranspirationData GetEvapotranspirationDataByDailyValues(List<DailyClimateData> customClimateData, int customTimeFrameStart, int customTimeFrameEnd)
        {
            var dictionary = this.GetNormalsForAllTwelveMonths(customClimateData, customTimeFrameStart, customTimeFrameEnd);
            var evapotranspirationNormals = dictionary[MonthlyNormalTypes.evapotranspiration];
            var evapotranspiration = new EvapotranspirationData()
            {
                January = evapotranspirationNormals[Months.January],
                February = evapotranspirationNormals[Months.February],
                March = evapotranspirationNormals[Months.March],
                April = evapotranspirationNormals[Months.April],
                May = evapotranspirationNormals[Months.May],
                June = evapotranspirationNormals[Months.June],
                July = evapotranspirationNormals[Months.July],
                August = evapotranspirationNormals[Months.August],
                September = evapotranspirationNormals[Months.September],
                October = evapotranspirationNormals[Months.October],
                November = evapotranspirationNormals[Months.November],
                December = evapotranspirationNormals[Months.December],
            };

            return evapotranspiration;
        }

        /// <summary>
        /// Get evapotranspirationdata for a timeframe
        /// </summary>
        /// <param name="customClimateData">the daily climate data needed to calc the Evapotranspiration data</param>
        /// <param name="timeFrame">the time frame to calculate for</param>
        /// <returns>EvapotranspirationData</returns>
        public EvapotranspirationData GetEvapotranspirationDataByDailyValues(List<DailyClimateData> customClimateData, TimeFrame timeFrame)
        {

            var dictionary = this.GetNormalsForAllTwelveMonths(customClimateData, timeFrame);
            var evapotranspirationNormals = dictionary[MonthlyNormalTypes.evapotranspiration];
            var evapotranspiration = new EvapotranspirationData()
            {
                January = evapotranspirationNormals[Months.January],
                February = evapotranspirationNormals[Months.February],
                March = evapotranspirationNormals[Months.March],
                April = evapotranspirationNormals[Months.April],
                May = evapotranspirationNormals[Months.May],
                June = evapotranspirationNormals[Months.June],
                July = evapotranspirationNormals[Months.July],
                August = evapotranspirationNormals[Months.August],
                September = evapotranspirationNormals[Months.September],
                October = evapotranspirationNormals[Months.October],
                November = evapotranspirationNormals[Months.November],
                December = evapotranspirationNormals[Months.December],
            };

            return evapotranspiration;
        }

        /// <summary>
        /// get precipitation for custom timeframe
        /// </summary>
        /// <param name="customClimateData">the daily climate data needed to calc the Evapotranspiration data</param>
        /// <param name="customTimeFrameStart">beginning of custom time frame</param>
        /// <param name="customTimeFrameEnd">end of custom time frame</param>
        /// <returns>PrecipitationData</returns>
        public PrecipitationData GetPrecipitationDataByDailyValues(List<DailyClimateData> customClimateData, int customTimeFrameStart, int customTimeFrameEnd)
        {
            var dictionary = this.GetNormalsForAllTwelveMonths(customClimateData, customTimeFrameStart, customTimeFrameEnd);
            var precipitationNormals = dictionary[MonthlyNormalTypes.precipitation];
            var precipitationData = new PrecipitationData()
            {
                January = precipitationNormals[Months.January],
                February = precipitationNormals[Months.February],
                March = precipitationNormals[Months.March],
                April = precipitationNormals[Months.April],
                May = precipitationNormals[Months.May],
                June = precipitationNormals[Months.June],
                July = precipitationNormals[Months.July],
                August = precipitationNormals[Months.August],
                September = precipitationNormals[Months.September],
                October = precipitationNormals[Months.October],
                November = precipitationNormals[Months.November],
                December = precipitationNormals[Months.December],
            };

            return precipitationData;
        }

        /// <summary>
        /// get precipitation for a specified timeframe
        /// </summary>
        /// <param name="customClimateData">the daily climate data needed to calc the Evapotranspiration data</param>
        /// <param name="timeFrame">the time frame to calculate for</param>
        /// <returns>PrecipitationData</returns>
        public PrecipitationData GetPrecipitationDataByDailyValues(List<DailyClimateData> customClimateData, TimeFrame timeFrame)
        {
            var dictionary = this.GetNormalsForAllTwelveMonths(customClimateData, timeFrame);
            var precipitationNormals = dictionary[MonthlyNormalTypes.precipitation];
            var precipitationData = new PrecipitationData()
            {
                January = precipitationNormals[Months.January],
                February = precipitationNormals[Months.February],
                March = precipitationNormals[Months.March],
                April = precipitationNormals[Months.April],
                May = precipitationNormals[Months.May],
                June = precipitationNormals[Months.June],
                July = precipitationNormals[Months.July],
                August = precipitationNormals[Months.August],
                September = precipitationNormals[Months.September],
                October = precipitationNormals[Months.October],
                November = precipitationNormals[Months.November],
                December = precipitationNormals[Months.December],
            };

            return precipitationData;
        }

        /// <summary>
        /// get temperature data for custom timeframe
        /// </summary>
        /// <param name="customClimateData">the daily climate data needed to calc the Evapotranspiration data</param>
        /// <param name="customTimeFrameStart">beginning of custom time frame</param>
        /// <param name="customTimeFrameEnd">end of custom time frame</param>
        /// <returns>TemperatureData</returns>
        public TemperatureData GetTemperatureDataByDailyValues(List<DailyClimateData> customClimateData, int customTimeFrameStart, int customTimeFrameEnd)
        {
            var dictionary = this.GetNormalsForAllTwelveMonths(customClimateData, customTimeFrameStart, customTimeFrameEnd);
            var temperatureNormals = dictionary[MonthlyNormalTypes.temperature];
            var temperatureData = new TemperatureData()
            {
                January = temperatureNormals[Months.January],
                February = temperatureNormals[Months.February],
                March = temperatureNormals[Months.March],
                April = temperatureNormals[Months.April],
                May = temperatureNormals[Months.May],
                June = temperatureNormals[Months.June],
                July = temperatureNormals[Months.July],
                August = temperatureNormals[Months.August],
                September = temperatureNormals[Months.September],
                October = temperatureNormals[Months.October],
                November = temperatureNormals[Months.November],
                December = temperatureNormals[Months.December],
            };

            return temperatureData;
        }

        /// <summary>
        /// get temperature data for a timeframe
        /// </summary>
        /// <param name="customClimateData">the daily climate data needed to calc the Evapotranspiration data</param>
        /// <param name="timeFrame">the time frame to calculate for</param>
        /// <returns>TemperatureData</returns>
        public TemperatureData GetTemperatureDataByDailyValues(List<DailyClimateData> customClimateData, TimeFrame timeFrame)
        {
            var dictionary = this.GetNormalsForAllTwelveMonths(customClimateData, timeFrame);
            var temperatureNormals = dictionary[MonthlyNormalTypes.temperature];
            var temperatureData = new TemperatureData()
            {
                January = temperatureNormals[Months.January],
                February = temperatureNormals[Months.February],
                March = temperatureNormals[Months.March],
                April = temperatureNormals[Months.April],
                May = temperatureNormals[Months.May],
                June = temperatureNormals[Months.June],
                July = temperatureNormals[Months.July],
                August = temperatureNormals[Months.August],
                September = temperatureNormals[Months.September],
                October = temperatureNormals[Months.October],
                November = temperatureNormals[Months.November],
                December = temperatureNormals[Months.December],
            };

            return temperatureData;
        }

        /// <summary>
        /// get normals for all twelve months for a custom time frame
        /// </summary>
        /// <param name="customClimateDatas">the daily climate data from which to calculate the normals</param>
        /// <param name="customTimeFrameStart">beginning of the timeframe</param>
        /// <param name="customTimeFrameEnd">end of the time frame</param>
        /// <returns>A dicitionary keyed with the data type (precipitation, temperature) to a dictionary mapping month to normal values</returns>
        public Dictionary<MonthlyNormalTypes, Dictionary<Months, double>> GetNormalsForAllTwelveMonths(List<DailyClimateData> customClimateDatas, int customTimeFrameStart, int customTimeFrameEnd)
        {
            int startYear = customTimeFrameStart;
            int endYear = customTimeFrameEnd;

            //Precipitation} 
            var precipNormal = new Dictionary<Months, double>();
            var listOfPrecips = new List<SpecificClimateValueType>();

            //Temperature 
            var temperatureNormals = new Dictionary<Months, double>();
            var listOfTemperatures = new List<SpecificClimateValueType>();

            //Evapotranspiration 
            var evapotransNormals = new Dictionary<Months, double>();
            var listOfEvapotrans = new List<SpecificClimateValueType>();

            foreach (var item in customClimateDatas)
            {
                //precip values
                var precip = new SpecificClimateValueType()
                {
                    ClimateNormalType = ClimateType.Precipitation,
                    JulianDay = item.JulianDay,
                    ClimateValue = item.MeanDailyPrecipitation,
                    Year = item.Year,
                };
                listOfPrecips.Add(precip);

                //temperature values
                var temperature = new SpecificClimateValueType()
                {
                    ClimateNormalType = ClimateType.Temperature,
                    JulianDay = item.JulianDay,
                    ClimateValue = item.MeanDailyAirTemperature,
                    Year = item.Year,
                };
                listOfTemperatures.Add(temperature);

                //evapotrans value
                var evapotrans = new SpecificClimateValueType()
                {
                    ClimateNormalType = ClimateType.Evapotranspiration,
                    JulianDay = item.JulianDay,
                    ClimateValue = item.MeanDailyPET,
                    Year = item.Year,
                };
                listOfEvapotrans.Add(evapotrans);
            }

            //Get the normals for each month 
            foreach (var months in Enum.GetValues(typeof(Months)).Cast<Months>())
            {
                var m = GetStartandEndJulianDays(months);

                var precip = GetClimateNormalValues(startYear, endYear, m.Item1, m.Item2, listOfPrecips);
                precipNormal.Add(months, precip);

                var temperature = GetClimateNormalValues(startYear, endYear, m.Item1, m.Item2, listOfTemperatures);
                temperatureNormals.Add(months, temperature);

                var evapotrans = GetClimateNormalValues(startYear, endYear, m.Item1, m.Item2, listOfEvapotrans);
                evapotransNormals.Add(months, evapotrans);
            }

            var result = new Dictionary<MonthlyNormalTypes, Dictionary<Months, double>>
            {
                { MonthlyNormalTypes.precipitation, precipNormal },
                { MonthlyNormalTypes.temperature, temperatureNormals },
                { MonthlyNormalTypes.evapotranspiration, evapotransNormals }
            };

            return result;
        }

        /// <summary>
        /// Determine each month's normal data based on NASA information (or other custom climate data)
        /// </summary>
        /// <param name="customClimateDatas">Climate information, likely from NASA</param>
        /// <returns>A dicitionary keyed with the data type (precipitation, temperature) to a dictionary mapping month to normal values</returns>
        public Dictionary<MonthlyNormalTypes, Dictionary<Months, double>> GetNormalsForAllTwelveMonths(List<DailyClimateData> customClimateDatas, TimeFrame timeFrame)
        {
            Tuple<int, int> period = this.GetTimeFrame(timeFrame);
            int startYear = period.Item1;
            int endYear = period.Item2;

            //Precipitation 
            var precipNormal = new Dictionary<Months, double>();
            var listOfPrecips = new List<SpecificClimateValueType>();

            //Temperature 
            var temperatureNormals = new Dictionary<Months, double>();
            var listOfTemperatures = new List<SpecificClimateValueType>();

            //Evapotranspiration 
            var evapotransNormals = new Dictionary<Months, double>();
            var listOfEvapotrans = new List<SpecificClimateValueType>();

            foreach (var item in customClimateDatas)
            {
                //precip values
                var precip = new SpecificClimateValueType()
                {
                    ClimateNormalType = ClimateType.Precipitation,
                    JulianDay = item.JulianDay,
                    ClimateValue = item.MeanDailyPrecipitation,
                    Year = item.Year,
                };
                listOfPrecips.Add(precip);

                //temperature values
                var temperature = new SpecificClimateValueType()
                {
                    ClimateNormalType = ClimateType.Temperature,
                    JulianDay = item.JulianDay,
                    ClimateValue = item.MeanDailyAirTemperature,
                    Year = item.Year,
                };
                listOfTemperatures.Add(temperature);

                //evapotrans value
                var evapotrans = new SpecificClimateValueType()
                {
                    ClimateNormalType = ClimateType.Evapotranspiration,
                    JulianDay = item.JulianDay,
                    ClimateValue = item.MeanDailyPET,
                    Year = item.Year,
                };
                listOfEvapotrans.Add(evapotrans);
            }

            //Get the normals for each month 
            foreach (var months in Enum.GetValues(typeof(Months)).Cast<Months>())
            {
                var m = GetStartandEndJulianDays(months);

                var precip = GetClimateNormalValues(startYear, endYear, m.Item1, m.Item2, listOfPrecips);
                precipNormal.Add(months, precip);

                var temperature = GetClimateNormalValues(startYear, endYear, m.Item1, m.Item2, listOfTemperatures);
                temperatureNormals.Add(months, temperature);

                var evapotrans = GetClimateNormalValues(startYear, endYear, m.Item1, m.Item2, listOfEvapotrans);
                evapotransNormals.Add(months, evapotrans);
            }

            var result = new Dictionary<MonthlyNormalTypes, Dictionary<Months, double>>
            {
                { MonthlyNormalTypes.precipitation, precipNormal },
                { MonthlyNormalTypes.temperature, temperatureNormals },
                { MonthlyNormalTypes.evapotranspiration, evapotransNormals }
            };

            return result;
        }

        /// <summary>
        /// Equation 1.6.2-6
        /// For a given period of years (for our uses its 30 years) and a specified day range (i.e. a month) you receive a 30 year average of a certain type for that month
        /// </summary>
        /// <param name="startYear">Beginning year of period to measure</param>
        /// <param name="endYear">End year of period to measure</param>
        /// <param name="startMonth">Beginning of the month (Julian Day)</param>
        /// <param name="endMonth">End of the month (Julian Day)</param>
        /// <param name="climateValue">List of values containing the climate normal type you wish to calculate (precipitation, temperature, evapotranspiration) NOTE: ensure the list is entirely comprised of one climate normal type"</param>
        /// <returns>30 year average of daily climate values over a given period, month, and type</returns>
        public double GetClimateNormalValues(int startYear, int endYear, int startMonth, int endMonth, List<SpecificClimateValueType> climateValue)
        {
            //it seems in the base form 1.6.2-6 gives the correct data for temperature since that needs an average daily temperature
            //Precipitation and PET however require a monthly total averaged over the 30 year period
            var outerSum = new List<double>();
            var climateType = climateValue[0].ClimateNormalType;
            for (int i = startYear; i <= endYear; i++)
            {
                //by year
                var climateDataForCurrentYear = climateValue.Where(x => x.Year == i);
                //by month
                var climateDataForCurrentDayRange = climateDataForCurrentYear.Where(y => y.JulianDay >= startMonth && y.JulianDay <= endMonth);
                //sum all climatevalues
                var innerSum = climateDataForCurrentDayRange.Select(z => z.ClimateValue).Sum();
                outerSum.Add(innerSum);
            }
            if (climateType == ClimateType.Temperature)
            {
                var numerator = outerSum.Sum();
                var denominator = ((endMonth - startMonth) * 30);

                var result = numerator / denominator;

                return result;
            }
            else 
            {
                var result = outerSum.Average();

                return result;
            }
        }

        #endregion

        #region Private Methods

        private Tuple<int, int> GetStartandEndJulianDays(Months months)
        {
            switch (months)
            {
                case Months.January:
                    return new Tuple<int, int>(1, 31);
                case Months.February:
                    return new Tuple<int, int>(32, 59);
                case Months.March:
                    return new Tuple<int, int>(60, 90);
                case Months.April:
                    return new Tuple<int, int>(91, 120);
                case Months.May:
                    return new Tuple<int, int>(121, 151);
                case Months.June:
                    return new Tuple<int, int>(152, 181);
                case Months.July:
                    return new Tuple<int, int>(182, 212);
                case Months.August:
                    return new Tuple<int, int>(213, 243);
                case Months.September:
                    return new Tuple<int, int>(244, 273);
                case Months.October:
                    return new Tuple<int, int>(274, 304);
                case Months.November:
                    return new Tuple<int, int>(305, 334);
                case Months.December:
                    return new Tuple<int, int>(335, 365);
                default:
                    throw new Exception($"{months} not found");
            }
        }

        private Tuple<int, int> GetTimeFrame(TimeFrame timeFrame)
        {
            switch (timeFrame)
            {
                case TimeFrame.NineteenEightyToNineteenNinety:
                    return new Tuple<int, int>(1970, 2000);

                case TimeFrame.NineteenNinetyToTwoThousand:
                    return new Tuple<int, int>(1980, 2010);

                case TimeFrame.TwoThousandToCurrent:
                    return new Tuple<int, int>(1990, 2020);

                default:
                    throw new Exception($"{timeFrame} not a valid period");
            }
        }
        #endregion
    }
}