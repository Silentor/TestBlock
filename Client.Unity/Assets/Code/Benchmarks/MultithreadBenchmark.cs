using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Assets.Code.Benchmarks
{
    public class MultithreadBenchmark : MonoBehaviour 
    {
        TestWorker[] _threads = new TestWorker[16];

        private const int TotalTasks = 1000000;

        // Use this for initialization
        void Start ()
        {
            print("Processors count: " + SystemInfo.processorCount);
            print("Warming up...");

            for (int i = 0; i < _threads.Length; i++)
            {
                _threads[i] = new TestWorker {Count = 10000};
                new Thread(_threads[i].Worker).Start();
            }

            var finished = _threads.Select(t => t.FinishHandle).ToArray();
            WaitHandle.WaitAll(finished);

        }

        private int threadsCount = 1;

        // Update is called once per frame
        void Update ()
        {
            print("Work with " + threadsCount + " threads");

            var finished = _threads.Take(threadsCount).Select(t => t.FinishHandle).ToArray();

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < threadsCount; i++)
            {
                _threads[i].Count = TotalTasks/threadsCount;
                _threads[i].EndHandle.Set();
            }

            WaitHandle.WaitAll(finished);

            print("Elapsed " + sw.ElapsedMilliseconds);

            threadsCount = threadsCount == 16 ? 1 : threadsCount + 1;
        }

        public class TestWorker
        {
            public AutoResetEvent EndHandle = new AutoResetEvent(false);
            public AutoResetEvent FinishHandle = new AutoResetEvent(false);
            public int Count = 0;

            public void Worker()
            {
                while (true)
                {
                    try
                    {
                        for (int i = 0; i < Count; i++)
                        {
                            Mathf.Sqrt(i);
                        }
                    }
                    catch (Exception ex)
                    {
                        print(ex);
                    }
                    finally
                    {
                        FinishHandle.Set();
                        EndHandle.WaitOne();
                    }
                }
            }
        }
    }
}
