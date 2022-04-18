using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PE2.Math;

namespace PE2.Graphics
{
    public static class Renderer
    {
        private static SKCanvas GL;
        private static bool canDraw;
        public static uint DrawCalls = 0;
        public static uint batches = 0;
         public static void Begin()
        //public static void Begin(SKCanvas canvas)
        {
            GL = new SKCanvas(Main.CurrentCamera.bmp);
            //GL = canvas;
            GL.Clear(new SKColor(0,0,0));

            canDraw = true;
            DrawCalls = 0;
            batches = 0;
        }

        private static SKRect ToRect(Vector2 pos, Vector2 size)
        {
            return new SKRect(pos.x, pos.y, pos.x+size.x, pos.y+size.y);
        }

        public static void Transform()
        {
            GL.Translate(Main.CurrentCamera.position.x, Main.CurrentCamera.position.y);
        }

        public static void End(ref SKCanvas canvas)
       // public static void End()
        {
            canDraw = false;
            GL.Flush();
            GL.Dispose();
            Main.CurrentCamera.process.Update();
            canvas.DrawRect(0, 0, Main.windowSettings.Size.x, Main.windowSettings.Size.y, Main.CurrentCamera.process.postproc.UseShader());
        }

        public static void DrawBitmap(SKBitmap bmp, Vector2 size, Vector2 pos, SKPaint paint = null)
        {
            if (!canDraw)
                return;
            if (paint != null)
                GL.DrawBitmap(bmp, ToRect(pos-size/2, size), paint);
            else
                GL.DrawBitmap(bmp, ToRect(pos-size/2, size), new SKPaint());
            DrawCalls++;
        }

        public static void DrawLine(SKPoint point1, SKPoint point2, SKPaint paint)
        {
            if (!canDraw)
                return;
            GL.DrawLine(point1, point2, paint);
            DrawCalls++;
        }

        public static void DrawText(string text, Vector2 position, SKColor color, int size = 12)
        {
            if (!canDraw)
                return;
            using (SKPaint paint = new SKPaint())
            {
                paint.Color = color;
                paint.TextSize = size;
                paint.Typeface = Main.font;
                GL.DrawText(text, position, paint);
            }
           
        }

        public static void DrawRect(Vector2 size, Vector2 position, SKPaint paint)
        {
            if (!canDraw)
                return;
            if(paint != null)
            GL.DrawRect(position.x, position.y, size.x, size.y, paint);
            //DrawCalls++;
        }

        public static void DrawPoly(SKPoint[] points, SKPaint paint)
        {
            if (!canDraw)
                return;

            GL.DrawPoints(SKPointMode.Polygon, points, paint);
            //DrawCalls++;
        }

        public static void DrawPath(SKPath path, SKPaint paint)
        {
            if (!canDraw)
                return;

            GL.DrawPath(path, paint);
            //DrawCalls++;
            //batches++;
        }

        public static void DrawCircle(Vector2 position, float radius, SKPaint paint)
        {
            if (!canDraw)
                return;

            GL.DrawCircle(position.x, position.y, radius, paint);
            //DrawCalls++;
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
