using Render.Engine;
using Render.Geometry;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ParallelExtensions;

namespace Render.Materials
{
    public class MatteMaterial : IMaterial
    {
        public MatteMaterial(ISpectrum spectrum)
        {
            Spectrum = spectrum;
        }

        public ISpectrum Spectrum { get; private set; }

        public IEnumerable<Task<LightRay>> GetRayContributors(Scene scene, IEnumerable<ISceneObject> exceptions, Vector fromPoint, Direction incoming, Direction normal, ISceneObject source)
        {
            var newExceptions = exceptions.Append(source);
            return scene.Objects.OfType<LightSource>().Except(newExceptions)
                .Select(l => new { Light = l, Ray = fromPoint.RayThrough(l.Shape.Center) })
                .Where(a => normal.UnitVector.Dot(a.Ray.Bearing.UnitVector) > 0)
                .Where(a => (scene.FirstCollision(a.Ray, out var intersection, newExceptions) == a.Light))
                .Select(a => RayCalculation.Run(scene, a.Ray, exceptions, source))
                .SelectMany(tlr => tlr)
                .Select(async tlr => (await tlr).TransformedBy(Spectrum));
        }
    }
}
