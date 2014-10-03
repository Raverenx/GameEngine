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

        private double FramesPerSecond
        {
            get
            {
                return 1 / (totalFrameTime / numFramesTracked);
            }
        }

        protected override void Initialize(SharpDxGraphicsSystem system)
        {
#if TEXT_RENDERER
            this.textRenderer = system.Renderer.TextRenderer;
#endif
            this.stopwatch = new Stopwatch();
            this.stopwatch.Start();
            this.frameTimes = new LinkedList<double>();
            for (int g = 0; g < numFramesTracked; g++)
            {
                frameTimes.AddLast(this.stopwatch.ElapsedMilliseconds / 1000.0);
            }

            system.Renderer.AddRenderable(this);
        }

        protected override void Uninitialize(SharpDxGraphicsSystem system)
        {
            system.Renderer.RemoveRenderable(this);
            
        }

        public void Render(SimpleRenderer renderer)
        {
            UpdateFrameCount();
#if TEXT_RENDERER
            textRenderer.DrawText(FramesPerSecond.ToString("####.00") + " FPS", this.Position);
#endif
        }

        private void UpdateFrameCount()
        {
            var first = frameTimes.First.Value;
            var second = frameTimes.First.Next.Value;
            var oldDiff = (second - first);

            var last = frameTimes.Last.Value;
            var now = this.stopwatch.ElapsedMilliseconds / 1000.0;
            var newDiff = (now - last);

            var firstNode = frameTimes.First;
            frameTimes.RemoveFirst();
            firstNode.Value = now;
            frameTimes.AddLast(firstNode);
            totalFrameTime += (newDiff - oldDiff);
        }

        public Matrix4x4 WorldMatrix
        {
            get { return Matrix4x4.Identity; }
        }
    }
}