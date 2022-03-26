using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PE2.Components;
using PE2.Graphics;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using OpenTK.Input;

namespace PE2
{

    public partial class Main : Form
    {
        public static bool run = true;

        public static Vector2 mousePosition = Vector2.Zero;

        public static List<GameObject> gameObjects = new List<GameObject>();
        public SKColor Background = new SKColor(200,200,200);
        public static int fps = 0;
        public SKBitmap bmp;


        public GameSettings gameSettings;
        public WindowSettings windowSettings;

        public Main(GameSettings gameSettings, WindowSettings windowSettings)
        {
            this.gameSettings = gameSettings;
            this.windowSettings = windowSettings;
            InitializeComponent();

            this.ClientSize = new Size((int)windowSettings.Size.x, (int)windowSettings.Size.y);
            this.Text = windowSettings.Title;
            
            skgl.VSync = windowSettings.VSync;

            bmp = SKBitmap.Decode("img.png");

            gameObjects.Add(new GameObject(new Vector2(20, 20), new Vector2(50, 50), new Sprite(bmp), new test()));

            gameObjects.Add(new GameObject(new Vector2(50, 50), new Vector2(150, 150), new Sprite(bmp)));


        }

        public static double DeltaTime;
        public static double Secondframe;
        public void Loop()
        {
            while(run)
            {
                try
                {
                    Thread.Sleep(1);

                    fps = (int)GetFps();

                    foreach (GameObject gm in gameObjects)
                        gm.PreUpdate();
                    PreUpdate();
                    this.BeginInvoke((MethodInvoker)delegate { skgl.Invalidate(); });
                    Input.Update();
                    foreach (GameObject gm in gameObjects)
                        gm.Update();
                    if (Input.isKeyDown(Key.Minus) && gameSettings.DebugMode && gridres >= 10)
                        gridres--;
                    else if(Input.isKeyDown(Key.Plus) && gameSettings.DebugMode && gridres <= 100)
                        gridres++;
                    Update();
                }
                catch (Exception ex)
                {

                }

            }
            

        }


        public virtual void Update() { }

        public virtual void PreUpdate() { }

        int gridres = 25;

        public int ParseNoRound(float f)
        {
            string s = f.ToString();
            if(s.Contains('.'))
                return int.Parse(s.Remove(s.IndexOf('.')));
            return int.Parse(s);
        }

        DateTime _lastCheckTime = DateTime.Now;
        long _frameCount = 0;

        // called whenever a map is updated
        void OnMapUpdated()
        {
            Interlocked.Increment(ref _frameCount);
        }

        // called every once in a while
        public double GetFps()
        {
            double secondsElapsed = (DateTime.Now - _lastCheckTime).TotalSeconds;
            long count = Interlocked.Exchange(ref _frameCount, 0);
            double fps = count / secondsElapsed;
            _lastCheckTime = DateTime.Now;
            return fps;
        }

        private void skglControl1_PaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
        {
            SKCanvas canvas = e.Surface.Canvas;
            canvas.Clear(Background);

            Renderer.Begin(canvas);
            Rectangle Bounds = skgl.ClientRectangle;
            if (gameSettings.DebugMode)
            {
                SKPath p = new SKPath();
                int xlines = Bounds.Width / gridres + 1;
                int ylines = Bounds.Height / gridres + 1;

                using (var paint = new SKPaint())
                {
                    paint.Color = new SKColor(64, 64, 64);
                    paint.Style = SKPaintStyle.Stroke;
                    paint.StrokeWidth = 1;

                    // Draw the Horizontal Grid Lines
                    for (int i = 0; i < ylines; i++)
                    {
                        var y = i * gridres;
                        var leftPoint = new SKPoint(Bounds.Left, y);
                        var rightPoint = new SKPoint(Bounds.Right, y);

                        p.AddPoly(new SKPoint[] { leftPoint, rightPoint });
                    }

                    // Draw the Vertical Grid Lines
                    for (int i = 0; i < xlines; i++)
                    {
                        var x = i * gridres;
                        var topPoint = new SKPoint(x, Bounds.Top);
                        var bottomPoint = new SKPoint(x, Bounds.Bottom);

                        p.AddPoly(new SKPoint[] { topPoint, bottomPoint });
                    }
                    Renderer.DrawPath(p, paint);
                }
            }

            foreach (GameObject gm in gameObjects)
                gm.Draw();
            OnMapUpdated();
            Renderer.End();
            Console.Write("\rDraw Calls: {0}    |    Batches: {1}    |    FPS: {2}     ", Renderer.DrawCalls, Renderer.batches, fps);
        }

        //  start/stop game loop

        private void OnLoad(object sender, EventArgs e)
        {
            Task.Factory.StartNew(Loop);
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            gameObjects.Clear();
            run = false;
        }



        private IContainer components = null;


        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.skgl = new SkiaSharp.Views.Desktop.SKGLControl();
            this.SuspendLayout();
            // 
            // skglControl1
            // 
            this.skgl.BackColor = System.Drawing.Color.Black;
            this.skgl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.skgl.Location = new System.Drawing.Point(0, 0);
            this.skgl.Name = "skglControl1";
            this.skgl.Size = new System.Drawing.Size(800, 450);
            this.skgl.TabIndex = 0;
            this.skgl.PaintSurface += new System.EventHandler<SkiaSharp.Views.Desktop.SKPaintGLSurfaceEventArgs>(this.skglControl1_PaintSurface);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.skgl);
            this.Name = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += OnLoad;
            this.ResumeLayout(false);

        }

        #endregion

        private SKGLControl skgl;
    }

   
}
