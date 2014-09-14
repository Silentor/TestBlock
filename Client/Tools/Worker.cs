using System;
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using NLog;
using Silentor.TB.Common.Tools;

namespace Silentor.TB.Client.Tools
{
    /// <summary>
    /// Simplest worker on background thread for parallel func processing, has input and output queues
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class Worker<TSource, TResult>
    {
        /// <summary>
        /// Count of succesfully processed items
        /// </summary>
        public int ProcessedCount { get { return _processedCount; } }

        public Worker([NotNull] Func<TSource, TResult> workerFunc)
        {
            if (workerFunc == null) throw new ArgumentNullException("workerFunc");
            _workerFunc = workerFunc;
            _thread = new Thread(WorkerLoop){IsBackground = true, Name = "ClientWorker"};
            _thread.Start();
        }

        /// <summary>
        /// Add value to process by worker
        /// </summary>
        /// <param name="source"></param>
        public void Add(TSource source)
        {
            lock (_inputQueue)
            {
                _inputQueue.Enqueue(source);
                _hasData.Set();
            }
        }

        public bool GetResult(out TResult result)
        {
            if(_outputQueue.Count > 0)
                lock (_outputQueue)
                {
                    if (_outputQueue.Count > 0)
                    {
                        result = _outputQueue.Dequeue();
                        return true;
                    }
                }

            result = default(TResult);
            return false;
        }

        public void Close()
        {
            _isStopped = true;
            lock (_inputQueue)
                _hasData.Set();
        }

        private void WorkerLoop()
        {
            while (!_isStopped)
            {
                _hasData.WaitOne();

                //Get source data
                var input = default(TSource);
                lock (_inputQueue)
                {
                    if (_isStopped)
                        return;

                    if (_inputQueue.Count > 0)
                        input = _inputQueue.Dequeue();
                    else
                    {
                        _hasData.Reset();
                        continue;
                    }
                }

                //Apply func
                try
                {
                    var result = _workerFunc(input);

                    //Set result to output queue
                    lock (_outputQueue)
                    {
                        _outputQueue.Enqueue(result);
                    }

                    Interlocked.Increment(ref _processedCount);
                }
                catch (Exception ex)
                {
                    Log.Error("Exception in worker function", ex);
                }
            }
        }

        private readonly Func<TSource, TResult> _workerFunc;
        private bool _isStopped;
        private Thread _thread;
        private readonly ManualResetEvent _hasData = new ManualResetEvent(false);
        private readonly Queue<TSource> _inputQueue = new Queue<TSource>();
        private readonly Queue<TResult> _outputQueue = new Queue<TResult>();
        private int _processedCount;

        private readonly Logger Log = LogManager.GetCurrentClassLogger();
    }
}
