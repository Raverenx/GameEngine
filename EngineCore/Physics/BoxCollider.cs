using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using EngineCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Physics
{
    public class BoxCollider : Component<BepuPhysicsSystem>
    {
        Box physicsBox = new Box(System.Numerics.Vector3.Zero, 1.0f, 1.0f, 1.0f, 1.0f);

        public Entity PhysicsEntity
        {
            get { return physicsBox; }
        }

        private float width, height, length = 1.0f;
        private float mass = 1.0f;

        public float Width
        {
            get { return width; }
            set
            {
                width = value;
                SetPhysicsBoxDimensions();
            }
        }

        public float Height
        {
            get { return height; }
            set
            {
                height = value;
                SetPhysicsBoxDimensions();
            }
        }

        public float Length
        {
            get { return length; }
            set
            {
                length = value;
                SetPhysicsBoxDimensions();
            }
        }

        public float Mass
        {
            get { return mass; }
            set
            {
                mass = value;
                physicsBox.Mass = mass;
            }
        }

        private void SetPhysicsBoxDimensions()
        {
            physicsBox.Length = length * Transform.Scale.Z;
            physicsBox.Width = width * Transform.Scale.X;
            physicsBox.Height = height * Transform.Scale.Y;
        }

        protected override void Initialize(BepuPhysicsSystem system)
        {
            this.Transform.PositionChanged += OnTransformPositionManuallyChanged;
            this.Transform.RotationChanged += OnTransformRotationManuallyChanged;

            physicsBox.PositionUpdated += Transform.OnPhysicsUpdate;
            system.AddOject(this.physicsBox, this.GameObject);
            OnTransformPositionManuallyChanged(this.Transform.Position);
            OnTransformRotationManuallyChanged(this.Transform.Rotation);
            OnScaleChanged(this.Transform.Scale);

            this.Transform.ScaleChanged += OnScaleChanged;
        }

        protected override void Uninitialize(BepuPhysicsSystem system)
        {
            this.Transform.PositionChanged -= OnTransformPositionManuallyChanged;
            this.Transform.RotationChanged -= OnTransformRotationManuallyChanged;

            physicsBox.PositionUpdated -= Transform.OnPhysicsUpdate;

            system.RemoveObject(this.physicsBox);
            this.Transform.ScaleChanged -= OnScaleChanged;
        }

        private void OnScaleChanged(System.Numerics.Vector3 obj)
        {
            SetPhysicsBoxDimensions();
        }

        private void OnTransformPositionManuallyChanged(System.Numerics.Vector3 position)
        {
            this.physicsBox.Position = position;
        }

        private void OnTransformRotationManuallyChanged(System.Numerics.Quaternion rotation)
        {
            this.physicsBox.Orientation = rotation;
        }
    }
}
