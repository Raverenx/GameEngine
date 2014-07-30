using EngineCore.Behaviours;
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
    public class SimpleFpsController : Behaviour
    {
        CharacterController controller;

        protected override void Initialize(Entities.EntityUpdateSystem system)
        {
            base.Initialize(system);
            this.controller = this.GameObject.GetComponent<CharacterController>();
        }

        protected override void Update()
        {
            HandleKeyboardMovement();
        }

        private void HandleKeyboardMovement()
        {
            Vector3 movementDirection = new Vector3();
            if (InputSystem.GetKey(Keys.W))
            {
                movementDirection += Transform.Forward;
            }
            if (InputSystem.GetKey(Keys.S))
            {
                movementDirection += -Transform.Forward;
            }
            if (InputSystem.GetKey(Keys.A))
            {
                movementDirection += -Transform.Right;
            }
            if (InputSystem.GetKey(Keys.D))
            {
                movementDirection += Transform.Right;
            }
            if (movementDirection != Vector3.Zero)
            {
                var normalized = Vector3.Normalize(movementDirection);
                normalized.Y = 0f;
                var motionDirection = new Vector2(normalized.X, normalized.Z);
                controller.SetMotionDirection(motionDirection * MovementSpeed * Time.DeltaTime);
            }
            else
            {
                controller.SetMotionDirection(Vector2.Zero);
            }

            if (InputSystem.GetKeyDown(Keys.Space))
            {
                JumpButtonPressed();
            }
        }

        private void JumpButtonPressed()
        {
            controller.BepuController.Jump();
        }

        public float MovementSpeed { get { return 5.0f; } }
    }
}
