using H.Core.Tools;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using H.Core.Calculators.Climate;
using System.Threading;

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
    /// </summary>
    public class NasaClimateProvider
    {
        #region Fields

        private EvapotranspirationCalculator _evapotranspirationCalculator = new EvapotranspirationCalculator();

        // Timeout is in seconds. 60s = 1 minute.
        private const int Timeout = 60;

        #endregion

        public NasaClimateProvider()
        {
            HTraceListener.AddTraceListener();
        }

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
                        content = getNasaApi.Result;
                    else
                        throw new Exception("NASA API couldn't be reached.");

                }
                catch (Exception e)
                {
                    Trace.TraceInformation($"NASA API: {e.Message}");

                    
                    return new List<DailyClimateData>();
                }

                if (string.IsNullOrWhiteSpace(content) == false)
                {
                    this.CacheData(latitude, longitude, content);
                }
                else
                {
                    // For some reason we got an empty string from web call

                    Trace.TraceInformation($"{nameof(NasaClimateProvider)}: there was an error while trying to download NASA climate data. String was empty.");

                    return new List<DailyClimateData>();
                }
            }

            Trace.TraceInformation($"{nameof(NasaClimateProvider)}: Content downloaded from NASA successfully");

            JObject jObject = JObject.Parse(content);

            var featuresValueArray = (JObject)jObject["properties"];
            if (featuresValueArray == null)
            {
                // This can occur when NASA takes the API offline for maintenance
                Trace.TraceInformation($"{nameof(NasaClimateProvider)}: there was an error while trying to download NASA climate data");
                return new List<DailyClimateData>();
            }

            //Precipitation and its enumerator to access the next data
            var rain = (JObject)featuresValueArray["parameter"]["PRECTOTCORR"];
            var rainE = rain.GetEnumerator();

            //temperature and its enumerator to access the next data
            var temperature = (JObject)featuresValueArray["parameter"]["T2M"];
            var temperatureE = temperature.GetEnumerator();

            // Humidity and its enumerator to access the next data
            var relativeHumidity = (JObject)featuresValueArray["parameter"]["RH2M"];
            var relativeHumidityE = relativeHumidity.GetEnumerator();

            // Radiation and its enumerator to access the next data
            var solarRadiation = (JObject)featuresValueArray["parameter"]["ALLSKY_SFC_SW_DWN"];
            var solarRadiationE = solarRadiation.GetEnumerator();

            //creating a temp file for the nasa data to live

            var customClimateData = new List<DailyClimateData>();

            //NOTE: NASA API does not provide evapotranspiration -this needs to be calculated by caller
            int julian = 1;
            while (rainE.MoveNext() && temperatureE.MoveNext() && relativeHumidityE.MoveNext() && solarRadiationE.MoveNext())
            {
                var data = new DailyClimateData();
                if (rainE.Current.Key.Substring(4, 4) == "0101")
                {
                    julian = 1;
                    data.JulianDay = julian;
                }

                data.Year = int.Parse(rainE.Current.Key.Substring(0, 4));
                data.MeanDailyAirTemperature = (double)temperatureE.Current.Value;
                data.MeanDailyPrecipitation = (double)rainE.Current.Value;
                data.RelativeHumidity = (double)relativeHumidityE.Current.Value;
                data.SolarRadiation = (double)solarRadiationE.Current.Value; // Note: Nasa provides this value with units of measurement of MJ/m^2/day which is what the Evapotranspiration calculator expects. No conversion is needed here
                data.MeanDailyPET = _evapotranspirationCalculator.CalculateReferenceEvapotranspiration(data.MeanDailyAirTemperature, data.SolarRadiation, data.RelativeHumidity);
                data.JulianDay = julian++;

                data.Date = new DateTime(data.Year, 1, 1).AddDays(data.JulianDay - 1);

                customClimateData.Add(data);
            }

            return customClimateData;
        }

        /// <summary>
        /// Get a correct api url depending on the user's locale.  French users need to tweak the url from using ',' to '.' 
        /// in the coordinate strings to make NASA accept the GET request
        /// </summary>
        /// <param name="latitude">the farm latitude</param>
        /// <param name="longitude">the farm longitude</param>
        /// <returns>a url with English number formating</returns>
        public string GetCorrectApiUrl(double latitude, double longitude)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            var startDate = "19810101";
            var endDate = DateTime.Today.ToString("yyyyMMdd");
            var format = "JSON";
            var baseUrl = @"https://power.larc.nasa.gov";
            var basePath = @"/api/temporal/daily/point?";

            // This will determine what units of measurement for the data
            // AG: Agroclimatology
            var userCommunity = "AG";

            // T2M: temperature at 2 meters (C)
            // PRECTOT: Precipitation Corrected (mm)
            // RH2M: relative humidity @ 2 m.
            // ALLSKY_SFC_SW_DWN: All Sky Insolation Incident on a Horizontal Surface (AKA solar radiation) (MJ/m^2/day *when AG community type is specified)
            var parameters = "T2M,PRECTOTCORR,RH2M,ALLSKY_SFC_SW_DWN";

            var latitudeString = latitude.ToString();
            var longitudeString = longitude.ToString();
            if (currentCulture == Infrastructure.InfrastructureConstants.FrenchCultureInfo)
            {
                latitudeString = latitude.ToString(Infrastructure.InfrastructureConstants.EnglishCultureInfo.NumberFormat);
                longitudeString = longitude.ToString(Infrastructure.InfrastructureConstants.EnglishCultureInfo.NumberFormat);
            }

            var parameterPath = $"parameters={parameters}&community={userCommunity}&longitude={longitudeString}&latitude={latitudeString}&start={startDate}&end={endDate}&format={format}";

            var englishURL = baseUrl + basePath + parameterPath;
            return englishURL;
        }

        private string GetCachedData(double latitude, double longitude)
        {
            var path = GetCachedPath(latitude, longitude);

            if (File.Exists(path))
            {
                const int expiryMinutes = 60;
                var fileInfo = new FileInfo(path);
                var expired = DateTime.Now.Subtract(fileInfo.LastAccessTime) > TimeSpan.FromMinutes(expiryMinutes);
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
            var webClient = new WebClient();
            string content = webClient.DownloadString(url);

            return content;
        }

        #endregion
    }
}
