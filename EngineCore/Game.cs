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
using EngineCore.Behaviours;

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
            OnGameObjectAdded(go);
            go.InitializeComponents(this);
        }
        public Action<GameObject> OnGameObjectAdded = (go) => { };

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
            AddStartingStuff();
            RunMainLoop();
        }

        private void AddStartingStuff()
        {
            graphicsSystem.Renderer.Light = new DirectionalLight(graphicsSystem.Renderer.Device, graphicsSystem.Renderer.DeviceContext, new Vector3(0.1f, -.3f, 1.0f), new Color4f(1,1,1,1));

            var camera = new GameObject();
            camera.Transform.Position = new Vector3(0, 5, -5);
            camera.AddComponent<Camera>();
            camera.AddComponent<CameraMovementController>();

            var box = GameObject.CreateBox(this.GraphicsSystem.Renderer, 3.0f, 3.0f, 3.0f);
            box.Transform.Position = new Vector3(0, 5, 15);

            var box2 = GameObject.CreateBox(this.GraphicsSystem.Renderer, 3.0f, 1.0f, 3.0f, 2.0f);
            box2.Transform.Position = new Vector3(1.5f, 10, 15);

            var box3 = GameObject.CreateBox(this.GraphicsSystem.Renderer, 18.0f, 0.33f, 0.5f, 1.0f);
            box3.Transform.Position = new Vector3(0, 18, 15.5f);

            box = GameObject.CreateBox(this.GraphicsSystem.Renderer, 2.0f, .15f, 2.0f, 0.3331f);
            box.Transform.Position = new Vector3(-6.5f, 19, 15.5f);

            box = GameObject.CreateBox(this.GraphicsSystem.Renderer, 0.5f, 0.5f, 0.5f, 0.01f);
            box.Transform.Position = new Vector3(6.5f, 32, 15.5f);

            box = GameObject.CreateBox(this.GraphicsSystem.Renderer, 1.0f, 1.0f, 1.0f);
            box.Transform.Position = new Vector3(4, 5, -10);
            box.GetComponent<Collider>().PhysicsEntity.LinearVelocity = new BEPUutilities.Vector3(0, 2, 15);

            box = GameObject.CreateBox(this.GraphicsSystem.Renderer, 1.0f, 1.0f, 1.0f);
            box.Transform.Position = new Vector3(4.9f, 5, 30);
            box.GetComponent<Collider>().PhysicsEntity.LinearVelocity = new BEPUutilities.Vector3(0, 2, -15);

            var floor = GameObject.CreateStaticBox(this.GraphicsSystem.Renderer, 50.0f, 1.0f, 50.0f);
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
