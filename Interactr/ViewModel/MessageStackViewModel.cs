using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Interactr.Model;
using Interactr.Reactive;
using Interactr.ViewModel.MessageStack;
using Message = Interactr.Model.Message;

namespace Interactr.ViewModel
{
    /// <summary>
    /// The message stack view model.
    /// </summary>
    public class MessageStackViewModel
    {
        /// <summary>
        /// The diagram model from which the messages are read.
        /// </summary>
        public Diagram Diagram { get; }

        /// <summary>
        /// Message view models that map to the messages in the diagram.
        /// The index of the message view model in the list is equal to the tick of the message view model.
        /// </summary>
        public IReadOnlyReactiveList<SequenceDiagramMessageViewModel> MessageViewModels { get; }

        /// <summary>
        /// The activation bar view models.
        /// </summary>
        public ReactiveList<ActivationBarViewModel> ActivationBars { get; } =
            new ReactiveArrayList<ActivationBarViewModel>();

        #region PendingInvokingMessageVM

        private readonly ReactiveProperty<PendingMessageViewModel> _pendingInvokingMessageVM =
            new ReactiveProperty<PendingMessageViewModel>();

        /// <summary>
        /// Viewmodel for the message that is currently being created by the user.
        /// </summary>
        public PendingMessageViewModel PendingInvokingMessageVM
        {
            get => _pendingInvokingMessageVM.Value;
            set => _pendingInvokingMessageVM.Value = value;
        }

        public IObservable<PendingMessageViewModel> PendingInvokingMessageVMChanged =>
            _pendingInvokingMessageVM.Changed;

        #endregion

        public MessageStackViewModel(Diagram diagram)
        {
            Diagram = diagram;

            // Create MessageViewModels from messages.
            MessageViewModels = Diagram.Messages.CreateDerivedList(msg =>
                new SequenceDiagramMessageViewModel(msg, 0)).ResultList;

            // When the diagram changes, recalculate layout.
            ReactiveExtensions.MergeEvents(
                diagram.Messages.OnAdd,
                diagram.Messages.OnDelete,
                diagram.Messages.ObserveEach(msg => msg.ReceiverChanged),
                diagram.Messages.ObserveEach(msg => msg.SenderChanged)
            ).Subscribe(_ => CalculateLayout());
        }

        /// <summary>
        /// Recalculate the layout of the messages and activation bars in the messageStack view model.
        /// </summary>
        private void CalculateLayout()
        {
            // Create a new list of activation bars
            List<ActivationBarViewModel> newBars = new List<ActivationBarViewModel>();

            // Iterate over messages, tick of message = index in list
            for (int i = 0; i < MessageViewModels.Count; i++)
            {
                MessageViewModels[i].Tick = i;
            }

            try
            {
                // For every stack frame, create a new activation bar
                foreach (StackFrame<SequenceDiagramMessageViewModel> frame
                    in MessageStackWalker.Walk(MessageViewModels))
                {
                    // This activation bar starts when the party is invoked, 
                    // or on the first sub-invocation in case of the initiator.
                    ActivationBarViewModel bar = new ActivationBarViewModel(
                        frame.Party,
                        frame.StartTick,
                        frame.EndTick,
                        frame.Level
                    );
                    newBars.Add(bar);

                    foreach (var subFrame in frame.SubFrames)
                    {
                        var seqInvocationMessage = subFrame.InvocationMessage;
                        var seqReturnMessage = subFrame.ReturnMessage;

                        // Each subinvocation is sent from this bar.
                        seqInvocationMessage.SenderActivationBar = bar;

                        // Each return message is received on this bar.
                        seqReturnMessage.ReceiverActivationBar = bar;
                    }

                    // If this frame is not the initiator frame
                    if (frame.InvocationMessage != null && frame.ReturnMessage != null)
                    {
                        var seqInvocationMessage = frame.InvocationMessage;
                        var seqReturnMessage = frame.ReturnMessage;

                        // The invocation message that starts this activation should arrive at this bar.
                        seqInvocationMessage.ReceiverActivationBar = bar;
                        // The return message that ends this activation should be sent from this bar.
                        seqReturnMessage.SenderActivationBar = bar;
                    }
                }

                // Set the new list of activation bars
                ActivationBars.Clear();
                newBars.Reverse(); // Fix for correct drawing order of activation bar.
                ActivationBars.AddRange(newBars);
            }
            catch (UnbalancedStackException)
            {
                // The message stack is in an invalid state, dont update UI.
            }
        }

