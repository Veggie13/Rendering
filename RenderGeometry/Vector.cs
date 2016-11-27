using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.Geometry
{
    public class Vector
    {
        public Vector(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }

        public double Magnitude { get { return Math.Sqrt(X * X + Y * Y + Z * Z); } }
        public Direction Direction { get { return new Direction(Math.Atan2(Z, X), Math.Atan2(Y, Math.Sqrt(X * X + Z * Z))); } }

        public Vector UnitVector
        {
            get
            {
                double mag = Magnitude;
                return new Vector(X / mag, Y / mag, Z / mag);
            }
        }

        public static Vector operator -(Vector a)
        {
            return a.ScaledBy(-1);
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return a.TranslatedBy(b.ScaledBy(-1));
        }

        public double Dot(Vector b)
        {
            return X * b.X + Y * b.Y + Z * b.Z;
        }

        public Vector Cross(Vector b)
        {
            return new Vector(Y * b.Z - Z * b.Y, Z * b.X - X * b.Z, X * b.Y - Y * b.X);
        }

        public Ray RayTowards(Direction direction)
        {
            return new Ray(this, direction);
        }

        public Ray RayThrough(Vector pos)
        {
            return new Ray(this, (pos - this).Direction);
        }

        public Vector ScaledBy(double scale)
        {
            return new Vector(X * scale, Y * scale, Z * scale);
        }

        public Vector TranslatedBy(Vector trans)
        {
            return new Vector(X + trans.X, Y + trans.Y, Z + trans.Z);
        }

        public Vector RotatedBy(Direction direction)
        {
            return this.Direction.RotatedBy(direction).UnitVector.ScaledBy(Magnitude);
        }
    }
}
