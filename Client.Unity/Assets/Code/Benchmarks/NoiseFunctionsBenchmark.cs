using System.Diagnostics;
using Silentor.TB.Common.Tools;
using UnityEngine;

namespace Assets.Code.Benchmarks
{
    public class NoiseFunctionsBenchmark : MonoBehaviour
    {

        // Use this for initialization
        void Start () 
        {
            print("Stopwatch: freq " + Stopwatch.Frequency + ", is high resolution " + Stopwatch.IsHighResolution);

            //Warm up
            for (int x = 0; x < 1000; x++)
            {
                for (int y = 0; y < 1000; y++)
                {
                    var a = FastIntPerlinNoise.noise(x/80f, y/80f, 1);
                    var b = SimplexNoise.noise(x/80f, y/80f);
                }
            }
        }

        void Update()
        {
            //Test fast int
            var sw = Stopwatch.StartNew();
            for (int x = 0; x < 1000; x++)
            {
                for (int y = 0; y < 1000; y++)
                {
                    var a = FastIntPerlinNoise.noise(x / 80f, y / 80f, 1);
                }
            }
            print("Fast int time " + sw.ElapsedMilliseconds);

            //Test fast int
            sw = Stopwatch.StartNew();
            for (int x = 0; x < 1000; x++)
            {
                for (int y = 0; y < 1000; y++)
                {
                    var a = FastIntPerlinNoise.noise(x / 80f, y / 80f, 2);
                }
            }
            print("Fast int 2 octaves time " + sw.ElapsedMilliseconds);

            //Test simplex
            sw = Stopwatch.StartNew();
            for (int x = 0; x < 1000; x++)
            {
                for (int y = 0; y < 1000; y++)
                {
                    var b = SimplexNoise.noise(x / 80f, y / 80f);
                }
            }
            print("Simplex 2d time " + sw.ElapsedMilliseconds);

            //Test simplex
            sw = Stopwatch.StartNew();
            for (int x = 0; x < 1000; x++)
            {
                for (int y = 0; y < 1000; y++)
                {
                    var b = SimplexNoise.noise(x / 80f, y / 80f) + SimplexNoise.noise(x / 80f, y / 80f);
                }
            }
            print("Simplex 2d 2 octaves time " + sw.ElapsedMilliseconds);

            //Test simplex
            sw = Stopwatch.StartNew();
            for (int x = 0; x < 1000; x++)
            {
                for (int y = 0; y < 1000; y++)
                {
                    var b = SimplexNoise.noise(x / 80f, y / 80f, (x + y) / 80f);
                }
            }
            print("Simplex 3d time " + sw.ElapsedMilliseconds);
        }

    }
}