        /// <summary>
        /// Creates a new LifeLineViewModel for <paramref name="party"/>.
        /// </summary>
        /// <param name="party">The party for which the life line will me created.</param>
        /// <returns>A new LifeLineViewModel.</returns>
        public LifeLineViewModel CreateLifeLineForParty(PartyViewModel party)
        {
            if (party == null)
            {
                throw new ArgumentNullException(nameof(party));
            }

            return new LifeLineViewModel(this, party);
        }

        /// <summary>
        /// Attempt to create a new pending message on suggestedTick with the specified party as the sender.
        /// If <paramref name="sender"/> is not on the top of the call stack at the specified tick, this method silently fails.
        /// </summary>
        /// <param name="sender">The party that sends the invocation message.</param>
        /// <param name="suggestedTick">The tick at which the message is sent.</param>
        public void CreatePendingMessage(Party sender, int suggestedTick)
        {
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            // Get the frame on the top of the stack at suggestedTick
            var stackFrame = MessageStack.MessageStackWalker.Walk(MessageViewModels)
                .FirstOrDefault(f => f.StartTick < suggestedTick && suggestedTick <= f.EndTick);

            // If sender is not on the top of the message stack at suggestedTick, then don't create a new message. 
            // If there is no frame on the top of the stack at that time, make sure that the message is sent by the initiator,
            // unless there are no messages in the diagram (because then no initiator has been assigned yet.)
            if ((stackFrame != null && stackFrame.Party != sender) ||
                (stackFrame == null && MessageViewModels.Count > 0 &&
                 sender != MessageViewModels.First().SenderActivationBar.Party))
            {
                return;
            }

            // Find the matching activation bar for this party and tick, if any.
            var targetActivationBar = ActivationBars
                .Where(bar => bar.Party == sender && suggestedTick > bar.StartTick && bar.EndTick > suggestedTick)
                .OrderByDescending(bar => bar.Level)
                .FirstOrDefault();

            // If no activation bar was found, create a dummy one.
            if (targetActivationBar == null)
            {
                targetActivationBar = new ActivationBarViewModel(sender, suggestedTick, suggestedTick, 0);
            }

            // Set pending message.
            PendingMessageViewModel pendingMsg = new PendingMessageViewModel
            {
                Tick = suggestedTick,
                Type = Message.MessageType.Invocation,
                Label = "invocation()",
                SenderActivationBar = targetActivationBar
            };
            PendingInvokingMessageVM = pendingMsg;
        }

        /// <summary>
        /// Add a new invocation/return message pair to the diagram based on the pending message, but only if it is valid.
        /// Resets the pending message to null.
        /// </summary>
        public void FinishPendingMessage()
        {
            if (PendingInvokingMessageVM == null)
            {
                throw new InvalidOperationException("There is no active pending message.");
            }

            // Reset pending message to null.
            var pendingMsg = PendingInvokingMessageVM;
            PendingInvokingMessageVM = null;

            // Make sure the pending message has a valid sender and receiver combination.
            if (pendingMsg.SenderActivationBar == null || pendingMsg.Receiver == null ||
                pendingMsg.SenderActivationBar.Party == pendingMsg.Receiver)
            {
                return;
            }

            // Find index in messages list where the new messages should be inserted.
            int i = 0;
            for (; i < MessageViewModels.Count; i++)
            {
                if (MessageViewModels[i].Tick >= pendingMsg.Tick)
                {
                    break;
                }
            }

            // Add invocation message.
            Message message =
                new Message(pendingMsg.SenderActivationBar.Party, pendingMsg.Receiver,
                    Message.MessageType.Invocation, pendingMsg.Label);
            Diagram.Messages.Insert(i, message);

            // Add return message.
            Message returnMessage =
                new Message(pendingMsg.Receiver, pendingMsg.SenderActivationBar.Party, Message.MessageType.Result, "return()");
            Diagram.Messages.Insert(i + 1, returnMessage);
        }
    }
}