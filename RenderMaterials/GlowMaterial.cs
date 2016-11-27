using Render.Engine;
using Render.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.Materials
{
    public class GlowMaterial : IMaterial
    {
        public GlowMaterial(Light light)
        {
            Emission = light;
        }

        public Light Emission { get; private set; }

        public async Task<IEnumerable<LightRay>> GetRayContributors(Scene scene, IEnumerable<ISceneObject> exceptions, Vector fromPoint, Direction incoming, Direction normal, ISceneObject source)
        {
            return new[] { new LightRay(Emission, (source.Shape.Center - fromPoint).Magnitude) };
        }
    }
}
