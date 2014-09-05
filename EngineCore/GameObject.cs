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
    public class GameObject
    {
        // Hold a back-reference to the game until I figure out how to encapsulate things
        Game game;

        MultiDictionary<Type, Component> components = new MultiDictionary<Type, Component>();

        public GameObject()
        {
            this.Transform = AddComponent<Transform>();

            if (GameObjectConstructed != null)
            {
                GameObjectConstructed(this);
            }
        }

        public static event Action<GameObject> GameObjectConstructed;

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

        public void AddComponent<T>(T component) where T : Component
        {
            this.components.Add(typeof(T), component);
            if (game != null)
            {
                InitializeSingleComponent(this.game, component);
            }
        }

        public void RemoveComponent<T>() where T : Component
        {
            T component = this.GetComponent<T>();
            if (game != null)
            {
                UninitializeSingleComponent(this.game, component);
            }
            this.components.Remove(new KeyValuePair<Type, Component>(typeof(T), component));
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

        private void UninitializeSingleComponent(Game game, Component component)
        {
            GameSystem system = null;
            Type dependencyType = component.GetDependency();
            if (dependencyType != null)
            {
                system = game.GetSystem(dependencyType);
            }
            component.Uninitialize(this, system);
        }

        public static GameObject CreateBox(float width, float height, float depth, float mass = 1.0f)
        {
            GameObject box = new GameObject();
            var boxRenderer = box.AddComponent<BoxRenderer>();
            boxRenderer.Scale = new Vector3(width, height, depth);
            var collider = box.AddComponent<BoxCollider>();
            collider.Width = width;
            collider.Height = height;
            collider.Length = depth;
            collider.Mass = mass;

            return box;
        }

        public static GameObject CreateStaticBox(float width, float height, float depth)
        {
            GameObject box = new GameObject();
            var boxRenderer = box.AddComponent<BoxRenderer>();
            boxRenderer.Scale = new Vector3(width, height, depth);
            var collider = box.AddComponent<BoxCollider>();
            collider.Width = width;
            collider.Height = height;
            collider.Length = depth;
            collider.Mass = -1.0f;

            return box;
        }
    }
}
