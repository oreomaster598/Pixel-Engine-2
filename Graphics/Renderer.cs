using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PE2.Graphics
{
    public static class Renderer
    {
        private static SKCanvas GL;
        private static bool canDraw;
        public static uint DrawCalls = 0;
        public static uint batches = 0;
        public static void Begin(SKCanvas canvas)
        {
            GL = canvas;
            canDraw = true;
            DrawCalls = 0;
            batches = 0;
        }

        public static void End()
        {
            canDraw = false;
        }

        public static void DrawBitmap(SKBitmap bmp, Vector2 size, Vector2 pos, SKPaint paint = null)
        {
            if (!canDraw)
                return;
            if (paint != null)
                GL.DrawBitmap(bmp.Resize(new SKImageInfo((int)size.x, (int)size.y), SKFilterQuality.None), pos, paint);
            else
                GL.DrawBitmap(bmp.Resize(new SKImageInfo((int)size.x, (int)size.y), SKFilterQuality.None), pos, paint);
            DrawCalls++;
        }

        public static void DrawLine(SKPoint point1, SKPoint point2, SKPaint paint)
        {
            if (!canDraw)
                return;

            GL.DrawLine(point1, point2, paint);
            DrawCalls++;
        }

        public static void DrawRect(Vector2 size, Vector2 position, SKPaint paint)
        {
            if (!canDraw)
                return;

            GL.DrawRect(position.x, position.y, size.x, size.y, paint);
            DrawCalls++;
        }

        public static void DrawPoly(SKPoint[] points, SKPaint paint)
        {
            if (!canDraw)
                return;

            GL.DrawPoints(SKPointMode.Polygon, points, paint);
            DrawCalls++;
        }

        public static void DrawPath(SKPath path, SKPaint paint)
        {
            if (!canDraw)
                return;

            GL.DrawPath(path, paint);
            DrawCalls++;
            batches++;
        }

        public static SKPoint[] CalculateVertices(SKPoint[] points, Vector2 position, Vector2 size)
        {
            List<SKPoint> lpoints = new List<SKPoint>();
            foreach(SKPoint p in points)
            {
                lpoints.Add(p*size+position);
            }
            return lpoints.ToArray();
        }
    }
}
