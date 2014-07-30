using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Utility
{
    public static class MathUtil
    {
        public static Vector3 NormalizedOrZero(this Vector3 vec)
        {
            float length = vec.Length();
            if (length < float.Epsilon)
            {
                return Vector3.Zero;
            }
            else
            {
                return vec / length;
            }
        }
    }
}
