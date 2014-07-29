using EngineCore.Components;
using EngineCore.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EngineCore.Behaviours
{
    class CameraMovementController : Behaviour
    {
        float previousMouseX;
        float previousMouseY;

        float currentYaw;
        float currentPitch;

        protected override void Update()
        {
            HandleKeyboardMovement();
            HandleMouseLook();
        }

        private void HandleMouseLook()
        {
            float newMouseX = InputSystem.MousePosition.X;
            float newMouseY = InputSystem.MousePosition.Y;
            if (InputSystem.GetMouseButton(MouseButtons.Left))
            {
                float xDelta = newMouseX - previousMouseX;
                float yDelta = newMouseY - previousMouseY;

                currentYaw += -xDelta * 0.01f;
                currentPitch += yDelta * 0.01f;

                this.Transform.Rotation = Quaternion.CreateFromYawPitchRoll(currentYaw,currentPitch, 0f);
            }

            this.previousMouseX = newMouseX;
            this.previousMouseY = newMouseY;
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
                movementDirection += Transform.Right;
            }
            if (InputSystem.GetKey(Keys.D))
            {
                movementDirection += -Transform.Right;
            }
            if (InputSystem.GetKey(Keys.E))
            {
                movementDirection += Transform.Up;
            }
            if (InputSystem.GetKey(Keys.Q))
            {
                movementDirection += -Transform.Up;
            }
            if (movementDirection != Vector3.Zero)
            {
                this.Transform.Position += Vector3.Normalize(movementDirection) *  (InputSystem.GetKey(Keys.LShiftKey) ? 1.8f : 0.5f);
            }
        }
    }
}
