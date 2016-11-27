using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.Engine
{
    public interface IRetina<TColor>
    {
        TColor this[Light light] { get; }
    }
}
