using H.Core.Calculators.Climate;
using H.Core.Tools;
using Newtonsoft.Json.Linq;
using SharpKml.Dom;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TimeSpan = System.TimeSpan;

namespace H.Core.Providers.Climate
{
    /// <summary>
    /// Web viewer
    /// https://power.larc.nasa.gov/data-access-viewer/
    ///
    /// Daily API sample guide 
    /// https://power.larc.nasa.gov/docs/services/api/temporal/daily/
    /// 
    /// Sample request
    /// https://power.larc.nasa.gov/api/pages/?urls.primaryName=Daily
    ///
    /// Parameter definitions:
    /// https://power.larc.nasa.gov/#resources
    ///
    /// All parameters:
    /// https://power.larc.nasa.gov/parameters/
    /// </summary>
    public class NasaClimateProvider
    {
        #region Fields

        private EvapotranspirationCalculator _evapotranspirationCalculator = new EvapotranspirationCalculator();

        // Timeout is in seconds. 60s = 1 minute.
        private const int Timeout = 60;
        private const int DataUndefined = -999;

        #endregion

        #region Constructors

        public NasaClimateProvider()
        {
            HTraceListener.AddTraceListener();
        }

        #endregion

        #region Properties

        public TimeSpan Expiry { get; set; } = TimeSpan.FromMinutes(60);

        /// <summary>
        /// The window used to check the precipitation scale, and how closely the days of it have to agree before the
        /// response is treated as uniformly scaled.
        /// </summary>
        private const string CalibrationStartDate = "20200601";
        private const string CalibrationEndDate = "20200630";
        private const int MinimumCalibrationDays = 3;
        private const double CalibrationAgreement = 0.01;

        private static readonly Dictionary<string, Tuple<DateTime, double>> ScalingFactors = new Dictionary<string, Tuple<DateTime, double>>();
        private static readonly object ScalingFactorsLock = new object();
        public DateTime StartDate { get; set; } = new DateTime(1981, 1, 1);
        public DateTime EndDate { get; set; } = DateTime.Now;

        #endregion

        #region Public Methods        

        public bool IsCached(double latitude, double longitude)
        {
            var path = this.GetCachedPath(latitude, longitude);

            return File.Exists(path);
        }

