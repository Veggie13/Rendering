using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.Geometry
{
    public interface IFiniteGeometricRegion : IGeometricRegion
    {
        RectangularRegion BoundingBox { get; }
        Vector Center { get; }

        bool Contains(IFiniteGeometricRegion region);

        bool GetSurfaceNormal(Vector pos, out Direction normal);
    }
}
