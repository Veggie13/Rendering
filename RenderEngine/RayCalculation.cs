using Render.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.Engine
{
    public class RayCalculation
    {
        private List<ISceneObject> _exceptions;
        private Scene _scene;
        private Ray _ray;

        public RayCalculation(Scene scene, Ray ray, IEnumerable<ISceneObject> exceptions, ISceneObject source = null)
        {
            _exceptions = exceptions.ToList();
            if (source != null)
            {
                _exceptions.Add(source);
            }
            _scene = scene;
            _ray = ray;
        }

        public async Task<IEnumerable<LightRay>> Run()
        {
            Vector intersectionPoint = null;
            ISceneObject intersected = _scene.FirstCollision(_ray, out intersectionPoint, _exceptions);
            if (intersected == null)
            {
                if (_ray.Bearing.Azimuth.ValueBetweenPI < 0)
                    return new[] { new LightRay(Light.None, 0) };
                return new[] { new LightRay(Light.White(0.1), 0) };
            }

            var rays = await intersected.GetLightRays(_scene, _exceptions, intersectionPoint, _ray.Bearing);
            return rays.Select(r => r.Extend((_ray.Origin - intersectionPoint).Magnitude)).ToList();
        }

        public static Task<IEnumerable<LightRay>> Run(Scene scene, Ray ray, IEnumerable<ISceneObject> exceptions, ISceneObject source = null)
        {
            return new RayCalculation(scene, ray, exceptions, source).Run();
        }
    }
}
