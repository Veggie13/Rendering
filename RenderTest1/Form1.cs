using Render.Engine;
using Render.Geometry;
using Render.Materials;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RenderTest1
{
    public partial class Form1 : Form
    {
        Scene _scene = new Scene();
        Camera _camera;
        IRetina<Color> _retina = new RGBRetina(100);
        Angle _angle = Angle.FromDegrees(45);

        public Form1()
        {
            InitializeComponent();

            _camera = new Camera()
            {
                Scene = _scene,
                Horizontal = _angle,
                Vertical = _angle
            };

            var reflectiveMaterial = new SpecularMaterial(ConstantSpectrum.Get(1));
            var whiteMaterial = new MatteMaterial(ConstantSpectrum.Get(1));
            var darkMaterial = new MatteMaterial(ConstantSpectrum.Get(0.1));
            
            _scene.AddObject(new LightSource(new Sphere(new Vector(0, 5, 10), 0.5), Light.White(5000)));

            _scene.AddObject(whiteMaterial.ToObject(new RectangularRegion(-11, -10, -20, 20, -10, 10))); // left
            _scene.AddObject(whiteMaterial.ToObject(new RectangularRegion(-11, 11, -20, 20, -11, -10))); // top
            _scene.AddObject(whiteMaterial.ToObject(new RectangularRegion(-11, 11, -20, 20, 10, 11))); // bottom
            _scene.AddObject(reflectiveMaterial.ToObject(new RectangularRegion(10, 11, -20, 20, -10, 10))); // right
            _scene.AddObject(reflectiveMaterial.ToObject(new RectangularRegion(-11, 11, 20, 21, -11, 11))); // back
            _scene.AddObject(reflectiveMaterial.ToObject(new RectangularRegion(-11, 11, -21, -20, -11, 11))); // behind
            _scene.AddObject(darkMaterial.ToObject(new RectangularRegion(2, 3, 9, 11, 4, 7))); // block
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            progressBar1.Maximum = 100;
            var imageTask = new Task<Color[,]>(() => _camera.Snap(panel1.Width, panel1.Height, _retina, new Progress<double>(progress =>
            {
                Invoke(new Action(() => { progressBar1.Value = (int)(100 * progress); }));
            })));
            imageTask.GetAwaiter().OnCompleted(() =>
            {
                var image = imageTask.Result;
                Bitmap bm = new Bitmap(panel1.Width, panel1.Height);
                for (int x = 0; x < panel1.Width; x++)
                {
                    for (int y = 0; y < panel1.Height; y++)
                    {
                        bm.SetPixel(x, y, image[x, y]);
                    }
                }

                panel1.BackgroundImage = bm;
            });
            imageTask.Start();
        }
    }

    static partial class Extensions
    {
        public static double RMS(this IEnumerable<double> values)
        {
            return Math.Sqrt(values.Select(v => v * v).Average());
        }

        public static double RMS<T>(this IEnumerable<T> items, Func<T, double> f)
        {
            return items.Select(i => f(i)).RMS();
        }

        public static Color Blend(this IEnumerable<Color> colors)
        {
            return Color.FromArgb((int)colors.RMS(c => (double)c.R), (int)colors.RMS(c => (double)c.G), (int)colors.RMS(c => (double)c.B));
        }
    }
}
