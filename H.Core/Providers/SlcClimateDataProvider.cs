#region Imports

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Content;
using H.Core.Enumerations;
using H.Core.Providers.Climate;
using H.Core.Providers.Evapotranspiration;
using H.Core.Providers.Precipitation;
using H.Core.Providers.Temperature;
using H.Core.Tools;
using H.Infrastructure;

#endregion

namespace H.Core.Providers
{
    /// <summary>
    /// </summary>
    public class SlcClimateDataProvider : ProviderBase
    {
        // with the added SLC data this new private class is necessary so that we can simplify data processing and get rid of anonymous types which cannot be passed out of functions
        private class SlcIntermediateClimateData
        {
            public int PolygonId { get; set; }
            public int Month { get; set; }
            public double AverageTemperature { get; set; }
            public double Precipitation { get; set; }
            public double PotentialEvapotranspiration { get; set; }
        }

        #region Fields

        private readonly List<TemperatureData> _temperatureDataList;
        private readonly List<PrecipitationData> _precipitationDataList;
        private readonly List<EvapotranspirationData> _evapotranspirationDataList;

        //a dictionary keyed by the time, mapped to temp, precip, and PET for that timeframe
        private Dictionary<TimeFrame, Tuple<List<TemperatureData>, List<PrecipitationData>, List<EvapotranspirationData>>> SlcListsGroupedByTimeFrame = new Dictionary<TimeFrame, Tuple<List<TemperatureData>, List<PrecipitationData>, List<EvapotranspirationData>>>();

        //lists for the appropriate timeframe these will get handed to 'SlcListsGroupedByTimeFrame' in 'BuildListsGroupedByTimeFrame'
        private readonly List<TemperatureData> _temperatureData1950_1980List;
        private readonly List<TemperatureData> _temperatureData1960_1990List;
        private readonly List<TemperatureData> _temperatureData1970_2000List;
        private readonly List<TemperatureData> _temperatureData1980_2010List;
        private readonly List<TemperatureData> _temperatureData1990_2017List;

        private readonly List<PrecipitationData> _precipitationData1950_1980List;
        private readonly List<PrecipitationData> _precipitationData1960_1990List;
        private readonly List<PrecipitationData> _precipitationData1970_2000List;
        private readonly List<PrecipitationData> _precipitationData1980_2010List;
        private readonly List<PrecipitationData> _precipitationData1990_2017List;

        private readonly List<EvapotranspirationData> _evapotranspirationData1950_1980List;
        private readonly List<EvapotranspirationData> _evapotranspirationData1960_1990List;
        private readonly List<EvapotranspirationData> _evapotranspirationData1970_2000List;
        private readonly List<EvapotranspirationData> _evapotranspirationData1980_2010List;
        private readonly List<EvapotranspirationData> _evapotranspirationData1990_2017List;
        #endregion

        #region Constructors

        public SlcClimateDataProvider()
        {
            HTraceListener.AddTraceListener();

            _temperatureDataList = new List<TemperatureData>();
            _precipitationDataList = new List<PrecipitationData>();
            _evapotranspirationDataList = new List<EvapotranspirationData>();

            _temperatureData1950_1980List = new List<TemperatureData>();
            _temperatureData1960_1990List = new List<TemperatureData>();
            _temperatureData1970_2000List = new List<TemperatureData>();
            _temperatureData1980_2010List = new List<TemperatureData>();
            _temperatureData1990_2017List = new List<TemperatureData>();

            _precipitationData1950_1980List = new List<PrecipitationData>();
            _precipitationData1960_1990List = new List<PrecipitationData>();
            _precipitationData1970_2000List = new List<PrecipitationData>();
            _precipitationData1980_2010List = new List<PrecipitationData>();
            _precipitationData1990_2017List = new List<PrecipitationData>();

            _evapotranspirationData1950_1980List = new List<EvapotranspirationData>();
            _evapotranspirationData1960_1990List = new List<EvapotranspirationData>();
            _evapotranspirationData1970_2000List = new List<EvapotranspirationData>();
            _evapotranspirationData1980_2010List = new List<EvapotranspirationData>();
            _evapotranspirationData1990_2017List = new List<EvapotranspirationData>();

            this.ReadFiles();
        }

        #endregion

        #region Properties

        #endregion

        #region Public Methods

