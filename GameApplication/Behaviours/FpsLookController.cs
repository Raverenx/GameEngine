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
    public class FpsLookController : Behaviour
    {
        public Transform Tracked { get; set; }
        private CharacterController cc;

        private float previousMouseX;
        private float previousMouseY;
        private float currentYaw;
        private float currentPitch;
        protected override void Update()
        {
            this.Transform.Position = Tracked.Position;
            this.cc = Tracked.GameObject.GetComponent<CharacterController>();
            HandleMouseMovement();
        }

        void HandleMouseMovement()
        {
            float newMouseX = InputSystem.MousePosition.X;
            float newMouseY = InputSystem.MousePosition.Y;

            float xDelta = newMouseX - previousMouseX;
            float yDelta = newMouseY - previousMouseY;

            if (InputSystem.GetMouseButton(MouseButtons.Left) || InputSystem.GetMouseButton(MouseButtons.Right))
            {
                currentYaw += xDelta * 0.01f;
                currentPitch += yDelta * 0.01f;

                this.Transform.Rotation = Quaternion.CreateFromYawPitchRoll(currentYaw, currentPitch, 0f);
                this.cc.BepuController.ViewDirection = this.Transform.Forward.ToBepuVector();
            }

            this.previousMouseX = newMouseX;
            this.previousMouseY = newMouseY;
        }
    }
}
