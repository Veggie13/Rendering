using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.Geometry
{
    public class Direction
    {
        public Direction(Angle azimuth, Angle elevation)
        {
            Azimuth = azimuth;
            Elevation = elevation;
        }

        public Angle Azimuth { get; private set; }
        public Angle Elevation { get; private set; }

        public Vector UnitVector { get { return new Vector(Elevation.Cos * Azimuth.Cos, Elevation.Sin, Elevation.Cos * Azimuth.Sin); } }

        public Direction Reverse { get { return new Direction(-Azimuth, -Elevation); } }

        public Ray RayFrom(Vector pos)
        {
            return new Ray(pos, this);
        }

        public Direction RotatedBy(Direction dir)
        {
            throw new NotImplementedException();
        }

        public Direction ReflectIn(Direction normal)
        {
            var unit = UnitVector;
            var basis1 = normal.UnitVector;
            var basis2 = basis1.Cross(unit).UnitVector;
            var basis3 = basis1.Cross(basis2);

            var reflection1 = basis3.ScaledBy(basis3.Dot(unit));
            var reflection2 = basis1.ScaledBy(basis1.Dot(unit));

            return reflection1.TranslatedBy(-reflection2).Direction;
        }
    }
}
