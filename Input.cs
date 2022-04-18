using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Input;
using PE2.Components;
using PE2.Math;

namespace PE2
{
    public static class Input
    {
        private static KeyboardState state;
        internal static MouseState mouseState;
        public static Vector2 MousePosition;


        internal static void Update()
        {
            state = Keyboard.GetState();
            mouseState = Mouse.GetState();
        }
        public static bool isKeyDown(Key key)
        {
            return state.IsKeyDown(key);
        }

        public static bool isKeyUp(Key key)
        {
            return state.IsKeyUp(key);
        }
        public static bool isMouseDown(MouseButton btn)
        {
            return mouseState.IsButtonDown(btn);
        }

        public static bool isMouseUp(MouseButton btn)
        {
            return mouseState.IsButtonUp(btn);
        }
    }
}
