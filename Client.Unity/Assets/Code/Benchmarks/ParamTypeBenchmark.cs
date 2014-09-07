using System.Diagnostics;
using Silentor.TB.Client.Maps;
using Silentor.TB.Common.Maps.Blocks;
using UnityEngine;

namespace Assets.Code.Benchmarks
{
    public class ParamTypeBenchmark : MonoBehaviour 
    {
        private BlockData[] _blockDatas = new BlockData[16*16*128];
        private Block _block = new Block(1);

        // Use this for initialization
        void Start () 
        {
            print("Stopwatch: freq " + Stopwatch.Frequency + ", is high resolution " + Stopwatch.IsHighResolution);

            //Warm up
            for (int x = 0; x < 16; x++)
                for (int y = 0; y < 128; y++)
                    for (int z = 0; z < 16; z++)
                    {
                        var x1 = (byte) x;
                        var y1 = (byte)y;
                        var z1 = (byte)z;

                        SetBlockInt(x, y, z, new BlockData(_block));
                        SetBlockByte(x1, y1, z1, new BlockData(_block));

                    }
        }

        void Update()
        {
            //Test byte params
            var sw2 = Stopwatch.StartNew();
            for (int i = 0; i < 100; i++)
                for (byte x = 0; x < 16; x++)
                    for (byte y = 0; y < 128; y++)
                        for (byte z = 0; z < 16; z++)
                            SetBlockByte(x, y, z, new BlockData(_block));
            print("Byte time " + sw2.ElapsedMilliseconds);

            //Test int params
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 100; i++)
                for (int x = 0; x < 16; x++)
                    for (int y = 0; y < 128; y++)
                        for (int z = 0; z < 16; z++)
                            SetBlockInt(x, y, z, new BlockData(_block));
            print("Int time " + sw.ElapsedMilliseconds);

        
        }

        private void SetBlockInt(int x, int y, int z, BlockData block)
        {
            _blockDatas[y + Chunk.SizeY*x + Chunk.SizeY*Chunk.SizeX*z] = block;
        }

        private void SetBlockByte(byte x, byte y, byte z, BlockData block)
        {
            _blockDatas[y + Chunk.SizeY * x + Chunk.SizeY * Chunk.SizeX * z] = block;
        }
    }
}