        public List<DailyClimateData> GetCustomClimateData(double latitude, double longitude)
        {
            var url = GetCorrectApiUrl(latitude, longitude);
            var content = this.GetCachedData(latitude, longitude);
            if (string.IsNullOrWhiteSpace(content))
            {
                Trace.TraceInformation($"Cached climate data for lat: {latitude}, long: {longitude} not found. Downloading now.");

                try
                {
                    // Run a task that forces the NASA API to timeout if the timeout property isn't able to gracefully time out the API call. If the API
                    // does not respond within this time (slow internet connection or API issues), we
                    // switch over to SLC data.
                    var getNasaApi = Task.Run(() => GetNasaApiString(url));
                    if (getNasaApi.Wait(TimeSpan.FromSeconds(Timeout)))
                    {
                        Trace.TraceInformation($"{nameof(NasaClimateProvider)}.{nameof(GetCustomClimateData)}, NASA API Task Status: {getNasaApi.Status}");
                        content = getNasaApi.Result;
                    }
                    else
                    {
                        Trace.TraceError($"{nameof(NasaClimateProvider)}.{nameof(GetCustomClimateData)}, NASA API Task Status: {getNasaApi.Status}");
                        throw new Exception("NASA API couldn't be reached or connection timed out.");
                    }
                }
                catch (Exception e)
                {
                    Trace.TraceInformation($"Could not load data from NASA API. Exception thrown.");
                    Trace.TraceError($"Exception occurred in {nameof(NasaClimateProvider)}.{nameof(GetCustomClimateData)}. Exception message: {e.Message}");
                    Trace.TraceError($"Inner Exception message: {e.InnerException}");
                    Trace.TraceInformation($"Returning SLC Data.");
                    return new List<DailyClimateData>();
                }

                if (string.IsNullOrWhiteSpace(content) == false)
                {
                    this.CacheData(latitude, longitude, content);
                }
                else
                {
                    // For some reason we got an empty string from web call

                    Trace.TraceError($"{nameof(NasaClimateProvider)}: there was an error while trying to download NASA climate data. String was empty.");
                    Trace.TraceInformation($"Returning SLC Data.");
                    return new List<DailyClimateData>();
                }
            }

            Trace.TraceInformation($"{nameof(NasaClimateProvider)}: Processing data received from NASA API.");

            JObject jObject = JObject.Parse(content);

            var featuresValueArray = (JObject)jObject["properties"];
            if (featuresValueArray == null)
            {
                // This can occur when NASA takes the API offline for maintenance
                Trace.TraceInformation($"{nameof(NasaClimateProvider)}: there was an error while trying to download NASA climate data");
                return new List<DailyClimateData>();
            }

            // Precipitation and its enumerator to access the next data
            var rain = (JObject)featuresValueArray["parameter"]["PRECTOTCORR"];
            var rainE = rain.GetEnumerator();

            // Temperature and its enumerator to access the next data
            var temperature = (JObject)featuresValueArray["parameter"]["T2M"];
            var temperatureE = temperature.GetEnumerator();

            // Humidity and its enumerator to access the next data
            var relativeHumidity = (JObject)featuresValueArray["parameter"]["RH2M"];
            var relativeHumidityE = relativeHumidity.GetEnumerator();

            // Radiation and its enumerator to access the next data
            var solarRadiation = (JObject)featuresValueArray["parameter"]["ALLSKY_SFC_SW_DWN"];
            var solarRadiationE = solarRadiation.GetEnumerator();

            // Minimum temperature and its enumerator to access the next data
            var minimumTemperature = (JObject)featuresValueArray["parameter"]["T2M_MIN"];
            var minimumTemperatureE = minimumTemperature.GetEnumerator();

            // Maximum temperature and its enumerator to access the next data
            var maximumTemperature = (JObject)featuresValueArray["parameter"]["T2M_MAX"];
            var maximumTemperatureE = maximumTemperature.GetEnumerator();

            // Creating a temp file for the NASA data

            var customClimateData = new List<DailyClimateData>();

            // NOTE: NASA API does not provide evapotranspiration - this needs to be calculated by caller
            int julian = 1;
            while (rainE.MoveNext() && 
                   temperatureE.MoveNext() && 
                   relativeHumidityE.MoveNext() && 
                   solarRadiationE.MoveNext() &&
                   minimumTemperatureE.MoveNext() &&
                   maximumTemperatureE.MoveNext())
            {
                var data = new DailyClimateData();
                if (rainE.Current.Key.Substring(4, 4) == "0101")
                {
                    julian = 1;
                    data.JulianDay = julian;
                }

                data.Year = int.Parse(rainE.Current.Key.Substring(0, 4));
                data.MeanDailyAirTemperature = (double)temperatureE.Current.Value;
                data.MinimumAirTemperature = (double)minimumTemperatureE.Current.Value;
                data.MaximumAirTemperature = (double)maximumTemperatureE.Current.Value;
                data.MeanDailyPrecipitation = (double)rainE.Current.Value;
                data.RelativeHumidity = (double)relativeHumidityE.Current.Value;
                data.SolarRadiation = (double)solarRadiationE.Current.Value; // Note: Nasa provides this value with units of measurement of MJ/m^2/day which is what the evapotranspiration calculator expects. No conversion is needed here

                if (this.IsValidDailyData(data) == false)
                {
                    continue;
                }

                data.MeanDailyPET = _evapotranspirationCalculator.CalculateReferenceEvapotranspiration(data.MeanDailyAirTemperature, data.SolarRadiation, data.RelativeHumidity);
                data.JulianDay = julian++;

                // Nasa will return -999 for unknown data values, replace with 0 for now until better solution is found
                if (Math.Abs(data.MeanDailyAirTemperature - DataUndefined) < double.Epsilon)
                {
                    data.MeanDailyAirTemperature = 0;
                }

                if (Math.Abs(data.MeanDailyPrecipitation - DataUndefined) < double.Epsilon)
                {
                    data.MeanDailyPrecipitation = 0;
                }

                if (Math.Abs(data.RelativeHumidity - DataUndefined) < double.Epsilon)
                {
                    data.RelativeHumidity = 0;
                }

                data.Date = new DateTime(data.Year, 1, 1).AddDays(data.JulianDay - 1);
                
                customClimateData.Add(data);
            }

            this.CorrectPrecipitationScale(latitude, longitude, customClimateData);

            return customClimateData;
        }

