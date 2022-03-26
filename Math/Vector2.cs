using SkiaSharp;

namespace PE2
{
    public struct Vector2
    {
        public float x;
        public float y;

        public static readonly Vector2 Zero = new Vector2(0f, 0f);
        public static Vector2 Parse(string s)
        {
            string[] sa = s.Split(',');
            float x = float.Parse(sa[0]);
            float y = float.Parse(sa[1]);

            return new Vector2(x, y);
        }

        public Vector2(float x = 0, float y = 0)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x + v2.x, v1.y + v2.y);
        }
        public static Vector2 operator -(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x - v2.x, v1.y - v2.y);
        }
        public static Vector2 operator *(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x * v2.x, v1.y * v2.y);
        }
        public static Vector2 operator /(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x / v2.x, v1.y / v2.y);
        }
        public static Vector2 operator *(Vector2 v1, float f)
        {
            return new Vector2(v1.x * f, v1.y * f);
        }
        public static Vector2 operator /(Vector2 v1, float f)
        {
            return new Vector2(v1.x / f, v1.y / f);
        }
        public static bool operator >(Vector2 v1, Vector2 v2)
        {
            return (v1.x > v2.x && v1.y > v2.y);
        }
        public static bool operator <(Vector2 v1, Vector2 v2)
        {
            return (v1.x < v2.x && v1.y < v2.y);
        }

        

        public override string ToString() => $"{x},{y}";

        public static implicit operator Vector2(SKPoint p) => new Vector2(p.X, p.Y);
        public static implicit operator SKPoint(Vector2 p) => new SKPoint(p.x, p.y);

        public void Clamp(Vector2 min, Vector2 max)
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
