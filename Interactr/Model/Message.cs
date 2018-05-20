using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
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

        /// <summary>
        ///  The label of the message. 
        /// </summary>
        public string Label
        {
            get => _label.Value;
            set
            {
                // In case of an invocation message only set the label
                // if the label has a valid format.
                if (Type == MessageType.Invocation)
                {
                    if (IsValidInvocationLabel(value))
                    {
                        _label.Value = value;
                    }
                    else throw new ArgumentException();
                }
                else
                {
                    _label.Value = value;
                }
            }
        }

        public IObservable<string> LabelChanged => _label.Changed;

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

        #region MessageNumber

        private readonly ReactiveProperty<string> _messageNumber = new ReactiveProperty<string>();

        /// <summary>
        ///  The message number of a message. 
        /// </summary>
        public string MessageNumber
        {
            get => _messageNumber.Value;
            set => _messageNumber.Value = value;
        }

        public IObservable<string> MessageNumberChanged => _messageNumber.Changed;

        #endregion

        /// <summary>
        /// Return True if the given label has a valid format for an InvocationLabel.
        /// <remarks>
        /// The label of an invocation message consists of a method name and an
        /// argument list. A method name starts with a lowercase letter and consists
        /// only of letters, digits, and underscores. An argument list is a parenthesized,
        /// comma-separated list of arguments. An argument is any sequence of characters,
        /// not including commas or parentheses.
        /// </remarks>
        /// <example> methodName(arg1,arg2) </example>
        /// </summary>
        /// <param name="label"> The label string.</param>
        /// <returns>A boolean indicating if it is a valid label.</returns>
        public static bool IsValidInvocationLabel(string label)
        {
            if (label == null)
            {
                return false;
            }

            // Check for a valid program structure.
            var isValidStructure = Regex.IsMatch(label, "^.*" + Regex.Escape("(") + ".*" + Regex.Escape(")") + "$");

            // Check for a valid method name and a valid arguments list.
            var methodNameIsValid = IsValidMethodName(InvocationLabelParser.RetrieveMethodNameFromLabel(label));
            var argumentsListIsValid = IsValidArgumentsList(InvocationLabelParser.RetrieveArgumentsFromLabel(label));

            // The label is valid if the methodName, the argumentsList and the structure are valid.
            return methodNameIsValid && argumentsListIsValid && isValidStructure;
        }

        /// <summary>
        /// Return True if a given method name has a valid format.
        /// </summary>
        /// <remarks>
        /// A method name starts with a lowercase letter and consists
        /// only of letters, digits, and underscores. An argument list is a parenthesized,
        /// </remarks>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public static bool IsValidMethodName(string methodName)
        {
            return methodName != null && Regex.IsMatch(methodName, "^[a-z][a-zA-Z0-9_]*$");
        }

        /// <summary>
        /// Return True if a given argumentList is valid.
        /// </summary>
        /// <remarks>
        /// The list is valid if every string in the list is an argument.
        /// An argument is any sequence of characters, not including commas or parentheses.
        /// </remarks>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static bool IsValidArgumentsList(List<string> arguments)
        {
            if (arguments == null)
            {
                return false;
            }

            foreach (var arg in arguments)
            {
                if (!Regex.IsMatch(arg, "^[^(,)\\s]*$") && arg != "")
                {
                    return false;
                }
            }

            return true;
        }
    }
}
