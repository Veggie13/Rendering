using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.Geometry
{
    public class Sphere : IFiniteGeometricRegion
    {
        public Sphere(Vector center, double radius)
        {
            Center = center;
            Radius = radius;
            BoundingBox = new RectangularRegion(Center.X - Radius, Center.X + Radius, Center.Z - Radius, Center.Z + Radius, Center.Y - Radius, Center.Y + Radius);
        }

        public double Radius { get; private set; }

        public Sphere TranslatedBy(Vector trans)
        {
            return new Sphere(Center.TranslatedBy(trans), Radius);
        }

        #region IFiniteGeometricRegion
        public RectangularRegion BoundingBox { get; private set; }

        public Vector Center { get; private set; }

        public bool Contains(IFiniteGeometricRegion region)
        {
            var bounds = region.BoundingBox;
            return Contains(bounds.MinCorner) && Contains(bounds.MaxCorner);
        }

        public bool GetSurfaceNormal(Vector pos, out Direction normal)
        {
            normal = (pos - Center).Direction;
            return true;
        }

        #region IGeometricRegion
        public bool Contains(Vector pos)
        {
            return ((Center - pos).Magnitude <= Radius);
        }

        public bool Intersects(IGeometricRegion region)
        {
            return BoundingBox.Intersects(region);
        }

        public HashSet<Vector> IntersectionsWith(Ray ray)
        {
            var angleVector = Center - ray.Origin;
            var rayBearing = ray.Bearing.UnitVector;
            double rayBearingDot = rayBearing.Dot(angleVector);
            var perpPoint = ray.Origin.TranslatedBy(rayBearing.ScaledBy(rayBearingDot));
            double minDistance = (perpPoint - Center).Magnitude;
            
            var results = new HashSet<Vector>();
            if (minDistance > Radius)
                return results;
            if (minDistance == Radius)
            {
                results.Add(perpPoint);
                return results;
            }

            double delta = Math.Sqrt(Radius * Radius - minDistance * minDistance);
            results.Add(ray.Origin.TranslatedBy(rayBearing.ScaledBy(rayBearingDot + delta)));
            results.Add(ray.Origin.TranslatedBy(rayBearing.ScaledBy(rayBearingDot - delta)));
            return results;
        }

        public bool GetFirstIntersectionWith(Ray ray, out Vector pos)
        {
            pos = null;

            var angleVector = Center - ray.Origin;
            var rayBearing = ray.Bearing.UnitVector;
            double rayBearingDot = rayBearing.Dot(angleVector);
            if (rayBearingDot < 0)
                return false;
            var perpPoint = ray.Origin.TranslatedBy(rayBearing.ScaledBy(rayBearingDot));
            double minDistance = (perpPoint - Center).Magnitude;

            if (minDistance > Radius)
                return false;
            if (minDistance == Radius)
            {
                pos = perpPoint;
                return true;
            }

            double delta = Math.Sqrt(Radius * Radius - minDistance * minDistance);
            pos = ray.Origin.TranslatedBy(rayBearing.ScaledBy(rayBearingDot - delta));
            return true;
        }

        IGeometricRegion IGeometricRegion.TranslatedBy(Vector trans)
        {
            return TranslatedBy(trans);
        }
        #endregion
        #endregion
    }
}