        /// <summary>
        /// Puts the precipitation series back to its true scale when the service has returned it multiplied.
        ///
        /// A request that reaches into the months the service has not finished reanalysing has returned every
        /// precipitation value in the response at twenty four times its true size, historical days included, while
        /// reporting the same units and leaving every other parameter alone. Holos asks for the record up to today,
        /// so its requests fall in that case.
        ///
        /// The factor is measured rather than assumed - see <see cref="CalculatePrecipitationScalingFactor"/> - so
        /// nothing is changed while the service answers consistently, and the correction stops applying by itself
        /// once the service is fixed.
        /// </summary>
        private void CorrectPrecipitationScale(
            double latitude,
            double longitude,
            List<DailyClimateData> customClimateData)
        {
            var factor = this.GetPrecipitationScalingFactor(latitude, longitude, customClimateData);
            if (factor <= 1)
            {
                return;
            }

            Trace.TraceWarning(
                $"{nameof(NasaClimateProvider)}: precipitation for lat: {latitude}, long: {longitude} was returned at " +
                $"{factor} times its true scale and has been divided back down.");

            foreach (var dailyClimateData in customClimateData)
            {
                dailyClimateData.MeanDailyPrecipitation = dailyClimateData.MeanDailyPrecipitation / factor;
            }
        }

        /// <summary>
        /// Asks the service a second time for a short window that ends well inside the settled part of the record,
        /// where the scale has been reliable, and compares those days against the same days in the full response.
        /// </summary>
        private double GetPrecipitationScalingFactor(
            double latitude,
            double longitude,
            List<DailyClimateData> customClimateData)
        {
            lock (ScalingFactorsLock)
            {
                var key = $"{latitude}_{longitude}";
                if (ScalingFactors.TryGetValue(key, out var cached) && DateTime.Now - cached.Item1 < this.Expiry)
                {
                    return cached.Item2;
                }

                var factor = 1.0;

                try
                {
                    var url = this.GetCalibrationApiUrl(latitude, longitude);

                    var getNasaApi = Task.Run(() => GetNasaApiString(url));
                    if (getNasaApi.Wait(TimeSpan.FromSeconds(Timeout)))
                    {
                        var calibrationValues = ParsePrecipitationByDate(getNasaApi.Result);

                        factor = CalculatePrecipitationScalingFactor(calibrationValues, customClimateData);
                    }
                    else
                    {
                        Trace.TraceError(
                            $"{nameof(NasaClimateProvider)}.{nameof(GetPrecipitationScalingFactor)}: the calibration " +
                            "request did not return in time, so the precipitation scale could not be checked.");
                    }
                }
                catch (Exception e)
                {
                    Trace.TraceError(
                        $"{nameof(NasaClimateProvider)}.{nameof(GetPrecipitationScalingFactor)}: the precipitation " +
                        $"scale could not be checked. Exception message: {e.Message}");
                }

                ScalingFactors[key] = new Tuple<DateTime, double>(DateTime.Now, factor);

                return factor;
            }
        }

        /// <summary>
        /// How many times larger the precipitation in a response is than the same days asked for on their own, or 1
        /// when it is the same size, cannot be told, or is smaller.
        ///
        /// Every day in a scaled response has been scaled by the same amount, so the days are required to agree with
        /// each other before anything is divided: a set of ratios that disagree means something other than a uniform
        /// scaling, which this is not entitled to correct. Dry days carry no information and are passed over.
        /// </summary>
        public static double CalculatePrecipitationScalingFactor(
            Dictionary<DateTime, double> calibrationValues,
            List<DailyClimateData> customClimateData)
        {
            if (calibrationValues == null || customClimateData == null)
            {
                return 1;
            }

            var byDate = new Dictionary<DateTime, double>();
            foreach (var dailyClimateData in customClimateData)
            {
                byDate[dailyClimateData.Date] = dailyClimateData.MeanDailyPrecipitation;
            }

            var ratios = new List<double>();
            foreach (var calibrationValue in calibrationValues)
            {
                if (calibrationValue.Value <= 0)
                {
                    continue;
                }

                if (byDate.TryGetValue(calibrationValue.Key, out var valueInResponse) && valueInResponse > 0)
                {
                    ratios.Add(valueInResponse / calibrationValue.Value);
                }
            }

            if (ratios.Count < MinimumCalibrationDays)
            {
                return 1;
            }

            var first = ratios[0];
            foreach (var ratio in ratios)
            {
                if (Math.Abs(ratio - first) > first * CalibrationAgreement)
                {
                    return 1;
                }
            }

            return first < 1 + CalibrationAgreement ? 1 : first;
        }

