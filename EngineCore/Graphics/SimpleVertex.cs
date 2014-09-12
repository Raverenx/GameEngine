using SharpDX;
using System.Runtime.InteropServices;
using System.Numerics;
using Vector3 = System.Numerics.Vector3;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace EngineCore.Graphics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SimpleVertex
    {
        public readonly Vector3 Position;
        private float __packing;
        public readonly Vector3 Normal;
        public readonly Color4f Color;

        public SimpleVertex(Vector3 position, Vector3 normal, Color4f color)
        {
            this.Color = color;
            this.Normal = Vector3.Normalize(normal);
            this.Position = position;

            this.__packing = 0f;
        }

        // Fields for use in an InputLayout struct.

        public static readonly int PositionOffset = 0;
        public static readonly int NormalOffset = 16;
        public static readonly int ColorOffset = 28;

        public static int Size { get { return Marshal.SizeOf(typeof(SimpleVertex)); } }

        public static readonly InputElement[] VertexInputLayout = new InputElement[]
        {
            new InputElement("Position", 0, Format.R32G32B32A32_Float, PositionOffset, 0),
            new InputElement("Normal", 0, Format.R32G32B32_Float, NormalOffset, 0),
            new InputElement("Color", 0, Format.R32G32B32A32_Float, ColorOffset, 0)
        };
    }
}