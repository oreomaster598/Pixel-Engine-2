using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PE2.Math;
using SkiaSharp;

namespace PE2.Graphics
{
    public class Bloom
    {
        public float Intensity = 1;
        public float Radius = 10000;
        public Vector3 Color = new Vector3(1, 1, 1);
    }

    public abstract class PostProcess
    {
        public Shader postproc;

        public PostProcess(Shader postproc)
        {
            this.postproc = postproc;
        }

        public abstract void Update();

    }

    public class PostProcessing : PostProcess
    {
        public PostProcessing(Shader postproc) : base(postproc) { }

        public Bloom bloom = new Bloom();

        public float Vignette = 0;
        public float ColorDepth = 100;

        public float Strength = 0.01f;
        public float ChromaticAberration = 0;

        public Vector3 ChannelMixer = new Vector3(1, 1, 1);


        public override void Update()
        {
            postproc.setUniforms(new Uniform("BloomSize", bloom.Radius), new Uniform("Strength", Strength), new Uniform("Separation", ChromaticAberration), new Uniform("Resolution", Main.CurrentCamera.resolution.ToArray()), new Uniform("ChannelMixer", ChannelMixer.ToArray()), new Uniform("Vignette", Vignette), new Uniform("ColorDepth", ColorDepth), new Uniform("Bloom", bloom.Intensity), new Uniform("BloomColor", bloom.Color.ToArray()));
            postproc.useBitmap(Main.CurrentCamera.bmp);
        }

    }
}
