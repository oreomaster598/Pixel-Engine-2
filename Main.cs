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
using Timer = System.Timers.Timer;
using PE2.Physics.Dynamics;
using PE2.Math;

namespace PE2
{
    public  class Main : Form
    {
        public static List<GameObject> gameObjects = new List<GameObject>();


        public static double DeltaTime;
        public static double Secondframe;
        public static bool run = true;
        public static int fps = 0;
        public static uint speed = 1;
        public static bool SkipFrame = false;
        public static World world;
        int gridres = 25;
        long _frameCount = 0;


        public static Camera CurrentCamera;

        public static Vector2 mousePosition = Vector2.Zero;

        public SKColor Background = new SKColor(128,128,128);
 

        public static GameSettings gameSettings;
        public static WindowSettings windowSettings;
        DateTime _lastCheckTime = DateTime.Now;
        public RadioButton radioButton1;
        public RadioButton radioButton2;
        public static SKTypeface font;

        public Main(GameSettings gameSettings, WindowSettings windowSettings)
        {
            Main.gameSettings = gameSettings;
            Main.windowSettings = windowSettings;
            InitializeComponent();

            this.ClientSize = new Size((int)windowSettings.Size.x, (int)windowSettings.Size.y);
            this.Text = windowSettings.Title;

            skgl.VSync = windowSettings.VSync;

            font = SKTypeface.Default;


            world = new World(new Physics.Collision.AABB() { UpperBound = new Vector2(1000, 1000), LowerBound = new Vector2(-1000, -1000) }, new Vector2(0, 20), true) ; 
            LoadContent();

            Application.Idle += Loop;


        }


        public static void RegisterGameObject(GameObject gm)
        {
            gm.index = (uint)gameObjects.Count;
            gameObjects.Add(gm);
        }

        public static void UnRegisterGameObject(GameObject gm)
        {
            foreach (Components.Component c in gm.components)
                c.OnDestroy();
            gameObjects.Remove(gm);
        }
        float timeStep = 1.0f / 60.0f;
        int velocityIterations = 8;
        int positionIterations = 1;
        public void Loop(object sender, EventArgs e)
        {
           // while(run)
           // {

                try
                {
                    //Cap fps at 60
                    Thread.Sleep((int)speed);
                    world.Step(timeStep, velocityIterations, positionIterations);

                    Point p = skgl.PointToClient(Cursor.Position);
                    Input.MousePosition = new Vector2(p.X, p.Y);

                    int tfps = (int)GetFps();
                    if(tfps > -1)
                        fps = tfps;

                    foreach (GameObject gm in gameObjects)
                        gm.PreUpdate();
                    PreUpdate();

                    if(run)
                        skgl.Invalidate();

                    Input.Update();
                    foreach (GameObject gm in gameObjects.ToArray())
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

           // }
            

        }

        public virtual void Update() { }

        public virtual void Draw() { }
        public virtual void PreDraw() { }


        public virtual void LoadContent() { }
        public virtual void UnloadContent() { }

        public virtual void PreUpdate() { }


        public int ParseNoRound(float f)
        {
            string s = f.ToString();
            if(s.Contains('.'))
                return int.Parse(s.Remove(s.IndexOf('.')));
            return int.Parse(s);
        }

   
        void OnMapUpdated()
        {
            Interlocked.Increment(ref _frameCount);
        }

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
            if(SkipFrame)
            {
                SkipFrame = false;
                return;
            }
            try
            {
                SKCanvas canvas = e.Surface.Canvas;
                //canvas.Clear(Background);

                Renderer.Begin();
                Rectangle Bounds = skgl.ClientRectangle;
                PreDraw();
                Renderer.Transform();

                foreach (GameObject gm in gameObjects)
                    gm.Draw();
                Draw();
                OnMapUpdated();
                Renderer.End(ref canvas);
                canvas.Flush();
                Console.Write("\rDraw Calls: {0}    |    Batches: {1}    |    FPS: {2}     |    Resolution: {3}", Renderer.DrawCalls, Renderer.batches, fps, CurrentCamera.resolution);
            }
            catch(Exception ex)
            {
                Debug.LogError(ex.Message, "PE Core");
            }
           
        }



        private void OnLoad(object sender, EventArgs e)
        {
            //Task.Factory.StartNew(Loop);
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnloadContent();
            gameObjects.Clear();
            skgl.Dispose();
            skgl = null;
            run = false;
        }



        private IContainer components = null;


        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                components = null;
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code


        private void InitializeComponent()
        {
            this.skgl = new SkiaSharp.Views.Desktop.SKGLControl();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // skgl
            // 
            this.skgl.BackColor = System.Drawing.Color.Transparent;
            this.skgl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.skgl.Location = new System.Drawing.Point(0, 0);
            this.skgl.Name = "skgl";
            this.skgl.Size = new System.Drawing.Size(284, 261);
            this.skgl.TabIndex = 0;
            this.skgl.VSync = false;
            this.skgl.PaintSurface += new System.EventHandler<SkiaSharp.Views.Desktop.SKPaintGLSurfaceEventArgs>(this.skglControl1_PaintSurface);
            // 
            // radioButton1
            // 
            this.radioButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radioButton1.AutoSize = true;
            this.radioButton1.BackColor = System.Drawing.Color.Transparent;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(214, 0);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(70, 17);
            this.radioButton1.TabIndex = 1;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Camera 1";
            this.radioButton1.UseVisualStyleBackColor = false;
            // 
            // radioButton2
            // 
            this.radioButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(214, 23);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(70, 17);
            this.radioButton2.TabIndex = 2;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Camera 2";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.skgl);
            this.Name = "Main";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.SizeChanged += new System.EventHandler(this.Main_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SKGLControl skgl;

        private void Main_SizeChanged(object sender, EventArgs e)
        {
            SkipFrame = true;
            windowSettings.Size = new Vector2(ClientSize.Width, ClientSize.Height);
        }

    }

   
}
