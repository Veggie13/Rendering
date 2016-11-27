using Render.Engine;
using Render.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.Materials
{
    public class SpecularMaterial : IMaterial
    {
        public SpecularMaterial(ISpectrum spectrum)
        {
            Spectrum = spectrum;
        }

        public ISpectrum Spectrum { get; private set; }

        public async Task<IEnumerable<LightRay>> GetRayContributors(Scene scene, IEnumerable<ISceneObject> exceptions, Vector fromPoint, Direction incoming, Direction normal, ISceneObject source)
        {
            Direction outgoing = incoming.ReflectIn(normal);
            var lightRays = await RayCalculation.Run(scene, outgoing.RayFrom(fromPoint), exceptions, source);
            return lightRays.Select(lightRay => new LightRay(lightRay.Light.TransformedBy(Spectrum), lightRay.InitialDistance).Extend(lightRay.Distance - lightRay.InitialDistance)).ToList();
        }
    }
}
