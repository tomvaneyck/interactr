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
        /// Represent the type the message.
        /// </summary>
        /// <remarks>
        /// It is impossible for a message to not have a type.
        /// </remarks>
        public MessageType Type { get; set; }

        /// <summary>
        /// Represent the message label.
        /// </summary>
        /// <remarks>
        /// There are no restrictions on the format of the label.
        /// </remarks>
        public string Label { get; set; }

        /// <summary>
        /// Represent the message sender.
        /// </summary>
        public Party Sender { get; set; }

        /// <summary>
        /// Represent the message receiver.
        /// </summary>
        public Party Receiver { get; set; }
    }
}