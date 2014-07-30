using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EngineCore.Entities
{
    public class EntityUpdateSystem : GameSystem
    {
        List<IUpdateableEntity> entities = new List<IUpdateableEntity>();

        public void AddEntity(IUpdateableEntity entity)
        {
            this.entities.Add(entity);
        }

        public bool RemoveEntity(IUpdateableEntity entity)
        {
            return this.entities.Remove(entity);
        }

        public EntityUpdateSystem(Game game)
            : base(game)
        {

        }

        public override void Update()
        {
            foreach (IUpdateableEntity entity in new List<IUpdateableEntity>(entities))
            {
                entity.Update();
            }
        }

        public override void Start()
        {

        }

        public override void Stop()
        {

        }
    }
}
