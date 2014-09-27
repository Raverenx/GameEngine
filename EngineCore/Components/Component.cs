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
        internal virtual IEnumerable<Type> GetDependencies() { return null; }

        internal void CoreInitialize(GameObject gameObject, IEnumerable<GameSystem> systems)
        {
            this.GameObject = gameObject;
            this.Transform = gameObject.Transform;

            this.Initialize(systems);
        }

        protected internal abstract void Initialize(IEnumerable<GameSystem> systems);
        protected internal abstract void Uninitialize(IEnumerable<GameSystem> systems);
    }

    /// <summary>
    /// An abstract Component type declaring one GameSystem dependency.
    /// </summary>
    /// <typeparam name="TSystem">The type of dependency.</typeparam>
    public abstract class Component<TSystem> : Component where TSystem : GameSystem
    {
        internal override IEnumerable<Type> GetDependencies() { yield return typeof(TSystem); }

        protected internal override sealed void Initialize(IEnumerable<GameSystem> systems)
        {
            this.Initialize((TSystem)systems.First());
        }

        protected internal override sealed void Uninitialize(IEnumerable<GameSystem> systems)
        {
            this.Uninitialize((TSystem)systems.First());
        }

        protected abstract void Initialize(TSystem system);
        protected abstract void Uninitialize(TSystem system);
    }

    public abstract class Component<TSystem1, TSystem2> : Component
        where TSystem1 : GameSystem
        where TSystem2 : GameSystem
    {
        internal override IEnumerable<Type> GetDependencies()
        {
            yield return typeof(TSystem1);
            yield return typeof(TSystem2);
        }

        protected internal override void Initialize(IEnumerable<GameSystem> systems)
        {
            TSystem1 system1 = (TSystem1)systems.Single(gs => gs.GetType() == typeof(TSystem1));
            TSystem2 system2 = (TSystem2)systems.Single(gs => gs.GetType() == typeof(TSystem2));

            this.Initialize(system1, system2);
        }

        protected internal override void Uninitialize(IEnumerable<GameSystem> systems)
        {
            TSystem1 system1 = (TSystem1)systems.Single(gs => gs.GetType() == typeof(TSystem1));
            TSystem2 system2 = (TSystem2)systems.Single(gs => gs.GetType() == typeof(TSystem2));

            this.Uninitialize(system1, system2);
        }

        protected abstract void Initialize(TSystem1 system1, TSystem2 system2);
        protected abstract void Uninitialize(TSystem1 system1, TSystem2 system2);
    }
}
