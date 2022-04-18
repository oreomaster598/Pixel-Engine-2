using PE2.Graphics;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PE2.Components
{
    public class Sprite : Component
    {
        SKBitmap bitmap;
        Shader shader;
        public Sprite(SKBitmap bitmap)
        {
            this.bitmap = bitmap;
        }

        public Sprite(SKBitmap bitmap, Shader shader)
        {
            this.bitmap = bitmap;
            this.shader = shader;
            this.shader.useBitmap(bitmap);
        }
        public Sprite(Shader shader)
        {
            this.shader = shader;
            //this.shader.useBitmap(bitmap);
        }

        public override void Draw()
        {
            if (shader != null)
                Renderer.DrawRect(gameObject.size, gameObject.position, new SKPaint() { Color = new SKColor(255,255,255)});
            else
                Renderer.DrawBitmap(bitmap, gameObject.size, gameObject.position);
        }

        public override void PreUpdate()
        {

        }

        public override void Update()
        {

        }
    }
}
