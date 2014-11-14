using System;
using BEPUutilities;

namespace BEPUik
{
    public class SingleBoneRevoluteConstraint : SingleBoneConstraint
    {
        private System.Numerics.Vector3 freeAxis;
        private System.Numerics.Vector3 constrainedAxis1;
        private System.Numerics.Vector3 constrainedAxis2;

        /// <summary>
        /// Gets or sets the direction to constrain the bone free axis to.
        /// </summary>
        public System.Numerics.Vector3 FreeAxis
        {
            get { return freeAxis; }
            set
            {
                freeAxis = value;
                constrainedAxis1 = System.Numerics.Vector3.Cross(freeAxis, Vector3Ex.Up);
                if (constrainedAxis1.LengthSquared() < Toolbox.Epsilon)
                {
                    constrainedAxis1 = System.Numerics.Vector3.Cross(freeAxis, Vector3Ex.Right);
                }
                constrainedAxis1.Normalize();
                constrainedAxis2 = System.Numerics.Vector3.Cross(freeAxis, constrainedAxis1);
            }
        }


        /// <summary>
        /// Axis of allowed rotation in the bone's local space.
        /// </summary>
        public System.Numerics.Vector3 BoneLocalFreeAxis;

        protected internal override void UpdateJacobiansAndVelocityBias()
        {
 

            linearJacobian = new Matrix3x3();

            System.Numerics.Vector3 boneAxis;
            QuaternionEx.Transform(ref BoneLocalFreeAxis, ref TargetBone.Orientation, out boneAxis);


            angularJacobian = new Matrix3x3
            {
                M11 = constrainedAxis1.X,
                M12 = constrainedAxis1.Y,
                M13 = constrainedAxis1.Z,
                M21 = constrainedAxis2.X,
                M22 = constrainedAxis2.Y,
                M23 = constrainedAxis2.Z
            };


            System.Numerics.Vector3 error;
            Vector3Ex.Cross(ref boneAxis, ref freeAxis, out error);
            System.Numerics.Vector2 constraintSpaceError;
            Vector3Ex.Dot(ref error, ref constrainedAxis1, out constraintSpaceError.X);
            Vector3Ex.Dot(ref error, ref constrainedAxis2, out constraintSpaceError.Y);
            velocityBias.X = errorCorrectionFactor * constraintSpaceError.X;
            velocityBias.Y = errorCorrectionFactor * constraintSpaceError.Y;


        }


    }
}
