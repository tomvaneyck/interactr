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
    public class ReactiveDictionaryTests : ReactiveTest
    {
        [Test]
        public void TestAddingAndRemoving()
        {
            // New dictionary
            ReactiveDictionary<string, int> dict = new ReactiveDictionary<string, int>();
            Assert.IsFalse(dict.TryGetValue("A", out int _));
            Assert.AreEqual(0, dict.Count);

            // Add {A, 1}
            dict["A"] = 1;
            Assert.IsTrue(dict.TryGetValue("A", out int value));
            Assert.AreEqual(1, value);
            Assert.AreEqual(1, dict.Count);

            Assert.IsTrue(dict.SequenceEqual(new[]
            {
                new KeyValuePair<string, int>("A", 1)
            }));
            Assert.IsTrue(dict.Keys.SequenceEqual(new[] { "A" }));
            Assert.IsTrue(dict.Values.SequenceEqual(new[] { 1 }));

            // Add {B, 1}
            dict.Add("B", 2);
            Assert.AreEqual(2, dict["B"]);
            Assert.AreEqual(2, dict.Count);

            // Remove A
            Assert.IsTrue(dict.Remove("A"));
            Assert.IsFalse(dict.TryGetValue("A", out int _));
            Assert.AreEqual(1, dict.Count);

            Assert.IsTrue(dict.ContainsKey("B"));
            Assert.IsFalse(dict.ContainsKey("A"));
            Assert.IsFalse(dict.ContainsKey("Z"));

            // Clear
            dict.Clear();
            Assert.AreEqual(0, dict.Count);
            Assert.IsTrue(dict.SequenceEqual(new KeyValuePair<string, int>[0]));
        }

        [Test]
        public void TestModifying()
        {
            // New dictionary
            ReactiveDictionary<string, int> dict = new ReactiveDictionary<string, int>
            {
                {"A", 1},
                {"B", 2},
                {"C", 3},
            };

            // Change B to 0
            dict["B"] = 0;
            Assert.AreEqual(0, dict["B"]);
            Assert.IsTrue(dict.TryGetValue("B", out int value));
            Assert.AreEqual(0, value);
        }

        [Test]
        public void TestOnValueChanged()
        {
            //Setup
            var scheduler = new TestScheduler();
            ReactiveDictionary<string, int> dict = new ReactiveDictionary<string, int>
            {
                {"A", 1},
                {"B", 2},
                {"C", 3},
            };

            //Define actions
            scheduler.Schedule(TimeSpan.FromTicks(10), () => dict["A"] = 0);
            scheduler.Schedule(TimeSpan.FromTicks(20), () => dict["Z"] = 1);
            scheduler.Schedule(TimeSpan.FromTicks(30), () => dict.Remove("Z"));
            var actual = scheduler.Start(() => dict.OnValueChanged, created: 0, subscribed: 0, disposed: 100);

            //Assert
            var expected = new[]
            {
                OnNext(10, new KeyValuePair<string, int>("A", 0)),
                OnNext(20, new KeyValuePair<string, int>("Z", 1))
            };
            ReactiveAssert.AreElementsEqual(expected, actual.Messages);
        }

        [Test]
        public void TestOnValueRemoved()
        {
            //Setup
            var scheduler = new TestScheduler();
            ReactiveDictionary<string, int> dict = new ReactiveDictionary<string, int>
            {
                {"A", 1},
                {"B", 2},
                {"C", 3},
            };

            //Define actions
            scheduler.Schedule(TimeSpan.FromTicks(10), () => dict["A"] = 0);
            scheduler.Schedule(TimeSpan.FromTicks(20), () => dict["Z"] = 1);
            scheduler.Schedule(TimeSpan.FromTicks(30), () => dict.Remove("Z"));
            scheduler.Schedule(TimeSpan.FromTicks(40), () => dict.Clear());
            var actual = scheduler.Start(() => dict.OnValueRemoved, created: 0, subscribed: 0, disposed: 100);

            //Assert
            var expected = new[]
            {
                OnNext(30, new KeyValuePair<string, int>("Z", 1)),
                OnNext(40, new KeyValuePair<string, int>("A", 0)),
                OnNext(40, new KeyValuePair<string, int>("B", 2)),
                OnNext(40, new KeyValuePair<string, int>("C", 3))
            };
            ReactiveAssert.AreElementsEqual(expected, actual.Messages);
        }
    }
}
