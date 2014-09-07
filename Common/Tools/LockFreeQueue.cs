﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Silentor.TB.Common.Tools
{
    public sealed class LockFreeQueue<T>
    {
        private sealed class Node
        {
            public readonly T Item;
            public Node Next;
            public Node(T item)
            {
                Item = item;
            }
        }
        private volatile Node _head;
        private volatile Node _tail;

        public LockFreeQueue()
        {
            _head = _tail = new Node(default(T));
        }

#pragma warning disable 420 // volatile semantics not lost as only by-ref calls are interlocked
        public void Enqueue(T item)
        {
            Node newNode = new Node(item);
            for (; ; )
            {
                Node curTail = _tail;
                if (Interlocked.CompareExchange(ref curTail.Next, newNode, null) == null)   //append to the tail if it is indeed the tail.
                {
                    Interlocked.CompareExchange(ref _tail, newNode, curTail);   //CAS in case we were assisted by an obstructed thread.
                    return;
                }
                else
                {
                    Interlocked.CompareExchange(ref _tail, curTail.Next, curTail);  //assist obstructing thread.
                }
            }
        }

        public bool TryDequeue(out T item)
        {
            for (; ; )
            {
                Node curHead = _head;
                Node curTail = _tail;
                Node curHeadNext = curHead.Next;
                if (curHead == curTail)
                {
                    if (curHeadNext == null)
                    {
                        item = default(T);
                        return false;
                    }
                    else
                        Interlocked.CompareExchange(ref _tail, curHeadNext, curTail);   // assist obstructing thread
                }
                else
                {
                    item = curHeadNext.Item;
                    if (Interlocked.CompareExchange(ref _head, curHeadNext, curHead) == curHead)
                    {
                        return true;
                    }
                }
            }
        }
#pragma warning restore 420
    }
}
