using System.Threading;

namespace Vostok.Logging.Core
{
    internal class BoundedBuffer<T> where T : class
    {
        public int Count => itemsCount;

        public BoundedBuffer(int capacity)
        {
            items = new T[capacity];
            canDrain = new ManualResetEventSlim();
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
                            if (currentCount == 0)
                                canDrain.Set();
                            return true;
                        }
                    }
                }
            }
        }

        public int Drain(T[] buffer, int index, int count)
        {
            if (itemsCount == 0)
                return 0;

            canDrain.Reset();

            var resultCount = 0;

            for (var i = 0; i < count; i++)
            {
                var itemIndex = (backPtr + i) % items.Length;
                var item = Interlocked.Exchange(ref items[itemIndex], null);
                if (item == null)
                    break;

                buffer[index + resultCount++] = item;
            }

            backPtr = (backPtr + resultCount) % items.Length;

            Interlocked.Add(ref itemsCount, -resultCount);

            return resultCount;
        }

        public void WaitForNewItems()
        {
            canDrain.Wait();
        }

        private readonly ManualResetEventSlim canDrain;
        private readonly T[] items;
        private int itemsCount;
        private int frontPtr;
        private volatile int backPtr;
    }
}