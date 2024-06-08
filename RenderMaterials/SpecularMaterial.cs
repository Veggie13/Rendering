using Render.Engine;
using Render.Geometry;
using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<Task<LightRay>> GetRayContributors(Scene scene, IEnumerable<ISceneObject> exceptions, Vector fromPoint, Direction incoming, Direction normal, ISceneObject source)
        {
            Direction outgoing = incoming.ReflectIn(normal);
            return RayCalculation.Run(scene, outgoing.RayFrom(fromPoint), exceptions, source)
                .Select(async tlr => (await tlr).TransformedBy(Spectrum));
        }
    }
}
