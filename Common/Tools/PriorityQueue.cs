using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Silentor.TB.Common.Tools
{
    /// <summary>
    /// Queue with priority. Not threadsafe
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class PriorityQueue<TItem> : ICollection
    {
        public PriorityQueue()
        {
            for (int i = 0; i < _queues.Length; i++)
                _queues[i] = new Queue<TItem>();
        }

        public void Enqueue(TItem item, ItemPriority itemPriority)
        {
            _queues[(byte)itemPriority].Enqueue(item);
            Count++;
        }

        public bool TryDequeue(out TItem item)
        {
            if (_queues[(byte) ItemPriority.High].Count > 0)
            {
                item = _queues[(byte) ItemPriority.High].Dequeue();
                Count--;
                return true;
            }
            if (_queues[(byte) ItemPriority.Normal].Count > 0)
            {
                item = _queues[(byte)ItemPriority.Normal].Dequeue();
                Count--;
                return true;
            }
            if (_queues[(byte) ItemPriority.Low].Count > 0)
            {
                item = _queues[(byte)ItemPriority.Low].Dequeue();
                Count--;
                return true;
            }

            item = default(TItem);
            return false;
        }

        public TItem Peek()
        {
            if (_queues[(byte)ItemPriority.High].Count > 0)
                return _queues[(byte)ItemPriority.High].Peek();
            if (_queues[(byte)ItemPriority.Normal].Count > 0)
                return _queues[(byte)ItemPriority.Normal].Peek();
            if (_queues[(byte)ItemPriority.Low].Count > 0)
                return _queues[(byte)ItemPriority.Low].Peek();

            return default(TItem);
        }

        private readonly Queue<TItem>[] _queues = new Queue<TItem>[Enum.GetValues(typeof(ItemPriority)).Length];

        public IEnumerator GetEnumerator()
        {
            foreach (var item in _queues[(byte)ItemPriority.High])
                yield return item;
            foreach (var item in _queues[(byte)ItemPriority.Normal])
                yield return item;
            foreach (var item in _queues[(byte)ItemPriority.Low])
                yield return item;
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int Count { get; private set; }
        public bool IsSynchronized { get { return false; } }
        public object SyncRoot { get { return null; } }
    }


    public enum ItemPriority : byte
    {
        High = 0,
        Normal,
        Low
    }
}
