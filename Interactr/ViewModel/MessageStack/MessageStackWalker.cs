﻿using System.Collections.Generic;
using System.Linq;
using Interactr.Model;

namespace Interactr.ViewModel.MessageStack
{
    public static class MessageStackWalker
    {
        /// <summary>
        /// Build a call stack from the given list of messages 
        /// and return each stack frame when it is popped from the stack.
        /// There is one stack frame for each pair of matching invocation and return messages.
        /// </summary>
        /// <remarks>
        /// Because stack frames are returned when they are popped, the deepest frames are returned first.
        /// </remarks>
        /// <param name="messages">The messages to build a stack with.</param>
        public static IEnumerable<StackFrame> Walk(IReadOnlyList<MessageViewModel> messages)
        {
            // Handle edge case where there are no messages.
            if (messages.Count == 0)
            {
                yield break;
            }

            // Store invocation information on a call stack.
            Stack<StackFrame.Builder> stack = new Stack<StackFrame.Builder>();

            // The initiator starts and ends the message sequence, and so is always on the stack.
            stack.Push(new StackFrame.Builder
            {
                Party = messages[0].Message.Sender
            });

            // Iterate over messages and maintain call stack.
            // An invocation message pushes a new frame on the stack
            // A return message pops the current frame from the stack.
            foreach (var messageVM in messages)
            {
                if (messageVM.MessageType == Message.MessageType.Invocation)
                {
                    StackFrame.Builder subFrame = new StackFrame.Builder
                    {
                        InvocationMessage = messageVM,
                        Party = messageVM.Message.Receiver
                    };

                    // Push the new frame on the call stack.
                    stack.Push(subFrame);
                }
                else
                {
                    // Pop invocation from call stack.
                    StackFrame.Builder frame = stack.Pop();

                    // Integrity check: each invocation message must have a matching return message.
                    Party invocator = frame.InvocationMessage.Message.Sender;
                    if (invocator != messageVM.Message.Receiver)
                    {
                        // Messages are not balanced, abort!
                        throw new UnbalancedStackException("Unbalanced message stack!");
                    }

                    // Complete frame data.
                    frame.ReturnMessage = messageVM;
                    frame.Level = stack.Count(f => f.InvocationMessage?.Message.Sender == frame.InvocationMessage.Message.Receiver);

                    StackFrame completedFrame = frame.Build();

                    // Add the new frame to the list of sub-frames on the current activation.
                    // This list is useful because each frame is handled when it is popped from the stack,
                    // and at that moment, its subframes have already been popped previously. 
                    // If we didn't keep a seperate list, the subframes would be lost.
                    stack.Peek().SubFrames.Add(completedFrame);

                    yield return completedFrame;
                }
            }

            if (stack.Count != 1)
            {
                // Messages are not balanced, abort!
                throw new UnbalancedStackException("Unbalanced message stack!");
            }

            // Pop initiator stack frame
            StackFrame initiatorFrame = stack.Pop().Build();
            yield return initiatorFrame;
        }
    }
}
