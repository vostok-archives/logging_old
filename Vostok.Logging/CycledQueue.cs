using System;
using System.Linq;
using System.Threading;

namespace Vostok.Logging
{
    public class CycledQueue<T>
    {
        public bool IsEmpty => frontPtr == backPtr;

        public int Count
        {
            get
            {
                var difference = frontPtr - backPtr;
                return difference < 0 
                    ? buffer.Length + difference
                    : difference;
            }
        }

        public CycledQueue(int capacity)
        {
            capacity = Math.Max(capacity + 1, 2);
            buffer = new T[capacity];
        }

        public void Enqueue(T item)
        {
            while (true)
            {
                var currentPtr = MoveFrontPointer();
                if (currentPtr != -1)
                {
                    buffer[currentPtr] = item;
                    return;
                }

                MoveBackPointer();
            }
        }

        public bool TryPeek(out T item)
        {
            item = default(T);
            var currentPtr = backPtr;
            var itemFromBuffer = buffer[currentPtr];

            if (currentPtr == frontPtr)
                return false;

            item = itemFromBuffer;
            return true;
        }

        public bool TryDequeue(out T item)
        {
            item = default(T);
            var currentPtr = MoveBackPointer();

            if (currentPtr == -1)
                return false;

            item = buffer[currentPtr];
            return true;
        }

        private int MoveBackPointer()
        {
            while (true)
            {
                var currentPtr = backPtr;
                if (currentPtr == frontPtr)
                    return -1;

                if (Interlocked.CompareExchange(ref backPtr, (currentPtr + 1) % buffer.Length, currentPtr) == currentPtr)
                    return currentPtr;
            }
        }

        private int MoveFrontPointer()
        {
            while (true)
            {
                var currentPtr = frontPtr;
                if ((currentPtr + 1) % buffer.Length == backPtr)
                    return -1;

                if (Interlocked.CompareExchange(ref frontPtr, (currentPtr + 1) % buffer.Length, currentPtr) == currentPtr)
                    return currentPtr;
            }
        }

        public override string ToString()
        {
            return string.Join(", ", buffer.Select(i => i?.ToString() ?? "null"));
        }

        private readonly T[] buffer;
        private int frontPtr;
        private int backPtr;
    }
}