using System.Threading;

namespace Vostok.Logging
{
    internal class BoundedBuffer<T> where T : class
    {
        public int Count => itemsCount;

        public BoundedBuffer(int capacity)
        {
            items = new T[capacity];
        }

        public bool TryAdd(T item)
        {
            while (true)
            {
                var currentCount = itemsCount;
                if (currentCount >= items.Length)
                    return false;

                if (Interlocked.CompareExchange(ref itemsCount, currentCount + 1, currentCount) == currentCount)
                {
                    while (true)
                    {
                        var currentFrontPtr = frontPtr;

                        if (Interlocked.CompareExchange(ref frontPtr, (currentFrontPtr + 1)%items.Length, currentFrontPtr) == currentFrontPtr)
                        {
                            items[currentFrontPtr] = item;
                            return true;
                        }
                    }
                }
            }
        }

        public void Drain(ref T[] buffer, int index, int count)
        {
            if(itemsCount == 0)
                return;

            var resultCount = 0;

            for (var i = 0; i < count; i++)
            {
                var itemIndex = (backPtr + i) % items.Length;
                var item = Interlocked.Exchange(ref items[itemIndex], null);
                if(item == null)
                    break;

                buffer[index + resultCount++] = item;
            }

            backPtr = (backPtr + resultCount) % items.Length;

            Interlocked.Add(ref itemsCount, -resultCount);
        }

        private readonly T[] items;
        private int itemsCount;
        private int frontPtr;
        private volatile int backPtr;
    }
}