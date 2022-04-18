using PE2.Components;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;
using PE2.Graphics;
using OpenTK.Audio.OpenAL;

using System.Runtime.InteropServices;
using PE2.Math;
using PE2.Media;
using System.Threading;

namespace PE2
{
    public class Game : Main
    {
        public static Camera cam1;
        public static Camera cam2;
        public GameObject player;

        public Game(GameSettings gameSettings, WindowSettings windowSettings) : base(gameSettings, windowSettings)
        {

        }
        public override void Update()
        {
            



        }

        SKBitmap bmp;
        SKBitmap BG;

        public override void LoadContent()
        {

            bmp = SKBitmap.Decode("player.png");
            BG = SKBitmap.Decode("BG.png");

            //Create Camera
            cam1 = new Camera(Vector2.Zero, windowSettings.Size, new PostProcessing(Shader.CompileFromFile("postproc.glsl")));
            ((PostProcessing)cam1.process).bloom.Intensity = 1f;
            ((PostProcessing)cam1.process).bloom.Radius = 3500f;

            cam2 = new Camera(Vector2.Zero, windowSettings.Size, new PostProcessing(Shader.CompileFromFile("postproc.glsl")));
            ((PostProcessing)cam2.process).bloom.Intensity = 0f;

            player = new GameObject(new Vector2(50,50), new Vector2(0,0), new Player(), new Sprite(bmp));


            CurrentCamera = cam1;
            RegisterGameObject(player);
        }

        public override void PreUpdate()
        {
            if(radioButton1.Checked && CurrentCamera != cam2)
                CurrentCamera = cam2;
            else if (radioButton2.Checked && CurrentCamera != cam1)
                CurrentCamera = cam1;
        }

        public override void PreDraw()
        {
            Renderer.DrawBitmap(BG, CurrentCamera.resolution, CurrentCamera.resolution / 2);
        }
        public override void Draw()
        {
            Renderer.DrawCircle(new Vector2(0,0), 20, new SKPaint() { Color = new SKColor(255,255,255), IsAntialias = true});
        }

    }

}
