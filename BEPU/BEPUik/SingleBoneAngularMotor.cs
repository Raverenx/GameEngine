using BEPUutilities;

namespace BEPUik
{
    public class SingleBoneAngularMotor : SingleBoneConstraint
    {
        /// <summary>
        /// Gets or sets the target orientation to apply to the target bone.
        /// </summary>
        public System.Numerics.Quaternion TargetOrientation;

        protected internal override void UpdateJacobiansAndVelocityBias()
        {
            linearJacobian = new Matrix3x3();
            angularJacobian = Matrix3x3.Identity;

            //Error is in world space. It gets projected onto the jacobians later.
            System.Numerics.Quaternion errorQuaternion;
            QuaternionEx.Conjugate(ref TargetBone.Orientation, out errorQuaternion);
            QuaternionEx.Multiply(ref TargetOrientation, ref errorQuaternion, out errorQuaternion);
            float angle;
            System.Numerics.Vector3 angularError;
            QuaternionEx.GetAxisAngleFromQuaternion(ref errorQuaternion, out angularError, out angle);
            Vector3Ex.Multiply(ref angularError, angle, out angularError);

            //This is equivalent to projecting the error onto the angular jacobian. The angular jacobian just happens to be the identity matrix!
            Vector3Ex.Multiply(ref angularError, errorCorrectionFactor, out velocityBias);
        }


    }
}
