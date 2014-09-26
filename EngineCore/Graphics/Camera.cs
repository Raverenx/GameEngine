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
        public ProjectionType ProjectionType
        {
            get { return projectionType; }
            set { projectionType = value; }
        }

        private ProjectionType projectionType = ProjectionType.Perspective;
        private SharpDX.Windows.RenderForm renderForm;

        protected override void Initialize(SharpDxGraphicsSystem system)
        {
            system.SetCamera(this);
            this.renderForm = system.Renderer.Form;
        }

        protected override void Uninitialize(SharpDxGraphicsSystem system) { }

        public Matrix4x4 GetViewMatrix()
        {
            var lookAt = this.Transform.Position + this.Transform.Forward;
            return MathUtil.CreateLookAtLH(this.Transform.Position, lookAt, this.Transform.Up);
        }

        public Matrix4x4 GetProjectionMatrix()
        {
            float windowRatio = (float)renderForm.ClientRectangle.Width / (float)renderForm.ClientRectangle.Height;
            switch (this.projectionType)
            {
                case ProjectionType.Perspective:
                    return MathUtil.CreatePerspectiveFovLH(1.05f, windowRatio, 0.1f, 1000.0f);
                case ProjectionType.Orthographic:
                    return MathUtil.CreateOrthographic(10, 10, .03f, 1000f);
                default:
                    throw new InvalidOperationException("Can't use ProjectionType value: " + this.projectionType);
            }
        }
    }
}

public enum ProjectionType
{
    Perspective,
    Orthographic
}