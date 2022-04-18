using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PE2.Math
{
    public struct Matrix2
    {
        public Vector2[] values;

        public Matrix2(Vector2[] values)
        {
            this.values = values;
        }
        public Matrix2(float x = 0, float y = 0, float xx = 0, float yy = 0)
        {
            this.values = new Vector2[] { new Vector2(x,y) , new Vector2(xx,yy)};
        }
    }
   
    public enum Op
    {
        Greater,
        Lesser,
        GreaterEqual,
        LesserEqual,
        Equal
    }
    public static class Mathf
    {
        public static bool OpIgnoreSign(Op Operator, float f1, float f2)
        {
            if (f1 < 0)
                f1 *= -1;
            if (f2 < 0)
                f2 *= -1;
            switch(Operator)
            {
                case Op.Greater:
                    return f1 > f2;
                case Op.Lesser:
                    return f1 < f2;
                case Op.GreaterEqual:
                    return f1 >= f2;
                case Op.LesserEqual:
                    return f1 <= f2;
                case Op.Equal:
                    return f1 == f2;
                default:
                    return false;
            }
        }

        public static float MinReturnZero(float value, float min)
        {
            if (value < min)
                return 0;
            return value;
        }
        public static float MaxReturnZero(float value, float max)
        {
            if (value > max)
                return 0;
            return value;
        }

    }
}