        public bool HasDataForPolygonId(int polygonId, TimeFrame timeFrame)
        {
            if (timeFrame == TimeFrame.ProjectionPeriod ||
                timeFrame == TimeFrame.TwoThousandToCurrent) return true;

            return SlcListsGroupedByTimeFrame[timeFrame].Item1.Any(x => x.PolygonId == polygonId) &&
                   SlcListsGroupedByTimeFrame[timeFrame].Item2.Any(x => x.PolygonId == polygonId) &&
                   SlcListsGroupedByTimeFrame[timeFrame].Item3.Any(x => x.PolygonId == polygonId);
        }

        public ClimateData GetClimateData(int currentPolygonId, TimeFrame timeFrame)
        {
            var polygonId = currentPolygonId;
            if (!HasDataForPolygonId(polygonId, timeFrame))
            {
                //we don't have climate data for the polygon
                Trace.TraceInformation($"{nameof(SlcClimateDataProvider)}.{nameof(GetClimateData)}: {currentPolygonId} is not associated with any SLC climate data. Asking user for another polygon to get data from.");
                return new ClimateData();
            }
            var evapotranspirationData = this.GetEvapotranspirationDataByPolygonId(polygonId, timeFrame);
            var precipitationData = this.GetPrecipitationDataByPolygonId(polygonId, timeFrame);
            var temperatureData = this.GetTemperatureDataByPolygonId(polygonId, timeFrame);

            return new ClimateData()
            {
                EvapotranspirationData = evapotranspirationData,
                PrecipitationData = precipitationData,
                TemperatureData = temperatureData,
            };
        }

        /// <summary>
        /// Get PET for polygon by timeframe.  If you pass in an invalid timeframe it looks in the original ClimateNormalsByPolygon file
        /// </summary>
        /// <param name="polygonId">the polygon to get data for</param>
        /// <param name="timeFrame">the time frame to select data from</param>
        /// <returns>EvapotranspirationData</returns>
        public EvapotranspirationData GetEvapotranspirationDataByPolygonId(int polygonId, TimeFrame timeFrame)
        {
            EvapotranspirationData result;
            //get the appropriate data for the timeframe
            switch (timeFrame)
            {
                case TimeFrame.NineteenFiftyToNineteenEighty:
                    result = SlcListsGroupedByTimeFrame[timeFrame].Item3.SingleOrDefault(x => x.PolygonId == polygonId);
                    break;
                case TimeFrame.NineteenSixtyToNineteenNinety:
                    result = SlcListsGroupedByTimeFrame[timeFrame].Item3.SingleOrDefault(x => x.PolygonId == polygonId);
                    break;
                case TimeFrame.NineteenSeventyToTwoThousand:
                    result = SlcListsGroupedByTimeFrame[timeFrame].Item3.SingleOrDefault(x => x.PolygonId == polygonId);
                    break;
                case TimeFrame.NineteenEightyToTwoThousandTen:
                    result = SlcListsGroupedByTimeFrame[timeFrame].Item3.SingleOrDefault(x => x.PolygonId == polygonId);
                    break;
                case TimeFrame.NineteenNinetyToTwoThousandSeventeen:
                    result = SlcListsGroupedByTimeFrame[timeFrame].Item3.SingleOrDefault(x => x.PolygonId == polygonId);
                    break;
                default:
                    Trace.TraceError($"{nameof(SlcClimateDataProvider)}.{nameof(GetEvapotranspirationDataByPolygonId)}: No SLC normals for '{timeFrame}' found. Defaulting to original SLC data.");
                    result = _evapotranspirationDataList.SingleOrDefault(x => x.PolygonId == polygonId);
                    break;
            }
            if (result != null)
            {
                return result;
            }

            Trace.TraceError($"{nameof(SlcClimateDataProvider)}.{nameof(GetEvapotranspirationDataByPolygonId)}. Evapotranspiration not found for polygon '{polygonId}'. Returning 0 for all values.");

            return new EvapotranspirationData();
        }

