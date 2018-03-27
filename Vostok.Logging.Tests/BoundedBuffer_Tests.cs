using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Vostok.Logging.Tests
{
    [TestFixture]
    internal class BoundedBuffer_Tests
    {
        [Test]
        public void TryAdd_should_return_true_when_buffer_has_free_space()
        {
            for (var i = 0; i < Capacity; i++)
                buffer.TryAdd(i.ToString()).Should().BeTrue();
        }

        [Test]
        public void TryAdd_should_return_false_when_buffer_is_full()
        {
            for (var i = 0; i < Capacity; i++)
                buffer.TryAdd(i.ToString());

            buffer.TryAdd("x").Should().BeFalse();
        }

        [Test]
        public void Drain_should_return_empty_array_when_buffer_is_empty()
        {
            buffer.Drain(ref drainResult, 0, drainResult.Length);
            drainResult.Should().Equal(Enumerable.Repeat((string) null, Capacity));
        }

        [TestCase(1)]
        [TestCase(Capacity - 1)]
        [TestCase(Capacity)]
        public void Drain_should_return_correct_result_and_remove_items_from_buffer(int count)
        {
            for (var i = 0; i < count; i++)
                buffer.TryAdd(i.ToString());

            buffer.Drain(ref drainResult, 0, drainResult.Length);
            drainResult.Should().Equal(Enumerable.Range(0, count).Select(i => i.ToString()));
            buffer.Drain(ref drainResult, 0, drainResult.Length);
            drainResult.Should().Equal(Enumerable.Repeat((string)null, Capacity));
        }

        [Test]
        public void Drain_should_make_room_for_new_items()
        {
            for (var i = 0; i < Capacity; i++)
                buffer.TryAdd(i.ToString()).Should().BeTrue();

            buffer.Drain(ref drainResult, 0, drainResult.Length);

            for (var i = 0; i < Capacity; i++)
                buffer.TryAdd(i.ToString()).Should().BeTrue();
        }

        [Test]
        public void TryAdd_and_Drain_should_behave_correctly_on_random_workloads()
        {
            var random = new Random();

            for (var i = 0; i < 10*1000; i++)
            {
                var count = random.Next(Capacity + 1);

                for (var j = 0; j < count; j++)
                {
                    buffer.TryAdd(j.ToString()).Should().BeTrue();
                }

                buffer.Drain(ref drainResult, 0, drainResult.Length);
                drainResult.Should().Equal(Enumerable.Range(0, count).Select(j => j.ToString()));
            }
        }

        [SetUp]
        public void SetUp()
        {
            buffer = new BoundedBuffer<string>(Capacity);
            drainResult = new string[Capacity];
        }

        private BoundedBuffer<string> buffer;
        private string[] drainResult;
        private const int Capacity = 5;
    }
}