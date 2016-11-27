using Render.Engine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderTest1
{
    class BnWRetina : IRetina<Color>
    {
        public Color this[Light light]
        {
            get { return light.Spectrum[0] > 0 ? Color.White : Color.Black; }
        }
    }

    class GreyScaleRetina : IRetina<Color>
    {
        public Color this[Light light]
        {
            get
            {
                double intensity = light.Spectrum[0];
                if (intensity > 1)
                    intensity = 1;
                if (intensity < 0)
                    intensity = 0;
                int val = (int)(255 * intensity);
                return Color.FromArgb(val, val, val);
            }
        }
    }

    class RGBRetina : IRetina<Color>
    {
        private const double CIE_Xr = 0.67, CIE_Yr = 0.33;
        private const double CIE_Xg = 0.21, CIE_Yg = 0.71;
        private const double CIE_Xb = 0.14, CIE_Yb = 0.08;
        private const double CIE_Xw = 0.3101, CIE_Yw = 0.3162;
        private static readonly InterpolatedSpectrum CIE_X, CIE_Y, CIE_Z;
        private static readonly double[,] CIE_Matrix;

        static RGBRetina()
        {
            Dictionary<double, double> xWavelengths = new Dictionary<double, double>();
            xWavelengths[400] = 0;
            xWavelengths[450] = 0.3;
            xWavelengths[500] = 0;
            xWavelengths[600] = 1;
            xWavelengths[700] = 0;
            CIE_X = new InterpolatedSpectrum(xWavelengths, false);

            Dictionary<double, double> yWavelengths = new Dictionary<double, double>();
            yWavelengths[450] = 0;
            yWavelengths[550] = 1;
            yWavelengths[700] = 0;
            CIE_Y = new InterpolatedSpectrum(yWavelengths, false);

            Dictionary<double, double> zWavelengths = new Dictionary<double, double>();
            zWavelengths[400] = 0;
            zWavelengths[450] = 1.75;
            zWavelengths[525] = 0;
            CIE_Z = new InterpolatedSpectrum(zWavelengths, false);

            var a = new double[,] {
                { CIE_Xr, CIE_Xg, CIE_Xb },
                { CIE_Yr, CIE_Yg, CIE_Yb },
                { 1 - CIE_Xr - CIE_Yr, 1 - CIE_Xg - CIE_Yg, 1 - CIE_Xb - CIE_Yb }
            };
            var b = new double[,] {
                { subdet(a, 1, 2, 1, 2), subdet(a, 2, 0, 1, 2), subdet(a, 0, 1, 1, 2) },
                { subdet(a, 1, 2, 2, 0), subdet(a, 2, 0, 2, 0), subdet(a, 0, 1, 2, 0) },
                { subdet(a, 1, 2, 0, 1), subdet(a, 2, 0, 0, 1), subdet(a, 0, 1, 0, 1) }
            };
            double invdet = 1.0 / (a[0, 0] * b[0, 0] - a[0, 1] * b[1, 0] + a[0, 2] * b[2, 0]);
            CIE_Matrix = new double[,] {
                { invdet * b[0,0], invdet * b[0, 1], invdet * b[0, 2] },
                { invdet * b[1,0], invdet * b[1, 1], invdet * b[1, 2] },
                { invdet * b[2,0], invdet * b[2, 1], invdet * b[2, 2] }
            };
        }

        private double _maxIntensity;

        public RGBRetina(double maxIntensity)
        {
            _maxIntensity = maxIntensity;
        }

        public Color this[Light light]
        {
            get
            {
                var xResponse = light.TransformedBy(CIE_X);
                var yResponse = light.TransformedBy(CIE_Y);
                var zResponse = light.TransformedBy(CIE_Z);

                double cieX = intensity(xResponse);
                double cieY = intensity(yResponse);
                double cieZ = intensity(zResponse);
                
                double cieSum = cieX + cieY + cieZ;
                if (cieSum == 0)
                    return Color.Black;
                double ciex = cieX / cieSum;
                double ciey = cieY / cieSum;
                double ciez = 1 - ciex - ciey;

                double cieJr, cieJg, cieJb;
                cieTransform(ciex, ciey, ciez, out cieJr, out cieJg, out cieJb);
                unitBound(ref cieJr);
                unitBound(ref cieJg);
                unitBound(ref cieJb);

                double brightness = 255 * cieY / _maxIntensity;

                int r = (int)(brightness * cieJr), g = (int)(brightness * cieJg), b = (int)(brightness * cieJb);
                if (r > 255) r = 255;
                if (g > 255) g = 255;
                if (b > 255) b = 255;
                return Color.FromArgb(r, g, b);
            }
        }

        private static double subdet(double[,] m, int row1, int row2, int col1, int col2)
        {
            return m[row1, col1] * m[row2, col2] - m[row1, col2] * m[row2, col1];
        }

        private static void cieTransform(double x, double y, double z, out double Jr, out double Jg, out double Jb)
        {
            Jr = x * CIE_Matrix[0, 0] + y * CIE_Matrix[0, 1] + z * CIE_Matrix[0, 2];
            Jg = x * CIE_Matrix[1, 0] + y * CIE_Matrix[1, 1] + z * CIE_Matrix[1, 2];
            Jb = x * CIE_Matrix[2, 0] + y * CIE_Matrix[2, 1] + z * CIE_Matrix[2, 2];
        }

        private static double intensity(Light light)
        {
            return light.Spectrum.Power(380, 780, 41);
        }

        private static void unitBound(ref double val)
        {
            if (val < 0) val = 0;
            if (val > 1) val = 1;
        }
    }
}