        /// <summary>
        /// Get precip data for a polygon by timeframe
        /// </summary>
        /// <param name="polygonId">Polygon to get data for</param>
        /// <param name="timeFrame">the time frame to get data for</param>
        /// <returns>PrecipitationData</returns>
        public PrecipitationData GetPrecipitationDataByPolygonId(int polygonId, TimeFrame timeFrame)
        {
            PrecipitationData result;
            switch (timeFrame)
            {
                case TimeFrame.NineteenFiftyToNineteenEighty:
                    result = SlcListsGroupedByTimeFrame[timeFrame].Item2.SingleOrDefault(x => x.PolygonId == polygonId);
                    break;
                case TimeFrame.NineteenSixtyToNineteenNinety:
                    result = SlcListsGroupedByTimeFrame[timeFrame].Item2.SingleOrDefault(x => x.PolygonId == polygonId);
                    break;
                case TimeFrame.NineteenSeventyToTwoThousand:
                    result = SlcListsGroupedByTimeFrame[timeFrame].Item2.SingleOrDefault(x => x.PolygonId == polygonId);
                    break;
                case TimeFrame.NineteenEightyToTwoThousandTen:
                    result = SlcListsGroupedByTimeFrame[timeFrame].Item2.SingleOrDefault(x => x.PolygonId == polygonId);
                    break;
                case TimeFrame.NineteenNinetyToTwoThousandSeventeen:
                    result = SlcListsGroupedByTimeFrame[timeFrame].Item2.SingleOrDefault(x => x.PolygonId == polygonId);
                    break;
                default:
                    Trace.TraceError($"{nameof(SlcClimateDataProvider)}.{nameof(GetPrecipitationDataByPolygonId)}: No SLC normals for '{timeFrame}' found. Defaulting to original SLC data.");
                    result = _precipitationDataList.SingleOrDefault(x => x.PolygonId == polygonId);
                    break;
            }
            if (result != null)
            {
                return result;
            }

            Trace.TraceError($"{nameof(SlcClimateDataProvider)}.{nameof(GetPrecipitationDataByPolygonId)}. Precipitation not found for polygon '{polygonId}'. Returning 0 for all values.");

            return new PrecipitationData();
        }

        /// <summary>
        /// Get temperature data for a polygon by timeframe
        /// </summary>
        /// <param name="polygonId">Polygon to get data for</param>
        /// <param name="timeFrame">time frame to get data for</param>
        /// <returns>TemperatureData</returns>
        public TemperatureData GetTemperatureDataByPolygonId(int polygonId, TimeFrame timeFrame)
        {
            TemperatureData result;
            switch (timeFrame)
            {
                case TimeFrame.NineteenFiftyToNineteenEighty:
                    result = SlcListsGroupedByTimeFrame[timeFrame].Item1.SingleOrDefault(x => x.PolygonId == polygonId);
                    break;
                case TimeFrame.NineteenSixtyToNineteenNinety:
                    result = SlcListsGroupedByTimeFrame[timeFrame].Item1.SingleOrDefault(x => x.PolygonId == polygonId);
                    break;
                case TimeFrame.NineteenSeventyToTwoThousand:
                    result = SlcListsGroupedByTimeFrame[timeFrame].Item1.SingleOrDefault(x => x.PolygonId == polygonId);
                    break;
                case TimeFrame.NineteenEightyToTwoThousandTen:
                    result = SlcListsGroupedByTimeFrame[timeFrame].Item1.SingleOrDefault(x => x.PolygonId == polygonId);
                    break;
                case TimeFrame.NineteenNinetyToTwoThousandSeventeen:
                    result = SlcListsGroupedByTimeFrame[timeFrame].Item1.SingleOrDefault(x => x.PolygonId == polygonId);
                    break;
                default:
                    Trace.TraceError($"{nameof(SlcClimateDataProvider)}.{nameof(GetTemperatureDataByPolygonId)}: No SLC normals for '{timeFrame}' found. Defaulting to original SLC data.");
                    result = _temperatureDataList.SingleOrDefault(x => x.PolygonId == polygonId);
                    break;
            }
            if (result != null)
            {
                return result;
            }
            else
            {
                Trace.TraceError($"{nameof(SlcClimateDataProvider)}.{nameof(GetTemperatureDataByPolygonId)}. Temperature not found for polygon '{polygonId}'. Returning 0 for all values.");

                return new TemperatureData();
            }
        }
        #endregion

