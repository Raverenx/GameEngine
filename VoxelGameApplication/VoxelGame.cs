using EngineCore;
using EngineCore.Graphics;
using EngineCore.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelGameApplication
{
    class VoxelGame : Game
    {
        protected override void PerformCustomInitialization()
        {
            GameObject camera = new GameObject();
            camera.AddComponent(new Camera());
            camera.AddComponent(new NoisePrinter());
            camera.AddComponent(new EngineCore.Behaviours.CameraMovementController());
            new GameObject().AddComponent<FpsTracker>();
            Chunk chunk = new ChunkGenerator().GenerateChunk(.9f);
            Debug.WriteLine(chunk.GetChunkDataAtCoordinate(5, 5, 5).BlockType);
            Debug.WriteLine(SimplexNoise.GenerateSimplexNoiseWithOctaves(5, 0, 0, 0));
            Debug.WriteLine(SimplexNoise.GenerateSimplexNoiseWithOctaves(5, 5, -10, 1));
            Debug.WriteLine(SimplexNoise.GenerateSimplexNoiseWithOctaves(5, 7, 5, 2));

            for (int x = 0; x < Chunk.ChunkWidth; x++)
            {
                for (int y = 0; y < Chunk.ChunkHeight; y++)
                {
                    for (int z = 0; z < Chunk.ChunkDepth; z++)
                    {
                        var pointData = chunk.GetChunkDataAtCoordinate(x, y, z);
                        if (pointData.BlockType == BlockType.Stone)
                        {
                            var box = GameObject.CreateStaticBox(1.0f, 1.0f, 1.0f);
                            box.Transform.Position = new System.Numerics.Vector3(x, y, z);
                        }
                    }
                }
            }
        }
    }

    public class ChunkGenerator
    {
        public Chunk GenerateChunk(float threshold)
        {
            Chunk chunk = new Chunk();
            for (int x = 0; x < Chunk.ChunkWidth; x++)
            {
                for (int y = 0; y < Chunk.ChunkHeight; y++)
                {
                    for (int z = 0; z < Chunk.ChunkDepth; z++)
                    {
                        float density = SimplexNoise.GenerateSingleNoiseValue(3* x, 2 * y -.1f, 3* z +.4f);
                        Debug.WriteLine(density);
                        chunk.DataBuffer[x + y * Chunk.ChunkWidth + z * Chunk.ChunkHeight * Chunk.ChunkDepth]
                            = new ChunkPointData(density > .2f ? BlockType.Stone : BlockType.Air);
                    }
                }
            }
            return chunk;
        }
    }

    public class Chunk
    {
        public const int ChunkWidth = 16, ChunkHeight = 16, ChunkDepth = 16;
        public ChunkPointData[] DataBuffer = new ChunkPointData[ChunkWidth * ChunkDepth * ChunkHeight];

        public unsafe ChunkPointData GetChunkDataAtCoordinate(int x, int y, int z)
        {
            fixed (ChunkPointData* dataPtr = DataBuffer)
            {
                return dataPtr[x + y * ChunkWidth + z * ChunkHeight * ChunkDepth];
            }
        }
    }

    [DebuggerDisplay("Type:{BlockType}")]
    public struct ChunkPointData
    {
        public readonly Byte Data;
        public ChunkPointData(Byte b) { this.Data = b; }
        public ChunkPointData(BlockType type) { this.Data = (byte)type; }
        public BlockType BlockType { get { return (BlockType)this.Data; } }
    }

    public enum BlockType : byte
    {
        Air = 0,
        Stone = 1
    }

    class NoisePrinter : Behaviour
    {
        float xVal = 0f, yVal = 12f, zVal = -50f;

        protected override void Update()
        {
            if (InputSystem.GetKeyDown(System.Windows.Forms.Keys.F))
            {
                Debug.WriteLine(
                    string.Format("<{0},{1},{2}>   ", xVal.ToString("00"), yVal.ToString("00"), zVal.ToString("00"))
                    + SimplexNoise.GenerateSimplexNoiseWithOctaves(2, xVal, yVal, zVal));
                xVal += 5f * (((DateTime.Now.Millisecond & 1) == 0) ? 1f : -1f);
                yVal += 2f * (((DateTime.Now.Millisecond & 3) == 0) ? 1f : -1f);
                zVal += 1f * (((DateTime.Now.Millisecond & 7) == 0) ? 1f : -1f);

                xVal++;
                yVal--;
                zVal += .5f;
            }
        }
    }

}
