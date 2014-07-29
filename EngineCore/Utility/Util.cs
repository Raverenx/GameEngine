using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Utility
{
    public static class Util
    {
        public static Matrix4x4 CreatePerspectiveProjection(float left, float right, float bottom, float top, float near, float far)
        {
            return new Matrix4x4(
            (2f * near) / (right - left), 0, 0, 0,
            0, (2f * near) / (top - bottom), 0, 0,
            (left + right) / (left - right), (top + bottom) / (bottom - top), far / (far - near), 1,
            0, 0, -near * (far / (far - near)), 0);
        }

        // Formula: DirectX reference pages
        public static Matrix4x4 CreatePerspectiveProjectionByFov(float fieldOfView, float aspectRatio, float near, float far)
        {
            float xScale = (float)(1.0 / Math.Tan(fieldOfView * 0.5f));
            float yScale = xScale / aspectRatio;
            float right = near / yScale;
            float top = near / xScale;
            return CreatePerspectiveProjection(-right, right, -top, top, near, far);
        }
    }
}