        #region Private Methods
        private void BuildSlcListsGroupedByTimeFrame()
        {
            SlcListsGroupedByTimeFrame[TimeFrame.NineteenFiftyToNineteenEighty] = new Tuple<List<TemperatureData>, List<PrecipitationData>, List<EvapotranspirationData>>(_temperatureData1950_1980List, _precipitationData1950_1980List, _evapotranspirationData1950_1980List);
            SlcListsGroupedByTimeFrame[TimeFrame.NineteenSixtyToNineteenNinety] = new Tuple<List<TemperatureData>, List<PrecipitationData>, List<EvapotranspirationData>>(_temperatureData1960_1990List, _precipitationData1960_1990List, _evapotranspirationData1960_1990List);
            SlcListsGroupedByTimeFrame[TimeFrame.NineteenSeventyToTwoThousand] = new Tuple<List<TemperatureData>, List<PrecipitationData>, List<EvapotranspirationData>>(_temperatureData1970_2000List, _precipitationData1970_2000List, _evapotranspirationData1970_2000List);
            SlcListsGroupedByTimeFrame[TimeFrame.NineteenEightyToTwoThousandTen] = new Tuple<List<TemperatureData>, List<PrecipitationData>, List<EvapotranspirationData>>(_temperatureData1980_2010List, _precipitationData1980_2010List, _evapotranspirationData1980_2010List);
            SlcListsGroupedByTimeFrame[TimeFrame.NineteenNinetyToTwoThousandSeventeen] = new Tuple<List<TemperatureData>, List<PrecipitationData>, List<EvapotranspirationData>>(_temperatureData1990_2017List, _precipitationData1990_2017List, _evapotranspirationData1990_2017List);
        }
        private void ReadFiles()
        {
            //add the timeframe lists to the dictionary
            BuildSlcListsGroupedByTimeFrame();

            //read the info from file
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var fileLines1950 = CsvResourceReader.GetFileLines(CsvResourceNames.ClimateNormalsByPolygon1950_1980).Skip(1).ToList();
            var fileLines1960 = CsvResourceReader.GetFileLines(CsvResourceNames.ClimateNormalsByPolygon1960_1990).Skip(1).ToList();
            var fileLines1970 = CsvResourceReader.GetFileLines(CsvResourceNames.ClimateNormalsByPolygon1970_2000).Skip(1).ToList();
            var fileLines1980 = CsvResourceReader.GetFileLines(CsvResourceNames.ClimateNormalsByPolygon1980_2010).Skip(1).ToList();
            var fileLines1990 = CsvResourceReader.GetFileLines(CsvResourceNames.ClimateNormalsByPolygon1990_2017).Skip(1).ToList();

            // the old file to read
            var originalSLCFileLines = CsvResourceReader.GetFileLines(CsvResourceNames.ClimateNormalsByPolygon).Skip(1).ToList();

            //the intermediate climate data for each timeframe
            var projections1950 = MakeProjections(cultureInfo, fileLines1950);
            var projections1960 = MakeProjections(cultureInfo, fileLines1960);
            var projections1970 = MakeProjections(cultureInfo, fileLines1970);
            var projections1980 = MakeProjections(cultureInfo, fileLines1980);
            var projections1990 = MakeProjections(cultureInfo, fileLines1990);

            //intermediate climate data for the original SLC file
            var oldProjections = MakeOldProjections(cultureInfo, originalSLCFileLines);


            //all the data grouped by a polygonId for each timeframe
            var projectionsGroupedByPolygonId1950 = projections1950.GroupBy(x => x.PolygonId);
            var projectionsGroupedByPolygonId1960 = projections1960.GroupBy(x => x.PolygonId);
            var projectionsGroupedByPolygonId1970 = projections1970.GroupBy(x => x.PolygonId);
            var projectionsGroupedByPolygonId1980 = projections1980.GroupBy(x => x.PolygonId);
            var projectionsGroupedByPolygonId1990 = projections1990.GroupBy(x => x.PolygonId);
            var projectionsGroupedByPolygonId = oldProjections.GroupBy(x => x.PolygonId);


            //creating the climate data (TemperatureData, PrecipitationData, and EvapotranspirationData)

            // 1950-1980
            PopulateListsByTimeFrame(TimeFrame.NineteenFiftyToNineteenEighty, projectionsGroupedByPolygonId1950);

            // 1960-1990
            PopulateListsByTimeFrame(TimeFrame.NineteenSixtyToNineteenNinety, projectionsGroupedByPolygonId1960);

            // 1970-2000
            PopulateListsByTimeFrame(TimeFrame.NineteenSeventyToTwoThousand, projectionsGroupedByPolygonId1970);

            // 1980-2010
            PopulateListsByTimeFrame(TimeFrame.NineteenEightyToTwoThousandTen, projectionsGroupedByPolygonId1980);

            // 1990-2017
            PopulateListsByTimeFrame(TimeFrame.NineteenNinetyToTwoThousandSeventeen, projectionsGroupedByPolygonId1990);

            //populate the original lists related to the old normals file
            PopulateOriginalClimateLists(projectionsGroupedByPolygonId);

            this.IsInitialized = true;

            Trace.TraceInformation($"{nameof(SlcClimateDataProvider)} has been initialized.");
        }

