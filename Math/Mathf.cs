using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PE2.Math
{
    public static class Mathf
    {

        public static bool Colliding(GameObject A, GameObject B) => A.position.x < B.position.x + B.size.x && A.position.x + A.size.x > B.position.x && A.position.y < B.position.y + B.size.y && A.position.y + A.size.y > B.position.y;
    }
}
