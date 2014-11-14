using System;
using BEPUutilities;

namespace BEPUik
{
    public class IKRevoluteJoint : IKJoint
    {
        private System.Numerics.Vector3 localFreeAxisA;
        /// <summary>
        /// Gets or sets the free axis in connection A's local space.
        /// Must be unit length.
        /// </summary>
        public System.Numerics.Vector3 LocalFreeAxisA
        {
            get { return localFreeAxisA; }
            set
            {
                localFreeAxisA = value;
                ComputeConstrainedAxes();
            }
        }

        private System.Numerics.Vector3 localFreeAxisB;
        /// <summary>
        /// Gets or sets the free axis in connection B's local space.
        /// Must be unit length.
        /// </summary>
        public System.Numerics.Vector3 LocalFreeAxisB
        {
            get { return localFreeAxisB; }
            set
            {
                localFreeAxisB = value;
                ComputeConstrainedAxes();
            }
        }



        /// <summary>
        /// Gets or sets the free axis attached to connection A in world space.
        /// This does not change the other connection's free axis.
        /// </summary>
        public System.Numerics.Vector3 WorldFreeAxisA
        {
            get { return QuaternionEx.Transform(localFreeAxisA, ConnectionA.Orientation); }
            set
            {
                LocalFreeAxisA = QuaternionEx.Transform(value, QuaternionEx.Conjugate(ConnectionA.Orientation));
            }
        }

        /// <summary>
        /// Gets or sets the free axis attached to connection B in world space.
        /// This does not change the other connection's free axis.
        /// </summary>
        public System.Numerics.Vector3 WorldFreeAxisB
        {
            get { return QuaternionEx.Transform(localFreeAxisB, ConnectionB.Orientation); }
            set
            {
                LocalFreeAxisB = QuaternionEx.Transform(value, QuaternionEx.Conjugate(ConnectionB.Orientation));
            }
        }

        private System.Numerics.Vector3 localConstrainedAxis1, localConstrainedAxis2;
        void ComputeConstrainedAxes()
        {
            System.Numerics.Vector3 worldAxisA = WorldFreeAxisA;
            System.Numerics.Vector3 error = System.Numerics.Vector3.Cross(worldAxisA, WorldFreeAxisB);
            float lengthSquared = error.LengthSquared();
            System.Numerics.Vector3 worldConstrainedAxis1, worldConstrainedAxis2;
            //Find the first constrained axis.
            if (lengthSquared > Toolbox.Epsilon)
            {
                //The error direction can be used as the first axis!
                Vector3Ex.Divide(ref error, (float)Math.Sqrt(lengthSquared), out worldConstrainedAxis1);
            }
            else
            {
                //There's not enough error for it to be a good constrained axis.
                //We'll need to create the constrained axes arbitrarily.
                Vector3Ex.Cross(ref Toolbox.UpVector, ref worldAxisA, out worldConstrainedAxis1);
                lengthSquared = worldConstrainedAxis1.LengthSquared();
                if (lengthSquared > Toolbox.Epsilon)
                {
                    //The up vector worked!
                    Vector3Ex.Divide(ref worldConstrainedAxis1, (float)Math.Sqrt(lengthSquared), out worldConstrainedAxis1);
                }
                else
                {
                    //The up vector didn't work. Just try the right vector.
                    Vector3Ex.Cross(ref Toolbox.RightVector, ref worldAxisA, out worldConstrainedAxis1);
                    worldConstrainedAxis1.Normalize();
                }
            }
            //Don't have to normalize the second constraint axis; it's the cross product of two perpendicular normalized vectors.
            Vector3Ex.Cross(ref worldAxisA, ref worldConstrainedAxis1, out worldConstrainedAxis2);

            localConstrainedAxis1 = QuaternionEx.Transform(worldConstrainedAxis1, QuaternionEx.Conjugate(ConnectionA.Orientation));
            localConstrainedAxis2 = QuaternionEx.Transform(worldConstrainedAxis2, QuaternionEx.Conjugate(ConnectionA.Orientation));
        }

        /// <summary>
        /// Constructs a new orientation joint.
        /// Orientation joints can be used to simulate the angular portion of a hinge.
        /// Orientation joints allow rotation around only a single axis.
        /// </summary>
        /// <param name="connectionA">First entity connected in the orientation joint.</param>
        /// <param name="connectionB">Second entity connected in the orientation joint.</param>
        /// <param name="freeAxis">Axis allowed to rotate freely in world space.</param>
        public IKRevoluteJoint(Bone connectionA, Bone connectionB, System.Numerics.Vector3 freeAxis)
            : base(connectionA, connectionB)
        {
            WorldFreeAxisA = freeAxis;
            WorldFreeAxisB = freeAxis;
        }

        protected internal override void UpdateJacobiansAndVelocityBias()
        {
            linearJacobianA = linearJacobianB = new Matrix3x3();

            //We know the one free axis. We need the two restricted axes. This amounts to completing the orthonormal basis.
            //We can grab one of the restricted axes using a cross product of the two world axes. This is not guaranteed
            //to be nonzero, so the normalization requires protection.

            System.Numerics.Vector3 worldAxisA, worldAxisB;
            QuaternionEx.Transform(ref localFreeAxisA, ref ConnectionA.Orientation, out worldAxisA);
            QuaternionEx.Transform(ref localFreeAxisB, ref ConnectionB.Orientation, out worldAxisB);

            System.Numerics.Vector3 error;
            Vector3Ex.Cross(ref worldAxisA, ref worldAxisB, out error);

            System.Numerics.Vector3 worldConstrainedAxis1, worldConstrainedAxis2;
            QuaternionEx.Transform(ref localConstrainedAxis1, ref ConnectionA.Orientation, out worldConstrainedAxis1);
            QuaternionEx.Transform(ref localConstrainedAxis2, ref ConnectionA.Orientation, out worldConstrainedAxis2);


            angularJacobianA = new Matrix3x3
            {
                M11 = worldConstrainedAxis1.X,
                M12 = worldConstrainedAxis1.Y,
                M13 = worldConstrainedAxis1.Z,
                M21 = worldConstrainedAxis2.X,
                M22 = worldConstrainedAxis2.Y,
                M23 = worldConstrainedAxis2.Z
            };
            Matrix3x3.Negate(ref angularJacobianA, out angularJacobianB);


            System.Numerics.Vector2 constraintSpaceError;
            Vector3Ex.Dot(ref error, ref worldConstrainedAxis1, out constraintSpaceError.X);
            Vector3Ex.Dot(ref error, ref worldConstrainedAxis2, out constraintSpaceError.Y);
            velocityBias.X = errorCorrectionFactor * constraintSpaceError.X;
            velocityBias.Y = errorCorrectionFactor * constraintSpaceError.Y;


        }
    }
}
