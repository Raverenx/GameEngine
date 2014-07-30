using EngineCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Components
{
    public abstract class Component
    {
        public GameObject GameObject { get; private set; }
        public virtual Type GetDependency() { return null; }
        internal virtual void Initialize(GameObject gameObject, GameSystem system)
        {
            this.GameObject = gameObject;
            this.Transform = gameObject.Transform;
        }
        public Transform Transform { get; private set; }

        internal virtual void Uninitialize(GameObject gameObject, GameSystem system)
        {
        }
    }

    public abstract class Component<TSystem> : Component where TSystem : GameSystem
    {
        public override Type GetDependency() { return typeof(TSystem); }
        internal override void Initialize(GameObject gameObject, GameSystem system)
        {
            base.Initialize(gameObject, system);
            this.Initialize((TSystem)system);
        }
        internal override void Uninitialize(GameObject gameObject, GameSystem system)
        {
            base.Uninitialize(gameObject, system);
            this.Uninitialize((TSystem)system);
        }
        protected abstract void Initialize(TSystem system);
        protected abstract void Uninitialize(TSystem system);
    }
}
