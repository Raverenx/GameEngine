using EngineCore.Components;
using EngineCore.Graphics;
using EngineCore.Utility;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace EngineCore.Entities
{
    public class BoxRenderer : Component<SharpDxGraphicsSystem>, IRenderable
    {
        private SimpleShader shader;
        private PolyMesh cubeMesh;

        public void Render(SimpleRenderer renderer)
        {
            renderer.SetWorldMatrix(this.WorldMatrix);
            shader.ApplyShader();
            cubeMesh.Render(renderer);
        }

        private Vector3 scale;
        public Vector3 Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                scaleMatrix = Matrix4x4.CreateScale(scale);
            }
        }
        private Matrix4x4 scaleMatrix = Matrix4x4.Identity;
        public System.Numerics.Matrix4x4 WorldMatrix
        {
            get { return this.scaleMatrix * Transform.WorldMatrix; }
        }

        protected override void Initialize(SharpDxGraphicsSystem system)
        {
            this.shader = new SimpleShader(system.Renderer.Device, system.Renderer.DeviceContext, Resources.LightShader, "VS", "PS",
                new InputElement[]
                {
                    new InputElement("Position", 0, Format.R32G32B32_Float, SimpleVertex.PositionOffset, 0),
                    new InputElement("Normal", 0, Format.R32G32B32_Float, SimpleVertex.NormalOffset, 0),
                    new InputElement("Color", 0, Format.R32G32B32A32_Float, SimpleVertex.ColorOffset, 0)
                });

            this.cubeMesh = new PolyMesh(system.Renderer.Device,
                new SimpleVertex[]
                {
                    // Top
                    new SimpleVertex(new Vector3(-.5f,.5f,.5f), new Vector3(0,1,0), Color4f.Red),
                    new SimpleVertex(new Vector3(.5f,.5f,.5f), new Vector3(0,1,0), Color4f.Red),
                    new SimpleVertex(new Vector3(.5f,.5f,-.5f), new Vector3(0,1,0), Color4f.Red),
                    new SimpleVertex(new Vector3(-.5f,.5f,-.5f), new Vector3(0,1,0), Color4f.Red),
                    // Bottom
                    new SimpleVertex(new Vector3(-.5f,-.5f,.5f), new Vector3(0,1,0), Color4f.Grey),
                    new SimpleVertex(new Vector3(.5f,-.5f,.5f),new Vector3(0,1,0), Color4f.Grey),
                    new SimpleVertex(new Vector3(.5f,-.5f,-.5f),new Vector3(0,1,0), Color4f.Grey),
                    new SimpleVertex(new Vector3(-.5f,-.5f,-.5f),new Vector3(0,1,0), Color4f.Grey),
                    // Left
                    new SimpleVertex(new Vector3(-.5f,-.5f,.5f),new Vector3(1,0,0), Color4f.Blue),
                    new SimpleVertex(new Vector3(-.5f,.5f,.5f),new Vector3(1,0,0),Color4f.Blue),
                    new SimpleVertex(new Vector3(-.5f,.5f,-.5f),new Vector3(1,0,0),Color4f.Blue),
                    new SimpleVertex(new Vector3(-.5f,-.5f,-.5f),new Vector3(1,0,0),Color4f.Blue),
                    // Right
                    new SimpleVertex(new Vector3(.5f,-.5f,.5f),new Vector3(-1,0,0), Color4f.Green),
                    new SimpleVertex(new Vector3(.5f,.5f,.5f),new Vector3(-1,0,0), Color4f.Green),
                    new SimpleVertex(new Vector3(.5f,.5f,-.5f), new Vector3(-1,0,0), Color4f.Green),
                    new SimpleVertex(new Vector3(.5f,-.5f,-.5f),new Vector3(-1,0,0),Color4f.Green),
                    // Front
                    new SimpleVertex(new Vector3(-.5f,.5f,.5f),new Vector3(0,0,1),Color4f.Yellow),
                    new SimpleVertex(new Vector3(.5f,.5f,.5f),new Vector3(0,0,1),Color4f.Yellow),
                    new SimpleVertex(new Vector3(.5f,-.5f,.5f),new Vector3(0,0,1),Color4f.Yellow),
                    new SimpleVertex(new Vector3(-.5f,-.5f,.5f),new Vector3(0,0,1),Color4f.Yellow),
                    // Back
                    new SimpleVertex(new Vector3(-.5f,.5f,-.5f),new Vector3(0,0,1),Color4f.Orange),
                    new SimpleVertex(new Vector3(.5f,.5f,-.5f),new Vector3(0,0,1),Color4f.Orange),
                    new SimpleVertex(new Vector3(.5f,-.5f,-.5f),new Vector3(0,0,1),Color4f.Orange),
                    new SimpleVertex(new Vector3(-.5f,-.5f,-.5f),new Vector3(0,0,1),Color4f.Orange)
                },
                new int[]
                {
                    0,1,2,0,2,3,
                    4,5,6,4,6,7,
                    8,9,10,8,10,11,
                    12,13,14,12,14,15,
                    16,17,18,16,18,19,
                    20,21,22,20,22,23
                });

            system.Renderer.Renderables.Add(this);
        }
    }
}
