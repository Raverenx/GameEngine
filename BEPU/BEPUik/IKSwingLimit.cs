using System;
using BEPUutilities;

namespace BEPUik
{
    /// <summary>
    /// Prevents two bones from rotating beyond a certain angle away from each other as measured by attaching an axis to each connected bone.
    /// </summary>
    public class IKSwingLimit : IKLimit
    {
        /// <summary>
        /// Gets or sets the axis attached to ConnectionA in its local space.
        /// </summary>
        public System.Numerics.Vector3 LocalAxisA;
        /// <summary>
        /// Gets or sets the axis attached to ConnectionB in its local space.
        /// </summary>
        public System.Numerics.Vector3 LocalAxisB;

        /// <summary>
        /// Gets or sets the axis attached to ConnectionA in world space.
        /// </summary>
        public System.Numerics.Vector3 AxisA
        {
            get { return QuaternionEx.Transform(LocalAxisA, ConnectionA.Orientation); }
            set { LocalAxisA = QuaternionEx.Transform(value, QuaternionEx.Conjugate(ConnectionA.Orientation)); }
        }

        /// <summary>
        ///  Gets or sets the axis attached to ConnectionB in world space.
        /// </summary>
        public System.Numerics.Vector3 AxisB
        {
            get { return QuaternionEx.Transform(LocalAxisB, ConnectionB.Orientation); }
            set { LocalAxisB = QuaternionEx.Transform(value, QuaternionEx.Conjugate(ConnectionB.Orientation)); }
        }

        private float maximumAngle;
        /// <summary>
        /// Gets or sets the maximum angle between the two axes allowed by the constraint.
        /// </summary>
        public float MaximumAngle
        {
            get { return maximumAngle; }
            set { maximumAngle = Math.Max(0, value); }
        }


        /// <summary>
        /// Builds a new swing limit. Prevents two bones from rotating beyond a certain angle away from each other as measured by attaching an axis to each connected bone.
        /// </summary>
        /// <param name="connectionA">First connection of the limit.</param>
        /// <param name="connectionB">Second connection of the limit.</param>
        /// <param name="axisA">Axis attached to connectionA in world space.</param>
        /// <param name="axisB">Axis attached to connectionB in world space.</param>
        /// <param name="maximumAngle">Maximum angle allowed between connectionA's axis and connectionB's axis.</param>
        public IKSwingLimit(Bone connectionA, Bone connectionB, System.Numerics.Vector3 axisA, System.Numerics.Vector3 axisB, float maximumAngle)
            : base(connectionA, connectionB)
        {
            AxisA = axisA;
            AxisB = axisB;
            MaximumAngle = maximumAngle;
        }

        protected internal override void UpdateJacobiansAndVelocityBias()
        {

            //This constraint doesn't consider linear motion.
            linearJacobianA = linearJacobianB = new Matrix3x3();

            //Compute the world axes.
            System.Numerics.Vector3 axisA, axisB;
            QuaternionEx.Transform(ref LocalAxisA, ref ConnectionA.Orientation, out axisA);
            QuaternionEx.Transform(ref LocalAxisB, ref ConnectionB.Orientation, out axisB);

            float dot;
            Vector3Ex.Dot(ref axisA, ref axisB, out dot);

            //Yes, we could avoid this acos here. Performance is not the highest goal of this system; the less tricks used, the easier it is to understand.
            float angle = (float)Math.Acos(MathHelper.Clamp(dot, -1, 1));

            //One angular DOF is constrained by this limit.
            System.Numerics.Vector3 hingeAxis;
            Vector3Ex.Cross(ref axisA, ref axisB, out hingeAxis);

            angularJacobianA = new Matrix3x3 { M11 = hingeAxis.X, M12 = hingeAxis.Y, M13 = hingeAxis.Z };
            angularJacobianB = new Matrix3x3 { M11 = -hingeAxis.X, M12 = -hingeAxis.Y, M13 = -hingeAxis.Z };

            //Note how we've computed the jacobians despite the limit being potentially inactive.
            //This is to enable 'speculative' limits.
            if (angle >= maximumAngle)
            {
                velocityBias = new System.Numerics.Vector3(errorCorrectionFactor * (angle - maximumAngle), 0, 0);
            }
            else
            {
                //The constraint is not yet violated. But, it may be- allow only as much motion as could occur without violating the constraint.
                //Limits can't 'pull,' so this will not result in erroneous sticking.
                velocityBias = new System.Numerics.Vector3(angle - maximumAngle, 0, 0);
            }


        }
    }
}
