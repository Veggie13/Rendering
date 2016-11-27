using Render.Engine;
using Render.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task<IEnumerable<LightRay>> GetRayContributors(Scene scene, IEnumerable<ISceneObject> exceptions, Vector fromPoint, Direction incoming, Direction normal, ISceneObject source)
        {
            return Materials
                .AsParallel()
                .Select(m => m.GetRayContributors(scene, exceptions, fromPoint, incoming, normal, source))
                .AwaitAll()
                .SelectMany(ray => ray);
        }
    }
}
