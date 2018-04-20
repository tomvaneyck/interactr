using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Interactr.Model;
using System.Reactive.Linq;
using Interactr.Reactive;
using Interactr.ViewModel.MessageStack;
using StackFrame = Interactr.ViewModel.MessageStack.StackFrame;

namespace Interactr.ViewModel
{
    /// <summary> The diagram view model for the communication diagram </summary>
    /// <see cref="DiagramViewModel"/>
    public class CommunicationDiagramViewModel : DiagramViewModel
    {
        public CommunicationDiagramViewModel(Diagram diagram) : base(diagram)
        {
            // Create message view models for every invocation message in the diagram model.
            MessageViewModels = Diagram.Messages.CreateDerivedList(msg => new MessageViewModel(msg)).ResultList;
            InvocationMessageViewModels = MessageViewModels.CreateDerivedList(msg => msg, msg => msg.MessageType == Message.MessageType.Invocation).ResultList;

            MessageViewModels.OnAdd.Where(mv => mv.Element.MessageType == Message.MessageType.Result).Subscribe(_ => SetMessageViewModelNumbers());
        }

        /// <summary>
        /// The Messages view models for messages that will be drawn in the communication diagram.
        /// </summary>
        /// <remarks>
        /// Only invocation messages get drawn in the communication diagram.
        /// </remarks>
        public readonly IReadOnlyReactiveList<MessageViewModel> MessageViewModels;

        /// <summary>
        /// The invocataion messages view models.
        /// </summary>
        public IReadOnlyReactiveList<MessageViewModel> InvocationMessageViewModels { get; set; }

        /// <summary>
        /// Calculate the new message view model numbers and set them for every invocation message in communication diagram view.
        /// </summary>
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
                            subFrame.InvocationMessage.MessageNumber = subFrameNum.ToString();
                            PrependNumAllSubFrames(subFrame.SubFrames,subFrameNum.ToString());
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

        private void PrependNumAllSubFrames(IReadOnlyList<StackFrame> subFrames,string messageNumber)
        {
            if (subFrames.Count == 0)
            {
                return;
            }

            foreach (var subsubFrame in subFrames)
            {
                subsubFrame.InvocationMessage.MessageNumber =
                    messageNumber.ToString() + "." + subsubFrame.InvocationMessage.MessageNumber;

                PrependNumAllSubFrames(subsubFrame.SubFrames,messageNumber);
            }
        }
    }
}