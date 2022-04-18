using PE2.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PE2
{
    public struct WindowSettings
    {
        public bool VSync;
        public Vector2 Size;
        public string Title;
        public int TargetFPS;
    }

    public struct GameSettings
    {
        public bool DebugMode;
    }
}
