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
        private double desiredFrameLength = 1.0 / 99999999.0;
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

            GameObject.GameObjectConstructed += AddGameObject; // This is absolutely required
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
#if DEBUG
                if (InputSystem.GetKeyDown(System.Windows.Forms.Keys.Pause))
                {
                    System.Diagnostics.Debugger.Break();
                }
#endif
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
            double elapsed = (afterFrameTime - beforeFrameTime).TotalSeconds;
            double sleepTime = desiredFrameLength - elapsed;
            if (sleepTime > 0.0)
            {
#if USE_SLEEP0
                DateTime finishTime = afterFrameTime + TimeSpan.FromSeconds(sleepTime);
                while (DateTime.UtcNow < finishTime)
                {
                    while (Thread.Yield()) { } // This can't be right
                }
#else
                Thread.Sleep((int)(sleepTime * 1000));
#endif
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
