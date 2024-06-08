using Render.Engine;
using Render.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.Materials
{
    public interface IMaterial
    {
        IEnumerable<Task<LightRay>> GetRayContributors(Scene scene, IEnumerable<ISceneObject> exceptions, Vector fromPoint, Direction incoming, Direction normal, ISceneObject source);
    }

    public static partial class MaterialExtensions
    {
        public static MaterialObject ToObject(this IMaterial material, IFiniteGeometricRegion shape)
        {
            return new MaterialObject(shape, material);
        }
    }
}