        /// <summary>
        /// The precipitation values of a response, by the day they belong to.
        /// </summary>
        public static Dictionary<DateTime, double> ParsePrecipitationByDate(string content)
        {
            var result = new Dictionary<DateTime, double>();
            if (string.IsNullOrWhiteSpace(content))
            {
                return result;
            }

            var parameters = (JObject)JObject.Parse(content)["properties"]?["parameter"];
            var precipitation = (JObject)parameters?["PRECTOTCORR"];
            if (precipitation == null)
            {
                return result;
            }

            foreach (var day in precipitation)
            {
                if (DateTime.TryParseExact(day.Key, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                {
                    var value = (double)day.Value;
                    if (Math.Abs(value - DataUndefined) > double.Epsilon)
                    {
                        result[date] = value;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// A month deep inside the settled record, asking for precipitation alone. Chosen in summer so there are wet
        /// days to compare, and left fixed so the window can never drift into the part of the record still being
        /// revised.
        /// </summary>
        private string GetCalibrationApiUrl(double latitude, double longitude)
        {
            var latitudeString = latitude.ToString(Infrastructure.InfrastructureConstants.EnglishCultureInfo.NumberFormat);
            var longitudeString = longitude.ToString(Infrastructure.InfrastructureConstants.EnglishCultureInfo.NumberFormat);

            return "https://power.larc.nasa.gov/api/temporal/daily/point?" +
                   $"parameters=PRECTOTCORR&community=AG&longitude={longitudeString}&latitude={latitudeString}" +
                   $"&start={CalibrationStartDate}&end={CalibrationEndDate}&format=JSON";
        }

        /// <summary>
        /// In 2025, US government was shutdown in October. Data after ~ October 5th is missing all values (we just check solar radiation here since that is sufficient to determine if the data set is missing).
        ///
        /// If solar radiation is undefined, skip this entry as it is needed for PET calculation. When this value is not returned, climate normals will need to be used
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool IsValidDailyData(DailyClimateData data)
        {
            if ((Math.Abs(data.SolarRadiation - (-999)) < double.Epsilon) && data.Year == 2025)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Get a correct api url depending on the user's locale. French users need to tweak the url from using ',' to '.' 
        /// in the coordinate strings to make NASA accept the GET request
        /// </summary>
        /// <param name="latitude">the farm latitude</param>
        /// <param name="longitude">the farm longitude</param>
        /// <returns>a url with English number formatting</returns>
        public string GetCorrectApiUrl(double latitude, double longitude)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            var startDate = this.StartDate.ToString("yyyyMMdd");
            var endDate = this.EndDate.ToString("yyyyMMdd");
            var format = "JSON";
            var baseUrl = @"https://power.larc.nasa.gov";
            var basePath = @"/api/temporal/daily/point?";

            // This will determine what units of measurement for the data
            // AG: Agroclimatology
            var userCommunity = "AG";

            /*
             * All parameters:
             *
             * https://power.larc.nasa.gov/parameters/
             */

            // T2M: temperature at 2 meters (C)
            // T2M_MIN: temperature at 2 meters (C) Minimum
            // T2M_MAX: temperature at 2 meters (C) Maximum
            // PRECTOT: Precipitation Corrected (mm)
            // RH2M: relative humidity @ 2 m.
            // ALLSKY_SFC_SW_DWN: All Sky Insolation Incident on a Horizontal Surface (AKA solar radiation) (MJ/m^2/day *when AG community type is specified)
            var parameters = "T2M,PRECTOTCORR,RH2M,ALLSKY_SFC_SW_DWN,T2M_MIN,T2M_MAX";

            var latitudeString = latitude.ToString(Infrastructure.InfrastructureConstants.EnglishCultureInfo.NumberFormat);
            var longitudeString = longitude.ToString(Infrastructure.InfrastructureConstants.EnglishCultureInfo.NumberFormat);

            var parameterPath = $"parameters={parameters}&community={userCommunity}&longitude={longitudeString}&latitude={latitudeString}&start={startDate}&end={endDate}&format={format}";

            var englishURL = baseUrl + basePath + parameterPath;
            return englishURL;
        }

        private string GetCachedData(double latitude, double longitude)
        {
            var path = GetCachedPath(latitude, longitude);

            if (File.Exists(path))
            {
                var fileInfo = new FileInfo(path);
                var expired = DateTime.Now.Subtract(fileInfo.LastAccessTime) > Expiry;
                if (expired)
                {
                    return string.Empty;
                }
                else
                {
                    fileInfo.LastAccessTime = DateTime.Now;
                }

                Trace.TraceInformation($"{nameof(NasaClimateProvider)}: cached data was found.");

                return File.ReadAllText(path);
            }
            else
            {
                return string.Empty;
            }
        }

        private string GetCachedPath(double latitude, double longitude)
        {
            return Path.GetTempPath() + "\\" + $"nasa_climate_data_lat_{latitude}_long_{longitude}";
        }

        /// <summary>
        /// Forgets everything held for a location, so the next request for it goes to the service.
        ///
        /// The response is kept on disk for an hour, and every read of it puts its expiry back - so a location that is
        /// being used never reaches that hour on its own. Asking for the data again is therefore not enough to replace
        /// it; what is held has to be thrown away first.
        /// </summary>
        public void ClearCachedData(double latitude, double longitude)
        {
            lock (ScalingFactorsLock)
            {
                ScalingFactors.Remove($"{latitude}_{longitude}");
            }

            var path = this.GetCachedPath(latitude, longitude);

            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);

                    Trace.TraceInformation($"{nameof(NasaClimateProvider)}: discarded the climate data held for lat: {latitude}, long: {longitude}.");
                }
            }
            catch (Exception e)
            {
                // Leaving it in place only means the next request is served from it, which is where this started.
                Trace.TraceError(
                    $"{nameof(NasaClimateProvider)}.{nameof(ClearCachedData)}: could not discard the climate data held " +
                    $"for lat: {latitude}, long: {longitude}. Exception message: {e.Message}");
            }
        }

        private void CacheData(double latitude, double longitude, string content)
        {
            Trace.TraceInformation($"Caching NASA climate data for lat: {latitude}, long: {longitude}");

            var path = this.GetCachedPath(latitude, longitude);

            File.WriteAllText(path, content); // Overwrite any existing file for now
        }

        /// <summary>
        /// Makes a call to the WebClient class to download the API url string. Used to setup a task that runs on a timer to initiate a timeout if the NASA API doesn't respond.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string GetNasaApiString(string url)
        {
            Trace.TraceInformation($"{nameof(NasaClimateProvider)}.{nameof(GetNasaApiString)} : Trying to access NASA API.");
            var webClient = new WebClient();
            string content = string.Empty;

            try
            {
                content = webClient.DownloadString(url);
            }
            catch (Exception e)
            {
                Trace.TraceError($"{nameof(NasaClimateProvider)}.{nameof(GetNasaApiString)}, {e.Message}");
            }

            if (content != string.Empty)
            {
                Trace.TraceInformation($"{nameof(NasaClimateProvider)}.{nameof(GetNasaApiString)} : API content downloaded successfully.");
            }
            else
            {
                Trace.TraceError($"{nameof(NasaClimateProvider)}.{nameof(GetNasaApiString)}, API content could not be downloaded.");
                Trace.TraceError($"{nameof(NasaClimateProvider)}.{nameof(GetNasaApiString)}, API url: {url}");
            }

            return content;
        }

        #endregion
    }
}
