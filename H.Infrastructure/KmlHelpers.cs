using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;

namespace H.Infrastructure
{
    public class KmlHelpers
    {
        private readonly List<string> FileNames = new List<string>
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

        public bool IsPointInPolygon(Polygon polygon, Vector point)
        {
            var inside = false;
            // Get the outer boundary of the polygon
            var outerBoundary = polygon.OuterBoundary;
            var coordinates = outerBoundary.LinearRing.Coordinates.ToList();

            var j = coordinates.Count - 1;
            for (var i = 0; i < coordinates.Count; i++)
            {
                if ((coordinates[i].Longitude < point.Longitude &&
                     coordinates[j].Longitude >= point.Longitude) ||
                    (coordinates[j].Longitude < point.Longitude &&
                     coordinates[i].Longitude >= point.Longitude))
                    if (coordinates[i].Latitude + (point.Longitude - coordinates[i].Longitude) /
                        (coordinates[j].Longitude - coordinates[i].Longitude) *
                        (coordinates[j].Latitude - coordinates[i].Latitude) < point.Latitude)
                        inside = !inside;

                j = i;
            }

            return inside;
        }

        public int GetPolygonFromCoordinate(double latitude, double longitude)
        {
            var point = new Vector(latitude, longitude);
            foreach (var name in FileNames)
            {
                var fileName = $@"{Assembly.GetExecutingAssembly().GetName().Name}.Resources.{name}";
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName))
                {
                    if (stream == null) return 0;

                    using (var reader = new StreamReader(stream))
                    {
                        var parser = new Parser();
                        parser.ParseString(reader.ReadToEnd(), false);
                        var root = (Kml)parser.Root;

                        var inside = false;

                        foreach (var placemark in root.Flatten().OfType<Placemark>())
                        {
                            if (placemark.Geometry is Polygon polygon) inside = IsPointInPolygon(polygon, point);

                            if (inside) return int.Parse(placemark.Name);
                        }
                    }
                }
            }

            return 0;
        }
    }
}