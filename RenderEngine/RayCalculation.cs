using Render.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Render.Engine
{
    public static class RayCalculation
    {
        private static IEnumerable<Task<LightRay>> DarknessRays
        {
            get { yield return Task.FromResult(LightRay.Dark); }
        }

        private static IEnumerable<Task<LightRay>> DistantRays
        {
            get { yield return Task.FromResult(new LightRay(Light.White(0.1), 0)); }
        }

        public static IEnumerable<Task<LightRay>> Run(Scene scene, Ray ray, IEnumerable<ISceneObject> exceptions, ISceneObject source = null)
        {
            if (source != null)
            {
                exceptions = exceptions.Append(source);
            }

            Vector intersectionPoint = null;
            ISceneObject intersected = scene.FirstCollision(ray, out intersectionPoint, exceptions);
            if (intersected == null)
            {
                if (ray.Bearing.Azimuth.ValueBetweenPI < 0)
                    return DarknessRays;
                return DistantRays;
            }

            var rays = intersected.GetLightRays(scene, exceptions, intersectionPoint, ray.Bearing);
            double extension = (ray.Origin - intersectionPoint).Magnitude;
            return rays.Select(async tlr => (await tlr).ExtendedBy(extension));
        }
    }
}
