using BEPUphysics;
using BEPUphysics.Entities;
using EngineCore.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using EngineCore;
using BEPUphysics.Entities.Prefabs;
using EngineCore.Entities;
using EngineCore.Components;
using EngineCore.Physics;

namespace EngineCore
{
    public class GameObject : IUpdateableEntity
    {
        // Hold a back-reference to the game until i figure out how to encapsulate things
        Game game;

        MultiDictionary<Type, Component> components = new MultiDictionary<Type, Component>();

        public GameObject()
        {
            Transform transform = new Transform();
            this.AddComponent<Transform>();
            this.Transform = transform;
        }

        public Transform Transform { get; private set; }

        public T GetComponent<T>() where T : Component
        {
            return (T)components[typeof(T)].FirstOrDefault();
        }

        public T AddComponent<T>() where T : Component, new()
        {
            T component = new T();
            this.components.Add(typeof(T), component);
            if (game != null)
            {
                InitializeSingleComponent(this.game, component);
            }
            return component;
        }

        public void Update()
        {

        }

        public void InitializeComponents(Game game)
        {
            this.game = game;
            foreach (var component in components.Values)
            {
                InitializeSingleComponent(game, component);
            }
        }

        private void InitializeSingleComponent(Game game, Component component)
        {
            GameSystem system = null;
            Type dependencyType = component.GetDependency();
            if (dependencyType != null)
            {
                system = game.GetSystem(dependencyType);
            }
            component.Initialize(this, system);
        }

        public static GameObject CreateBox(SimpleRenderer renderer, float width, float height, float depth, float mass = 1.0f)
        {
            GameObject box = new GameObject();
            var boxRenderer = box.AddComponent<BoxRenderer>();
            boxRenderer.Scale = new Vector3(width, height, depth);
            var physicsEntity = new BEPUphysics.Entities.Prefabs.Box(Vector3.Zero.ToBepuVector(), width, height, depth, mass);
            var collider = box.AddComponent<Collider>();
            collider.PhysicsEntity = physicsEntity;

            return box;
        }

        public static GameObject CreateStaticBox(SimpleRenderer renderer, float width, float height, float depth)
        {
            GameObject box = new GameObject();
            var boxRenderer = box.AddComponent<BoxRenderer>();
            boxRenderer.Scale = new Vector3(width, height, depth);
            var physicsEntity = new BEPUphysics.Entities.Prefabs.Box(Vector3.Zero.ToBepuVector(), width, height, depth);
            var collider = box.AddComponent<Collider>();
            collider.PhysicsEntity = physicsEntity;

            return box;
        }
    }
}
