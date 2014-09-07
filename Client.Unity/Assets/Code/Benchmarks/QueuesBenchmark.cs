using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Silentor.TB.Common.Tools;
using UnityEngine;

namespace Assets.Code.Benchmarks
{
    public class QueuesBenchmark : MonoBehaviour 
    {
        private LockFreeQueue<TaskClass> _lockfreeClass = new LockFreeQueue<TaskClass>();
        private LockFreeQueue<TaskStruct> _lockfreeStruct = new LockFreeQueue<TaskStruct>();
        private Queue<TaskClass> _lockClass = new Queue<TaskClass>();
        private Queue<TaskStruct> _lockStruct = new Queue<TaskStruct>();
        private const int _tasksPerThread = 5000;
        private const int _threads = 10;


        private struct TaskStruct : ITask
        {
            public TaskStruct(int priority) : this()
            {
                Priority = priority;
            }

            public Action Do { get; private set; }
            public Action CompleteCallback { get; private set; }
            public int Priority { get; private set; }
        }

        private interface ITask
        {
            Action Do { get; }
            Action CompleteCallback { get; }
            int Priority { get; }
        }

        private class TaskClass : ITask
        {
            public TaskClass()
            {}

            public TaskClass(int priority)
            {
                Priority = priority;
            }

            public Action Do { get; private set; }
            public Action CompleteCallback { get; private set; }
            public int Priority { get; private set; }
        }

        // Use this for initialization
        void Start () 
        {
            //Warm up
            for (int i = 0; i < _threads; i++)
                ThreadPool.QueueUserWorkItem(o => Produce(_lockfreeClass));

            Thread.Sleep(10);                   //To prevent starving
            Consume(_lockfreeClass);

            UnityEngine.Debug.Log("Start tests");

            StartCoroutine(Tester());
        }

        IEnumerator Tester()
        {
            while (true)
            {
                var sw = Stopwatch.StartNew();

                for (int i = 0; i < _threads; i++)
                    ThreadPool.QueueUserWorkItem(o => Produce(_lockfreeClass));

                Thread.Sleep(10); //To prevent starving
                Consume(_lockfreeClass);

                sw.Stop();
                UnityEngine.Debug.Log("Lock free Class task elapsed: " + sw.ElapsedMilliseconds);
                sw.Reset();

                yield return null;

                //------------------------------------------

                sw = Stopwatch.StartNew();

                for (int i = 0; i < _threads; i++)
                    ThreadPool.QueueUserWorkItem(o => Produce(_lockClass));

                Thread.Sleep(10); //To prevent starving
                Consume(_lockClass);

                sw.Stop();
                UnityEngine.Debug.Log("Lock Class task elapsed: " + sw.ElapsedMilliseconds);
                sw.Reset();

                yield return null;
            }

        }

        void Produce<TAsk>(LockFreeQueue<TAsk> queue) where TAsk : ITask, new()
        {
            for (int i = 0; i < _tasksPerThread; i++)
            {
                var task = new TAsk();
            
                queue.Enqueue(task);
            }
        }

        void Produce<TAsk>(Queue<TAsk> queue) where TAsk : ITask, new()
        {
            for (int i = 0; i < _tasksPerThread; i++)
            {
                var task = new TAsk();

                lock(queue)
                    queue.Enqueue(task);
            }
        }

        void Consume<TAsk>(LockFreeQueue<TAsk> queue) where TAsk : ITask
        {
            TAsk task;
            int count = 0;

            while (true)
            {
                if (queue.TryDequeue(out task))
                {
                    if (++count == _threads * _tasksPerThread)
                        return;
                }
                else
                    UnityEngine.Debug.Log("Consumer starving");

            }
        }

        void Consume<TAsk>(Queue<TAsk> queue) where TAsk : ITask
        {
            TAsk task;
            int count = 0;

            bool starving = false;

            while (true)
            {
                lock (queue)
                {
                    if (queue.Count > 0)
                        task = queue.Dequeue();
                    else starving = true;
                }

                if (!starving)
                {
                    if (++count == _threads*_tasksPerThread)
                        return;
                }
                else
                {
                    UnityEngine.Debug.Log("Consumer starving");
                    starving = false;
                }

            }
        }
    }
}
