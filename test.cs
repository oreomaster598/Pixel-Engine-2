using PE2.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;
using Script = PE2.Components.Component;

namespace PE2
{
    public class test : Script
    {
        public override void Update()
        {
            Vector2 position = gameObject.position;
            if (Input.isKeyDown(Key.A))
                gameObject.position -= new Vector2(1, 0);
            if (Input.isKeyDown(Key.D))
                gameObject.position -= new Vector2(-1, 0);
            if (Input.isKeyDown(Key.W))
                gameObject.position -= new Vector2(0, 1);
            if (Input.isKeyDown(Key.S))
                gameObject.position -= new Vector2(0, -1);

            foreach(GameObject gm in Main.gameObjects)
            {
                if (gm == gameObject)
                    continue;

                if (Math.Mathf.Colliding(gameObject, gm))
                    gameObject.position = position;
            }
        }
    }
}
