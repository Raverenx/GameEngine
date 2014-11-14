﻿using System;
using BEPUutilities;

namespace BEPUik
{
    /// <summary>
    /// Keeps the anchor points on two bones at the same distance.
    /// </summary>
    public class IKDistanceJoint : IKJoint
    {
        /// <summary>
        /// Gets or sets the offset in connection A's local space from the center of mass to the anchor point.
        /// </summary>
        public System.Numerics.Vector3 LocalAnchorA;
        /// <summary>
        /// Gets or sets the offset in connection B's local space from the center of mass to the anchor point.
        /// </summary>
        public System.Numerics.Vector3 LocalAnchorB;

        /// <summary>
        /// Gets or sets the offset in world space from the center of mass of connection A to the anchor point.
        /// </summary>
        public System.Numerics.Vector3 AnchorA
        {
            get { return ConnectionA.Position + QuaternionEx.Transform(LocalAnchorA, ConnectionA.Orientation); }
            set { LocalAnchorA = QuaternionEx.Transform(value - ConnectionA.Position, QuaternionEx.Conjugate(ConnectionA.Orientation)); }
        }

        /// <summary>
        /// Gets or sets the offset in world space from the center of mass of connection A to the anchor point.
        /// </summary>
        public System.Numerics.Vector3 AnchorB
        {
            get { return ConnectionB.Position + QuaternionEx.Transform(LocalAnchorB, ConnectionB.Orientation); }
            set { LocalAnchorB = QuaternionEx.Transform(value - ConnectionB.Position, QuaternionEx.Conjugate(ConnectionB.Orientation)); }
        }

        private float distance;
        /// <summary>
        /// Gets or sets the distance that the joint connections should be kept from each other.
        /// </summary>
        public float Distance
        {
            get { return distance; }
            set { distance = Math.Max(0, value); }
        }

        /// <summary>
        /// Constructs a new distance joint.
        /// </summary>
        /// <param name="connectionA">First bone connected by the joint.</param>
        /// <param name="connectionB">Second bone connected by the joint.</param>
        /// <param name="anchorA">Anchor point on the first bone in world space.</param>
        /// <param name="anchorB">Anchor point on the second bone in world space.</param>
        public IKDistanceJoint(Bone connectionA, Bone connectionB, System.Numerics.Vector3 anchorA, System.Numerics.Vector3 anchorB)
            : base(connectionA, connectionB)
        {
            AnchorA = anchorA;
            AnchorB = anchorB;
            Vector3Ex.Distance(ref anchorA, ref anchorB, out distance);
        }

        protected internal override void UpdateJacobiansAndVelocityBias()
        {
            //Transform the anchors and offsets into world space.
            System.Numerics.Vector3 offsetA, offsetB;
            QuaternionEx.Transform(ref LocalAnchorA, ref ConnectionA.Orientation, out offsetA);
            QuaternionEx.Transform(ref LocalAnchorB, ref ConnectionB.Orientation, out offsetB);
            System.Numerics.Vector3 anchorA, anchorB;
            Vector3Ex.Add(ref ConnectionA.Position, ref offsetA, out anchorA);
            Vector3Ex.Add(ref ConnectionB.Position, ref offsetB, out anchorB);

            //Compute the distance.
            System.Numerics.Vector3 separation;
            Vector3Ex.Subtract(ref anchorB, ref anchorA, out separation);
            float currentDistance = separation.Length();

            //Compute jacobians
            System.Numerics.Vector3 linearA;
#if !WINDOWS
            linearA = new System.Numerics.Vector3();
#endif
            if (currentDistance > Toolbox.Epsilon)
            {
                linearA.X = separation.X / currentDistance;
                linearA.Y = separation.Y / currentDistance;
                linearA.Z = separation.Z / currentDistance;

                velocityBias = new System.Numerics.Vector3(errorCorrectionFactor * (currentDistance - distance), 0, 0);
            }
            else
            {
                velocityBias = new System.Numerics.Vector3();
                linearA = new System.Numerics.Vector3();
            }

            System.Numerics.Vector3 angularA, angularB;
            Vector3Ex.Cross(ref offsetA, ref linearA, out angularA);
            //linearB = -linearA, so just swap the cross product order.
            Vector3Ex.Cross(ref linearA, ref offsetB, out angularB);

            //Put all the 1x3 jacobians into a 3x3 matrix representation.
            linearJacobianA = new Matrix3x3 { M11 = linearA.X, M12 = linearA.Y, M13 = linearA.Z };
            linearJacobianB = new Matrix3x3 { M11 = -linearA.X, M12 = -linearA.Y, M13 = -linearA.Z };
            angularJacobianA = new Matrix3x3 { M11 = angularA.X, M12 = angularA.Y, M13 = angularA.Z };
            angularJacobianB = new Matrix3x3 { M11 = angularB.X, M12 = angularB.Y, M13 = angularB.Z };

        }
    }
}
