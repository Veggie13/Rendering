using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.Geometry
{
    public class RectangularRegion : IFiniteGeometricRegion
    {
        public RectangularRegion(double left, double right, double front, double back, double top, double bottom)
        {
            Left = left;
            Right = right;
            Front = front;
            Back = back;
            Top = top;
            Bottom = bottom;
        }

        public double Left { get; private set; }
        public double Right { get; private set; }
        public double Front { get; private set; }
        public double Back { get; private set; }
        public double Top { get; private set; }
        public double Bottom { get; private set; }

        public Vector MinCorner { get { return new Vector(Left, Top, Front); } }
        public Vector MaxCorner { get { return new Vector(Right, Bottom, Back); } }

        public double XSpan { get { return Right - Left; } }
        public double YSpan { get { return Bottom - Top; } }
        public double ZSpan { get { return Back - Front; } }

        public double Volume { get { return XSpan * YSpan * ZSpan; } }

        public bool Contains(RectangularRegion region)
        {
            return (region.Left >= Left)
                && (region.Right <= Right)
                && (region.Front >= Front)
                && (region.Back <= Back)
                && (region.Top >= Top)
                && (region.Bottom <= Bottom);
        }

        public bool Intersects(IFiniteGeometricRegion region)
        {
            return Intersects(region.BoundingBox);
        }

        public RectangularRegion TranslatedBy(Vector trans)
        {
            return new RectangularRegion(Left + trans.X, Right + trans.X, Front + trans.Z, Back + trans.Z, Top + trans.Y, Bottom + trans.Y);
        }

        #region IFiniteGeometricRegion
        public RectangularRegion BoundingBox { get { return this; } }

        public Vector Center { get { return new Vector((Left + Right) / 2, (Top + Bottom) / 2, (Front + Back) / 2); } }

        public bool Contains(IFiniteGeometricRegion region)
        {
            return Contains(region.BoundingBox);
        }

        public bool GetSurfaceNormal(Vector pos, out Direction normal)
        {
            Vector relative = pos - Center;
            double px = relative.X / XSpan;
            double py = relative.Y / YSpan;
            double pz = relative.Z / ZSpan;

            double pmax = new[] { Math.Abs(px), Math.Abs(py), Math.Abs(pz) }.Max();

            // Left/Right
            if (pmax == Math.Abs(px))
            {
                // Right
                if (px > 0)
                {
                    normal = new Direction(0, 0);
                }
                // Left
                else
                {
                    normal = new Direction(Math.PI, 0);
                }
            }
            // Top/Bottom
            else if (pmax == Math.Abs(py))
            {
                // Bottom
                if (py > 0)
                {
                    normal = new Direction(0, Math.PI / 2);
                }
                // Top
                else
                {
                    normal = new Direction(0, -Math.PI / 2);
                }
            }
            // Front/Back
            else
            {
                // Back
                if (pz > 0)
                {
                    normal = new Direction(Math.PI / 2, 0);
                }
                // Front
                else
                {
                    normal = new Direction(-Math.PI / 2, 0);
                }
            }

            return true;
        }

        #region IGeometricRegion
        public bool Contains(Vector pos)
        {
            return (pos.X >= Left)
                && (pos.X <= Right)
                && (pos.Y >= Top)
                && (pos.Y <= Bottom)
                && (pos.Z >= Front)
                && (pos.Z <= Back);
        }

        public bool Intersects(IGeometricRegion region)
        {
            if (region is IFiniteGeometricRegion)
            {
                return Intersects(region as IFiniteGeometricRegion);
            }
            return region.Intersects(this);
        }

        public HashSet<Vector> IntersectionsWith(Ray ray)
        {
            Vector xPos, yPos, zPos;
            HashSet<Vector> results = new HashSet<Vector>();
            if (ray.AtX(Left, out xPos) && xPos.Y >= Top && xPos.Y <= Bottom && xPos.Z >= Front && xPos.Z <= Back)
                results.Add(xPos);
            if (ray.AtX(Right, out xPos) && xPos.Y >= Top && xPos.Y <= Bottom && xPos.Z >= Front && xPos.Z <= Back)
                results.Add(xPos);
            if (ray.AtY(Top, out yPos) && yPos.X >= Left && yPos.X <= Right && yPos.Z >= Front && yPos.Z <= Back)
                results.Add(yPos);
            if (ray.AtY(Bottom, out yPos) && yPos.X >= Left && yPos.X <= Right && yPos.Z >= Front && yPos.Z <= Back)
                results.Add(yPos);
            if (ray.AtZ(Front, out zPos) && zPos.Y >= Top && zPos.Y <= Bottom && zPos.X >= Left && zPos.X <= Right)
                results.Add(zPos);
            if (ray.AtZ(Back, out zPos) && zPos.Y >= Top && zPos.Y <= Bottom && zPos.X >= Left && zPos.X <= Right)
                results.Add(zPos);

            return results;
        }

        public bool GetFirstIntersectionWith(Ray ray, out Vector pos)
        {
            var results = IntersectionsWith(ray);
            pos = results.OrderBy(v => (v - ray.Origin).Magnitude).FirstOrDefault();
            return results.Any();
        }

        IGeometricRegion IGeometricRegion.TranslatedBy(Vector trans)
        {
            return TranslatedBy(trans);
        }
        #endregion
        #endregion
    }
}
