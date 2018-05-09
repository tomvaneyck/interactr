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
            // Create a copy of messageViewModels because the original can be modified while iterating the elements.
            // which results in a concurrent modification error.
            IReadOnlyList<MessageViewModel> copyMessageViewModels = MessageViewModels.ToList();
            try
            {
                foreach (var stackFrame in MessageStackWalker.Walk(copyMessageViewModels))
                {
                    if (stackFrame.SubFrames.Count != 0)
                    {
                        foreach (var subFrame in stackFrame.SubFrames)
                        {
                            if (subFrame.InvocationMessage != null && message == subFrame.InvocationMessage.Message)
                            {
                                // Delete the invocation and return message.
                                DeleteSingleMessage(subFrame.InvocationMessage.Message);
                                DeleteSingleMessage(subFrame.ReturnMessage.Message);
                                
                                // Delete the invocation and return message of all the subframes.
                                foreach (var subsubFrame in subFrame.SubFrames)
                                {
                                    DeleteMessage(subsubFrame.InvocationMessage.Message);
                                    DeleteMessage(subsubFrame.ReturnMessage.Message);
                                }
                            }
                        }
                    }
                }
            }
            catch (UnbalancedStackException)
            {
                Debug.Print("Stack was unbalanced.");
            }
        }

        private void DeleteSingleMessage(Message message)
        {
            Diagram.Messages.Remove(message);
        }
    }
}