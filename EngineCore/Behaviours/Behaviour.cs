using EngineCore.Components;
using EngineCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Behaviours
{
    public abstract class Behaviour : Component<EntityUpdateSystem>, IUpdateableEntity
    {
        protected override void Initialize(EntityUpdateSystem system)
        {
            system.AddEntity(this);
        }

        void IUpdateableEntity.Update()
        {
            this.Update();
        }

        protected abstract void Update();
    }
}