        /// <summary>
        /// populate the lists of normals (temperature, precip, evapotranspiration) connected to the old climate file not bound by a specific timeframe
        /// </summary>
        /// <param name="projectionsGroupedByPolygonId">intermediate climate data grouped by polygon id</param>
        private void PopulateOriginalClimateLists(IEnumerable<IGrouping<int, SlcIntermediateClimateData>> projectionsGroupedByPolygonId)
        {
            foreach (var grouping in projectionsGroupedByPolygonId)
            {
                var polygonId = grouping.Key;

                var temperatureData = new TemperatureData
                {
                    PolygonId = polygonId,
                    January = grouping.ElementAt(0).AverageTemperature,
                    February = grouping.ElementAt(1).AverageTemperature,
                    March = grouping.ElementAt(2).AverageTemperature,
                    April = grouping.ElementAt(3).AverageTemperature,
                    May = grouping.ElementAt(4).AverageTemperature,
                    June = grouping.ElementAt(5).AverageTemperature,
                    July = grouping.ElementAt(6).AverageTemperature,
                    August = grouping.ElementAt(7).AverageTemperature,
                    September = grouping.ElementAt(8).AverageTemperature,
                    October = grouping.ElementAt(9).AverageTemperature,
                    November = grouping.ElementAt(10).AverageTemperature,
                    December = grouping.ElementAt(11).AverageTemperature
                };

                //temperature list
                _temperatureDataList.Add(temperatureData);

                var precipitationData = new PrecipitationData
                {
                    PolygonId = polygonId,
                    January = grouping.ElementAt(0).Precipitation,
                    February = grouping.ElementAt(1).Precipitation,
                    March = grouping.ElementAt(2).Precipitation,
                    April = grouping.ElementAt(3).Precipitation,
                    May = grouping.ElementAt(4).Precipitation,
                    June = grouping.ElementAt(5).Precipitation,
                    July = grouping.ElementAt(6).Precipitation,
                    August = grouping.ElementAt(7).Precipitation,
                    September = grouping.ElementAt(8).Precipitation,
                    October = grouping.ElementAt(9).Precipitation,
                    November = grouping.ElementAt(10).Precipitation,
                    December = grouping.ElementAt(11).Precipitation
                };

                //precipitation list
                _precipitationDataList.Add(precipitationData);

                var evapotranspirationData = new EvapotranspirationData
                {
                    PolygonId = polygonId,
                    January = grouping.ElementAt(0).PotentialEvapotranspiration,
                    February = grouping.ElementAt(1).PotentialEvapotranspiration,
                    March = grouping.ElementAt(2).PotentialEvapotranspiration,
                    April = grouping.ElementAt(3).PotentialEvapotranspiration,
                    May = grouping.ElementAt(4).PotentialEvapotranspiration,
                    June = grouping.ElementAt(5).PotentialEvapotranspiration,
                    July = grouping.ElementAt(6).PotentialEvapotranspiration,
                    August = grouping.ElementAt(7).PotentialEvapotranspiration,
                    September = grouping.ElementAt(8).PotentialEvapotranspiration,
                    October = grouping.ElementAt(9).PotentialEvapotranspiration,
                    November = grouping.ElementAt(10).PotentialEvapotranspiration,
                    December = grouping.ElementAt(11).PotentialEvapotranspiration
                };


                //PET list
                _evapotranspirationDataList.Add(evapotranspirationData);
            }
        }

