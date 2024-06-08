using Render.Engine;
using Render.Geometry;
using System.Collections.Generic;
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

        public IEnumerable<Task<LightRay>> GetRayContributors(Scene scene, IEnumerable<ISceneObject> exceptions, Vector fromPoint, Direction incoming, Direction normal, ISceneObject source)
        {
            yield return Task.FromResult(new LightRay(Emission, (source.Shape.Center - fromPoint).Magnitude));
        }
    }
}
