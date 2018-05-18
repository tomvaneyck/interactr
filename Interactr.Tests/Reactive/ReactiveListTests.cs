using NUnit.Framework;
using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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
            ReactiveList<string> list = new ReactiveArrayList<string>();
            Assert.AreEqual(0, list.Count);
            list.Add("B");
            Assert.AreEqual(1, list.Count);
            list.Insert(0, "A");
            list.Insert(2, "C");
            Assert.IsTrue(list.SequenceEqual(new[] {"A", "B", "C"}));
        }

        [Test]
        public void TestRemoving()
        {
            ReactiveList<string> list = new ReactiveArrayList<string> {"A", "B", "C"};
            Assert.AreEqual(3, list.Count);
            Assert.IsTrue(list.Remove("B"));
            Assert.AreEqual(2, list.Count);
            list.RemoveAt(1);
            list.RemoveAt(0);
            Assert.IsTrue(list.SequenceEqual(new string[0]));
        }

        [Test]
        public void TestMoving()
        {
            ReactiveList<string> list = new ReactiveArrayList<string> { "A", "B", "C" };
            Assert.AreEqual(3, list.Count);
            list.Move("B", list.Count);
            Assert.AreEqual(3, list.Count);
            Assert.IsTrue(list.SequenceEqual(new[] { "A", "C", "B" }));

            list.Move("A", 0);
            Assert.AreEqual(3, list.Count);
            Assert.IsTrue(list.SequenceEqual(new[] { "A", "C", "B" }));
        }

        [Test]
        public void TestMovingMultipleOccurances()
        {
            ReactiveList<string> list = new ReactiveArrayList<string> { "A", "B", "A", "C" };
            list.Move("A", list.Count);
            Assert.IsTrue(list.SequenceEqual(new[] { "B", "A", "C", "A" }));
        }

        [Test]
        public void TestContains()
        {
            ReactiveList<string> list = new ReactiveArrayList<string> {"A", "B", "C"};
            Assert.IsTrue(list.Contains("A"));
            Assert.IsFalse(list.Contains("Z"));
        }

        [Test]
        public void TestCopyTo()
        {
            string[] copy = new string[4];
            ReactiveList<string> list = new ReactiveArrayList<string> {"A", "B", "C"};
            list.CopyTo(copy, 1);
            Assert.IsTrue(copy.SequenceEqual(new string[] {null, "A", "B", "C"}));
        }

        [Test]
        public void TestOnAdd()
        {
            //Setup
            var scheduler = new TestScheduler();
            ReactiveList<string> list = new ReactiveArrayList<string>();

            //Define actions
            scheduler.Schedule(TimeSpan.FromTicks(10), () => list.Add("A"));
            scheduler.Schedule(TimeSpan.FromTicks(20), () => list.Insert(1, "B"));
            scheduler.Schedule(TimeSpan.FromTicks(30), () => list.Add("C"));
            var actual = scheduler.Start(() => list.OnAdd, created: 0, subscribed: 0, disposed: 100);

            //Assert
            var expected = new[]
            {
                OnNext(10, ("A", 0)),
                OnNext(20, ("B", 1)),
                OnNext(30, ("C", 2))
            };
            ReactiveAssert.AreElementsEqual(expected, actual.Messages);
        }

        [Test]
        public void TestOnRemove()
        {
            //Setup
            var scheduler = new TestScheduler();
            ReactiveList<string> list = new ReactiveArrayList<string>
            {
                "A",
                "B",
                "C"
            };

            //Define actions
            scheduler.Schedule(TimeSpan.FromTicks(10), () => list.Remove("A"));
            scheduler.Schedule(TimeSpan.FromTicks(20), () => list.RemoveAt(0));
            scheduler.Schedule(TimeSpan.FromTicks(30), () => list.Remove("C"));
            var actual = scheduler.Start(() => list.OnDelete, created: 0, subscribed: 0, disposed: 100);

            //Assert
            var expected = new[]
            {
                OnNext(10, ("A", 0)),
                OnNext(20, ("B", 0)),
                OnNext(30, ("C", 0))
            };
            ReactiveAssert.AreElementsEqual(expected, actual.Messages);
        }

        [Test]
        public void TestClear()
        {
            //Setup
            var scheduler = new TestScheduler();
            ReactiveList<string> list = new ReactiveArrayList<string>
            {
                "A",
                "B",
                "C"
            };

            //Define actions
            scheduler.Schedule(TimeSpan.FromTicks(10), () => list.Clear());
            var actual = scheduler.Start(() => list.OnDelete, created: 0, subscribed: 0, disposed: 100);

            //Assert
            var expected = new[]
            {
                OnNext(10, ("C", 2)),
                OnNext(10, ("B", 1)),
                OnNext(10, ("A", 0))
            };
            ReactiveAssert.AreElementsEqual(expected, actual.Messages);
        }

        [Test]
        public void TestReplace()
        {
            //Setup
            var scheduler = new TestScheduler();
            ReactiveList<string> list = new ReactiveArrayList<string>
            {
                "A",
                "B",
                "C"
            };

            //Define actions
            scheduler.Schedule(TimeSpan.FromTicks(10), () => list[1] = "Z");
            var actual = scheduler.Start(() =>
                Observable.Merge(
                    list.OnDelete.Select(e => (Element: e, WasAdded: false)),
                    list.OnAdd.Select(e => (Element: e, WasAdded: true))
                ), created: 0, subscribed: 0, disposed: 100);

            //Assert
            var expected = new[]
            {
                OnNext(10, (("B", 1), false)),
                OnNext(10, (("Z", 1), true))
            };
            ReactiveAssert.AreElementsEqual(expected, actual.Messages);
        }

        [Test]
        public void TestObserveEach()
        {
            //Setup
            var scheduler = new TestScheduler();
            var dummy1 = new DummyTestingClass {Identifier = "A"};
            var dummy2 = new DummyTestingClass {Identifier = "B"};
            ReactiveList<DummyTestingClass> list = new ReactiveArrayList<DummyTestingClass>
            {
                dummy1
            };

            //Define actions
            scheduler.Schedule(TimeSpan.FromTicks(10), () => dummy1.TestObservable.OnNext("First test event"));
            scheduler.Schedule(TimeSpan.FromTicks(20), () => list.Add(dummy2));
            scheduler.Schedule(TimeSpan.FromTicks(30), () => dummy2.TestObservable.OnNext("Second test event"));
            scheduler.Schedule(TimeSpan.FromTicks(40), () => list.Remove(list[0]));
            scheduler.Schedule(TimeSpan.FromTicks(50), () => dummy1.TestObservable.OnNext("Third test event"));
            scheduler.Schedule(TimeSpan.FromTicks(60), () => list.Remove(list[0]));
            var actual = scheduler.Start(() => list.ObserveEach(d => d.TestObservable), created: 0, subscribed: 0,
                disposed: 100);

            //Assert
            var expected = new[]
            {
                OnNext(10, (dummy1, "First test event")),
                OnNext(30, (dummy2, "Second test event"))
            };
            ReactiveAssert.AreElementsEqual(expected, actual.Messages);
        }

        [Test]
        public void TestObserveEachIndexChange()
        {
            //Setup
            var scheduler = new TestScheduler();
            var dummy1 = new DummyTestingClass { Identifier = "A" };
            var dummy2 = new DummyTestingClass { Identifier = "B" };
            var dummy3 = new DummyTestingClass { Identifier = "B" };
            ReactiveList<DummyTestingClass> list = new ReactiveArrayList<DummyTestingClass>
            {
                dummy2
            };

            //Define actions
            scheduler.Schedule(TimeSpan.FromTicks(10), () => list.Insert(0, dummy3));
            scheduler.Schedule(TimeSpan.FromTicks(20), () => list.Add(dummy1));
            scheduler.Schedule(TimeSpan.FromTicks(30), () => list.Remove(dummy3));
            scheduler.Schedule(TimeSpan.FromTicks(40), () => list.Remove(dummy1));
            scheduler.Schedule(TimeSpan.FromTicks(50), () => dummy1.TestObservable.OnNext("Test event"));
            var actual = scheduler.Start(() => list.ObserveEach(d => d.TestObservable), created: 0, subscribed: 0, disposed: 100);

            //Assert
            Assert.AreEqual(0, actual.Messages.Count);
        }

        class DummyTestingClass
        {
            public string Identifier { get; set; }
            public Subject<string> TestObservable = new Subject<string>();
        }
    }
}