using BEPUphysics.Entities;
using EngineCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Physics
{
    public class Collider : Component<BepuPhysicsSystem>
    {
        private Entity physicsEntity;
        private BepuPhysicsSystem system;

        public Entity PhysicsEntity
        {
            get { return physicsEntity; }
            set
            {
                var old = physicsEntity;
                physicsEntity = value;
                if (system != null)
                {
                    if (old != null)
                    {
                        system.RemoveObject(old);
                    }
                    system.AddOject(value);
                }

                value.PositionUpdated += Transform.OnPhysicsUpdate;
            }
        }

        protected override void Initialize(BepuPhysicsSystem system)
        {
            this.Transform.PositionChanged += OnTransformPositionManuallyChanged;
            this.Transform.RotationChanged += OnTransformRotationManuallyChanged;

            this.system = system;
            if (this.physicsEntity != null)
            {
                system.AddOject(this.physicsEntity);
                OnTransformPositionManuallyChanged(this.Transform.Position);
                OnTransformRotationManuallyChanged(this.Transform.Rotation);
            }
        }

        protected override void Uninitialize(BepuPhysicsSystem system)
        {
            this.Transform.PositionChanged -= OnTransformPositionManuallyChanged;
            this.Transform.RotationChanged -= OnTransformRotationManuallyChanged;

            if (this.physicsEntity != null)
            {
                system.RemoveObject(this.physicsEntity);
            }
        }

        private void OnTransformPositionManuallyChanged(System.Numerics.Vector3 position)
        {
            this.physicsEntity.Position = position.ToBepuVector();
        }

        private void OnTransformRotationManuallyChanged(System.Numerics.Quaternion rotation)
        {
            this.physicsEntity.Orientation = rotation.ToBepuQuaternion();
        }
    }
}
