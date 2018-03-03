using System;
using System.Collections.Generic;
using System.Linq;
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

        private const string ValidPartyLabel1 = "instanceName;Classname";
        private const string ValidPartyLabel2 = "instanceName2;Classname2";

        // Test party
        private Party _testSender1;
        private Party _testReceiver1;
        private Party _testSender2;
        private Party _testReceiver2;

        // Test message
        private Message _defaultTestMessage;

        //Result collectors
        private List<string> _labelResultCollector;
        private List<Message.MessageType> _messageTypeResultCollector;
        private List<Party> _senderResultCollector;
        private List<Party> _receiveResultCollector;

        //Scheduler 
        private TestScheduler _scheduler;

        [OneTimeSetUp]
        public void init()
        {
            // initialize parties
            _testSender1 = new Party(Party.PartyType.Actor, ValidPartyLabel1);
            _testReceiver1 = new Party(Party.PartyType.Actor, ValidPartyLabel2);
            _testSender2 = new Party(Party.PartyType.Object, ValidPartyLabel1);
            _testReceiver2 = new Party(Party.PartyType.Object, ValidPartyLabel2);

            //Initialize result collectors.
            _labelResultCollector = new List<string>();
            _messageTypeResultCollector = new List<Message.MessageType>();
            _receiveResultCollector = new List<Party>();
            _senderResultCollector = new List<Party>();
        }

        [SetUp]
        public void before()
        {
            // initialize scheduler 
            _scheduler = new TestScheduler();

            // Schedule creation of the defaultTestMessage.
            _scheduler.Schedule(TimeSpan.FromTicks(10), () =>
                _defaultTestMessage = new Message(_testSender1, _testReceiver1, DefaultMessageType, MessageLabel1));
        }

        [Test]
        public void NoLabelChangeObservableTest()
        {
            var expected = new[] {ReactiveTest.OnNext(10, MessageLabel1)};
            var actual = _scheduler.Start(() => _defaultTestMessage.labelChanged).Messages;
            ReactiveAssert.AreElementsEqual(expected, actual);
        }

        [Test]
        public void AddOneLabelObservableTest()
        {
            // Define the actions. 
            _scheduler.Schedule(TimeSpan.FromTicks(10), () => _defaultTestMessage.Label = MessageLabel2);

            // Assert that the resultcollector contains the initial label and new label.
            Assert.That(_labelResultCollector, Has.Count.EqualTo(2));
            Assert.That(_labelResultCollector, Has.Member(MessageLabel1));
            Assert.That(_labelResultCollector, Has.Member(MessageLabel2));
        }

        [Test]
        public void AddTwoLabelsObservableTest()
        {
            _defaultTestMessage.Label = MessageLabel1;
            _defaultTestMessage.Label = MessageLabel2;

            // Assert that resultcollector contains 3 labels: the initial label and the 2 changes. 
            Assert.That(_labelResultCollector, Has.Count.EqualTo(3));
            Assert.That(_labelResultCollector, Has.Member(MessageLabel1));
            Assert.That(_labelResultCollector, Has.Member(MessageLabel2));
        }

        [Test]
        public void NoMessageTypeChangeObservableTest()
        {
            // Assert that the result collector only has the initial value.
            Assert.That(_messageTypeResultCollector, Has.Count.EqualTo(1));
            Assert.That(_messageTypeResultCollector, Has.Member(_defaultTestMessage.Type));
        }

        [Test]
        public void ChangeMessageTypeOnceObservableTest()
        {
            Message.MessageType newMessageType = Message.MessageType.Result;
            _defaultTestMessage.Type = newMessageType;

            // Assert that the resultcollector contains the initial value and the new value.
            Assert.That(_messageTypeResultCollector, Has.Count.EqualTo(2));
            Assert.That(_messageTypeResultCollector, Has.Member(DefaultMessageType));
            Assert.That(_messageTypeResultCollector, Has.Member(newMessageType));
        }

        [Test]
        public void ChangeMessageTypeTwiceObservablesTest()
        {
            _defaultTestMessage.Type = Message.MessageType.Result;
            _defaultTestMessage.Type = DefaultMessageType;

            // Assert that resultcollector contains 3 labels. 
            Assert.That(_messageTypeResultCollector, Has.Count.EqualTo(3));
        }

        [Test]
        public void NoSenderChangeObservableTest()
        {
            //Assert that the result collector only has the initial value.
            Assert.That(_senderResultCollector, Has.Count.EqualTo(1));
            Assert.That(_senderResultCollector, Has.Member(_testSender1));
        }

        [Test]
        public void ChangeSenderOnceObservableTest()
        {
            _defaultTestMessage.Sender = _testSender2;

            //Assert that the resultcollector contains the initial value and the new value.
            Assert.That(_senderResultCollector, Has.Count.EqualTo(2));
            Assert.That(_senderResultCollector, Has.Member(_testSender1));
            Assert.That(_senderResultCollector, Has.Member(_testSender2));
        }

        [Test]
        public void ChangeSenderTwiceObservablesTest()
        {
            _defaultTestMessage.Sender = _testSender2;
            _defaultTestMessage.Sender = _testSender1;

            // Assert that resultcollector contains 3 labels. 
            Assert.That(_senderResultCollector, Has.Count.EqualTo(3));
        }

        [Test]
        public void NoReceiverChangeObservableTest()
        {
            //Assert that the result collector only has the initial value.
            Assert.That(_receiveResultCollector, Has.Count.EqualTo(1));
            Assert.That(_receiveResultCollector, Has.Member(_testReceiver1));
        }

        [Test]
        public void ChangeReceiverOnceObservableTest()
        {
            _defaultTestMessage.Receiver = _testReceiver2;

            //Assert that the resultcollector contains the initial value and the new value.
            Assert.That(_receiveResultCollector, Has.Count.EqualTo(2));
            Assert.That(_receiveResultCollector, Has.Member(_testReceiver1));
            Assert.That(_receiveResultCollector, Has.Member(_testReceiver2));
        }

        [Test]
        public void ChangeReceiverTwiceObservablesTest()
        {
            _defaultTestMessage.Receiver = _testReceiver2;
            _defaultTestMessage.Receiver = _testReceiver1;

            // Assert that resultcollector contains 3 labels. 
            Assert.That(_receiveResultCollector, Has.Count.EqualTo(3));
        }
    }
}