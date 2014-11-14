using BEPUutilities;

namespace BEPUik
{
    /// <summary>
    /// Attempts to maintain the relative orientation between two bones.
    /// </summary>
    public class IKAngularJoint : IKJoint
    {
        /// <summary>
        /// Gets or sets the rotation from connection A's orientation to connection B's orientation in A's local space.
        /// </summary>
        public System.Numerics.Quaternion GoalRelativeOrientation;


        /// <summary>
        /// Constructs a 3DOF angular joint which tries to keep two bones in angular alignment.
        /// </summary>
        /// <param name="connectionA">First bone to connect to the joint.</param>
        /// <param name="connectionB">Second bone to connect to the joint.</param>
        public IKAngularJoint(Bone connectionA, Bone connectionB)
            : base(connectionA, connectionB)
        {  
            System.Numerics.Quaternion orientationAConjugate;
            QuaternionEx.Conjugate(ref ConnectionA.Orientation, out orientationAConjugate);
            //Store the orientation from A to B in A's local space in the GoalRelativeOrientation.
            QuaternionEx.Concatenate(ref ConnectionB.Orientation, ref orientationAConjugate, out GoalRelativeOrientation);

        }

        protected internal override void UpdateJacobiansAndVelocityBias()
        {
            linearJacobianA = linearJacobianB = new Matrix3x3();
            angularJacobianA = new Matrix3x3 { M11 = 1, M22 = 1, M33 = 1 };
            angularJacobianB = new Matrix3x3 { M11 = -1, M22 = -1, M33 = -1 };

            //The error is computed using this equation:
            //GoalRelativeOrientation * ConnectionA.Orientation * Error = ConnectionB.Orientation
            //GoalRelativeOrientation is the original rotation from A to B in A's local space.
            //Multiplying by A's orientation gives us where B *should* be.
            //Of course, B won't be exactly where it should be after initialization.
            //The Error component holds the difference between what is and what should be.
            //Error = (GoalRelativeOrientation * ConnectionA.Orientation)^-1 * ConnectionB.Orientation
            System.Numerics.Quaternion bTarget;
            QuaternionEx.Concatenate(ref GoalRelativeOrientation, ref ConnectionA.Orientation, out bTarget);
            System.Numerics.Quaternion bTargetConjugate;
            QuaternionEx.Conjugate(ref bTarget, out bTargetConjugate);

            System.Numerics.Quaternion error;
            QuaternionEx.Concatenate(ref bTargetConjugate, ref ConnectionB.Orientation, out error);

            //Convert the error into an axis-angle vector usable for bias velocity.
            float angle;
            System.Numerics.Vector3 axis;
            QuaternionEx.GetAxisAngleFromQuaternion(ref error, out axis, out angle);

            velocityBias.X = errorCorrectionFactor * axis.X * angle;
            velocityBias.Y = errorCorrectionFactor * axis.Y * angle;
            velocityBias.Z = errorCorrectionFactor * axis.Z * angle;


        }
    }
}
