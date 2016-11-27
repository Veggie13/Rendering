using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.Geometry
{
    public class Ray
    {
        public Ray(Vector origin, Direction bearing)
        {
            Origin = origin;
            Bearing = bearing;
        }

        public Vector Origin { get; private set; }
        public Direction Bearing { get; private set; }

        public Vector PointAtLength(double length)
        {
            return Origin.TranslatedBy(Bearing.UnitVector.ScaledBy(length));
        }
        
        public bool AtX(double x, out Vector pos)
        {
            pos = null;
            var unit = Bearing.UnitVector;
            if (unit.X == 0)
                return false;
            double relX = x - Origin.X;
            double s = relX / unit.X;
            if (s < 0)
                return false;
            pos = Origin.TranslatedBy(unit.ScaledBy(s));
            return true;
        }

        public bool AtY(double y, out Vector pos)
        {
            pos = null;
            var unit = Bearing.UnitVector;
            if (unit.Y == 0)
                return false;
            double relY = y - Origin.Y;
            double s = relY / unit.Y;
            if (s < 0)
                return false;
            pos = Origin.TranslatedBy(unit.ScaledBy(s));
            return true;
        }

        public bool AtZ(double z, out Vector pos)
        {
            pos = null;
            var unit = Bearing.UnitVector;
            if (unit.Z == 0)
                return false;
            double relZ = z - Origin.Z;
            double s = relZ / unit.Z;
            if (s < 0)
                return false;
            pos = Origin.TranslatedBy(unit.ScaledBy(s));
            return true;
        }

        public Ray TranslatedBy(Vector trans)
        {
            return new Ray(Origin.TranslatedBy(trans), Bearing);
        }

        public Ray RotatedBy(Direction dir)
        {
            return new Ray(Origin, Bearing.RotatedBy(dir));
        }
    }
}
