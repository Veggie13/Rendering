﻿using Render.Engine;
using Render.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.Materials
{
    public class MaterialObject : ISceneObject
    {
        public MaterialObject(IFiniteGeometricRegion shape, IMaterial material)
        {
            Shape = shape;
            Material = material;
        }

        public IMaterial Material { get; private set; }

        #region ISceneObject
        public IFiniteGeometricRegion Shape { get; private set; }

        public Task<IEnumerable<LightRay>> GetLightRays(Scene scene, IEnumerable<ISceneObject> exceptions, Vector fromPoint, Direction incoming)
        {
            Direction normal = null;
            if (!Shape.GetSurfaceNormal(fromPoint, out normal))
            {
                throw new ArgumentException("fromPoint");
            }

            return Material.GetRayContributors(scene, exceptions, fromPoint, incoming, normal, this);
        }
        #endregion
    }
}
