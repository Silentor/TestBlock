using System.Collections;
using System.Diagnostics;
using System.IO;
using Silentor.TB.Common.Config;
using Silentor.TB.Common.Maps.Blocks;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Common.Network.Messages;
using Silentor.TB.Common.Network.Serialization;
using UnityEngine;

namespace Assets.Code.Benchmarks
{
    public class SerializationSpeedBenchmark : MonoBehaviour 
    {
        private MessageSerializer _serializer;

        // Use this for initialization
        void Start()
        {
            print("Stopwatch: freq " + Stopwatch.Frequency + ", is high resolution " + Stopwatch.IsHighResolution);

            _serializer = new MessageSerializer();

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

            var sw = Stopwatch.StartNew();
            var buffer = _serializer.Serialize(first);
            var second = _serializer.Deserialize(buffer);

            print("Serialized/deserialized time: " + sw.ElapsedMilliseconds + " ms, data size: " + buffer.LengthBytes);
        }

    
    }
}
