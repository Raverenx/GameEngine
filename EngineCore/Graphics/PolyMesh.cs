using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Matrix4x4 = System.Numerics.Matrix4x4;

namespace EngineCore.Graphics
{
    public class PolyMesh
    {
        /// <summary>
        /// The vertex buffer of the mesh
        /// </summary>
        public SharpDX.Direct3D11.Buffer VertexBuffer { get; set; }
        /// <summary>
        /// The normal buffer of the mesh
        /// </summary>
        public SharpDX.Direct3D11.Buffer NormalBuffer { get; set; }
        /// <summary>
        /// The index buffer of the mesh
        /// </summary>
        public SharpDX.Direct3D11.Buffer IndexBuffer { get; set; }
        private Device device;
        private int indexCount;
        public PolyMesh(Device device, SimpleVertex[] vertices, int[] indices)
        {
            this.device = device;
            this.VertexBuffer = SharpDX.Direct3D11.Buffer.Create<SimpleVertex>(device, BindFlags.VertexBuffer, vertices);
            this.IndexBuffer = SharpDX.Direct3D11.Buffer.Create<int>(device, BindFlags.IndexBuffer, indices);
            this.indexCount = indices.Length;
        }

        public void Render(SimpleRenderer renderer)
        {
            var inputAssembler = renderer.DeviceContext.InputAssembler;
            inputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            inputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, SimpleVertex.Size, 0));
            inputAssembler.SetIndexBuffer(IndexBuffer, SharpDX.DXGI.Format.R32_UInt, 0);
            renderer.DeviceContext.DrawIndexed(indexCount, 0, 0);
        }
    }
}
