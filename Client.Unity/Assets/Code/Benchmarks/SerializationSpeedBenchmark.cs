using System.Collections;
using System.Diagnostics;
using System.IO;
using ModestTree;
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

            _serializer = new MessageSerializer(new MessageFactory());

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
                yield return StartCoroutine(InternalBench());
            }
        }

        private class WorldConfigStub : IGlobeConfig
        {
            public int Seed { get; set; }

            public Bounds2i Bounds { get; private set; }
        }

        IEnumerator InternalBench()
        {
            //Init environment
            var blockSet = new BlockSet();
            var wrld = new WorldConfigStub(){Seed = 666};

            //Test
            var generator = new TestGenerator(wrld, blockSet);
            var first = generator.GenerateSync(Vector2i.Zero);

            int length;
           
            //Test compressed ser/deser
            var sw = Stopwatch.StartNew();
            var buffer = _serializer.Serialize(new ChunkMessage(first), out length, true);
            var second = _serializer.Deserialize(buffer);
            var time = sw.ElapsedMilliseconds;

            print("Compressed serialized/deserialized time: " + time + " ms, data size: " + length);

            yield return new WaitForSeconds(0.5f);

            //Test compressed ser/deser
            sw.Reset();
            sw.Start();
            buffer = _serializer.Serialize(new ChunkMessage(first), out length, false);
            second = _serializer.Deserialize(buffer);
            time = sw.ElapsedMilliseconds;

            print("Uncompressed serialized/deserialized time: " + time + " ms, data size: " + length);

            yield return new WaitForSeconds(0.5f);
        }
    }
}
