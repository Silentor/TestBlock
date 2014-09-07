using System.Diagnostics;
using Silentor.TB.Common.Maps.Geometry;
using UnityEngine;

namespace Assets.Code.Benchmarks
{
    public class ParametersBenchmark : MonoBehaviour
    {

        // Use this for initialization
        void Start () 
        {
            //Warm up
            for (int i = 0; i < 1000; i++)
            {
                var x = i + 1;
                var y = i + i;
                var z = i*i;
                var pos = new Vector3i(x, y, z);
                TestMethod1(x, y, z);
                TestMethod2(pos);
            }
        }
	
        // Update is called once per frame
        void Update ()
        {
            var x = Time.frameCount + 1;
            var y = x + x;
            var z = x * y;
            var pos = new Vector3i(x, y, z);

            var result = 0;
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 10000000; i++)
            {
                result ^= TestMethod2(pos);
            }
            UnityEngine.Debug.Log("Test method 1: " + sw.ElapsedMilliseconds);

            result = 0;
            sw = Stopwatch.StartNew();
            for (int i = 0; i < 10000000; i++)
            {
                result ^= TestMethod1(x, y, z);
	        
            }
            UnityEngine.Debug.Log("Test method 2: " + sw.ElapsedMilliseconds);
        }

        int TestMethod1(int x, int y, int z)
        {
            return x*x + y*y + z*z;
        }

        int TestMethod2(Vector3i pos)
        {
            return pos.X * pos.X + pos.Y * pos.Y + pos.Z * pos.Z;
        }

    }
}
