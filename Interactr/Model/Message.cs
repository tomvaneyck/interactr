using System;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Windows.Forms;
using Interactr.Reactive;

namespace Interactr.Model
{
    /// <summary>
    /// Represent a message passed between parties.
    /// </summary>
    public class Message
    {
        private readonly ReactiveProperty<MessageType> _type = new ReactiveProperty<MessageType>();
        private readonly ReactiveProperty<string> _label = new ReactiveProperty<string>();
        private readonly ReactiveProperty<Party> _receiver = new ReactiveProperty<Party>();
        private readonly ReactiveProperty<Party> _sender = new ReactiveProperty<Party>();

        public Message(Party sender, Party receiver, MessageType type, string label)
        {
            Sender = sender;
            Receiver = receiver;
            Type = type;
            Label = label;
        }

        /// <summary>
        /// Enum that represents the type of a message.
        /// </summary>
        public enum MessageType
        {
            Invocation,
            Result
        }

        /// <summary>
        /// A stream of message types that have been changed.
        /// </summary>
        public IObservable<MessageType> TypeChanged => _type.Changed;

        ///<summary>
        /// A stream of labels that have been changed.
        /// </summary>
        public IObservable<string> LabelChanged => _label.Changed;

        ///<summary>
        /// A stream of senders that have been changed.
        /// </summary>
        public IObservable<Party> SenderChanged => _sender.Changed;

        ///<summary>
        /// A stream of receivers that have been changed.
        /// </summary>
        public IObservable<Party> ReceiverChanged => _receiver.Changed;


        /// <summary>
        /// Represent the message type.
        /// </summary>

        public MessageType Type
        {
            get => _type.Value;
            set => _type.Value = value;
        }

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

        /// <summary>
        /// Represent the message sender.
        /// </summary>
        public Party Sender
        {
            get => _sender.Value;
            set => _sender.Value = value;
        }

        /// <summary>
        /// Represent the message receiver.
        /// </summary>
        public Party Receiver
        {
            get => _receiver.Value;
            set => _receiver.Value = value;
        }
    }
}