using System;
using Interactr.Reactive;

namespace Interactr.Model
{
    /// <summary>
    /// Represent a message passed between parties.
    /// </summary>
    public class Message
    {
        public Message(Party sender, Party receiver, MessageType type, string label)
        {
            Sender = sender;
            Receiver = receiver;
            Label = label;
            Type = type;
        }

        /// <summary>
        /// Enum that represents the type of a message.
        /// </summary>
        public enum MessageType
        {
            Invocation,
            Result
        }

        #region Type

        // The type cannot be changed after creation of the message.
        public MessageType Type { get; }

        #endregion

        #region Label

        private readonly ReactiveProperty<string> _label = new ReactiveProperty<string>();

        ///<summary>
        /// A stream of labels that have been changed.
        /// </summary>
        public IObservable<string> LabelChanged => _label.Changed;

        /// <summary>
        /// Represent the message label.
        /// </summary>
        /// <remarks>
        /// There are no restrictions on the format of the label.
        /// </remarks>
        public string Label
        {
            get => _label.Value;
            set => _label.Value = value;
        }

        #endregion

        #region Sender

        private readonly ReactiveProperty<Party> _sender = new ReactiveProperty<Party>();

        ///<summary>
        /// A stream of senders that have been changed.
        /// </summary>
        public IObservable<Party> SenderChanged => _sender.Changed;

        /// <summary>
        /// Represent the message sender.
        /// </summary>
        public Party Sender
        {
            get => _sender.Value;
            set => _sender.Value = value;
        }

        #endregion

        #region Receiver

        private readonly ReactiveProperty<Party> _receiver = new ReactiveProperty<Party>();

        ///<summary>
        /// A stream of receivers that have been changed.
        /// </summary>
        public IObservable<Party> ReceiverChanged => _receiver.Changed;

        /// <summary>
        /// Represent the message receiver.
        /// </summary>
        public Party Receiver
        {
            get => _receiver.Value;
            set => _receiver.Value = value;
        }

        #endregion
    }
}