﻿using SharpDX;
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
    public class PolyMesh : IDisposable
    {
        private IList<System.Numerics.Vector3> vertices;
        private IList<int> indices;

        /// <summary>
        /// Returns the list of positions of this mesh's vertices.
        /// </summary>
        public IList<System.Numerics.Vector3> Vertices { get { return this.vertices; } }
        /// <summary>
        /// Returns the list of indices of this mesh.
        /// </summary>
        public IList<int> Indices { get { return this.indices; } }

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
            this.vertices = vertices.Select(sv => sv.Position).ToList();
            this.indices = indices;

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

        public void Dispose()
        {
            VertexBuffer.Dispose();
            NormalBuffer.Dispose();
            IndexBuffer.Dispose();
        }
    }
}
