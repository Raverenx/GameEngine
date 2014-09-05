using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Vector3 = System.Numerics.Vector3;
using Matrix4x4 = System.Numerics.Matrix4x4;
using EngineCore.Components;
using EngineCore.Utility;

namespace EngineCore.Graphics
{
    public class Camera : Component<SharpDxGraphicsSystem>
    {
        public Vector3 Position { get { return Transform.Position; } }
        public Vector3 Forward { get { return Transform.Forward; } }
        public Vector3 Up { get { return Transform.Up; } }
        public Vector3 Right { get { return Transform.Right; } }

        internal Matrix4x4 GetViewMatrix()
        {
            var lookAt = this.Position + this.Forward;
            Vector3 position = this.Position;
            Vector3 up = this.Transform.Up;
            return MathUtil.CreateLookAtLH(position, lookAt, up);
        }

        protected override void Initialize(SharpDxGraphicsSystem system)
        {
            system.SetCamera(this);
        }

        protected override void Uninitialize(SharpDxGraphicsSystem system)
        {

        }
    }
}
