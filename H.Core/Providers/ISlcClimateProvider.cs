using H.Core.Enumerations;
using H.Core.Providers.Climate;
using H.Core.Providers.Evapotranspiration;
using H.Core.Providers.Precipitation;
using H.Core.Providers.Temperature;

namespace H.Core.Providers
{
    public interface ISlcClimateProvider
    {
        bool HasDataForPolygonId(int polygonId, TimeFrame timeFrame);

        ClimateData GetClimateData(int currentPolygonId, TimeFrame timeFrame);

        /// <summary>
        ///     Get precip data for a polygon by timeframe
        /// </summary>
        /// <param name="polygonId">Polygon to get data for</param>
        /// <param name="timeFrame">the time frame to get data for</param>
        /// <returns>PrecipitationData</returns>
        PrecipitationData GetPrecipitationDataByPolygonId(int polygonId, TimeFrame timeFrame);

        /// <summary>
        ///     Get temperature data for a polygon by timeframe
        /// </summary>
        /// <param name="polygonId">Polygon to get data for</param>
        /// <param name="timeFrame">time frame to get data for</param>
        /// <returns>TemperatureData</returns>
        TemperatureData GetTemperatureDataByPolygonId(int polygonId, TimeFrame timeFrame);

        /// <summary>
        ///     Get PET for polygon by timeframe.  If you pass in an invalid timeframe it looks in the original
        ///     ClimateNormalsByPolygon file
        /// </summary>
        /// <param name="polygonId">the polygon to get data for</param>
        /// <param name="timeFrame">the time frame to select data from</param>
        /// <returns>EvapotranspirationData</returns>
        EvapotranspirationData GetEvapotranspirationDataByPolygonId(int polygonId, TimeFrame timeFrame);
    }
}