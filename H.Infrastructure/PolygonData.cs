
using SharpKml.Base;

namespace H.Infrastructure
{
    internal sealed class PolygonData
    {
        private readonly int _id;
        private readonly Vector[] _coordinates;

        public PolygonData(int id, Vector[] coordinates)
        {
            _id = id;
            _coordinates = coordinates;
        }

        public int Id { get { return _id; } }

        public Vector[] Coordinates { get { return _coordinates; } }
    }
}
