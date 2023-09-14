using System.Reflection;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
using H.Core.Enumerations;

namespace H.Avalonia.Infrastructure
{
    /// <summary>
    /// Contains set of methods that help us process and parse kml files.
    /// </summary>
    public class KmlHelpers
    {
        /// <summary>
        /// A list of tuples containing the provinces and their respective Kml files.
        /// </summary>
        private readonly List<(Province Province, string FileName)> _fileNames = new()
        {
            (Province.Ontario, "ON_No_Styles.kml"),
            (Province.Saskatchewan, "SK_No_Styles.kml"),
            (Province.Quebec, "QC_No_Styles.kml"),
            (Province.NovaScotia, "NS_No_Styles.kml"), 
            (Province.PrinceEdwardIsland, "PEI_No_Styles.kml"),
            (Province.Newfoundland, "NFLD_No_Styles.kml"),
            (Province.NewBrunswick, "NB_No_Styles.kml"),
            (Province.Manitoba, "MB_No_Styles.kml"),
            (Province.Alberta, "Alberta_No_Styles.kml"),
            (Province.BritishColumbia, "BC_No_Styles.kml")
        };
        
        public Task? LoadPolygonsAsync;

        /// <summary>
        /// Contains a map of Provinces and their SLC polygons. Each value for a key is a tuple that contains the
        /// polygon ID and its respective <see cref="Polygon"/> coordinates in the format specified by sharpkml 
        /// </summary>
        public readonly Dictionary<Province, List<(int polygonId, Polygon sharpKmlPolygon)>> PolygonMap = new();

        
        public KmlHelpers()
        {
            Task.Run(InitializePolygonsAsync);
        }

        /// <summary>
        /// Asynchronously loads the SLC polygons.
        /// </summary>
        private async Task InitializePolygonsAsync()
        {
            LoadPolygonsAsync = LoadAllPolygonsFromKmlFilesAsync();
            await LoadPolygonsAsync;
        }
        
        /// <summary>
        /// Returns the polygon number based on the latitude and longitude values.
        /// </summary>
        /// <param name="latitude">The latitude value of the coordinate.</param>
        /// <param name="longitude">The longitude value of the coordinate</param>
        /// <returns>Returns the value of the Polygon if the latitude/longitude values are within a polygon. Returns 0 if no polygon is found.</returns>
        public async Task<int> GetPolygonFromCoordinateAsync(double latitude, double longitude)
        {
            return await Task.Run(() =>
            {
                var point = new Vector(latitude, longitude);
                LoadPolygonsAsync?.Wait();
                foreach (var entry in PolygonMap)
                {
                    var polygonList = entry.Value;
                    // Set a bool that indicates if the point is inside the polygon
                    bool inside = false;

                    // Loop through all the Placemarks in the KML file
                    foreach (var item in polygonList)
                    {
                        inside = IsPointInPolygon(item.sharpKmlPolygon, point);
                        if (inside)
                        {
                            return item.polygonId;
                        }
                    }
                }
                return 0;
            });
        }
        
        /// <summary>
        /// Checks if a point is within a polygon.
        /// </summary>
        /// <param name="polygon">The polygon that the point is checked against</param>
        /// <param name="point">The point that we need to check is within the polygon.</param>
        /// <returns>Returns true if the point is within the polygon, false otherwise.</returns>
        private bool IsPointInPolygon(Polygon polygon, Vector point)
        {
            bool inside = false;
            // Get the outer boundary of the polygon
            OuterBoundary outerBoundary = polygon.OuterBoundary;
            List<Vector> coordinates = outerBoundary.LinearRing.Coordinates.ToList();

            int j = coordinates.Count - 1;
            for (int i = 0; i < coordinates.Count; i++)
            {
                if (coordinates[i].Longitude < point.Longitude &&
                    coordinates[j].Longitude >= point.Longitude ||
                    coordinates[j].Longitude < point.Longitude &&
                    coordinates[i].Longitude >= point.Longitude)
                {
                    if (coordinates[i].Latitude + (point.Longitude - coordinates[i].Longitude) /
                        (coordinates[j].Longitude - coordinates[i].Longitude) *
                        (coordinates[j].Latitude - coordinates[i].Latitude) < point.Latitude)
                    {
                        inside = !inside;
                    }
                }
                j = i;
            }
            return inside;
        }

        /// <summary>
        /// Async loads all the polygons from the KML files.
        /// </summary>
        private async Task LoadAllPolygonsFromKmlFilesAsync()
        {
            await Task.Run(() =>
            {
                foreach (var entry in _fileNames)
                {
                    var polygonList = new List<(int polygonId, Polygon sharpKmlPolygon)>();
                    var ass = Assembly.GetExecutingAssembly();
                    var fileName = $@"{Assembly.GetExecutingAssembly().GetName().Name}.Resources.{entry.FileName}";
                    using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName);
                    if (stream == null) return;

                    using var reader = new StreamReader(stream);
                    var parser = new Parser();
                    // Parse the stream of the kml file
                    parser.ParseString(reader.ReadToEnd(), false);
                    // Get the root element
                    Kml root = (Kml)parser.Root;

                    // Loop through all the Placemarks in the KML file
                    foreach (var placemark in root.Flatten().OfType<Placemark>())
                    {
                        if (placemark.Geometry is Polygon polygon)
                        {
                            polygonList.Add((int.Parse(placemark.Name), polygon));
                        }
                    }
                    PolygonMap.TryAdd(entry.Province, polygonList);
                }
            });
        }
    }
}
