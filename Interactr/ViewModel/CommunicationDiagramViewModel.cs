using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using Interactr.Model;
using Interactr.Reactive;
using Interactr.View.Framework;
using Interactr.ViewModel.MessageStack;

namespace Interactr.ViewModel
{
    /// <summary> The diagram view model for the sequence diagram </summary>
    /// <see cref="DiagramViewModel"/>
    public class CommunicationDiagramViewModel : DiagramViewModel
    {
        public CommunicationDiagramViewModel(Diagram diagram) : base(diagram)
        {
            // Create message view models for every invocation message in the diagram model.
            MessageViewModels = Diagram.Messages.CreateDerivedList(msg => new CommunicationDiagramMessageViewModel(msg)).ResultList;
            InvocationMessageViewModels = MessageViewModels.CreateDerivedList(msg => msg, msg => msg.MessageType == Message.MessageType.Invocation).ResultList;

            MessageViewModels.OnAdd.Where(mv => mv.Element.MessageType == Message.MessageType.Result).Subscribe(_ => SetMessageViewModelNumbers());
        }

        /// <summary>
        /// The Messages view models for messages that will be drawn in the communication diagram.
        /// </summary>
        /// <remarks>
        /// Only invocation messages get drawn in the communication diagram.
        /// </remarks>
        public readonly IReadOnlyReactiveList<CommunicationDiagramMessageViewModel> MessageViewModels;

        /// <summary>
        /// The invocataion messages view models.
        /// </summary>
        public IReadOnlyReactiveList<CommunicationDiagramMessageViewModel> InvocationMessageViewModels { get; set; }

        private void SetMessageViewModelNumbers()
        {
            try
            { 
                foreach (var stackFrame in MessageStackWalker.Walk(MessageViewModels))
                {
                    if (stackFrame.SubFrames.Count != 0)
                    {
                        int subFrameNum = 1;
                        foreach (var subFrame in stackFrame.SubFrames)
                        {
                            subFrame.InvocationMessage.MessageNumber = subFrameNum.ToString() + subFrame.InvocationMessage.MessageNumber;
                            subFrameNum++;
                        }
                    }
                }
            }
            catch (UnbalancedStackException)
            {
                Debug.Print("Stack was unbalanced.");
            }
        }
    }
}