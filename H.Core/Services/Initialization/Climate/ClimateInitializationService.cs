using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Documents;
using H.Core.Calculators.Climate;
using H.Core.Models;
using H.Core.Providers.Climate;

namespace H.Core.Services.Initialization.Climate
{
    public class ClimateInitializationService : IClimateInitializationService
    {
        #region Fields

        private readonly NasaClimateProvider _nasaClimateProvider = new NasaClimateProvider();
        private readonly CustomFileClimateDataProvider _customFileClimateProvider = new CustomFileClimateDataProvider();
        private readonly ClimateNormalCalculator _climateNormalCalculator = new ClimateNormalCalculator();

        #endregion

        #region Constructors

        #endregion

        #region Public Methods

        public void InitializeClimate(Farm farm)
        {
            var acquisition = farm.ClimateAcquisition;
            List<DailyClimateData> dailyClimateData;

            if (acquisition == Farm.ChosenClimateAcquisition.InputFile)
            {
                dailyClimateData = _customFileClimateProvider.GetDailyClimateData(farm.ClimateDataFileName);
            }
            else
            {
                dailyClimateData = _nasaClimateProvider.GetCustomClimateData(farm.Latitude, farm.Longitude);
            }

            this.InitializeClimate(farm, dailyClimateData);
        }

        /// <summary>
        /// Replaces a farm's climate with a fresh download, and reports whether it did.
        ///
        /// A farm keeps the climate it was built with, so a farm that was given bad data goes on using it however many
        /// times it is opened. This is what puts it right, and it is deliberately something the user asks for: the
        /// farm's results move when the climate underneath them does.
        ///
        /// The two halves are also available separately - <see cref="DownloadClimateData"/> and
        /// <see cref="ReplaceClimate"/> - because the download is slow and touches nothing on the farm, so a caller with
        /// a screen to keep responsive can run it on another thread and replace the climate once it is back.
        /// </summary>
        public bool ReDownloadClimate(Farm farm)
        {
            return this.ReplaceClimate(farm, this.DownloadClimateData(farm));
        }

        /// <summary>
        /// A fresh copy of the climate for a farm's location, or an empty list when the service could not be reached.
        /// Changes nothing on the farm.
        ///
        /// The provider holds the last response for a location and renews it on every read, so asking again would be
        /// answered from the copy being replaced. What it holds is discarded first.
        /// </summary>
        public List<DailyClimateData> DownloadClimateData(Farm farm)
        {
            if (farm == null)
            {
                return new List<DailyClimateData>();
            }

            _nasaClimateProvider.ClearCachedData(farm.Latitude, farm.Longitude);

            return _nasaClimateProvider.GetCustomClimateData(farm.Latitude, farm.Longitude);
        }

        /// <summary>
        /// Replaces a farm's daily climate with the series given and recalculates its normals. Returns false, and leaves
        /// the farm exactly as it was, when there is nothing to replace it with - a failed download comes back empty, and
        /// clearing first would leave the farm with no climate at all.
        ///
        /// The farm's daily records are added to rather than replaced by <see cref="InitializeClimate(Farm, IEnumerable{DailyClimateData})"/>,
        /// so they are cleared here first. Left in place, the farm would hold two of every day and report twice the
        /// rainfall.
        /// </summary>
        public bool ReplaceClimate(Farm farm, List<DailyClimateData> dailyClimateData)
        {
            if (farm == null || dailyClimateData == null || dailyClimateData.Any() == false)
            {
                return false;
            }

            farm.ClimateData.DailyClimateData.Clear();

            this.InitializeClimate(farm, dailyClimateData);

            return true;
        }

        public void InitializeClimate(Farm farm, int startYear, int endYear)
        {
            var dailyClimateData = _nasaClimateProvider.GetCustomClimateData(farm.Latitude, farm.Longitude);
            var climateForPeriod = dailyClimateData.Where(x => x.Date.Year >= startYear && x.Date.Year <= endYear).ToList();

            farm.ClimateData.DailyClimateData.AddRange(climateForPeriod);

            this.SetClimateNormals(farm, climateForPeriod);
        }

        public void InitializeClimate(Farm farm, IEnumerable<DailyClimateData> dailyData)
        {
            var dailyDataList = dailyData.ToList();

            farm.ClimateData.DailyClimateData.AddRange(dailyDataList);

            this.SetClimateNormals(farm, dailyDataList);
        }

        public void SetClimateNormals(Farm farm, IEnumerable<DailyClimateData> climateForPeriod)
        {
            var climateList = climateForPeriod.ToList();

            var startYear = climateList.Min(x => x.Date.Year);
            var endYear = climateList.Max(x => x.Date.Year);

            var temperatureNormals = _climateNormalCalculator.GetTemperatureDataByDailyValues(climateList, startYear, endYear);
            var precipitationNormals = _climateNormalCalculator.GetPrecipitationDataByDailyValues(climateList, startYear, endYear);
            var evapotranspirationNormals = _climateNormalCalculator.GetEvapotranspirationDataByDailyValues(climateList, startYear, endYear);

            farm.ClimateData.EvapotranspirationData = evapotranspirationNormals;
            farm.ClimateData.PrecipitationData = precipitationNormals;
            farm.ClimateData.TemperatureData = temperatureNormals;
        }

        #endregion

        #region Private Methods



        #endregion
    }
}