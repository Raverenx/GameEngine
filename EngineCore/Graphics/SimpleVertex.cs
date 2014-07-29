using SharpDX;
using System.Runtime.InteropServices;
using System.Numerics;
using Vector3 = System.Numerics.Vector3;

namespace EngineCore.Graphics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SimpleVertex
    {
        public readonly Vector3 Position;
        public readonly Vector3 Normal;
        public readonly Color4f Color;

        public SimpleVertex(Vector3 position, Vector3 normal, Color4f color)
        {
            this.Color = color;
            this.Normal = normal;
            this.Position = position;
        }

        // Fields for use in an InputLayout struct.

        public static readonly int PositionOffset = 0;
        public static readonly int NormalOffset = 12;
        // Same as above.
        public static readonly int ColorOffset = 24;

        public static int Size { get { return Marshal.SizeOf(typeof(SimpleVertex)); } }
    }
}