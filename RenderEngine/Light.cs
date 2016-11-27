using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.Engine
{
    public class Light
    {
        public static readonly Light None = new Light(ConstantSpectrum.Zero);

        public static Light White(double intensity)
        {
            if (intensity == 0)
                return Light.None;
            return new Light(ConstantSpectrum.Get(intensity));
        }

        public Light(ISpectrum spectrum)
        {
            Spectrum = spectrum;
        }

        public ISpectrum Spectrum { get; private set; }

        public Light Attenuated(double attenuation)
        {
            if (attenuation == 0)
                return Light.None;
            return new Light(Spectrum.MultiplyBy(ConstantSpectrum.Get(attenuation)));
        }

        public Light TransformedBy(ISpectrum trans)
        {
            if (trans == ConstantSpectrum.Zero)
                return Light.None;
            return new Light(Spectrum.MultiplyBy(trans));
        }
    }

    public static class LightExtensions
    {
        public static Light Combined(this IEnumerable<Light> sources)
        {
            if (!sources.Any())
            {
                return Light.None;
            }
            return new Light(sources.Select(l => l.Spectrum).Sum());
        }
    }
}
