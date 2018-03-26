using System;
using FluentAssertions;
using NUnit.Framework;

namespace Vostok.Logging.Tests
{
    [TestFixture]
    internal class CycledQueue_Tests
    {
        [Test]
        public void Count_should_be_zero_if_queue_was_not_in_use()
        {
            queue.Count.Should().Be(0);
        }

        [Test]
        public void Count_should_return_one_if_one_item_was_enqueued()
        {
            queue.Enqueue(new IdentificatedObject());
            queue.Count.Should().Be(1);
        }

        [Test]
        public void Count_should_return_QueueCapacity_if_more_then_capacity_value_items_were_enqueued()
        {
            for(var i = 0; i < QueueCapacity + 1; i++)
                queue.Enqueue(new IdentificatedObject());

            queue.Count.Should().Be(QueueCapacity);
        }

        [Test]
        public void TryDequeue_should_return_false_if_queue_was_not_in_use()
        {
            queue.TryDequeue(out var _).Should().BeFalse();
        }

        [Test]
        public void TryDequeue_should_return_true_if_one_item_was_enqueued()
        {
            queue.Enqueue(new IdentificatedObject());
            queue.TryDequeue(out var _).Should().BeTrue();
        }

        [Test]
        public void TryDequeue_should_return_item_equals_enqueued_one()
        {
            var item = new IdentificatedObject();
            queue.Enqueue(item);
            queue.TryDequeue(out var dequeuedItem);
            dequeuedItem.Should().Be(item);
        }

        [Test]
        public void TryDequeue_should_return_second_enqueued_item_if_first_one_was_rewrited_by_enqueue()
        {
            var item = new IdentificatedObject();
            for (var i = 0; i < QueueCapacity + 1; i++)            
                queue.Enqueue(i == 1 ? item : new IdentificatedObject());

            queue.TryDequeue(out var dequeuedItem);
            dequeuedItem.Should().Be(item);
        }

        [Test]
        public void TryPeek_should_return_false_if_queue_was_not_in_use()
        {
            queue.TryPeek(out var _).Should().BeFalse();
        }

        [Test]
        public void TryPeek_should_return_true_if_one_item_was_enqueued()
        {
            queue.Enqueue(new IdentificatedObject());
            queue.TryPeek(out var _).Should().BeTrue();
        }

        [Test]
        public void TryPeek_should_return_item_equals_enqueued_one()
        {
            var item = new IdentificatedObject();
            queue.Enqueue(item);
            queue.TryPeek(out var peekedItem);
            peekedItem.Should().Be(item);
        }

        [Test]
        public void TryPeek_should_return_second_enqueued_item_if_first_one_was_rewrited_by_enqueue()
        {
            var item = new IdentificatedObject();
            for (var i = 0; i < QueueCapacity + 1; i++)          
                queue.Enqueue(i == 1 ? item : new IdentificatedObject());

            queue.TryPeek(out var peekedItem);
            peekedItem.Should().Be(item);
        }

        [SetUp]
        public void SetUp()
        {
            queue = new CycledQueue<IdentificatedObject>(QueueCapacity);
        }

        private CycledQueue<IdentificatedObject> queue;
        private const int QueueCapacity = 5;

        private class IdentificatedObject
        {
            private readonly Guid id = Guid.NewGuid();

            public override string ToString()
            {
                return id.ToString();
            }
        }
    }
}