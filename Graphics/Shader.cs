using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace PE2.Graphics
{
    public struct Uniform
    {
        public string name;
        public float[] value;

        public Uniform(string name, float value)
        {
            this.name = name;
            this.value = new float[] { value };
        }
        public Uniform(string name, float[] value)
        {
            this.name = name;
            this.value = value;
        }
    }
    public class Shader
    {
        public SKRuntimeEffect effect;
        public SKRuntimeEffectUniforms uniforms;
        public SKRuntimeEffectChildren children;
        public static Shader Compile(string sksl)
        {
            string errorText = "";
            Shader s = new Shader();
            SKRuntimeEffect effect = SKRuntimeEffect.Create(sksl, out errorText);
            Console.WriteLine(errorText);
            s.children = new SKRuntimeEffectChildren(effect);
            s.uniforms = new SKRuntimeEffectUniforms(effect);
            s.effect = effect;
            return s;
            
        }

        public static Shader CompileFromFile(string path)
        {
            Shader s = new Shader();
            SKRuntimeEffect effect = SKRuntimeEffect.Create(File.ReadAllText(path), out var errorText);
            s.children = new SKRuntimeEffectChildren(effect);
            s.uniforms = new SKRuntimeEffectUniforms(effect);
            s.effect = effect;
            return s;

        }

        public void setUniforms(params Uniform[] uniforms)
        {
            foreach (Uniform uniform in uniforms)
            {
                this.uniforms = new SKRuntimeEffectUniforms(effect);
                if (uniform.value.Length > 1)
                    this.uniforms.Add(uniform.name, uniform.value);
                else if (uniform.value.Length > 0)
                    this.uniforms.Add(uniform.name, uniform.value[0]);
            }

        }

        public void useBitmap(SKBitmap bitmap)
        {
            if (bitmap != null)
            {
                SKShader textureShader = bitmap.ToShader();
                children.Add("color_map", textureShader);
            }
        }

        public SKPaint UseShader()
        {
            SKShader sdr = effect.ToShader(true, uniforms, children);


            SKPaint paint = new SKPaint();
            paint.Shader = sdr;
            return paint;
        }
    }
}
