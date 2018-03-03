using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;
using Interactr.Reactive;
using Microsoft.Reactive.Testing;

namespace Interactr.Tests.Reactive
{
    [TestFixture]
    public class ReactiveListTests : ReactiveTest
    {
        [Test]
        public void TestAdding()
        {
            ReactiveList<string> list = new ReactiveList<string>();
            Assert.AreEqual(0, list.Count);
            list.Add("B");
            Assert.AreEqual(1, list.Count);
            list.Insert(0, "A");
            list.Insert(2, "C");
            Assert.IsTrue(list.SequenceEqual(new[] { "A", "B", "C" }));
        }

        [Test]
        public void TestRemoving()
        {
            ReactiveList<string> list = new ReactiveList<string> {"A", "B", "C"};
            Assert.AreEqual(3, list.Count);
            Assert.IsTrue(list.Remove("B"));
            Assert.AreEqual(2, list.Count);
            list.RemoveAt(1);
            list.RemoveAt(0);
            Assert.IsTrue(list.SequenceEqual(new string[0]));
        }

        [Test]
        public void TestContains()
        {
            ReactiveList<string> list = new ReactiveList<string> { "A", "B", "C" };
            Assert.IsTrue(list.Contains("A"));
            Assert.IsFalse(list.Contains("Z"));
        }

        [Test]
        public void TestCopyTo()
        {
            string[] copy = new string[4];
            ReactiveList<string> list = new ReactiveList<string> { "A", "B", "C" };
            list.CopyTo(copy, 1);
            Assert.IsTrue(copy.SequenceEqual(new string[]{null, "A", "B", "C"}));
        }

        [Test]
        public void TestOnAdd()
        {
            //Setup
            var scheduler = new TestScheduler();
            ReactiveList<string> list = new ReactiveList<string>();

            //Define actions
            scheduler.Schedule(TimeSpan.FromTicks(10), () => list.Add("A"));
            scheduler.Schedule(TimeSpan.FromTicks(20), () => list.Insert(1, "B"));
            scheduler.Schedule(TimeSpan.FromTicks(30), () => list.Add("C"));
            var actual = scheduler.Start(() => list.OnAdd, created: 0, subscribed: 0, disposed:100);
            
            //Assert
            var expected = new[]
            {
                OnNext(10, "A"),
                OnNext(20, "B"),
                OnNext(30, "C")
            };
            ReactiveAssert.AreElementsEqual(expected, actual.Messages);
        }

        [Test]
        public void TestOnRemove()
        {
            //Setup
            var scheduler = new TestScheduler();
            ReactiveList<string> list = new ReactiveList<string>
            {
                "A", "B", "C"
            };

            //Define actions
            scheduler.Schedule(TimeSpan.FromTicks(10), () => list.Remove("A"));
            scheduler.Schedule(TimeSpan.FromTicks(20), () => list.RemoveAt(0));
            scheduler.Schedule(TimeSpan.FromTicks(30), () => list.Remove("C"));
            var actual = scheduler.Start(() => list.OnDelete, created: 0, subscribed: 0, disposed: 100);

            //Assert
            var expected = new[]
            {
                OnNext(10, "A"),
                OnNext(20, "B"),
                OnNext(30, "C")
            };
            ReactiveAssert.AreElementsEqual(expected, actual.Messages);
        }

        [Test]
        public void TestClear()
        {
            //Setup
            var scheduler = new TestScheduler();
            ReactiveList<string> list = new ReactiveList<string>
            {
                "A", "B", "C"
            };

            //Define actions
            scheduler.Schedule(TimeSpan.FromTicks(10), () => list.Clear());
            var actual = scheduler.Start(() => list.OnDelete, created: 0, subscribed: 0, disposed: 100);

            //Assert
            var expected = new[]
            {
                OnNext(10, "A"),
                OnNext(10, "B"),
                OnNext(10, "C"),
            };
            ReactiveAssert.AreElementsEqual(expected, actual.Messages);
        }

        [Test]
        public void TestReplace()
        {
            //Setup
            var scheduler = new TestScheduler();
            ReactiveList<string> list = new ReactiveList<string>
            {
                "A", "B", "C"
            };

            //Define actions
            scheduler.Schedule(TimeSpan.FromTicks(10), () => list[1] = "Z");
            var actualDelete = scheduler.Start(() => list.OnDelete, created: 0, subscribed: 0, disposed: 100);
            var actualAdd = scheduler.Start(() => list.OnAdd, created: 0, subscribed: 0, disposed: 100);

            //Assert
            var expectedDelete = new[]
            {
                OnNext(10, "B")
            };
            var expectedAdd = new[]
            {
                OnNext(10, "Z")
            };
            ReactiveAssert.AreElementsEqual(expectedDelete, actualDelete.Messages);
            ReactiveAssert.AreElementsEqual(expectedAdd, actualAdd.Messages);
        }
    }
}
