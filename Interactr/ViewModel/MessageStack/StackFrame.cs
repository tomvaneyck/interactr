using System;
using System.Collections.Generic;
using System.Linq;
using Interactr.Model;

namespace Interactr.ViewModel.MessageStack
{
    /// <summary>
    /// Represents an activation frame in the call stack.
    /// </summary>
    public class StackFrame<T> : IEquatable<StackFrame<T>> where T : MessageViewModel
    {
        /// <summary>
        /// The amount of frames on the stack below this one with the same party.
        /// </summary>
        public int Level { get; }

        /// <summary>
        /// Represents the party that is active during this frame.
        /// This is the party that receives InvocationMessage and sends ReturnMessage.
        /// </summary>
        public Party Party { get; }

        /// <summary>
        /// The message that started this frame. Can be null for the initiator.
        /// </summary>
        public T InvocationMessage { get; }

        /// <summary>
        /// The message that ended this frame. Can be null for the initiator.
        /// </summary>
        public T ReturnMessage { get; }

        /// <summary>
        /// The list sub-frames that were invoked from this frame.
        /// </summary>
        public IReadOnlyList<StackFrame<T>> SubFrames { get; }

        /// <summary>
        /// The tick at which Party becomes active for this frame.
        /// </summary>
        public int StartTick => InvocationMessage?.Tick ?? SubFrames.FirstOrDefault()?.InvocationMessage.Tick ?? 0;

        /// <summary>
        /// The tick at which Party stops being active for this frame.
        /// </summary>
        public int EndTick => ReturnMessage?.Tick ?? SubFrames.LastOrDefault()?.ReturnMessage.Tick ?? StartTick;

        private StackFrame(Builder builder)
        {
            Level = builder.Level;
            Party = builder.Party;
            InvocationMessage = builder.InvocationMessage;
            ReturnMessage = builder.ReturnMessage;
            SubFrames = builder.SubFrames.ToArray();
        }

        #region Equality & HashCode

        public bool Equals(StackFrame<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Level == other.Level &&
                   Equals(Party, other.Party) &&
                   Equals(InvocationMessage, other.InvocationMessage) &&
                   Equals(ReturnMessage, other.ReturnMessage) &&
                   Enumerable.SequenceEqual(SubFrames, other.SubFrames);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StackFrame<T>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Level;
                hashCode = (hashCode * 397) ^ (Party != null ? Party.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (InvocationMessage != null ? InvocationMessage.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ReturnMessage != null ? ReturnMessage.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(StackFrame<T> left, StackFrame<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(StackFrame<T> left, StackFrame<T> right)
        {
            return !Equals(left, right);
        }

        #endregion

        public class Builder
        {
            /// <summary>
            /// The amount of frames on the stack below this one with the same party.
            /// </summary>
            public int Level { get; set; }

            /// <summary>
            /// Represents the party that is active during this frame.
            /// This is the party that receives InvocationMessage and sends ReturnMessage.
            /// </summary>
            public Party Party { get; set; }

            /// <summary>
            /// The message that started this frame. Can be null for the initiator.
            /// </summary>
            public T InvocationMessage { get; set; } 

            /// <summary>
            /// The message that ended this frame. Can be null for the initiator.
            /// </summary>
            public T ReturnMessage { get; set; }

            /// <summary>
            /// The list sub-frames that were invoked from this frame.
            /// </summary>
            public IList<StackFrame<T>> SubFrames { get; } = new List<StackFrame<T>>();

            public StackFrame<T> Build()
            {
                return new StackFrame<T>(this);
            }
        }
    }
}