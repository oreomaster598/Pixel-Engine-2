using PE2.Math;
using PE2.Physics.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PE2.Components
{
    public class RigidBody : Component
    {
        public Body body;
        public BodyDef bodyDef;
        bool isstatic;

        public RigidBody(bool isstatic = false)
        {
            this.isstatic = isstatic;
        }
        public override void Load()
        {
            if (isstatic)
            {
                bodyDef = new BodyDef();
                bodyDef.Position = gameObject.position;
                body = Main.world.CreateBody(bodyDef);

                PolygonDef shapeDef = new PolygonDef();
                shapeDef.SetAsBox(gameObject.size.x, gameObject.size.y);

                body.CreateFixture(shapeDef);
                body.SetMassFromShapes();
            }
            else
            {
                bodyDef = new BodyDef();
                bodyDef.Position = gameObject.position;
                body = Main.world.CreateBody(bodyDef);

                PolygonDef shapeDef = new PolygonDef();
                shapeDef.SetAsBox(gameObject.size.x, gameObject.size.y);


                shapeDef.Density = 1.0f;
                shapeDef.Friction = 0.3f;


                body.CreateFixture(shapeDef);
                body.SetMassFromShapes();
            }
        }
        public override void Update()
        {
            if(!isstatic)
            gameObject.position = body.GetPosition();
        }
    }
}
