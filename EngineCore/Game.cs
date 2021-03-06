﻿using EngineCore.Entities;
using EngineCore.Graphics;
using EngineCore.Input;
using EngineCore.Physics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;

#if USE_SLEEP0 && USE_THREADYIELD
#error USE_SLEEP0 and USE_THREADYIELD are mutually exclusive, define only one.
#endif

namespace EngineCore
{
    public abstract class Game
    {
        public GameSystemCollection Systems { get; set; }
        private double desiredFrameLength = 1.0 / 60.0;
        private bool running = false;

        private ImmutableArray<GameObject> gameObjects = ImmutableArray<GameObject>.Empty;
        public IReadOnlyCollection<GameObject> GameObjects
        {
            get { return gameObjects; }
        }
        private void AddGameObject(GameObject go)
        {
            this.gameObjects = this.gameObjects.Add(go);
            go.InitializeComponents(this);
        }

        private SharpDxGraphicsSystem graphicsSystem;
        public SharpDxGraphicsSystem GraphicsSystem
        {
            get { return graphicsSystem; }
        }

        public Game()
        {
            this.Systems = new GameSystemCollection();
            this.Systems.Add(new EntityUpdateSystem(this));
            this.Systems.Add(new BepuPhysicsSystem(this));
            graphicsSystem = new SharpDxGraphicsSystem(this);
            this.Systems.Add(graphicsSystem);
            this.Systems.Add(new InputSystem(this));

            GameObject.GameObjectConstructed += AddGameObject;  // This is absolutely required

            this.previousFrameStartTime = DateTime.UtcNow;
        }

        public void Start()
        {
            Debug.WriteLine("Starting main game loop.");
            this.running = true;
            StartSystems();
            PerformCustomInitialization();
            RunMainLoop();
        }

        protected abstract void PerformCustomInitialization();

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
#if USE_THREADYIELD || USE_SLEEP0
                DateTime finishTime = afterFrameTime + TimeSpan.FromSeconds(sleepTime);
                while (DateTime.UtcNow < finishTime)
                {
#if USE_THREADYIELD
                    Thread.Yield();
#elif USE_SLEEP0
                    Thread.Sleep(0);
#endif
                }
#else
                Thread.Sleep((int)(sleepTime * 1000));
#endif
            }
#if MONITOR_SLOWRUNNING
            else
            {
                Debug.WriteLine("Running slowly, no sleep time.");
            }
#endif
        }

        internal void Exit()
        {
            this.running = false;
        }


    }
}
