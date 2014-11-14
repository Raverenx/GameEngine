using System;
using BEPUutilities;

namespace BEPUik
{
    /// <summary>
    /// Keeps the anchor points on two bones at the same distance.
    /// </summary>
    public class IKPointOnLineJoint : IKJoint
    {
        /// <summary>
        /// Gets or sets the offset in connection A's local space from the center of mass to the anchor point of the line.
        /// </summary>
        public System.Numerics.Vector3 LocalLineAnchor;

        private System.Numerics.Vector3 localLineDirection;
        /// <summary>
        /// Gets or sets the direction of the line in connection A's local space.
        /// Must be unit length.
        /// </summary>
        public System.Numerics.Vector3 LocalLineDirection
        {
            get { return localLineDirection; }
            set
            {
                localLineDirection = value;
                ComputeRestrictedAxes();
            }
        }


        /// <summary>
        /// Gets or sets the offset in connection B's local space from the center of mass to the anchor point which will be kept on the line.
        /// </summary>
        public System.Numerics.Vector3 LocalAnchorB;

        /// <summary>
        /// Gets or sets the world space location of the line anchor attached to connection A.
        /// </summary>
        public System.Numerics.Vector3 LineAnchor
        {
            get { return ConnectionA.Position + QuaternionEx.Transform(LocalLineAnchor, ConnectionA.Orientation); }
            set { LocalLineAnchor = QuaternionEx.Transform(value - ConnectionA.Position, QuaternionEx.Conjugate(ConnectionA.Orientation)); }
        }

        /// <summary>
        /// Gets or sets the world space direction of the line attached to connection A.
        /// Must be unit length.
        /// </summary>
        public System.Numerics.Vector3 LineDirection
        {
            get { return QuaternionEx.Transform(localLineDirection, ConnectionA.Orientation); }
            set { LocalLineDirection = QuaternionEx.Transform(value, QuaternionEx.Conjugate(ConnectionA.Orientation)); }
        }

        /// <summary>
        /// Gets or sets the offset in world space from the center of mass of connection B to the anchor point.
        /// </summary>
        public System.Numerics.Vector3 AnchorB
        {
            get { return ConnectionB.Position + QuaternionEx.Transform(LocalAnchorB, ConnectionB.Orientation); }
            set { LocalAnchorB = QuaternionEx.Transform(value - ConnectionB.Position, QuaternionEx.Conjugate(ConnectionB.Orientation)); }
        }

        private System.Numerics.Vector3 localRestrictedAxis1, localRestrictedAxis2;
        void ComputeRestrictedAxes()
        {
            System.Numerics.Vector3 cross;
            Vector3Ex.Cross(ref localLineDirection, ref Toolbox.UpVector, out cross);
            float lengthSquared = cross.LengthSquared();
            if (lengthSquared > Toolbox.Epsilon)
            {
                Vector3Ex.Divide(ref cross, (float)Math.Sqrt(lengthSquared), out localRestrictedAxis1);
            }
            else
            {
                //Oops! The direction is aligned with the up vector.
                Vector3Ex.Cross(ref localLineDirection, ref Toolbox.RightVector, out cross);
                Vector3Ex.Normalize(ref cross, out localRestrictedAxis1);
            }
            //Don't need to normalize this; cross product of two unit length perpendicular vectors.
            Vector3Ex.Cross(ref localRestrictedAxis1, ref localLineDirection, out localRestrictedAxis2);
        }

        /// <summary>
        /// Constructs a new point on line joint.
        /// </summary>
        /// <param name="connectionA">First bone connected by the joint.</param>
        /// <param name="connectionB">Second bone connected by the joint.</param>
        /// <param name="lineAnchor">Anchor point of the line attached to the first bone in world space.</param>
        /// <param name="lineDirection">Direction of the line attached to the first bone in world space. Must be unit length.</param>
        /// <param name="anchorB">Anchor point on the second bone in world space which tries to stay on connection A's line.</param>
        public IKPointOnLineJoint(Bone connectionA, Bone connectionB, System.Numerics.Vector3 lineAnchor, System.Numerics.Vector3 lineDirection, System.Numerics.Vector3 anchorB)
            : base(connectionA, connectionB)
        {
            LineAnchor = lineAnchor;
            LineDirection = lineDirection;
            AnchorB = anchorB;

        }

