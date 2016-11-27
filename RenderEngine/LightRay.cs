using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.Engine
{
    public class LightRay
    {
        public LightRay(Light light, double initDistance)
        {
            Light = light;
            Distance = InitialDistance = initDistance;
        }

        private LightRay(Light light, double initDistance, double distance)
        {
            Light = light;
            Distance = distance;
            InitialDistance = initDistance;
        }

        public Light Light { get; private set; }
        public double InitialDistance { get; private set; }
        public double Distance { get; private set; }

        public Light AttenutatedLight
        {
            get
            {
                if (InitialDistance == 0)
                    return Light;
                return Light.Attenuated(Math.Pow(InitialDistance / Distance, 2));
            }
        }

        public LightRay Extend(double further)
        {
            return new LightRay(Light, InitialDistance, Distance + further);
        }
    }
}
