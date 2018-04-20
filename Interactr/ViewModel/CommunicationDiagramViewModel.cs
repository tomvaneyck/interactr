using System.Linq;
using Interactr.Model;
using Interactr.Reactive;
using Interactr.View.Framework;
﻿using Interactr.Model;


namespace Interactr.ViewModel
{
    /// <summary> The diagram view model for the communication diagram </summary>
    /// <see cref="DiagramViewModel"/>
    public class CommunicationDiagramViewModel : DiagramViewModel
    {
        /// <summary>
        /// The Messages view models for messages that will be drawn in the communication diagram.
        /// </summary>
        /// <remarks>
        /// Only invocation messages get drawn in the communication diagram.
        /// </remarks>
        public IReadOnlyReactiveList<MessageViewModel> MessageViewModels { get; }

        public CommunicationDiagramViewModel(Diagram diagram) : base(diagram)
        {
            // Create message view models for every invocation message in the diagram model.
            MessageViewModels = Diagram.Messages.CreateDerivedList(msg => new MessageViewModel(msg),
                msg => msg.Type == Message.MessageType.Invocation).ResultList;
        }

        /// <summary>
        /// Delete a message from the model.
        /// </summary>
        /// <remarks></remarks>
        /// <param name="message">Also propagates to the viewmodel and deletes the message in the viewmodel.</param>
        public void DeleteMessage(Message message)
        {
            Diagram.Messages.Remove(message);
        }
    }
}