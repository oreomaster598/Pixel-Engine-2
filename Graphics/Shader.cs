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

        public static string Default = "half4 main(vec2 frag) { return half4(1,1,1,1); }";
        public static string Texture = "uniform fragmentProcessor map; uniform vec2 size; uniform vec2 isize; half4 main(vec2 frag) { vec2 scale = isize/size; return sample(map, frag*scale); }";
        private static bool Failed = false;
        public static Shader Compile(string sksl)
        {
            string errorText = "";
            Shader s = new Shader();
            SKRuntimeEffect effect = SKRuntimeEffect.Create(sksl, out errorText);
            if(effect == null)
            {
                Debug.LogError(errorText, "Shader");
                effect = SKRuntimeEffect.Create(Default, out errorText);
                Failed = true;
            }
            s.children = new SKRuntimeEffectChildren(effect);
            s.uniforms = new SKRuntimeEffectUniforms(effect);
            s.effect = effect;
            return s;
            
        }

        public static Shader CompileFromFile(string path)
        {
            string errorText = "";
            Shader s = new Shader();
            SKRuntimeEffect effect = SKRuntimeEffect.Create(File.ReadAllText(path),out errorText);

            if (effect == null)
            {
                Debug.LogError(errorText, "Shader");
                effect = SKRuntimeEffect.Create(Default, out errorText);
                Failed = true;
            }

            s.children = new SKRuntimeEffectChildren(effect);
            s.uniforms = new SKRuntimeEffectUniforms(effect);
            s.effect = effect;
            return s;

        }

        public void setUniforms(params Uniform[] uniforms)
        {
            if (Failed)
                return;
            foreach (Uniform uniform in uniforms)
            {
                this.uniforms[uniform.name] = uniform.value;
            }

        }

        public void useBitmap(SKBitmap bitmap)
        {
            if (Failed)
                return;
            if (bitmap != null)
            {
                    children.Add("color_map", bitmap.ToShader());

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