        /// <summary>
        /// Create the Temp, precip and evap data for each list of each timeframe. Thes are the climate normals
        /// </summary>
        /// <param name="timeFrame">time frame to determine which list to populate</param>
        /// <param name="projectionsGroupedByPolygonId">list of intermediate climate data grouped by polygonId</param>
        private void PopulateListsByTimeFrame(TimeFrame timeFrame, IEnumerable<IGrouping<int, SlcIntermediateClimateData>> projectionsGroupedByPolygonId)
        {
            foreach (var grouping in projectionsGroupedByPolygonId)
            {
                var polygonId = grouping.Key;

                var temperatureData = new TemperatureData
                {
                    PolygonId = polygonId,
                    January = grouping.ElementAt(0).AverageTemperature,
                    February = grouping.ElementAt(1).AverageTemperature,
                    March = grouping.ElementAt(2).AverageTemperature,
                    April = grouping.ElementAt(3).AverageTemperature,
                    May = grouping.ElementAt(4).AverageTemperature,
                    June = grouping.ElementAt(5).AverageTemperature,
                    July = grouping.ElementAt(6).AverageTemperature,
                    August = grouping.ElementAt(7).AverageTemperature,
                    September = grouping.ElementAt(8).AverageTemperature,
                    October = grouping.ElementAt(9).AverageTemperature,
                    November = grouping.ElementAt(10).AverageTemperature,
                    December = grouping.ElementAt(11).AverageTemperature
                };

                //temperature list
                SlcListsGroupedByTimeFrame[timeFrame].Item1.Add(temperatureData);

                var precipitationData = new PrecipitationData
                {
                    PolygonId = polygonId,
                    January = grouping.ElementAt(0).Precipitation,
                    February = grouping.ElementAt(1).Precipitation,
                    March = grouping.ElementAt(2).Precipitation,
                    April = grouping.ElementAt(3).Precipitation,
                    May = grouping.ElementAt(4).Precipitation,
                    June = grouping.ElementAt(5).Precipitation,
                    July = grouping.ElementAt(6).Precipitation,
                    August = grouping.ElementAt(7).Precipitation,
                    September = grouping.ElementAt(8).Precipitation,
                    October = grouping.ElementAt(9).Precipitation,
                    November = grouping.ElementAt(10).Precipitation,
                    December = grouping.ElementAt(11).Precipitation
                };

                //precipitation list
                SlcListsGroupedByTimeFrame[timeFrame].Item2.Add(precipitationData);

                var evapotranspirationData = new EvapotranspirationData
                {
                    PolygonId = polygonId,
                    January = grouping.ElementAt(0).PotentialEvapotranspiration,
                    February = grouping.ElementAt(1).PotentialEvapotranspiration,
                    March = grouping.ElementAt(2).PotentialEvapotranspiration,
                    April = grouping.ElementAt(3).PotentialEvapotranspiration,
                    May = grouping.ElementAt(4).PotentialEvapotranspiration,
                    June = grouping.ElementAt(5).PotentialEvapotranspiration,
                    July = grouping.ElementAt(6).PotentialEvapotranspiration,
                    August = grouping.ElementAt(7).PotentialEvapotranspiration,
                    September = grouping.ElementAt(8).PotentialEvapotranspiration,
                    October = grouping.ElementAt(9).PotentialEvapotranspiration,
                    November = grouping.ElementAt(10).PotentialEvapotranspiration,
                    December = grouping.ElementAt(11).PotentialEvapotranspiration
                };


                //PET list
                SlcListsGroupedByTimeFrame[timeFrame].Item3.Add(evapotranspirationData);
            }
        }

        /// <summary>
        /// Create a 'list' of intermediate climate data used to create the final climate data
        /// </summary>
        /// <param name="cultureInfo">the culture info</param>
        /// <param name="fileLines">file contents read from resources</param>
        /// <returns>IEnumberable of the intermediate climate data</returns>
        private IEnumerable<SlcIntermediateClimateData> MakeProjections(System.Globalization.CultureInfo cultureInfo, List<string[]> fileLines)
        {
            return from line in fileLines
                   select new SlcIntermediateClimateData
                   {
                       PolygonId = int.Parse(line.ElementAt(0), cultureInfo),
                       Month = int.Parse(line.ElementAt(1), cultureInfo),
                       AverageTemperature = double.Parse(line.ElementAt(2), cultureInfo),
                       Precipitation = double.Parse(line.ElementAt(3), cultureInfo),
                       PotentialEvapotranspiration = double.Parse(line.ElementAt(4), cultureInfo)
                   };
        }

        //this has to be different than MakeProjections() b/c the original SLC file has 2 extra datapoints called Tmax and Tmin which are located
        //in column 2 and 3 thus offsetting the relevant data that the original file has with the new files from 1950-2017
        private IEnumerable<SlcIntermediateClimateData> MakeOldProjections(System.Globalization.CultureInfo cultureInfo, List<string[]> originalSLCFileLines)
        {
            return from line in originalSLCFileLines
                   select new SlcIntermediateClimateData
                   {
                       PolygonId = int.Parse(line.ElementAt(0), cultureInfo),
                       Month = int.Parse(line.ElementAt(1), cultureInfo),
                       AverageTemperature = double.Parse(line.ElementAt(4), cultureInfo),
                       Precipitation = double.Parse(line.ElementAt(5), cultureInfo),
                       PotentialEvapotranspiration = double.Parse(line.ElementAt(6), cultureInfo)
                   };

        }
        #endregion
    }
}