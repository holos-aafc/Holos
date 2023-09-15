using NetTopologySuite.Geometries;

namespace H.Avalonia.Infrastructure
{
    public static class SharpKmlExtensions
    {
        /// <summary>
        /// Converts a sharpkml based polygon into a Well-known text (WKT) representation using <see cref="NetTopologySuite"/> library.
        /// </summary>
        /// <param name="kmlPolygon">A polygon extracted from a kml file using the SharpKml library</param>
        /// <returns>A string representation of a polygon based on Well-known text (WKT).</returns>
        public static string ToWkt(this SharpKml.Dom.Polygon? kmlPolygon)
        {
            var mapHelpers = new MapHelpers();
            if (kmlPolygon == null)
                return string.Empty;

            // Convert KML coordinates to NetTopologySuite coordinates
            List<Coordinate> netTopologyCoordinates = new();
            foreach (var item in kmlPolygon.OuterBoundary.LinearRing.Coordinates)
            {
                var point = mapHelpers.ConvertLatLongtToSphericalMercator(item.Latitude, item.Longitude);
                var coordinate = new Coordinate(point.x, point.y);
                netTopologyCoordinates.Add(coordinate);
            }
            // Create a LinearRing using the coordinates
            LinearRing linearRing = new LinearRing(netTopologyCoordinates.ToArray());

            // Create the polygon using the LinearRing
            Polygon polygon = new Polygon(linearRing);

            // Convert the polygon to a WKT string
            return polygon.AsText();
        }
    }
}