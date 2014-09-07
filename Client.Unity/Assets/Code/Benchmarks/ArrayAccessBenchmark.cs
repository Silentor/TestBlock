using System.Diagnostics;
using Silentor.TB.Common.Maps.Blocks;
using UnityEngine;

namespace Assets.Code.Benchmarks
{
    public class ArrayAccessBenchmark : MonoBehaviour 
    {
        private BlockData[][][] _blockDatas = new BlockData[16][][];
        private BlockData[] _blockDatas2 = new BlockData[16*16*128];
        private Block _block = new Block(1);

        // Use this for initialization
        void Start () 
        {
            print("Stopwatch: freq " + Stopwatch.Frequency + ", is high resolution " + Stopwatch.IsHighResolution);

            //Warm up
            for (int i = 0; i < 100; i++)
            {
                TestJagged(_blockDatas);
                TestSimple(_blockDatas2);
            }
        }

        void Update()
        {
            //Test jagged
            var sw = Stopwatch.StartNew();
            for (int j = 0; j < 100; j++)
                TestJagged(_blockDatas);
            print("Jagged time " + sw.ElapsedMilliseconds);

            //Test simple
            sw = Stopwatch.StartNew();
            for (int k = 0; k < 100; k++)
                TestSimple(_blockDatas2);
            print("Simple time " + sw.ElapsedMilliseconds);
        }

        private void TestJagged(BlockData[][][] arr)
        {
            for (int i = 0; i < 16; i++)
            {
                arr[i] = new BlockData[16][];
                for (int j = 0; j < 16; j++)
                {
                    arr[i][j] = new BlockData[128];
                    for (int k = 0; k < 128; k++)
                    {
                        arr[i][j][k] = new BlockData(_block);
                    }
                }
            }

        }

        private void TestSimple(BlockData[] arr)
        {
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    for (int k = 0; k < 128; k++)
                    {
                        arr[k + 128*i + 128*16*j] = new BlockData(_block);
                    }
                }
            }
        }
    }
}