        protected internal override void UpdateJacobiansAndVelocityBias()
        {

            //Transform local stuff into world space
            System.Numerics.Vector3 worldRestrictedAxis1, worldRestrictedAxis2;
            QuaternionEx.Transform(ref localRestrictedAxis1, ref ConnectionA.Orientation, out worldRestrictedAxis1);
            QuaternionEx.Transform(ref localRestrictedAxis2, ref ConnectionA.Orientation, out worldRestrictedAxis2);

            System.Numerics.Vector3 worldLineAnchor;
            QuaternionEx.Transform(ref LocalLineAnchor, ref ConnectionA.Orientation, out worldLineAnchor);
            Vector3Ex.Add(ref worldLineAnchor, ref ConnectionA.Position, out worldLineAnchor);
            System.Numerics.Vector3 lineDirection;
            QuaternionEx.Transform(ref localLineDirection, ref ConnectionA.Orientation, out lineDirection);

            System.Numerics.Vector3 rB;
            QuaternionEx.Transform(ref LocalAnchorB, ref ConnectionB.Orientation, out rB);
            System.Numerics.Vector3 worldPoint;
            Vector3Ex.Add(ref rB, ref ConnectionB.Position, out worldPoint);

            //Find the point on the line closest to the world point.
            System.Numerics.Vector3 offset;
            Vector3Ex.Subtract(ref worldPoint, ref worldLineAnchor, out offset);
            float distanceAlongAxis;
            Vector3Ex.Dot(ref offset, ref lineDirection, out distanceAlongAxis);

            System.Numerics.Vector3 worldNearPoint;
            Vector3Ex.Multiply(ref lineDirection, distanceAlongAxis, out offset);
            Vector3Ex.Add(ref worldLineAnchor, ref offset, out worldNearPoint);
            System.Numerics.Vector3 rA;
            Vector3Ex.Subtract(ref worldNearPoint, ref ConnectionA.Position, out rA);

            //Error
            System.Numerics.Vector3 error3D;
            Vector3Ex.Subtract(ref worldPoint, ref worldNearPoint, out error3D);

            System.Numerics.Vector2 error;
            Vector3Ex.Dot(ref error3D, ref worldRestrictedAxis1, out error.X);
            Vector3Ex.Dot(ref error3D, ref worldRestrictedAxis2, out error.Y);

            velocityBias.X = errorCorrectionFactor * error.X;
            velocityBias.Y = errorCorrectionFactor * error.Y;


            //Set up the jacobians
            System.Numerics.Vector3 angularA1, angularA2, angularB1, angularB2;
            Vector3Ex.Cross(ref rA, ref worldRestrictedAxis1, out angularA1);
            Vector3Ex.Cross(ref rA, ref worldRestrictedAxis2, out angularA2);
            Vector3Ex.Cross(ref worldRestrictedAxis1, ref rB, out angularB1);
            Vector3Ex.Cross(ref worldRestrictedAxis2, ref rB, out angularB2);

            //Put all the 1x3 jacobians into a 3x3 matrix representation.
            linearJacobianA = new Matrix3x3
            {
                M11 = worldRestrictedAxis1.X,
                M12 = worldRestrictedAxis1.Y,
                M13 = worldRestrictedAxis1.Z,
                M21 = worldRestrictedAxis2.X,
                M22 = worldRestrictedAxis2.Y,
                M23 = worldRestrictedAxis2.Z
            };
            Matrix3x3.Negate(ref linearJacobianA, out linearJacobianB);

            angularJacobianA = new Matrix3x3
            {
                M11 = angularA1.X,
                M12 = angularA1.Y,
                M13 = angularA1.Z,
                M21 = angularA2.X,
                M22 = angularA2.Y,
                M23 = angularA2.Z
            };
            angularJacobianB = new Matrix3x3
            {
                M11 = angularB1.X,
                M12 = angularB1.Y,
                M13 = angularB1.Z,
                M21 = angularB2.X,
                M22 = angularB2.Y,
                M23 = angularB2.Z
            };
        }
    }
}
