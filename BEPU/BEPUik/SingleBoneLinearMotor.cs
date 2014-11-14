using BEPUutilities;

namespace BEPUik
{
    public class SingleBoneLinearMotor : SingleBoneConstraint
    {
        /// <summary>
        /// Gets or sets the target position to apply to the target bone.
        /// </summary>
        public System.Numerics.Vector3 TargetPosition;

        /// <summary>
        /// Gets or sets the offset in the bone's local space to the point which will be pulled towards the target position.
        /// </summary>
        public System.Numerics.Vector3 LocalOffset;


        public System.Numerics.Vector3 Offset
        {
            get { return QuaternionEx.Transform(LocalOffset, TargetBone.Orientation); }
            set { LocalOffset = QuaternionEx.Transform(value, QuaternionEx.Conjugate(TargetBone.Orientation)); }
        }

        protected internal override void UpdateJacobiansAndVelocityBias()
        {
            linearJacobian = Matrix3x3.Identity;
            System.Numerics.Vector3 r;
            QuaternionEx.Transform(ref LocalOffset, ref TargetBone.Orientation, out r);
            Matrix3x3.CreateCrossProduct(ref r, out angularJacobian);
            //Transposing a skew symmetric matrix is equivalent to negating it.
            Matrix3x3.Transpose(ref angularJacobian, out angularJacobian);

            System.Numerics.Vector3 worldPosition;
            Vector3Ex.Add(ref TargetBone.Position, ref r, out worldPosition);

            //Error is in world space.
            System.Numerics.Vector3 linearError;
            Vector3Ex.Subtract(ref TargetPosition, ref worldPosition, out linearError);
            //This is equivalent to projecting the error onto the linear jacobian. The linear jacobian just happens to be the identity matrix!
            Vector3Ex.Multiply(ref linearError, errorCorrectionFactor, out velocityBias);
        }


    }
}
