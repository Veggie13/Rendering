using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.Geometry
{
    public interface IGeometricRegion
    {
        bool Contains(Vector pos);

        bool Intersects(IGeometricRegion region);
        HashSet<Vector> IntersectionsWith(Ray ray);
        bool GetFirstIntersectionWith(Ray ray, out Vector pos);

        IGeometricRegion TranslatedBy(Vector trans);
    }

    static partial class Extensions
    {
        public static HashSet<Vector> IntersectionsWith(this Ray ray, IGeometricRegion region)
        {
            return region.IntersectionsWith(ray);
        }

        public static bool GetFirstIntersectionWith(this Ray ray, IGeometricRegion region, out Vector pos)
        {
            return region.GetFirstIntersectionWith(ray, out pos);
        }
    }
}
