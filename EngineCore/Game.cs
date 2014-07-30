using EngineCore.Entities;
using EngineCore.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EngineCore.Physics;
using EngineCore.Input;

namespace EngineCore
{
    public class Game
    {
        public GameSystemCollection Systems { get; set; }
        private double desiredFrameLength = 1.0 / 60.0;
        private bool running = false;

        private List<GameObject> gameObjects = new List<GameObject>();
        public IReadOnlyList<GameObject> GameObjects
        {
            get { return gameObjects; }
        }
        private void AddGameObject(GameObject go)
        {
            this.gameObjects.Add(go);
            go.InitializeComponents(this);
        }

        private SharpDxGraphicsSystem graphicsSystem;
        public SharpDxGraphicsSystem GraphicsSystem
        {
            get { return graphicsSystem; }
        }

        public Game()
        {
            this.previousFrameStartTime = DateTime.UtcNow;
            this.Systems = new GameSystemCollection();
            this.Systems.Add(new EntityUpdateSystem(this));
            this.Systems.Add(new BepuPhysicsSystem(this));
            graphicsSystem = new SharpDxGraphicsSystem(this);
            this.Systems.Add(graphicsSystem);
            this.Systems.Add(new InputSystem(this));

            GameObject.GameObjectConstructed += AddGameObject;
        }

        public void Start()
        {
            Debug.WriteLine("Starting main game loop.");
            this.running = true;
            StartSystems();
            PerformCustomInitialization();
            RunMainLoop();
        }

        protected virtual void PerformCustomInitialization()
        {
            throw new NotImplementedException();
        }

        private void StartSystems()
        {
            foreach (var system in this.Systems)
            {
                system.Start();
            }
        }

        private void RunMainLoop()
        {
            while (running)
            {
                RunSingleFrame();
            }
            Debug.WriteLine("Exiting");
        }

        private DateTime previousFrameStartTime;
        private void RunSingleFrame()
        {
            DateTime beforeFrameTime = DateTime.UtcNow;
            float elapsedSinceLastFrame = (float)(beforeFrameTime - previousFrameStartTime).TotalSeconds;
            Time.SetDeltaTime(elapsedSinceLastFrame);
            previousFrameStartTime = beforeFrameTime;
            foreach (GameSystem system in this.Systems)
            {
                system.Update();
            }
            DateTime afterFrameTime = DateTime.UtcNow;
            var elapsed = (afterFrameTime - beforeFrameTime).TotalSeconds;
            var sleepTime = desiredFrameLength - elapsed;
            if (sleepTime > 0.0)
            {
                //Debug.WriteLine("Sleep time: " + sleepTime);
                Thread.Sleep((int)(sleepTime * 1000));
            }
            else
            {
                Debug.WriteLine("Running slowly, no sleep time.");
            }
        }

        internal void Exit()
        {
            this.running = false;
        }

        internal GameSystem GetSystem(Type dependencyType)
        {
            return this.Systems.Single(sys => sys.GetType() == dependencyType);
        }
    }
}
