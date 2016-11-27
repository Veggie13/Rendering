using Render.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.Engine
{
    public class Scene
    {
        private HashSet<ISceneObject> _objects = new HashSet<ISceneObject>();
        public IEnumerable<ISceneObject> Objects { get { return _objects; } }

        public void AddObject(ISceneObject item)
        {
            _objects.Add(item);
        }

        public ISceneObject FirstCollision(Ray ray, out Vector intersection, IEnumerable<ISceneObject> exceptions)
        {
            var item = _objects.Except(exceptions).Select(o =>
                {
                    Vector pos;
                    bool success = o.Shape.GetFirstIntersectionWith(ray, out pos);
                    return new { Success = success, Position = pos, Item = o };
                })
                .Where(a => a.Success)
                .OrderBy(a => (a.Position - ray.Origin).Magnitude)
                .FirstOrDefault();
            if (item == null)
            {
                intersection = null;
                return null;
            }

            intersection = item.Position;
            return item.Item;
        }
    }
}
