using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Interactr.Model;
using NUnit.Framework;
using System.Reactive.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Reactive.Concurrency;
using Microsoft.Reactive.Testing;
using static System.Object;

namespace Interactr.Tests.Model
{
    public class PartyObservablesTest
    {
        private const Party.PartyType DefaultPartyType = Party.PartyType.Actor;

        //Test labels
        private const string ValidLabel1 = "instanceName:Classname";
        private const string ValidLabel2 = "instanceName2:Classname2";

        // Test parties
        private Party _defaultTestParty;

        //Scheduler 
        private TestScheduler _scheduler;

        [SetUp]
        public void Before()
        {
            // Initialize testParty
            _defaultTestParty = new Party(DefaultPartyType, ValidLabel1);

            // Initialize scheduler 
            _scheduler = new TestScheduler();
        }

        [Test]
        public void NoLabelChangeObservableTest()
        {
            // Subsription on LabelChanged happens on tick 0, tick 1 gives the value of the label.
            var expected = new[] {ReactiveTest.OnNext(1, ValidLabel1)};
            var actual = _scheduler.Start(() => _defaultTestParty.LabelChanged, 0, 0, 1000).Messages;

            // Assert
            ReactiveAssert.AreElementsEqual(expected, actual);
        }

        [Test]
        public void AddOneLabelObservableTest()
        {
            // Define the actions. 
            _scheduler.Schedule(TimeSpan.FromTicks(10), () => _defaultTestParty.Label = ValidLabel2);

            var expected = new[]
            {
                ReactiveTest.OnNext(1, ValidLabel1),
                ReactiveTest.OnNext(10, ValidLabel2)
            };
            var actual = _scheduler.Start(() => _defaultTestParty.LabelChanged, 0, 0, 1000).Messages;

            // Assert
            ReactiveAssert.AreElementsEqual(expected, actual);
        }

        [Test]
        public void AddTwoLabelsObservableTest()
        {
            // Define the actions. 
            _scheduler.Schedule(TimeSpan.FromTicks(10), () => _defaultTestParty.Label = ValidLabel2);
            _scheduler.Schedule(TimeSpan.FromTicks(20), () => _defaultTestParty.Label = ValidLabel1);

            var expected = new[]
            {
                ReactiveTest.OnNext(1, ValidLabel1),
                ReactiveTest.OnNext(10, ValidLabel2),
                ReactiveTest.OnNext(20, ValidLabel1)
            };
            var actual = _scheduler.Start(() => _defaultTestParty.LabelChanged, 0, 0, 1000).Messages;

            // Assert
            ReactiveAssert.AreElementsEqual(expected, actual);
        }

        [Test]
        public void NoTypeChangeObservableTest()
        {
            var expected = new[] {ReactiveTest.OnNext(1, DefaultPartyType)};
            var actual = _scheduler.Start(() => _defaultTestParty.TypeChanged, 0, 0, 1000).Messages;

            // Assert
            ReactiveAssert.AreElementsEqual(expected, actual);
        }

        [Test]
        public void ChangePartyTypeOnceObservableTest()
        {
            // Define the actions. 
            _scheduler.Schedule(TimeSpan.FromTicks(10), () => _defaultTestParty.Type = Party.PartyType.Object);

            var expected = new[]
            {
                ReactiveTest.OnNext(1, DefaultPartyType),
                ReactiveTest.OnNext(10, Party.PartyType.Object)
            };
            var actual = _scheduler.Start(() => _defaultTestParty.TypeChanged, 0, 0, 1000).Messages;

            // Assert
            ReactiveAssert.AreElementsEqual(expected, actual);
        }

        [Test]
        public void ChangePartyTypeTwiceObservablesTest()
        {
            // Define the actions. 
            _scheduler.Schedule(TimeSpan.FromTicks(10), () => _defaultTestParty.Type = Party.PartyType.Object);
            _scheduler.Schedule(TimeSpan.FromTicks(20), () => _defaultTestParty.Type = DefaultPartyType);

            var expected = new[]
            {
                ReactiveTest.OnNext(1, DefaultPartyType),
                ReactiveTest.OnNext(10, Party.PartyType.Object),
                ReactiveTest.OnNext(20, DefaultPartyType)
            };
            var actual = _scheduler.Start(() => _defaultTestParty.TypeChanged, 0, 0, 1000).Messages;

            // Assert
            ReactiveAssert.AreElementsEqual(expected, actual);
        }
    }
}