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
    public class ReactivePropertyTests : ReactiveTest
    {
        [Test]
        public void TestSetGet()
        {
            ReactiveProperty<string> property = new ReactiveProperty<string>();
            Assert.IsNull(property.Value);
            property.Value = "A";
            Assert.AreEqual("A", property.Value);
            property.Value = "B";
            Assert.AreEqual("B", property.Value);
        }

        [Test]
        public void TestChangedObservable()
        {
            //Setup
            var scheduler = new TestScheduler();
            ReactiveProperty<string> property = new ReactiveProperty<string>();

            //Define actions
            scheduler.Schedule(TimeSpan.FromTicks(10), () => property.Value = "A");
            scheduler.Schedule(TimeSpan.FromTicks(20), () => property.Value = "B");
            scheduler.Schedule(TimeSpan.FromTicks(30), () => property.Value = "C");
            var actual = scheduler.Start(() => property.Changed, created: 0, subscribed: 0, disposed: 100);

            //Assert
            var expected = new[]
            {
                OnNext(1, (string)null),
                OnNext(10, "A"),
                OnNext(20, "B"),
                OnNext(30, "C")
            };
            ReactiveAssert.AreElementsEqual(expected, actual.Messages);
        }
    }
}
