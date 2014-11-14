using System;
using BEPUutilities;

namespace BEPUik
{
    public class IKSwivelHingeJoint : IKJoint
    {
        /// <summary>
        /// Gets or sets the free hinge axis attached to connection A in its local space.
        /// </summary>
        public System.Numerics.Vector3 LocalHingeAxis;
        /// <summary>
        /// Gets or sets the free twist axis attached to connection B in its local space.
        /// </summary>
        public System.Numerics.Vector3 LocalTwistAxis;


        /// <summary>
        /// Gets or sets the free hinge axis attached to connection A in world space.
        /// </summary>
        public System.Numerics.Vector3 WorldHingeAxis
        {
            get { return QuaternionEx.Transform(LocalHingeAxis, ConnectionA.Orientation); }
            set
            {
                LocalHingeAxis = QuaternionEx.Transform(value, QuaternionEx.Conjugate(ConnectionA.Orientation));
            }
        }

        /// <summary>
        /// Gets or sets the free twist axis attached to connection B in world space.
        /// </summary>
        public System.Numerics.Vector3 WorldTwistAxis
        {
            get { return QuaternionEx.Transform(LocalTwistAxis, ConnectionB.Orientation); }
            set
            {
                LocalTwistAxis = QuaternionEx.Transform(value, QuaternionEx.Conjugate(ConnectionB.Orientation));
            }
        }

        /// <summary>
        /// Constructs a new constraint which allows relative angular motion around a hinge axis and a twist axis.
        /// </summary>
        /// <param name="connectionA">First connection of the pair.</param>
        /// <param name="connectionB">Second connection of the pair.</param>
        /// <param name="worldHingeAxis">Hinge axis attached to connectionA.
        /// The connected bone will be able to rotate around this axis relative to each other.</param>
        /// <param name="worldTwistAxis">Twist axis attached to connectionB.
        /// The connected bones will be able to rotate around this axis relative to each other.</param>
        public IKSwivelHingeJoint(Bone connectionA, Bone connectionB, System.Numerics.Vector3 worldHingeAxis, System.Numerics.Vector3 worldTwistAxis)
            : base(connectionA, connectionB)
        {
            WorldHingeAxis = worldHingeAxis;
            WorldTwistAxis = worldTwistAxis;
        }

        protected internal override void UpdateJacobiansAndVelocityBias()
        {
            linearJacobianA = linearJacobianB = new Matrix3x3();


            //There are two free axes and one restricted axis.
            //The constraint attempts to keep the hinge axis attached to connection A and the twist axis attached to connection B perpendicular to each other.
            //The restricted axis is the cross product between the twist and hinge axes.

            System.Numerics.Vector3 worldTwistAxis, worldHingeAxis;
            QuaternionEx.Transform(ref LocalHingeAxis, ref ConnectionA.Orientation, out worldHingeAxis);
            QuaternionEx.Transform(ref LocalTwistAxis, ref ConnectionB.Orientation, out worldTwistAxis);

            System.Numerics.Vector3 restrictedAxis;
            Vector3Ex.Cross(ref worldHingeAxis, ref worldTwistAxis, out restrictedAxis);
            //Attempt to normalize the restricted axis.
            float lengthSquared = restrictedAxis.LengthSquared();
            if (lengthSquared > Toolbox.Epsilon)
            {
                Vector3Ex.Divide(ref restrictedAxis, (float)Math.Sqrt(lengthSquared), out restrictedAxis);
            }
            else
            {
                restrictedAxis = new System.Numerics.Vector3();
            }


            angularJacobianA = new Matrix3x3
              {
                  M11 = restrictedAxis.X,
                  M12 = restrictedAxis.Y,
                  M13 = restrictedAxis.Z,
              };
            Matrix3x3.Negate(ref angularJacobianA, out angularJacobianB);

            float error;
            Vector3Ex.Dot(ref worldHingeAxis, ref worldTwistAxis, out error);
            error = (float)Math.Acos(MathHelper.Clamp(error, -1, 1)) - MathHelper.PiOver2;

            velocityBias = new System.Numerics.Vector3(errorCorrectionFactor * error, 0, 0);


        }
    }
}
