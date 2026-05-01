using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace H.Infrastructure
{
    public class KmlHelpers
    {
        private readonly List<string> FileNames = new List<string>()
        {
            "ON_No_Styles.kml",
            "SK_No_Styles.kml",
            "QC_No_Styles.kml",
            "NS_No_Styles.kml",
            "PEI_No_Styles.kml",
            "NFLD_No_Styles.kml",
            "NB_No_Styles.kml",
            "MB_No_Styles.kml",
            "Alberta_No_Styles.kml",
            "BC_No_Styles.kml"
        };

        private readonly List<PolygonData> _polygons;

        public KmlHelpers()
        {
            _polygons = FileNames
                .AsParallel()
                .SelectMany(filename =>
                {
                    var assembly = typeof(KmlHelpers).Assembly;
                    var resourceName = $@"{assembly.GetName().Name}.Resources.{filename}";
                    using (var stream = assembly.GetManifestResourceStream(resourceName))
                    {
                        if (stream != null)
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                var parser = new Parser();
                                parser.ParseString(reader.ReadToEnd(), false);
                                Kml root = (Kml)parser.Root;

                                return root
                                    .Flatten()
                                    .OfType<Placemark>()
                                    .Where(p => p.Geometry is Polygon)
                                    .Select(p =>
                                        new PolygonData(
                                            int.Parse(p.Name),
                                            ((Polygon)p.Geometry).OuterBoundary.LinearRing.Coordinates.ToArray()));
                            }
                        }

                    }
                    return new List<PolygonData>();
                })
                .ToList();
        }

        public int GetPolygonFromCoordinate(double latitude, double longitude)
        {
            var match = _polygons
                .AsParallel()
                .FirstOrDefault(p => IsPointInPolygon(p.Coordinates, new Vector(latitude, longitude)));

            return match != null ? match.Id : 0;
        }

        private static bool IsPointInPolygon(Vector[] coordinates, Vector point)
        {
            bool inside = false;

            int j = coordinates.Length - 1;
            for (int i = 0; i < coordinates.Length; i++)
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
    }
}
