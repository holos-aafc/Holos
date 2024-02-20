using System.Globalization;
using Mapsui;
using Mapsui.Projections;
using Newtonsoft.Json;

namespace H.Avalonia.Infrastructure
{
    /// <summary>
    /// A set of functions to help facilitate the use of maps and access external APIs that enable features required in map controls.
    /// </summary>
    public class MapHelpers
    {
        private readonly string key = "WFJiS2U2YnVZRjFnRkRNQXc5NzJ+M2RLN2t4WGNsNkxlRVNQUDBybTRHZ35BcGdWNW5FajhRdk0tYW9BVC1nUXVwZXBRNjdSZmJLcFByOU5rQTlWb2FlVXRTcXNuTkdIUXhZUDBuUk5vazJj";
        private readonly string _bingApiKey;
        private readonly HttpClient _httpClient;

        public MapHelpers()
        {
            byte[] data = Convert.FromBase64String(key);
            _bingApiKey = System.Text.Encoding.UTF8.GetString(data);
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Takes in an address representation and returns location coordinates for that address.
        /// </summary>
        /// <param name="address">The address that is converted to location coordinates. The address can be in any form including
        /// full address or certain parts of an address like street, city or zip/postal code</param>
        /// <returns>Returns a tuple that includes the coordinates of the location.</returns>
        /// <exception cref="HttpRequestException">Thrown when the HTTPS request is denied by the API</exception>
        /// <exception cref="NullReferenceException">Thrown when the retrieved data is null.</exception>
        public async Task<(double latitude, double longitude)> GetLocationFromAddressAsync(string? address)
        {
            string bingUrl = $"http://dev.virtualearth.net/REST/v1/Locations?q={Uri.EscapeDataString(address)}&key={_bingApiKey}";
            try
            {
                var point = (latitude: 0d, longitude: 0d);
                HttpResponseMessage responseMessage = await _httpClient.GetAsync(bingUrl);
                responseMessage.EnsureSuccessStatusCode();
                string responseBody = await responseMessage.Content.ReadAsStringAsync();

                // Parse the JSON response
                dynamic result = JsonConvert.DeserializeObject(responseBody) ?? throw new NullReferenceException();

                // Extract the latitude and longitude
                point.latitude = result.resourceSets[0].resources[0].point.coordinates[0];
                point.longitude = result.resourceSets[0].resources[0].point.coordinates[1];

                return point;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($@"HttpRequestException thrown: {e.Message}");
                return default;
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine($@"Null Reference Exception Thrown: {e.Message}");
                return default;
            }
        }

        /// <summary>
        /// Takes in a location (latitude and longitude) and returns an address based on that location.
        /// </summary>
        /// <param name="latitude">The latitude coordinate of the location.</param>
        /// <param name="longitude">The longitude coordinate of the location.</param>
        /// <returns>Returns a string denoting the address of the location.</returns>
        /// <exception cref="HttpRequestException">Thrown when the HTTPS request is denied by the API</exception>
        /// <exception cref="NullReferenceException">Thrown when the retrieved data is null.</exception>
        public async Task<string?> GetAddressFromLocationAsync(double latitude, double longitude)
        {
            var latitudeStr = latitude.ToString(CultureInfo.InvariantCulture);
            var longitudeStr = longitude.ToString(CultureInfo.InvariantCulture);
            string bingUrl = $"http://dev.virtualearth.net/REST/v1/Locations/{latitude},{longitude}?o=json&key={_bingApiKey}";
            try
            {
                HttpResponseMessage responseMessage = await _httpClient.GetAsync(bingUrl);
                responseMessage.EnsureSuccessStatusCode();
                string responseBody = await responseMessage.Content.ReadAsStringAsync();

                // Parse the JSON response
                dynamic result = JsonConvert.DeserializeObject(responseBody) ?? throw new NullReferenceException();

                // Extract the address from the returned json request
                var address = result.resourceSets[0].resources[0].address.formattedAddress;

                return address;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($@"HttpRequestException thrown: {e.Message}");
                return string.Empty;
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine($@"Null Reference Exception Thrown: {e.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Converts a latitude/longitude coordinate to Web Mercator projection.
        /// </summary>
        /// <param name="latitude">The latitude coordinate of the location.</param>
        /// <param name="longitude">The longitude coordinate of the location.</param>
        /// <returns>Returns a <see cref="Tuple"/> that represents the cartesian coordinate projection of the location based on Web Mercator
        /// projection. In the Spherical Mercator projection, X and Y represent the projected east-west and north-south
        /// positions of points on the Earth's surface, respectively</returns>
        public (double x, double y) ConvertLatLongtToSphericalMercator(double latitude, double longitude)
        {
            var item = SphericalMercator.FromLonLat(lon: longitude, lat: latitude);
            return item;
        }

        /// <summary>
        /// Converts a <see cref="MPoint"/> that is represented as Spherical Mercator coordinate to latitude/longitude values. 
        /// </summary>
        /// <param name="mPoint">The point in Spherical Mercator format that needs to be converted to lat/long values.</param>
        /// <returns>Returns a <see cref="Tuple"/> that represents the latitude and longitude values of the location.</returns>
        public (double latitude, double longitude) ConvertSphericalMercatorToCoordinate(MPoint mPoint)
        {
            var longLatPoint = SphericalMercator.ToLonLat(mPoint);

            return (longLatPoint.Y, longLatPoint.X);
        }
    }
}