using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Interactr.Model;
using Interactr.Reactive;
using Interactr.View.Framework;
using Interactr.ViewModel.MessageStack;

namespace Interactr.ViewModel
{
    /// <summary>
    /// The ViewModel for a diagram.
    /// </summary>
    /// <remarks> A view model represents the data you want to display on your view
    /// and is responsible for interaction with the data objects from the model.</remarks>
    public abstract class DiagramViewModel
    {
        protected const string ValidLabel = "instanceName:ClassName";

        #region IsVisible

        private readonly ReactiveProperty<bool> _isVisible = new ReactiveProperty<bool>();

        /// <summary>
        /// Indicate if the diagramModel is visible.
        /// </summary>
        public bool IsVisible
        {
            get => _isVisible.Value;
            set => _isVisible.Value = value;
        }

        /// <summary>
        /// Observable that emits the new IsVisible value when it is changed.
        /// </summary>
        public IObservable<bool> IsVisibleChanged => _isVisible.Changed;

        #endregion

        /// <summary>
        /// The underlying diagram associated with the diagram view model.
        /// </summary>
        /// <remarks>The underlying diagram can not be changed after the diagramViewModel construction.</remarks>
        public Diagram Diagram { get; }

        /// <summary>
        /// An abstract property for the message viewmodels.
        /// </summary>
        protected abstract IReadOnlyList<MessageViewModel> MessageViewModels { get; }

        /// <summary>
        /// The partyViewModels included in this diagram view model.
        /// </summary>
        public IReadOnlyReactiveList<PartyViewModel> PartyViewModels { get; }

        protected DiagramViewModel(Diagram diagram)
        {
            Diagram = diagram;
            PartyViewModels = Diagram.Parties.CreateDerivedList(party => new PartyViewModel(party)).ResultList;
        }

        /// <summary>
        /// Add a new party at the specified point. 
        /// </summary>
        /// <param name="point"> The point on the screen where the party is added.</param>
        public void AddParty(Point point)
        {
            Party party = new Party(Party.PartyType.Actor, ValidLabel);
            Diagram.Parties.Add(party);
            PartyViewModels.First(vm => vm.Party == party).Position = point;
        }

        /// <summary>
        /// Delete the given party.
        /// </summary>
        public void DeleteParty(Party party)
        {
            Diagram.Parties.Remove(party);
        }

        /// <summary>
        /// Delete a message from the model, this results in the deletion of all the nested submessages
        /// of this message as well.
        /// </summary>
        /// <remarks></remarks>
        /// <param name="message">Also propagates to the viewmodel and deletes the message in the viewmodel.</param>
        public void DeleteMessage(Message message)
        {
            Diagram.Messages.RemoveAll(GetMessagesToDelete(message, MessageViewModels));
        }

        /// <summary>
        /// Retrieve all the messages that should be deleted as a consequence of one message that gets deleted.
        /// This means all messages that present in the subframes of this message and the message itself.
        /// </summary>
        /// <param name="message">The message that is being deleted</param>
        /// <param name="messageList"> The list we ant to delete from</param>
        /// <returns></returns>
        private static IEnumerable<Message> GetMessagesToDelete(Message message,
            IReadOnlyList<MessageViewModel> messageList)
        {
            // There can only ever be one stackframe that has our message as an invocation message.
            var stackFrame = MessageStackWalker.Walk(messageList)
                .FirstOrDefault(frame => frame.InvocationMessage?.Message == message);

            if (stackFrame != null)
            {
                // The invocation message and it's corresponding return message 
                // that should get deleted.
                yield return stackFrame.InvocationMessage.Message;
                yield return stackFrame.ReturnMessage.Message;

                foreach (var subFrame in stackFrame.SubFrames)
                {
                    if (subFrame.InvocationMessage != null)
                    {
                        // yield return all the messages that get deleted as a result of the deletion of message,
                        // This happens through recursively calling GetMessagesToDelete with the invocation message 
                        // of the subframe as the new target delete message.
                        foreach (var messageToDelete in GetMessagesToDelete(subFrame.InvocationMessage.Message,
                            messageList))
                        {
                            yield return messageToDelete;
                        }
                    }
                }
            }
        }
    }
}