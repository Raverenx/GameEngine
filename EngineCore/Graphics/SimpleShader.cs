using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Graphics
{
    public class SimpleShader
    {
        private Device device;
        private DeviceContext deviceContext;

        private VertexShader vertexShader;
        private PixelShader pixelShader;
        private InputLayout inputLayout;

        public SimpleShader(Device device, DeviceContext deviceContext, string fileName, string vsEntryPoint, string psEntryPoint, InputElement[] inputElements)
        {
            this.device = device;
            this.deviceContext = deviceContext;
            var compiledVertexShader = ShaderBytecode.CompileFromFile(fileName, vsEntryPoint, "vs_5_0");
            var compiledPixelShader = ShaderBytecode.CompileFromFile(fileName, psEntryPoint, "ps_5_0");

            this.vertexShader = new VertexShader(device, compiledVertexShader);
            this.pixelShader = new PixelShader(device, compiledPixelShader);

            var shaderSignature = ShaderSignature.GetInputSignature(compiledVertexShader);
            this.inputLayout = new InputLayout(device, shaderSignature, inputElements);
        }

        /// <summary>
        /// Applies this shader by setting the input layout, vertex shader, and pixel shader.
        /// </summary>
        public void ApplyShader()
        {
            deviceContext.InputAssembler.InputLayout = this.inputLayout;
            deviceContext.VertexShader.Set(this.vertexShader);
            deviceContext.PixelShader.Set(this.pixelShader);
        }
    }
}
