using System;
using BEPUutilities;

namespace BEPUik
{
    public class SingleBoneAngularPlaneConstraint : SingleBoneConstraint
    {
        /// <summary>
        /// Gets or sets normal of the plane which the bone's axis will be constrained to..
        /// </summary>
        public System.Numerics.Vector3 PlaneNormal;



        /// <summary>
        /// Axis to constrain to the plane in the bone's local space.
        /// </summary>
        public System.Numerics.Vector3 BoneLocalAxis;

        protected internal override void UpdateJacobiansAndVelocityBias()
        {
 

            linearJacobian = new Matrix3x3();

            System.Numerics.Vector3 boneAxis;
            QuaternionEx.Transform(ref BoneLocalAxis, ref TargetBone.Orientation, out boneAxis);

            System.Numerics.Vector3 jacobian;
            Vector3Ex.Cross(ref boneAxis, ref PlaneNormal, out jacobian);

            angularJacobian = new Matrix3x3
            {
                M11 = jacobian.X,
                M12 = jacobian.Y,
                M13 = jacobian.Z,
            };


            Vector3Ex.Dot(ref boneAxis, ref PlaneNormal, out velocityBias.X);
            velocityBias.X = -errorCorrectionFactor * velocityBias.X;


        }


    }
}
