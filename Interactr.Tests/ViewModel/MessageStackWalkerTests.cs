using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Interactr.Model;
using Interactr.ViewModel;
using Interactr.ViewModel.MessageStack;
using NUnit.Framework;
using MessageStackWalker = Interactr.ViewModel.MessageStack.MessageStackWalker;

namespace Interactr.Tests.ViewModel
{
    [TestFixture]
    class MessageStackWalkerTests
    {
        private static string _testLabel = "label()";

        [Test]
        public void TestSimpleInvocation()
        {
            Party party1 = new Party(Party.PartyType.Actor, "classname:Party1");
            Party party2 = new Party(Party.PartyType.Object, "classname:Party2");

            List<SequenceDiagramMessageViewModel> messages = new List<SequenceDiagramMessageViewModel>
            {
                new SequenceDiagramMessageViewModel(
                    new Message(party1, party2, Message.MessageType.Invocation, _testLabel), 0),
                new SequenceDiagramMessageViewModel(
                    new Message(party2, party1, Message.MessageType.Result, _testLabel), 1)
            };

            StackFrame<SequenceDiagramMessageViewModel> expectedFrame1 =
                new StackFrame<SequenceDiagramMessageViewModel>.Builder
                {
                    Party = party2,
                    InvocationMessage = messages[0],
                    Level = 0,
                    ReturnMessage = messages[1],
                    SubFrames = { }
                }.Build();

            StackFrame<SequenceDiagramMessageViewModel> expectedFrame2 =
                new StackFrame<SequenceDiagramMessageViewModel>.Builder
                {
                    Party = party1,
                    InvocationMessage = null,
                    Level = 0,
                    ReturnMessage = null,
                    SubFrames = {expectedFrame1}
                }.Build();

            StackFrame<SequenceDiagramMessageViewModel>[] frames =
                MessageStackWalker.Walk(messages.AsReadOnly()).ToArray();
            Assert.IsTrue(Enumerable.SequenceEqual(frames, new[]
            {
                expectedFrame1, expectedFrame2
            }));
        }

        [Test]
        public void TestMultiInvocation()
        {
            Party party1 = new Party(Party.PartyType.Actor, "classname:Party1");
            Party party2 = new Party(Party.PartyType.Object, "classname:Party2");

            List<SequenceDiagramMessageViewModel> messages = new List<SequenceDiagramMessageViewModel>
            {
                new SequenceDiagramMessageViewModel(
                    new Message(party1, party2, Message.MessageType.Invocation, _testLabel), 0),
                new SequenceDiagramMessageViewModel(
                    new Message(party2, party1, Message.MessageType.Result, _testLabel), 1),
                new SequenceDiagramMessageViewModel(
                    new Message(party1, party2, Message.MessageType.Invocation, _testLabel), 2),
                new SequenceDiagramMessageViewModel(
                    new Message(party2, party1, Message.MessageType.Result, _testLabel), 3)
            };

            StackFrame<SequenceDiagramMessageViewModel> expectedFrame1 =
                new StackFrame<SequenceDiagramMessageViewModel>.Builder
                {
                    Party = party2,
                    InvocationMessage = messages[0],
                    Level = 0,
                    ReturnMessage = messages[1],
                    SubFrames = { }
                }.Build();

            StackFrame<SequenceDiagramMessageViewModel> expectedFrame2 =
                new StackFrame<SequenceDiagramMessageViewModel>.Builder
                {
                    Party = party2,
                    InvocationMessage = messages[2],
                    Level = 0,
                    ReturnMessage = messages[3],
                    SubFrames = { }
                }.Build();

            StackFrame<SequenceDiagramMessageViewModel> expectedFrame3 =
                new StackFrame<SequenceDiagramMessageViewModel>.Builder
                {
                    Party = party1,
                    InvocationMessage = null,
                    Level = 0,
                    ReturnMessage = null,
                    SubFrames = {expectedFrame1, expectedFrame2}
                }.Build();

            StackFrame<SequenceDiagramMessageViewModel>[] frames =
                MessageStackWalker.Walk(messages.AsReadOnly()).ToArray();
            Assert.IsTrue(Enumerable.SequenceEqual(frames, new[]
            {
                expectedFrame1, expectedFrame2, expectedFrame3
            }));
        }

