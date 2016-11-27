using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.Geometry
{
    public struct Angle
    {
        private double _angle;

        private Angle(double angle)
        {
            _angle = bound2PI(angle, 0);
        }

        public static implicit operator Angle(double angle)
        {
            return new Angle(angle);
        }

        public static Angle FromDegrees(double degAgrees)
        {
            return new Angle(Math.PI * degAgrees / 180);
        }

        public double Value0To2PI { get { return _angle; } }
        public double ValueBetweenPI { get { return bound2PI(_angle, -Math.PI); } }
        public double Value0ToPI { get { return boundPI(_angle, 0); } }
        public double ValueBetweenRights { get { return boundPI(_angle, -Math.PI / 2); } }
        public double Value0ToRight { get { return bound(_angle, 0, Math.PI / 2); } }

        public double Sin { get { return Math.Sin(_angle); } }
        public double Cos { get { return Math.Cos(_angle); } }

        public double ToDegrees(double minAngle = 0, double fullAngle = 360)
        {
            double degrees = 180 * _angle / Math.PI;
            return ((degrees - minAngle) % fullAngle) + minAngle; 
        }

        public static Angle operator -(Angle a)
        {
            return new Angle(-a._angle);
        }
        
        public static Angle operator +(Angle a, Angle b)
        {
            return new Angle(a._angle + b._angle);
        }

        public static Angle operator -(Angle a, Angle b)
        {
            return new Angle(a._angle - b._angle);
        }

        public override string ToString()
        {
            return "deg: " + ToDegrees().ToString();
        }

        private static double bound(double angle, double minAngle, double fullAngle)
        {
            return ((angle - minAngle) % fullAngle) + minAngle;
        }

        private static double bound2PI(double angle, double minAngle)
        {
            return bound(angle, minAngle, 2 * Math.PI);
        }

        private static double boundPI(double angle, double minAngle)
        {
            return bound(angle, minAngle, Math.PI);
        }
    }
}
