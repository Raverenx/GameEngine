using EngineCore;
using EngineCore.Behaviours;
using EngineCore.Entities;
using EngineCore.Graphics;
using EngineCore.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameApplication
{

    static class Program
    {
        private static Vector<Single> _hack;
        static Program()
        {
            _hack = Vector<Single>.One;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            Game game = new BoxGame();
            game.Start();
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = (Exception)e.ExceptionObject;

            var result = MessageBox.Show("ERROR:" + Environment.NewLine + exception
                + Environment.NewLine + Environment.NewLine + "Yes = Debug, No = Cancel",
                "Error", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                System.Diagnostics.Debugger.Launch();
            }
        }

        public class BoxGame : Game
        {
            protected override void PerformCustomInitialization()
            {
                AddStartingStuff();
            }

            private void AddStartingStuff()
            {
                this.GraphicsSystem.Renderer.Light = new DirectionalLight(GraphicsSystem.Renderer.Device, GraphicsSystem.Renderer.DeviceContext, new Vector3(0.1f, -.3f, 1.0f), new Color4f(1, 1, 1, 1));

                var character = new GameObject();
                character.AddComponent<CharacterController>();
                character.AddComponent<SimpleFpsController>();
                character.Transform.Position = new Vector3(0, 2f, 5f);

                var camera = new GameObject();
                camera.AddComponent<Camera>();
                camera.AddComponent<BoxLauncher>();
                var fpsLookController = camera.AddComponent<FpsLookController>();
                fpsLookController.Tracked = character.Transform;

                var box = GameObject.CreateBox(3.0f, 3.0f, 3.0f, 6f);
                box.Transform.Position = new Vector3(0, 5, 15);

                var box2 = GameObject.CreateBox(3.0f, 1.0f, 3.0f, 4.0f);
                box2.Transform.Position = new Vector3(1.5f, 10, 15);

                var box3 = GameObject.CreateBox(18.0f, 0.33f, 0.5f, 2.0f);
                box3.Transform.Position = new Vector3(0, 18, 15.5f);

                box = GameObject.CreateBox(2.0f, .15f, 2.0f, 0.666f);
                box.Transform.Position = new Vector3(-6.5f, 19, 15.5f);

                box = GameObject.CreateBox(0.5f, 0.5f, 0.5f, 0.1f);
                box.Transform.Position = new Vector3(6.5f, 32, 15.5f);

                box = GameObject.CreateBox(1.0f, 1.0f, 1.0f);
                box.Transform.Position = new Vector3(4, 5, -10);
                box.GetComponent<BoxCollider>().PhysicsEntity.LinearVelocity = new Vector3(0, 2, 15).ToBepuVector();

                box = GameObject.CreateBox(1.0f, 1.0f, 1.0f);
                box.Transform.Position = new Vector3(4.9f, 5, 30);
                box.GetComponent<BoxCollider>().PhysicsEntity.LinearVelocity = new Vector3(0, 2, -15).ToBepuVector();

                var floor = GameObject.CreateStaticBox(50.0f, 1.0f, 50.0f);
                var wall1 = GameObject.CreateStaticBox(50.0f, 11.0f, 0.5f);
                wall1.Transform.Position = new Vector3(0, 5.5f, 25f);

                var wall2 = GameObject.CreateStaticBox(50.0f, 11.0f, 0.5f);
                wall2.Transform.Position = new Vector3(0, 5.5f, -25f);

                var wall3 = GameObject.CreateStaticBox(0.5f, 11.0f, 50f);
                wall3.Transform.Position = new Vector3(25f, 5.5f, 0f);

                var wall4 = GameObject.CreateStaticBox(0.5f, 11.0f, 50f);
                wall4.Transform.Position = new Vector3(-25f, 5.5f, 0f);

                var framerateTracker = new GameObject();
                framerateTracker.AddComponent<FpsTracker>();
            }
        }
    }
}
