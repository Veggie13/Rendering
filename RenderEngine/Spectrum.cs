using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.Engine
{
    public interface ISpectrum
    {
        double this[double coord] { get; }
    }

    public class ConstantSpectrum : ISpectrum
    {
        public static readonly ConstantSpectrum Zero = new ConstantSpectrum(0);
        public static readonly ConstantSpectrum Unit = new ConstantSpectrum(1);

        public static ConstantSpectrum Get(double val)
        {
            if (val < 0)
                throw new ArgumentException("ConstantSpectrum(double val)");
            if (val == 0)
                return Zero;
            if (val == 1)
                return Unit;
            return new ConstantSpectrum(val);
        }
        
        private ConstantSpectrum(double val)
        {
            Value = val;
        }

        public double Value { get; private set; }

        public double this[double coord] { get { return Value; } }
    }

    class MultipliedSpectrum : ISpectrum
    {
        public MultipliedSpectrum(IEnumerable<ISpectrum> spectra)
        {
            Spectra = spectra.ToList();
        }

        public IEnumerable<ISpectrum> Spectra { get; private set; }

        public double this[double coord]
        {
            get
            {
                return Spectra.Select(s => s[coord]).Aggregate(1.0, (seed, v) => seed * v);
            }
        }
    }

    class AddedSpectrum : ISpectrum
    {
        public AddedSpectrum(IEnumerable<ISpectrum> spectra)
        {
            Spectra = spectra.ToList();
        }

        public IEnumerable<ISpectrum> Spectra { get; private set; }

        public double this[double coord]
        {
            get
            {
                return Spectra.Select(s => s[coord]).Aggregate(0.0, (seed, v) => seed + v);
            }
        }
    }

    public class InterpolatedSpectrum : ISpectrum
    {
        private Dictionary<double, double> _values;
        private bool _extendEndpoints;

        public InterpolatedSpectrum(Dictionary<double, double> values, bool extendEndpoints)
        {
            _values = new Dictionary<double, double>(values);
            _extendEndpoints = extendEndpoints;
        }

        public double this[double coord]
        {
            get
            {
                var keys = _values.Keys.OrderBy(v => v);
                if (coord < keys.First())
                {
                    return _extendEndpoints ? keys.First() : 0;
                }
                if (coord > keys.Last())
                {
                    return _extendEndpoints ? keys.Last() : 0;
                }
                if (_values.ContainsKey(coord))
                {
                    return _values[coord];
                }
                double lower = keys.Last(v => v < coord);
                double upper = keys.First(v => v > coord);
                return (_values[lower] + _values[upper]) / 2;
            }
        }
    }

    public static class SpectrumExtensions
    {
        static MultipliedSpectrum Product(this IEnumerable<MultipliedSpectrum> spectra)
        {
            var allSubSpectra = spectra.SelectMany(s => s.Spectra);
            return new MultipliedSpectrum(allSubSpectra);
        }

        public static ISpectrum Product(this IEnumerable<ISpectrum> spectra)
        {
            var allProdSpectra = spectra.OfType<MultipliedSpectrum>();
            var subProd1 = allProdSpectra.Product();
            var subProd2 = new MultipliedSpectrum(spectra.Except(allProdSpectra));
            if (!subProd1.Spectra.Any() && !subProd2.Spectra.Any())
            {
                throw new ArgumentException("Product(spectra)");
            }
            if (!subProd1.Spectra.Any())
            {
                if (subProd2.Spectra.Skip(1).Any())
                {
                    return subProd2;
                }
                return subProd2.Spectra.First();
            }
            if (!subProd2.Spectra.Any())
            {
                if (subProd1.Spectra.Skip(1).Any())
                {
                    return subProd1;
                }
                return subProd1.Spectra.First();
            }
            return subProd1.MultiplyBy(subProd2);
        }

        public static ISpectrum MultiplyBy(this ISpectrum a, ISpectrum b)
        {
            MultipliedSpectrum ma = a as MultipliedSpectrum, mb = b as MultipliedSpectrum;
            if (ma != null && mb != null)
            {
                return new MultipliedSpectrum(ma.Spectra.Concat(mb.Spectra));
            }
            if (ma != null)
            {
                return new MultipliedSpectrum(ma.Spectra.Concat(new ISpectrum[] { b }));
            }
            if (mb != null)
            {
                return new MultipliedSpectrum(mb.Spectra.Concat(new ISpectrum[] { a }));
            }
            if (a is ConstantSpectrum && b is ConstantSpectrum)
            {
                ConstantSpectrum ca = a as ConstantSpectrum, cb = b as ConstantSpectrum;
                return ConstantSpectrum.Get(ca.Value * cb.Value);
            }
            return new MultipliedSpectrum(new ISpectrum[] { a, b });
        }

        static AddedSpectrum Sum(this IEnumerable<AddedSpectrum> spectra)
        {
            var allSubSpectra = spectra.SelectMany(s => s.Spectra);
            return new AddedSpectrum(allSubSpectra);
        }

        public static ISpectrum Sum(this IEnumerable<ISpectrum> spectra)
        {
            var allSumSpectra = spectra.OfType<AddedSpectrum>();
            var subSum1 = allSumSpectra.Sum();
            var subSum2 = new AddedSpectrum(spectra.Except(allSumSpectra));
            if (!subSum1.Spectra.Any() && !subSum2.Spectra.Any())
            {
                throw new ArgumentException("Sum(spectra)");
            }
            if (!subSum1.Spectra.Any())
            {
                if (subSum2.Spectra.Skip(1).Any())
                {
                    return subSum2;
                }
                return subSum2.Spectra.First();
            }
            if (!subSum2.Spectra.Any())
            {
                if (subSum1.Spectra.Skip(1).Any())
                {
                    return subSum1;
                }
                return subSum1.Spectra.First();
            }
            return subSum1.AddedTo(subSum2);
        }

        public static ISpectrum AddedTo(this ISpectrum a, ISpectrum b)
        {
            AddedSpectrum aa = a as AddedSpectrum, ab = b as AddedSpectrum;
            if (aa != null && ab != null)
            {
                return new AddedSpectrum(aa.Spectra.Concat(ab.Spectra));
            }
            if (aa != null)
            {
                return new AddedSpectrum(aa.Spectra.Concat(new ISpectrum[] { b }));
            }
            if (ab != null)
            {
                return new AddedSpectrum(ab.Spectra.Concat(new ISpectrum[] { a }));
            }
            if (a is ConstantSpectrum && b is ConstantSpectrum)
            {
                ConstantSpectrum ca = a as ConstantSpectrum, cb = b as ConstantSpectrum;
                return ConstantSpectrum.Get(ca.Value + cb.Value);
            }
            return new AddedSpectrum(new ISpectrum[] { a, b });
        }

        public static double Power(this ISpectrum spectrum, double minWavelength, double maxWavelength, int numSamples)
        {
            double result = 0;
            double sampleWidth = (maxWavelength - minWavelength) / numSamples;
            for (int sample = 0; sample < numSamples; sample++)
            {
                double sampleWavelength = minWavelength + sample * sampleWidth;
                result += spectrum[sampleWavelength] * sampleWidth;
            }
            return result;
        }
    }
}
