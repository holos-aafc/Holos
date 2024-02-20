using NetTopologySuite.Geometries;

namespace H.Avalonia.Infrastructure
{
    public static class SharpKmlExtensions
    {
        private static MapHelpers _mapHelpers = new();

        /// <summary>
        /// Converts a sharpkml based polygon into a Well-known text (WKT) representation using <see cref="NetTopologySuite"/> library.
        /// </summary>
        /// <param name="kmlPolygon">A polygon extracted from a kml file using the SharpKml library</param>
        /// <returns>A string representation of a polygon based on Well-known text (WKT).</returns>
        public static string ToWkt(this SharpKml.Dom.Polygon? kmlPolygon)
        {
            if (kmlPolygon == null)
                return string.Empty;
            
            var outerRing = CreateOuterRingMercator(kmlPolygon);
            var innerRings = CreateInnerRingMercator(kmlPolygon);
            var polygon = CreateNetTopologyPolygonMercator(outerRing, innerRings);

            // Convert the polygon to a WKT string
            return polygon.AsText();
        }

        /// <summary>
        /// Creates the outer-ring of a polygon. Uses a SharpKml <see cref="SharpKml.Dom.Polygon"/> as an input and converts the
        /// coordinates to a <see cref="LinearRing"/> as part of <see cref="NetTopologySuite"/>.
        /// </summary>
        /// <param name="kmlPolygon">A Sharpkml polygon</param>
        /// <returns>Returns a <see cref="LinearRing"/> that represents the outer coordinates of a polygon in mercator coordinates.</returns>
        private static LinearRing CreateOuterRingMercator(SharpKml.Dom.Polygon? kmlPolygon)
        {
            // Convert KML coordinates to NetTopologySuite coordinates for the outer ring
            List<Coordinate> netTopologyCoordinates = new();
            foreach (var item in kmlPolygon.OuterBoundary.LinearRing.Coordinates)
            {
                var point = _mapHelpers.ConvertLatLongtToSphericalMercator(item.Latitude, item.Longitude);
                var coordinate = new Coordinate(point.x, point.y);
                netTopologyCoordinates.Add(coordinate);
            }

            // Create a LinearRing using the coordinates for the outer ring
            LinearRing outerRing = new LinearRing(netTopologyCoordinates.ToArray());

            return outerRing;
        }

        /// <summary>
        /// Creates the inner-ring of a polygon that represents the hole inside a polygon. Uses a SharpKml <see cref="SharpKml.Dom.Polygon"/> as an input and converts the
        /// coordinates to a list of <see cref="LinearRing"/> as part of <see cref="NetTopologySuite"/>.
        /// </summary>
        /// <param name="kmlPolygon">A Sharpkml polygon</param>
        /// <returns>Returns a list of <see cref="LinearRing"/> that represents the inner coordinates of a polygon in mercator coordinates.</returns>
        private static List<LinearRing> CreateInnerRingMercator(SharpKml.Dom.Polygon? kmlPolygon)
        {
            // Create a list to hold inner rings (holes)
            List<LinearRing> innerRings = new List<LinearRing>();

            // Convert inner ring (hole) coordinates to NetTopologySuite coordinates
            foreach (var innerBoundary in kmlPolygon.InnerBoundary)
            {
                List<Coordinate> innerNetTopologyCoordinates = new();
                foreach (var item in innerBoundary.LinearRing.Coordinates)
                {
                    var point = _mapHelpers.ConvertLatLongtToSphericalMercator(item.Latitude, item.Longitude);
                    var coordinate = new Coordinate(point.x, point.y);
                    innerNetTopologyCoordinates.Add(coordinate);
                }
                // Create a LinearRing for the inner ring and add it to the list
                innerRings.Add(new LinearRing(innerNetTopologyCoordinates.ToArray()));
            }

            return innerRings;
        }

        
        /// <summary>
        /// Creates a <see cref="NetTopologySuite"/> polygon using Mercator projection coordinates.
        /// </summary>
        /// <param name="outerRing">The outer ring coordinates of the polygon</param>
        /// <param name="innerRings">The inner ring coordinates of the polygon representing the holes inside the main polygon</param>
        /// <returns>A <see cref="NetTopologySuite"/> polygon using Mercator projection coordinates.</returns>
        private static Polygon CreateNetTopologyPolygonMercator(LinearRing outerRing, List<LinearRing> innerRings)
        {
            // Create the polygon using the outer ring and inner rings (holes)
            return new Polygon(outerRing, innerRings.ToArray());
        }
    }
}