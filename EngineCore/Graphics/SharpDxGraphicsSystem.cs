using EngineCore.Entities;
using EngineCore.Graphics;
using EngineCore.Physics;
using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace EngineCore.Graphics
{
    public class SharpDxGraphicsSystem : GameSystem
    {
        SimpleRenderer renderer;

        public SimpleRenderer Renderer
        {
            get { return renderer; }
            set { renderer = value; }
        }

        private Thread thread;
        public SharpDxGraphicsSystem(Game game)
            : base(game)
        {
            this.thread = new Thread(ThreadStartFunc);
        }

        private void ThreadStartFunc(object obj)
        {
            renderer = new SimpleRenderer();
            renderer.Form.MouseDown += OnMouseDown;
            renderer.Form.KeyDown += OnKeyDown;
            renderer.Form.FormClosing += OnFormClosing;
            renderer.Renderables = new List<IRenderable>();
            Application.Run(renderer.Form);
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            Game.Exit();
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("Clicked at " + e.X + ", " + e.Y);
            if (e.Button == MouseButtons.Left)
            {
                renderer.Form.Text += "+";
            }
            else if (e.Button == MouseButtons.Right)
            {
                renderer.Form.Text = renderer.Form.Text.Substring(0, renderer.Form.Text.Length - 1);
            }
        }

        private void FireBoxForward()
        {
            GameObject box  = GameObject.CreateBox(this.renderer, 0.2f, 0.2f, 0.2f, .2f);
            box.GetComponent<Collider>().PhysicsEntity.LinearVelocity = (renderer.MainCamera.Forward * 25.0f).ToBepuVector();
            box.Transform.Position = renderer.MainCamera.Position + renderer.MainCamera.Forward * 1.5f;
            Game.AddGameObject(box);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F)
            {
                FireBoxForward();
            }
        }

        public override void Update()
        {
            renderer.RenderFrame();
        }

        public override void Start()
        {
            this.thread.Start();
            while (this.renderer == null)
            {
                Thread.Yield();
            }
        }

        public override void Stop()
        {

        }


        internal void SetCamera(Camera camera)
        {
            renderer.MainCamera = camera;
        }
    }
}
