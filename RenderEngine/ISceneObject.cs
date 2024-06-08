using Render.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.Engine
{
    public interface ISceneObject
    {
        IFiniteGeometricRegion Shape { get; }
        IEnumerable<Task<LightRay>> GetLightRays(Scene scene, IEnumerable<ISceneObject> exceptions, Vector fromPoint, Direction incoming);
    }
}
