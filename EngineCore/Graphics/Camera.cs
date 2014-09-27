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
        private Matrix4x4 viewMatrix;
        private Matrix4x4 projectionMatrix;
        private float fieldOfViewRadians = 1.05f;
        private ProjectionType projectionType = ProjectionType.Perspective;
        private SharpDX.Windows.RenderForm renderForm;

        /// <summary>
        /// Gets or sets the field of view angle, in radians.
        /// </summary>
        public float FieldOfViewRadians
        {
            get { return fieldOfViewRadians; }
            set
            {
                fieldOfViewRadians = value;
                if (projectionType == global::ProjectionType.Perspective)
                {
                    RecalculateProjectionMatrix();
                }
            }
        }

        /// <summary>
        /// Sets the camera's projection type.
        /// </summary>
        public ProjectionType ProjectionType
        {
            get { return projectionType; }
            set
            {
                projectionType = value;
                RecalculateProjectionMatrix();
            }
        }

        /// <summary>
        /// Gets the current view matrix, based on the orientation of the camera.
        /// </summary>
        /// <returns></returns>
        public Matrix4x4 GetViewMatrix()
        {
            return viewMatrix;
        }

        /// <summary>
        /// Gets the current projection matrix, based on camera settings.
        /// </summary>
        /// <returns></returns>
        public Matrix4x4 GetProjectionMatrix()
        {
            return projectionMatrix;
        }

        protected override void Initialize(SharpDxGraphicsSystem system)
        {
            system.SetCamera(this);
            this.renderForm = system.Renderer.Form;
            this.renderForm.Resize += OnFormResized;
            this.Transform.PositionChanged += OnTransformPositionChanged;
            this.Transform.RotationChanged += OnTransformRotationChanged;
            this.RecalculateViewMatrix();
            this.RecalculateProjectionMatrix();
        }

        protected override void Uninitialize(SharpDxGraphicsSystem system) { }

        private void OnFormResized(object sender, EventArgs e)
        {
            RecalculateProjectionMatrix();
        }

        private void RecalculateViewMatrix()
        {
            var lookAt = this.Transform.Position + this.Transform.Forward;
            this.viewMatrix = MathUtil.CreateLookAtLH(this.Transform.Position, lookAt, this.Transform.Up);
        }

        private void RecalculateProjectionMatrix()
        {
            float windowRatio = (float)renderForm.ClientRectangle.Width / (float)renderForm.ClientRectangle.Height;

            switch (this.projectionType)
            {
                case ProjectionType.Perspective:
                    this.projectionMatrix = MathUtil.CreatePerspectiveFovLH(fieldOfViewRadians, windowRatio, 0.1f, 1000.0f);
                    break;
                case ProjectionType.Orthographic:
                    this.projectionMatrix = MathUtil.CreateOrthographic(10, 10, .03f, 1000f);
                    break;
            }
        }

        private void OnTransformRotationChanged(Quaternion obj)
        {
            RecalculateViewMatrix();
        }

        private void OnTransformPositionChanged(Vector3 obj)
        {
            RecalculateViewMatrix();
        }
    }
}

public enum ProjectionType
{
    Perspective,
    Orthographic
}