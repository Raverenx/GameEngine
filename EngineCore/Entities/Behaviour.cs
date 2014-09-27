using EngineCore.Components;
using EngineCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore
{
    public abstract class Behaviour : Component<EntityUpdateSystem>, IUpdateableEntity
    {
        protected override void Initialize(EntityUpdateSystem system)
        {
            system.AddEntity(this);
        }

        protected override void Uninitialize(EntityUpdateSystem system)
        {
            system.RemoveEntity(this);
        }

        void IUpdateableEntity.Update()
        {
            this.Update();
        }

        protected abstract void Update();

    }

    public abstract class Behaviour<TSystem> : Component<EntityUpdateSystem, TSystem>, IUpdateableEntity
        where TSystem : GameSystem
    {

        protected override void Initialize(EntityUpdateSystem system1, TSystem system2)
        {
            system1.AddEntity(this);
            this.Initialize(system2);
        }

        protected override void Uninitialize(EntityUpdateSystem system1, TSystem system2)
        {
            system1.RemoveEntity(this);
            this.Uninitialize(system2);
        }

        void IUpdateableEntity.Update()
        {
            this.Update();
        }

        protected abstract void Update();
        protected abstract void Initialize(TSystem system);
        protected abstract void Uninitialize(TSystem system);
    }

        public abstract class Behaviour<TSystem> : Component<EntityUpdateSystem, TSystem>, IUpdateableEntity
        where TSystem : GameSystem
    {

        protected override void Initialize(EntityUpdateSystem system1, TSystem system2)
        {
            system1.AddEntity(this);
            this.Initialize(system2);
        }

        protected override void Uninitialize(EntityUpdateSystem system1, TSystem system2)
        {
            system1.RemoveEntity(this);
            this.Uninitialize(system2);
        }

        void IUpdateableEntity.Update()
        {
            this.Update();
        }

        protected abstract void Update();
        protected abstract void Initialize(TSystem system);
        protected abstract void Uninitialize(TSystem system);
    }
}
