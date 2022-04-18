using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using PE2.Math;

namespace PE2.Graphics
{
    public class Camera
    {
        public Vector2 position;
        public Vector2 resolution;


        public PostProcess process;
        public SKBitmap bmp;
        public Camera(Vector2 position, Vector2 resolution, PostProcess process)
        {
            this.position = position;
            this.process = process;
            this.resolution = resolution;

            bmp = new SKBitmap(new SKImageInfo((int)resolution.x, (int)resolution.y));
        }

        /// <summary>
        /// Call after changing resolution 
        /// </summary>
        public void UpdateBitmap()
        {
            bmp = new SKBitmap(new SKImageInfo((int)resolution.x, (int)resolution.y));
        }
    }
}
