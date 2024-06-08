using Render.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParallelExtensions;

namespace Render.Engine
{
    public class Camera
    {
        private struct Coord
        {
            public int X;
            public int Y;
        }

        private class CoordVectorCalculator
        {
            public double RealHalfWidth;
            public double RealHalfHeight;
            public int Width;
            public int Height;

            public Vector GetCoordVector(Coord coord)
            {
                return new Vector((2 * coord.X / (double)(Width - 1) - 1) * RealHalfWidth, (2 * coord.Y / (double)(Height - 1) - 1) * RealHalfHeight, 1);
            }
        }

        private static readonly ISceneObject[] EmptyExceptions = new ISceneObject[0];

        public Camera()
        {
            Origin = new Vector(0, 0, 0);
        }

        public Scene Scene { get; set; }

        public Angle Horizontal { get; set; }

        public Angle Vertical { get; set; }

        public Vector Origin { get; private set; }

        private TColor[,] snap<TColor>(int width, int height, IRetina<TColor> retina, IProgress<double> progress)
        {
            TColor[,] pixels = new TColor[width, height];

            var calc = new CoordVectorCalculator()
            {
                RealHalfWidth = Math.Tan(Horizontal.Value0ToRight),
                RealHalfHeight = Math.Tan(Vertical.Value0ToRight),
                Width = width,
                Height = height
            };

            int count = 0;
            double total = 2 * width * height;
            var coords = new HashSet<Coord>(getCoordinates(width, height));
            coords.AsParallel()
                .Select(c =>
                {
                    try { return new { Coord = c, Rays = getPixelRays(calc, c) }; }
                    finally { lock (progress) progress.Report(++count / total); }
                })
                .ForAll(item =>
                {
                    var totalLight = item.Rays.LazyAwait().Select(r => r.AttenutatedLight).Combined();
                    pixels[item.Coord.X, item.Coord.Y] = retina[totalLight];
                    lock (progress) { progress.Report(++count / total); }
                });

            return pixels;
        }

        public TColor[,] Snap<TColor>(int width, int height, IRetina<TColor> retina, IProgress<double> progress = null)
        {
            return snap(width, height, retina, progress ?? new Progress<double>());
        }

        public Task<TColor[,]> SnapAsync<TColor>(int width, int height, IRetina<TColor> retina, IProgress<double> progress)
        {
            return Task.Run(() => snap(width, height, retina, progress));
        }

        private static IEnumerable<Coord> getCoordinates(int width, int height)
        {
            return Enumerable.Range(0, width).SelectMany(x => Enumerable.Range(0, height).Select(y => new Coord() { X = x, Y = y }));
        }

        private IEnumerable<Task<LightRay>> getPixelRays(CoordVectorCalculator calc, Coord coord)
        {
            return RayCalculation.Run(this.Scene, Origin.RayThrough(calc.GetCoordVector(coord)), EmptyExceptions);
        }
    }
}
