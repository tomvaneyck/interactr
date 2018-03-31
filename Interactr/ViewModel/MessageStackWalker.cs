using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactr.Model;

namespace Interactr.ViewModel
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
        public static IEnumerable<Frame> Walk(IReadOnlyList<SequenceDiagramMessageViewModel> messages)
        {
            // Handle edge case where there are no messages.
            if (messages.Count == 0)
            {
                yield break;
            }

            // Store invocation information on a call stack.
            Stack<Frame> stack = new Stack<Frame>();

            // The initiator starts and ends the message sequence, and so is always on the stack.
            stack.Push(new Frame(messages[0].Message.Sender));

            // Iterate over messages and maintain call stack.
            // An invocation message pushes a new frame on the stack
            // A return message pops the current frame from the stack.
            foreach (var messageVM in messages)
            {
                if (messageVM.MessageType == Message.MessageType.Invocation)
                {
                    Frame subFrame = new Frame(messageVM);

                    // Add the new frame to the list of sub-frames on the current activation.
                    // This list is useful because each frame is handled when it is popped from the stack,
                    // and at that moment, its subframes have already been popped previously. 
                    // If we didn't keep a seperate list, the subframes would be lost.
                    stack.Peek().SubFrames.Add(subFrame);

                    // Push the new frame on the call stack.
                    stack.Push(subFrame);
                }
                else
                {
                    // Pop invocation from call stack.
                    Frame frame = stack.Pop();

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

                    yield return frame;
                }
            }

            if (stack.Count != 1)
            {
                // Messages are not balanced, abort!
                throw new UnbalancedStackException("Unbalanced message stack!");
            }

            // Pop initiator stack frame
            Frame initiatorFrame = stack.Pop();
            yield return initiatorFrame;
        }

        /// <summary>
        /// Represents an activation frame in the call stack.
        /// </summary>
        public class Frame
        {
            /// <summary>
            /// The amount of frames on the stack below this one with the same party.
            /// </summary>
            public int Level { get; set; }

            /// <summary>
            /// Represents the party that is active during this frame.
            /// This is the party that receives InvocationMessage and sends ReturnMessage.
            /// </summary>
            public Party Party { get; }

            /// <summary>
            /// The message that started this frame. Can be null for the initiator.
            /// </summary>
            public SequenceDiagramMessageViewModel InvocationMessage { get; }

            /// <summary>
            /// The message that ended this frame. Can be null for the initiator.
            /// </summary>
            public SequenceDiagramMessageViewModel ReturnMessage { get; set; }

            /// <summary>
            /// The list sub-frames that were invoked from this frame.
            /// </summary>
            public IList<Frame> SubFrames { get; } = new List<Frame>();

            /// <summary>
            /// The tick at which Party becomes active for this frame.
            /// </summary>
            public int StartTick => InvocationMessage?.Tick ?? SubFrames.FirstOrDefault()?.InvocationMessage.Tick ?? 0;

            /// <summary>
            /// The tick at which Party stops being active for this frame.
            /// </summary>
            public int EndTick => ReturnMessage?.Tick ?? SubFrames.LastOrDefault()?.ReturnMessage.Tick ?? StartTick;

            public Frame(Party party)
            {
                Party = party;
            }

            public Frame(SequenceDiagramMessageViewModel invocationMessage)
            {
                InvocationMessage = invocationMessage;
                Party = invocationMessage.Message.Receiver;
            }
        }

        /// <summary>
        /// Exception that is thrown when a stack cannot be evaluated because it is unbalanced.
        /// A message stack is balanced when each invocation message has a return message, 
        /// and the messages are correctly ordered so that the stack frames have proper nesting.
        /// </summary>
        public class UnbalancedStackException : Exception
        {
            public UnbalancedStackException(string msg) : base(msg)
            { }
        }
    }
}