        [Test]
        public void Test3Parties()
        {
            Party party1 = new Party(Party.PartyType.Actor, "classname:Party1");
            Party party2 = new Party(Party.PartyType.Object, "classname:Party2");
            Party party3 = new Party(Party.PartyType.Object, "classname:Party3");

            List<SequenceDiagramMessageViewModel> messages = new List<SequenceDiagramMessageViewModel>
            {
                new SequenceDiagramMessageViewModel(
                    new Message(party1, party2, Message.MessageType.Invocation, _testLabel), 0),
                new SequenceDiagramMessageViewModel(
                    new Message(party2, party3, Message.MessageType.Invocation, _testLabel), 1),
                new SequenceDiagramMessageViewModel(new Message(party3, party2, Message.MessageType.Result, _testLabel),
                    2),
                new SequenceDiagramMessageViewModel(new Message(party2, party1, Message.MessageType.Result, _testLabel),
                    3)
            };

            StackFrame<SequenceDiagramMessageViewModel> expectedFrame1 =
                new StackFrame<SequenceDiagramMessageViewModel>.Builder
                {
                    Party = party3,
                    InvocationMessage = messages[1],
                    Level = 0,
                    ReturnMessage = messages[2],
                    SubFrames = { }
                }.Build();

            StackFrame<SequenceDiagramMessageViewModel> expectedFrame2 =
                new StackFrame<SequenceDiagramMessageViewModel>.Builder
                {
                    Party = party2,
                    InvocationMessage = messages[0],
                    Level = 0,
                    ReturnMessage = messages[3],
                    SubFrames = {expectedFrame1}
                }.Build();

            StackFrame<SequenceDiagramMessageViewModel> expectedFrame3 =
                new StackFrame<SequenceDiagramMessageViewModel>.Builder
                {
                    Party = party1,
                    InvocationMessage = null,
                    Level = 0,
                    ReturnMessage = null,
                    SubFrames = {expectedFrame2}
                }.Build();

            StackFrame<SequenceDiagramMessageViewModel>[] frames =
                MessageStackWalker.Walk(messages.AsReadOnly()).ToArray();
            Assert.IsTrue(Enumerable.SequenceEqual(frames, new[]
            {
                expectedFrame1, expectedFrame2, expectedFrame3
            }));
        }

        [Test]
        public void TestMultiLevel()
        {
            Party party1 = new Party(Party.PartyType.Actor, "classname:Party1");
            Party party2 = new Party(Party.PartyType.Object, "classname:Party2");

            List<SequenceDiagramMessageViewModel> messages = new List<SequenceDiagramMessageViewModel>
            {
                new SequenceDiagramMessageViewModel(
                    new Message(party1, party2, Message.MessageType.Invocation, _testLabel), 0),
                new SequenceDiagramMessageViewModel(
                    new Message(party2, party1, Message.MessageType.Invocation, _testLabel), 1),
                new SequenceDiagramMessageViewModel(
                    new Message(party1, party2, Message.MessageType.Result, _testLabel), 2),
                new SequenceDiagramMessageViewModel(
                    new Message(party2, party1, Message.MessageType.Result, _testLabel), 3)
            };

            StackFrame<SequenceDiagramMessageViewModel> expectedFrame1 =
                new StackFrame<SequenceDiagramMessageViewModel>.Builder
                {
                    Party = party1,
                    InvocationMessage = messages[1],
                    Level = 1,
                    ReturnMessage = messages[2],
                    SubFrames = { }
                }.Build();

            StackFrame<SequenceDiagramMessageViewModel> expectedFrame2 =
                new StackFrame<SequenceDiagramMessageViewModel>.Builder
                {
                    Party = party2,
                    InvocationMessage = messages[0],
                    Level = 0,
                    ReturnMessage = messages[3],
                    SubFrames = {expectedFrame1}
                }.Build();

            StackFrame<SequenceDiagramMessageViewModel> expectedFrame3 =
                new StackFrame<SequenceDiagramMessageViewModel>.Builder
                {
                    Party = party1,
                    InvocationMessage = null,
                    Level = 0,
                    ReturnMessage = null,
                    SubFrames = {expectedFrame2}
                }.Build();

            StackFrame<SequenceDiagramMessageViewModel>[] frames =
                MessageStackWalker.Walk(messages.AsReadOnly()).ToArray();
            Assert.IsTrue(Enumerable.SequenceEqual(frames, new[]
            {
                expectedFrame1, expectedFrame2, expectedFrame3
            }));
        }
    }
}