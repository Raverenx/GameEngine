using BEPUphysics;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using EngineCore.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace EngineCore.Physics
{
    public class BepuPhysicsSystem : GameSystem
    {
        private Space space;
        public BepuPhysicsSystem(Game game)
            : base(game)
        {
            this.space = new Space(new BEPUutilities.Threading.ParallelLooper());
            game.OnGameObjectAdded += this.OnGameObjectAdded;
        }

        private void OnGameObjectAdded(GameObject obj)
        {

        }

        public override void Update()
        {
            space.Update(Time.DeltaTime);
        }

        public void AddEntity(Entity entity)
        {
            space.Add(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            space.Remove(entity);
        }

        public override void Start()
        {
            space.ForceUpdater.Gravity = new BEPUutilities.Vector3(0f, -9.81f, 0f);
        }

        public override void Stop()
        {
        }
    }
}
