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
    public class MatteMaterial : IMaterial
    {
        public MatteMaterial(ISpectrum spectrum)
        {
            Spectrum = spectrum;
        }

        public ISpectrum Spectrum { get; private set; }

        public async Task<IEnumerable<LightRay>> GetRayContributors(Scene scene, IEnumerable<ISceneObject> exceptions, Vector fromPoint, Direction incoming, Direction normal, ISceneObject source)
        {
            var newExceptions = exceptions.Concat(new[] { source });
            var lights = scene.Objects.OfType<Render.Materials.LightSource>().Except(newExceptions);
            var lightBearings = lights.Select(l => new { Light = l, Ray = fromPoint.RayThrough(l.Shape.Center) }).ToList();
            var facingLights = lightBearings.Where(a => normal.UnitVector.Dot(a.Ray.Bearing.UnitVector) > 0).ToList();
            var visibleLights = facingLights.Where(a =>
            {
                Vector intersection;
                return (scene.FirstCollision(a.Ray, out intersection, newExceptions) == a.Light);
            }).ToList();
            var lightRays = visibleLights
                .AwaitAll(a => RayCalculation.Run(scene, a.Ray, exceptions, source), (a, r) => r)
                .SelectMany(r => r);
            return lightRays.Select(lightRay => new LightRay(lightRay.Light.TransformedBy(Spectrum), lightRay.InitialDistance).Extend(lightRay.Distance - lightRay.InitialDistance)).ToList();
        }
    }
}
