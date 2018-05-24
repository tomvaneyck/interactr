using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using Interactr.Model;
using Interactr.Reactive;

namespace Interactr.ViewModel
{
    /// <summary>
    /// Contain the shared functionality and properties of message view models.
    /// </summary>
    public class MessageViewModel
    {
        /// <summary>
        /// Reference to the Message model.
        /// </summary>
        public Message Message { get; }

        #region MessageNumber

        private readonly ReactiveProperty<string> _messageNumber = new ReactiveProperty<string>();

        /// <summary>
        /// The number of this message, used for displaying in the label.
        /// </summary>
        public string MessageNumber
        {
            get => _messageNumber.Value;
            set => _messageNumber.Value = value;
        }

        /// <summary>
        /// An observable that emits the new label when it has changed.
        /// </summary>
        public IObservable<string> MessageNumberChanged => _messageNumber.Changed;

        #endregion

        #region Tick

        private readonly ReactiveProperty<int> _tick = new ReactiveProperty<int>();

        /// <summary>
        /// Mark the position of this call in the sequence of messages.
        /// </summary>
        public int Tick
        {
            get => _tick.Value;
            set => _tick.Value = value;
        }

        public IObservable<int> TickChanged => _tick.Changed;

        #endregion

        public ILabelVM Label { get; }

        #region Type

        /// <summary>
        /// The message type of the message. 
        /// </summary>
        /// <remarks>
        /// Is always consistent with the model because the type of a message cannot change
        /// </remarks>
        public Message.MessageType MessageType => Message.Type;

        #endregion

        #region CanApplyLabel

        private readonly ReactiveProperty<bool> _canApplyLabel = new ReactiveProperty<bool>();

        /// <summary>
        /// Indicate if the label is valid and thus can be applied in the model.
        /// </summary>
        public bool CanApplyLabel
        {
            get => _canApplyLabel.Value;
            set => _canApplyLabel.Value = value;
        }

        public IObservable<bool> CanApplyLabelChanged => _canApplyLabel.Changed;

        #endregion

        public MessageViewModel(Message message)
        {
            Message = message;

            // Set CanApplyLabel to true if the message is of the ResultType
            if (MessageType == Message.MessageType.Result)
            {
                CanApplyLabel = true;
                Label = new ReturnMessageLabelVM();
            }
            else
            {
                Label = new InvocationMessageLabelVM();
            }

            // Bidirectional bind between the message number in the viewmodel and model.
            Message.MessageNumberChanged.Subscribe(m => MessageNumber = m);
            MessageNumberChanged.Subscribe(m => Message.MessageNumber = m);

            // Bind the label in the viewmodel to the label in the model.
            message.LabelChanged.Subscribe(newLabelText =>
            {
                if (Label.Label != newLabelText)
                {
                    Label.Label = newLabelText;
                }
            });

            // Update CanApplyLabel when the label changes.
            Label.LabelChanged.Where(_ => MessageType == Message.MessageType.Invocation)
                .Select(Message.IsValidInvocationLabel).Subscribe(isValid => CanApplyLabel = isValid);

            if (Message.Type == Message.MessageType.Invocation)
            {
                InvocationMessageLabelVM invLabel = Label as InvocationMessageLabelVM;

                Debug.Print(Label.ToString());
                Debug.Print(invLabel.ToString());
                // Update the label on a change in the methodName or methodArguments.
                invLabel.MethodNameChanged.MergeEvents(invLabel.MethodArgumentsChanged).Subscribe(_ =>
                {
                    var newLabel = invLabel.Label ?? "";
                    newLabel += "(";

                    if (invLabel.MethodArguments != null)
                    {
                        foreach (var arg in invLabel.MethodArguments)
                        {
                            newLabel += arg + ",";
                        }
                    }

                    if (newLabel[newLabel.Length - 1] == ',')
                    {
                        newLabel = newLabel.Substring(0, newLabel.Length - 1);
                    }

                    newLabel += ")";
                });
            }
        }

        /// <summary>
        /// Set the model label to the label in the viewmodel.
        /// </summary>
        public void ApplyLabel()
        {
            Message.Label = Label.Label;
        }
    }
}