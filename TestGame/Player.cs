using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;
using PE2.Components;
using PE2.Graphics;
using PE2.Math;

namespace PE2
{
    public class Player : Component
    {
        public override void Load()
        {

        }

        public override void Update()
        {
            if (Input.isKeyDown(Key.A))
                gameObject.position -= new Vector2(1, 0);
            if (Input.isKeyDown(Key.D))
                gameObject.position -= new Vector2(-1, 0);
            if (Input.isKeyDown(Key.W))
                gameObject.position -= new Vector2(0, 1);
            if (Input.isKeyDown(Key.S))
                gameObject.position -= new Vector2(0, -1);

            //if(Main.CurrentCamera.position != gameObject.position)
            Main.CurrentCamera.position = (-gameObject.position)+Main.CurrentCamera.resolution/2;
        }
    }
}
