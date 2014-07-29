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
        public virtual void Initialize(GameObject gameObject, GameSystem system)
        {
            this.GameObject = gameObject;
            this.Transform = gameObject.Transform;
        }
        public Transform Transform { get; private set; }
    }

    public abstract class Component<TSystem> : Component where TSystem : GameSystem
    {
        public override Type GetDependency() { return typeof(TSystem); }
        public override void Initialize(GameObject gameObject, GameSystem system)
        {
            base.Initialize(gameObject, system);
            this.Initialize((TSystem)system);
        }
        protected abstract void Initialize(TSystem system);
    }
}
