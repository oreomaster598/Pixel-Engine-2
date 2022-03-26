using PE2.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PE2
{
    public class GameObject
    {
        public Vector2 position;
        public Vector2 size;
        public Component[] components;

        public GameObject(Vector2 size, Vector2 position, params Component[] components)
        {
            this.size = size;
            this.position = position;
            this.components = components;

            foreach (Component component in this.components)
            {
                component.gameObject = this;
                component.Start();
            }

        }


        public Component GetComponent<T>()
        {
            foreach(Component c in components)
            {
                if(c.GetType().IsAssignableFrom(typeof(T)))
                {
                    return c;
                }
            }
            return null;
        }
        public void Update()
        {
            foreach(Component component in components)
                component.Update();
        }
        public void PreUpdate()
        {
            foreach (Component component in components)
                component.PreUpdate();
        }
        public void Draw()
        {
            foreach (Component component in components)
                component.Draw();
        }
    }
}
