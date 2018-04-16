using System.Linq;
using Interactr.Model;
using Interactr.Reactive;
using Interactr.View.Framework;

namespace Interactr.ViewModel
{
    /// <summary> The diagram view model for the sequence diagram </summary>
    /// <see cref="DiagramViewModel"/>
    public class CommunicationDiagramViewModel : DiagramViewModel
    {
        public CommunicationDiagramViewModel(Diagram diagram) : base(diagram)
        {
            // Create message view models for every invocation message in the diagram model.
            MessageViewModels = Diagram.Messages.CreateDerivedList(msg => new CommunicationDiagramMessageViewModel(msg),
                msg => msg.Type == Message.MessageType.Invocation).ResultList;
        }

        /// <summary>
        /// The Messages view models for messages that will be drawn in the communication diagram.
        /// </summary>
        /// <remarks>
        /// Only invocation messages get drawn in the communication diagram.
        /// </remarks>
        public readonly IReadOnlyReactiveList<CommunicationDiagramMessageViewModel> MessageViewModels;
    }
}