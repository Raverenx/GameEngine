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
        /// <summary>
        /// Retrieves the GameObject holding this Component.
        /// </summary>
        public GameObject GameObject { get; private set; }

        /// <summary>
        /// Retrieves the Transform component from the GameObject this Component is attached to.
        /// A Transform is always present on GameObjects.
        /// </summary>
        public Transform Transform { get; private set; }

        /// <summary>
        /// Obtains the system type which this Component is dependent on.
        /// </summary>
        /// <returns></returns>
        internal virtual Type GetDependency() { return null; }

        internal void CoreInitialize(GameObject gameObject, GameSystem system)
        {
            this.GameObject = gameObject;
            this.Transform = gameObject.Transform;

            this.Initialize(system);
        }

        protected internal abstract void Initialize(GameSystem system);
        protected internal abstract void Uninitialize(GameSystem system);
    }

    /// <summary>
    /// An abstract Component type declaring one GameSystem dependency.
    /// </summary>
    /// <typeparam name="TSystem">The type of dependency.</typeparam>
    public abstract class Component<TSystem> : Component where TSystem : GameSystem
    {
        internal override Type GetDependency() { return typeof(TSystem); }

        protected internal override sealed void Initialize(GameSystem system)
        {
            this.Initialize((TSystem)system);
        }

        protected internal override sealed void Uninitialize(GameSystem system)
        {
            this.Uninitialize((TSystem)system);
        }

        protected abstract void Initialize(TSystem system);
        protected abstract void Uninitialize(TSystem system);
    }
}
