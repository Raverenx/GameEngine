﻿using BEPUutilities;

namespace BEPUik
{
    //Keeps the anchors from two connections near each other.
    public class IKBallSocketJoint : IKJoint
    {
        /// <summary>
        /// Gets or sets the offset in connection A's local space from the center of mass to the anchor point.
        /// </summary>
        public System.Numerics.Vector3 LocalOffsetA;
        /// <summary>
        /// Gets or sets the offset in connection B's local space from the center of mass to the anchor point.
        /// </summary>
        public System.Numerics.Vector3 LocalOffsetB;

        /// <summary>
        /// Gets or sets the offset in world space from the center of mass of connection A to the anchor point.
        /// </summary>
        public System.Numerics.Vector3 OffsetA
        {
            get { return QuaternionEx.Transform(LocalOffsetA, ConnectionA.Orientation); }
            set { LocalOffsetA = QuaternionEx.Transform(value, QuaternionEx.Conjugate(ConnectionA.Orientation)); }
        }

        /// <summary>
        /// Gets or sets the offset in world space from the center of mass of connection B to the anchor point.
        /// </summary>
        public System.Numerics.Vector3 OffsetB
        {
            get { return QuaternionEx.Transform(LocalOffsetB, ConnectionB.Orientation); }
            set { LocalOffsetB = QuaternionEx.Transform(value, QuaternionEx.Conjugate(ConnectionB.Orientation)); }
        }

        /// <summary>
        /// Builds a ball socket joint.
        /// </summary>
        /// <param name="connectionA">First connection in the pair.</param>
        /// <param name="connectionB">Second connection in the pair.</param>
        /// <param name="anchor">World space anchor location used to initialize the local anchors.</param>
        public IKBallSocketJoint(Bone connectionA, Bone connectionB, System.Numerics.Vector3 anchor)
            : base(connectionA, connectionB)
        {
            OffsetA = anchor - ConnectionA.Position;
            OffsetB = anchor - ConnectionB.Position;
        }

        protected internal override void UpdateJacobiansAndVelocityBias()
        {
            linearJacobianA = Matrix3x3.Identity;
            //The jacobian entries are is [ La, Aa, -Lb, -Ab ] because the relative velocity is computed using A-B. So, negate B's jacobians!
            linearJacobianB = new Matrix3x3 { M11 = -1, M22 = -1, M33 = -1 };
            System.Numerics.Vector3 rA;
            QuaternionEx.Transform(ref LocalOffsetA, ref ConnectionA.Orientation, out rA);
            Matrix3x3.CreateCrossProduct(ref rA, out angularJacobianA);
            //Transposing a skew-symmetric matrix is equivalent to negating it.
            Matrix3x3.Transpose(ref angularJacobianA, out angularJacobianA);

            System.Numerics.Vector3 worldPositionA;
            Vector3Ex.Add(ref ConnectionA.Position, ref rA, out worldPositionA);

            System.Numerics.Vector3 rB;
            QuaternionEx.Transform(ref LocalOffsetB, ref ConnectionB.Orientation, out rB);
            Matrix3x3.CreateCrossProduct(ref rB, out angularJacobianB);

            System.Numerics.Vector3 worldPositionB;
            Vector3Ex.Add(ref ConnectionB.Position, ref rB, out worldPositionB);

            System.Numerics.Vector3 linearError;
            Vector3Ex.Subtract(ref worldPositionB, ref worldPositionA, out linearError);
            Vector3Ex.Multiply(ref linearError, errorCorrectionFactor, out velocityBias);

        }
    }
}