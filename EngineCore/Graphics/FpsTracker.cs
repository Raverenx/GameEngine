using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Matrix4x4 = System.Numerics.Matrix4x4;
using EngineCore.Components;

namespace EngineCore.Graphics
{
    public class FpsTracker : Component<SharpDxGraphicsSystem>, IRenderable 
    {
        public Vector2 Position { get; set; }

        private int numFramesTracked = 50;
        private LinkedList<double> frameTimes;
        private double totalFrameTime;

        private SimpleText textRenderer;
        private Stopwatch stopwatch;
        private string fontName = "Fonts/textfont.dds";

        private double FramesPerSecond
        {
            get
            {
                return 1 / (totalFrameTime / numFramesTracked);
            }
        }

        protected override void Initialize(SharpDxGraphicsSystem system)
        {
            this.textRenderer = system.Renderer.TextRenderer;
            this.stopwatch = new Stopwatch();
            this.stopwatch.Start();
            this.frameTimes = new LinkedList<double>();
            for (int g = 0; g < numFramesTracked; g++)
            {
                frameTimes.AddLast(this.stopwatch.ElapsedMilliseconds / 1000.0);
            }

            system.Renderer.Renderables.Add(this);
        }

        protected override void Uninitialize(SharpDxGraphicsSystem system)
        {
            system.Renderer.Renderables.Remove(this);
            
        }

        public void Render(SimpleRenderer renderer)
        {
            UpdateFrameCount();
            textRenderer.DrawText(FramesPerSecond.ToString("####.00") + " FPS", this.Position);
        }

        private void UpdateFrameCount()
        {
            var first = frameTimes.First.Value;
            var second = frameTimes.ElementAt(1);
            var oldDiff = (second - first);

            var last = frameTimes.Last.Value;
            var now = this.stopwatch.ElapsedMilliseconds / 1000.0;
            var newDiff = (now - last);

            frameTimes.RemoveFirst();
            frameTimes.AddLast(now);

            totalFrameTime += (newDiff - oldDiff);
        }

        public Matrix4x4 WorldMatrix
        {
            get { return Matrix4x4.Identity; }
        }
    }
}
