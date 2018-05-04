using System;
using Interactr.Model;
using Microsoft.Reactive.Testing;
using System.Reactive.Concurrency;
using NUnit.Framework;

namespace Interactr.Tests.Model
{
    [TestFixture]
    public class MessageObservablesTest
    {
        private const Message.MessageType DefaultMessageType = Message.MessageType.Invocation;

        //Test labels
        private const string MessageLabel1 = "testLabel1";
        private const string MessageLabel2 = "testLabel2";

        private const string ValidPartyLabel1 = "instanceName:Classname";
        private const string ValidPartyLabel2 = "instanceName2:Classname2";

        // Test parties
        private Party _testSender1;
        private Party _testReceiver1;
        private Party _testSender2;
        private Party _testReceiver2;

        // Test message
        private Message _defaultTestMessage;

        //Scheduler 
        private TestScheduler _scheduler;

        [OneTimeSetUp]
        public void Init()
        {
            // initialize parties
            _testSender1 = new Party(Party.PartyType.Actor, ValidPartyLabel1);
            _testReceiver1 = new Party(Party.PartyType.Actor, ValidPartyLabel2);
            _testSender2 = new Party(Party.PartyType.Object, ValidPartyLabel1);
            _testReceiver2 = new Party(Party.PartyType.Object, ValidPartyLabel2);
        }

        [SetUp]
        public void Before()
        {
            // initialize scheduler 
            _scheduler = new TestScheduler();

            // Initialize default message.
            _defaultTestMessage = new Message(_testSender1, _testReceiver1, DefaultMessageType, MessageLabel1);
        }

        [Test]
        public void NoLabelChangeObservableTest()
        {
            // Subsription on LabelChanged happens on tick 0, tick 1 gives the value of the label.
            var expected = new[] {ReactiveTest.OnNext(1, MessageLabel1)};
            var actual = _scheduler.Start(() => _defaultTestMessage.LabelChanged, 0, 0, 1000).Messages;

            // Assert
            ReactiveAssert.AreElementsEqual(expected, actual);
        }

        [Test]
        public void AddOneLabelObservableTest()
        {
            // Define the actions. 
            _scheduler.Schedule(TimeSpan.FromTicks(10), () => _defaultTestMessage.Label = MessageLabel2);

            var expected = new[]
            {
                ReactiveTest.OnNext(1, MessageLabel1),
                ReactiveTest.OnNext(10, MessageLabel2)
            };
            var actual = _scheduler.Start(() => _defaultTestMessage.LabelChanged, 0, 0, 1000).Messages;

            // Assert
            ReactiveAssert.AreElementsEqual(expected, actual);
        }

        [Test]
        public void AddTwoLabelsObservableTest()
        {
            // Define the actions. 
            _scheduler.Schedule(TimeSpan.FromTicks(10), () => _defaultTestMessage.Label = MessageLabel2);
            _scheduler.Schedule(TimeSpan.FromTicks(20), () => _defaultTestMessage.Label = MessageLabel1);

            var expected = new[]
            {
                ReactiveTest.OnNext(1, MessageLabel1),
                ReactiveTest.OnNext(10, MessageLabel2),
                ReactiveTest.OnNext(20, MessageLabel1)
            };
            var actual = _scheduler.Start(() => _defaultTestMessage.LabelChanged, 0, 0, 1000).Messages;

            // Assert
            ReactiveAssert.AreElementsEqual(expected, actual);
            Assert.AreEqual(MessageLabel1, _defaultTestMessage.Label);
        }

        [Test]
        public void NoSenderChangeObservableTest()
        {
            var expected = new[]
            {
                ReactiveTest.OnNext(1, _testSender1),
            };
            var actual = _scheduler.Start(() => _defaultTestMessage.SenderChanged, 0, 0, 1000).Messages;

            // Assert
            ReactiveAssert.AreElementsEqual(expected, actual);
        }

        [Test]
        public void ChangeSenderOnceObservableTest()
        {
            // Define the actions. 
            _scheduler.Schedule(TimeSpan.FromTicks(10), () => _defaultTestMessage.Sender = _testSender2);

            var expected = new[]
            {
                ReactiveTest.OnNext(1, _testSender1),
                ReactiveTest.OnNext(10, _testSender2),
            };
            var actual = _scheduler.Start(() => _defaultTestMessage.SenderChanged, 0, 0, 1000).Messages;

            // Assert
            ReactiveAssert.AreElementsEqual(expected, actual);
        }

        [Test]
        public void ChangeSenderTwiceObservablesTest()
        {
            // Define the actions. 
            _scheduler.Schedule(TimeSpan.FromTicks(10), () => _defaultTestMessage.Sender = _testSender2);

            _scheduler.Schedule(TimeSpan.FromTicks(20),
                () => _defaultTestMessage.Sender = _testSender1);

            var expected = new[]
            {
                ReactiveTest.OnNext(1, _testSender1),
                ReactiveTest.OnNext(10, _testSender2),
                ReactiveTest.OnNext(20, _testSender1)
            };
            var actual = _scheduler.Start(() => _defaultTestMessage.SenderChanged, 0, 0, 1000).Messages;

            // Assert
            ReactiveAssert.AreElementsEqual(expected, actual);
        }

        [Test]
        public void NoReceiverChangeObservableTest()
        {
            var expected = new[]
            {
                ReactiveTest.OnNext(1, _testReceiver1),
            };
            var actual = _scheduler.Start(() => _defaultTestMessage.ReceiverChanged, 0, 0, 1000).Messages;

            // Assert
            ReactiveAssert.AreElementsEqual(expected, actual);
        }

        [Test]
        public void ChangeReceiverOnceObservableTest()
        {
            // Define the actions. 
            _scheduler.Schedule(TimeSpan.FromTicks(10), () => _defaultTestMessage.Receiver = _testReceiver2);

            var expected = new[]
            {
                ReactiveTest.OnNext(1, _testReceiver1),
                ReactiveTest.OnNext(10, _testReceiver2),
            };
            var actual = _scheduler.Start(() => _defaultTestMessage.ReceiverChanged, 0, 0, 1000).Messages;

            // Assert
            ReactiveAssert.AreElementsEqual(expected, actual);
        }

        [Test]
        public void ChangeReceiverTwiceObservablesTest()
        {
            // Define the actions. 
            _scheduler.Schedule(TimeSpan.FromTicks(10), () => _defaultTestMessage.Receiver = _testReceiver2);
            _scheduler.Schedule(TimeSpan.FromTicks(20), () => _defaultTestMessage.Receiver = _testReceiver1);

            var expected = new[]
            {
                ReactiveTest.OnNext(1, _testReceiver1),
                ReactiveTest.OnNext(10, _testReceiver2),
                ReactiveTest.OnNext(20, _testReceiver1),
            };
            var actual = _scheduler.Start(() => _defaultTestMessage.ReceiverChanged, 0, 0, 1000).Messages;

            // Assert
            ReactiveAssert.AreElementsEqual(expected, actual);
        }
    }
}