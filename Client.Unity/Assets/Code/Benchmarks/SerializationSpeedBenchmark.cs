using System.Collections;
using System.Diagnostics;
using System.IO;
using ProtoBuf;
using Silentor.TB.Common.Config;
using Silentor.TB.Common.Maps.Blocks;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Common.Network.Messages;
using UnityEngine;

namespace Assets.Code.Benchmarks
{
    public class SerializationSpeedBenchmark : MonoBehaviour 
    {
        // Use this for initialization
        void Start()
        {
            print("Stopwatch: freq " + Stopwatch.Frequency + ", is high resolution " + Stopwatch.IsHighResolution);

            //Warm up
            for (int i = 0; i < 10; i++)
            {
                InternalBench();
            }

            StartCoroutine(Bench());
        }

        IEnumerator Bench()
        {
            while (true)
            {
                InternalBench();
                yield return new WaitForSeconds(1);
            }
        }

        private class WorldConfigStub : IGlobeConfig
        {
            public int Seed { get; set; }

            public Bounds2i Bounds { get; private set; }
        }

        void InternalBench()
        {
            //Init environment
            var blockSet = new BlockSet();
            var wrld = new WorldConfigStub(){Seed = 666};

            //Test
            var generator = new TestGenerator(wrld, blockSet);
            var first = generator.GenerateSync(Vector2i.Zero);

            var stream = new MemoryStream();

            var sw = Stopwatch.StartNew();
            Serializer.Serialize(stream, first);
            stream.Position = 0;
            var second = Serializer.Deserialize<ChunkContents>(stream);

            print("Serialized/deserialized time: " + sw.ElapsedMilliseconds + " ms, data size: " + stream.Length);
        }

    
    }
}
