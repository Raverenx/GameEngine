using EngineCore.Components;
using EngineCore.Input;
using EngineCore.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EngineCore.Behaviours
{
    public class BoxLauncher : Behaviour
    {
        private GameObject heldBox;
        private Tracker tracker;
        protected override void Update()
        {
            if (InputSystem.GetKey(Keys.F))
            {
                FireBoxForward();
            }
            if (InputSystem.GetKeyDown(Keys.Y))
            {
                StartStopHoldingBoxPressed();
            }
            if (InputSystem.GetKeyDown(Keys.Oemplus))
            {
                PlusButtonPressed();
            }
            if (InputSystem.GetKeyDown(Keys.OemMinus))
            {
                MinusButtonPressed();
            }
        }

        private void MinusButtonPressed()
        {
            if (heldBox != null)
            {
                heldBox.Transform.Scale -= new Vector3(.2f, 0, .2f);
                FixHoldOffset();
            }
        }

        private void PlusButtonPressed()
        {
            if (heldBox != null)
            {
                heldBox.Transform.Scale += new Vector3(.2f, 0, .2f);
                FixHoldOffset();
            }
        }

        private void FixHoldOffset()
        {
            tracker.Offset = new Vector3(0, 0, heldBox.Transform.Scale.Z + 2.0f);
        }

        private void StartStopHoldingBoxPressed()
        {
            if (heldBox != null)
            {
                StopHoldingBox();
            }
            else
            {
                StartHoldingBox();
            }
        }

        private void StopHoldingBox()
        {
            this.heldBox.RemoveComponent<Tracker>();
            this.heldBox.GetComponent<BoxCollider>().PhysicsEntity.Mass = 10.0f;
            this.heldBox = null;
        }

        private void StartHoldingBox()
        {
            this.heldBox = GameObject.CreateStaticBox(2.0f, 2.0f, 2.0f);
            tracker = this.heldBox.AddComponent<Tracker>();
            tracker.TrackedObject = this.Transform;
            tracker.Offset = new Vector3(0, 0, 3.0f);
        }

        private void FireBoxForward()
        {
            GameObject box = GameObject.CreateBox(0.2f, 0.2f, 0.2f, .2f);
            box.GetComponent<BoxCollider>().PhysicsEntity.LinearVelocity = (this.Transform.Forward * 25.0f).ToBepuVector();
            box.Transform.Position = this.Transform.Position + this.Transform.Forward * 1.5f;

            //// Uncomment this if you want insane box firing
            //box = GameObject.CreateBox(0.2f, 0.2f, 0.2f, .2f);
            //box.GetComponent<BoxCollider>().PhysicsEntity.LinearVelocity = (this.Transform.Forward * 25.0f).ToBepuVector();
            //box.Transform.Position = this.Transform.Position + this.Transform.Forward * 1.5f + this.Transform.Right * .5f;

            //box = GameObject.CreateBox(0.2f, 0.2f, 0.2f, .2f);
            //box.GetComponent<BoxCollider>().PhysicsEntity.LinearVelocity = (this.Transform.Forward * 25.0f).ToBepuVector();
            //box.Transform.Position = this.Transform.Position + this.Transform.Forward * 1.5f - this.Transform.Right * .5f;

            //box = GameObject.CreateBox(0.2f, 0.2f, 0.2f, .2f);
            //box.GetComponent<BoxCollider>().PhysicsEntity.LinearVelocity = (this.Transform.Forward * 25.0f).ToBepuVector();
            //box.Transform.Position = this.Transform.Position + this.Transform.Forward * 1.5f + this.Transform.Up * .5f;

            //box = GameObject.CreateBox(0.2f, 0.2f, 0.2f, .2f);
            //box.GetComponent<BoxCollider>().PhysicsEntity.LinearVelocity = (this.Transform.Forward * 25.0f).ToBepuVector();
            //box.Transform.Position = this.Transform.Position + this.Transform.Forward * 1.5f - this.Transform.Up * .5f;
        }

        protected override void Uninitialize(Entities.EntityUpdateSystem system)
        {

        }
    }

    public class Tracker : Behaviour
    {
        public Transform TrackedObject { get; set; }
        public Vector3 Offset { get; set; }

        protected override void Update()
        {
            this.Transform.Position = TrackedObject.Position + Vector3.Transform(Offset, TrackedObject.Rotation);
        }
    }

}
