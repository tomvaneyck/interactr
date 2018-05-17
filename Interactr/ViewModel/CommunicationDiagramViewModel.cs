﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Interactr.Model;
using System.Reactive.Linq;
using Interactr.Reactive;
using Interactr.ViewModel.MessageStack;
using StackFrame = Interactr.ViewModel.MessageStack.StackFrame<Interactr.ViewModel.MessageViewModel>;

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
        private readonly IReadOnlyReactiveList<MessageViewModel> _messageViewModelsReactive; 

        /// <summary>
        /// The message view models for the communication diagram, but in IReadOnlyList form.
        /// <remarks>
        /// This is necessary because the compiler cannot derive inheritance for generic types
        /// in DerivedList which causes the compiler to complain at compileTime when specifying IReadOnlyDerivedList.
        /// </remarks>
        /// </summary>
        protected override IReadOnlyList<MessageViewModel> MessageViewModels
        {
            get => _messageViewModelsReactive;
        }

        public CommunicationDiagramViewModel(Diagram diagram) : base(diagram)
        {
            // Create message view models for every invocation message in the diagram model.
            _messageViewModelsReactive = Diagram.Messages.CreateDerivedList(msg => new MessageViewModel(msg)).ResultList;
            InvocationMessageViewModels = _messageViewModelsReactive
                .CreateDerivedList(msg => msg, msg => msg.MessageType == Message.MessageType.Invocation).ResultList;

            // Set the numbers for the messages in the message view models.
            SetMessageViewModelNumbers();
            _messageViewModelsReactive.OnAdd.Where(mv => mv.Element.MessageType == Message.MessageType.Result)
                .Subscribe(_ => SetMessageViewModelNumbers());
        }

        /// <summary>
        /// The view models of invocation messages.
        /// </summary>
        public IReadOnlyReactiveList<MessageViewModel> InvocationMessageViewModels { get; }

        /// <summary>
        /// Calculate the new message view model numbers and set them for every invocation message in communication diagram view model.
        /// </summary>
        private void SetMessageViewModelNumbers()
        {
            try
            {
                foreach (var stackFrame in MessageStackWalker.Walk<MessageViewModel>(_messageViewModelsReactive))
                {
                    if (stackFrame.SubFrames.Count != 0)
                    {
                        int subFrameNum = 1;
                        foreach (var subFrame in stackFrame.SubFrames)
                        {
                            subFrame.InvocationMessage.MessageNumber = subFrameNum.ToString();
                            PrependNumberToAllSubFrames(subFrame.SubFrames, subFrameNum.ToString());
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

        /// <summary>
        /// Prepend the given messageNumber string to the messagNumber of
        /// all the subframes and nested subframes in subFrames.
        /// </summary>
        /// <param name="subFrames"> The subframes to prepend the messageNumber to</param>
        /// <param name="messageNumber">The messageNumber to prepend</param>
        private void PrependNumberToAllSubFrames(IReadOnlyList<StackFrame<MessageViewModel>> subFrames,
            string messageNumber)
        {
            foreach (var subsubFrame in subFrames)
            {
                // Set the message Number of the invocationMessage in the subsubFrame.
                subsubFrame.InvocationMessage.MessageNumber =
                    messageNumber + "." + subsubFrame.InvocationMessage.MessageNumber;

                // Recursively call PrependNumberToAllSubFrames untill the number of subframes is zero.
                PrependNumberToAllSubFrames(subsubFrame.SubFrames, messageNumber);
            }
        }
    }
}