using Render.Engine;
using Render.Geometry;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ParallelExtensions;

namespace Render.Materials
{
    public class CompositeMaterial : IMaterial
    {
        public CompositeMaterial(params IMaterial[] materials)
        {
            Materials = materials.ToList();
        }

        public IEnumerable<IMaterial> Materials { get; private set; }

        public IEnumerable<Task<LightRay>> GetRayContributors(Scene scene, IEnumerable<ISceneObject> exceptions, Vector fromPoint, Direction incoming, Direction normal, ISceneObject source)
        {
            return Materials
                .Select(m => m.GetRayContributors(scene, exceptions, fromPoint, incoming, normal, source))
                .SelectMany(tlr => tlr);
        }
    }
}
