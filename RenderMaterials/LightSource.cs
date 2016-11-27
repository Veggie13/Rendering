using Render.Engine;
using Render.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.Materials
{
    public class LightSource : MaterialObject
    {
        public LightSource(IFiniteGeometricRegion shape, Light emission)
            : base(shape, new GlowMaterial(emission))
        {
        }

        public Light Emission { get { return (Material as GlowMaterial).Emission; } }
    }
}
