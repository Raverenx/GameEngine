using System.Numerics;
namespace EngineCore.Graphics
{
    public interface IRenderable
    {
        void Render(SimpleRenderer renderer);
        Matrix4x4 WorldMatrix { get; }
    }
}
