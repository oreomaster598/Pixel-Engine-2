using SkiaSharp;

namespace PE2.Math
{
    public struct Vector3
    {
        public float x;
        public float z;
        public float y;

        public static readonly Vector3 Zero = new Vector3(0f, 0f);
        public static Vector3 Parse(string s)
        {
            string[] sa = s.Split(',');
            float x = float.Parse(sa[0]);
            float y = float.Parse(sa[1]);

            return new Vector3(x, y);
        }

        public Vector3(float x = 0, float y = 0, float z = 0)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Vector3 operator +(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }
        public static Vector3 operator -(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }
        public static Vector3 operator *(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
        }
        public static Vector3 operator /(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z);
        }
        public static Vector3 operator *(Vector3 v1, float f)
        {
            return new Vector3(v1.x * f, v1.y * f, v1.z * f);
        }
        public static Vector3 operator /(Vector3 v1, float f)
        {
            return new Vector3(v1.x / f, v1.y / f, v1.z / f);
        }
        public static Vector3 operator +(Vector3 v1, float f)
        {
            return new Vector3(v1.x + f, v1.y + f, v1.z + f);
        }
        public static Vector3 operator -(Vector3 v1, float f)
        {
            return new Vector3(v1.x - f, v1.y - f, v1.z - f);
        }
        public static bool operator >(Vector3 v1, Vector3 v2)
        {
            return (v1.x > v2.x && v1.y > v2.y && v1.z > v2.z);
        }
        public static bool operator <(Vector3 v1, Vector3 v2)
        {
            return (v1.x < v2.x && v1.y < v2.y && v1.z < v2.z);
        }
        public static bool operator ==(Vector3 v1, Vector3 v2)
        {
            return (v1.x == v2.x && v1.y == v2.y & v1.z == v2.z);
        }
        public static bool operator !=(Vector3 v1, Vector3 v2)
        {
            return !(v1.x == v2.x && v1.y == v2.y && v1.z == v2.z);
        }
        public static Vector3 Cap(Vector3 min, Vector3 max, Vector3 v)
        {
            if (v > max)
                return max;
            if(v < min)
                return min;
            return v;
        }
        public static Vector3 Cap(float min, float max, Vector3 v)
        {
            float x = v.x;
            float y = v.y;
            float z = v.z;
            if (v.x >= max)
                x = max;
            if (v.y >= max)
                y = max;
            if (v.z >= max)
                z = max;
            if (v.x <= min)
                x = min;
            if (v.y <= min)
                y = min;
            if (v.z <= min)
                z = min;


            return new Vector3(x,y,z);
        }


        public override string ToString() => $"{x},{y}";

        public static implicit operator Vector3(SKPoint p) => new Vector3(p.X, p.Y);
        public static implicit operator SKPoint(Vector3 p) => new SKPoint(p.x, p.y);

        public void Clamp(Vector3 min, Vector3 max)
        {
            if (this.x < min.x)
                this.x = min.x;
            if (this.y < min.y)
                this.y = min.y;
            if (this.x > max.x)
                this.x = max.x;
            if (this.y > max.y)
                this.y = max.y;
        }

        public float[] ToArray()
        {
            return new float[] { x, y, z };
        }

        public float this[int index]
        {
            get
            {
                if (index == 0)
                    return x;
                else if (index == 1)
                    return y;
                return 0;
            }
            set
            {
                if (index == 0)
                    x = value;
                else if (index == 1)
                    y = value;
            }
        }
    }
}
