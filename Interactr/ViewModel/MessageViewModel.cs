using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        #region Label

        private readonly ReactiveProperty<string> _methodName = new ReactiveProperty<string>();

        public string MethodName
        {
            get => _methodName.Value;
            private set => _methodName.Value = value;
        }

        private readonly ReactiveProperty<List<string>> _methodArguments = new ReactiveProperty<List<string>>();

        public List<string> MethodArguments
        {
            get => _methodArguments.Value;
            private set => _methodArguments.Value = value;
        }

        /// <summary>
        /// The label to be displayed, includes the messageNumber and the label if the messageNumber is present.
        /// Is equal to the Label when the messageNumber is not Present.
        /// </summary>
        public string DisplayLabel => MessageNumber != null ? MessageNumber + ":" + Label : Label;

        private readonly ReactiveProperty<string> _label = new ReactiveProperty<string>();

        public string Label
        {
            get => _label.Value;
            set => _label.Value = value;
        }

        public IObservable<string> LabelChanged => _label.Changed;

        #endregion

        #region Type

        /// <summary>
        /// The message type of the message. 
        /// </summary>
        /// <remarks>
        /// Is always consistent with the model because the type of a message cannot change
        /// </remarks>
        public Message.MessageType MessageType => Message.Type;

        #endregion

        public MessageViewModel(Message message)
        {
            Message = message;

            // Propagate changes in the model to the viewmodel.
            message.LabelChanged.Subscribe(newLabelText =>
            {
                if (Label != newLabelText)
                {
                    Label = newLabelText;
                }
            });

//            // Update the methodName and the method arguments when the label in the viewmodel changes.
//            LabelChanged.Subscribe(newLabelText =>
//                {
//                    // Only update if the value is new.
//                    var newMethodName = InvocationLabelParser.RetrieveMethodNameFromLabel(newLabelText);
//                    var newMethodArguments = InvocationLabelParser.RetrieveArgumentsFromLabel(newLabelText);
//
//                    // Make sure the observables don't go in an infinite loop.
//                    if (newMethodName != MethodName)
//                    {
//                        MethodName = newMethodName;
//                    }
//
//                    if (newMethodArguments != MethodArguments)
//                    {
//                        MethodArguments = newMethodArguments;
//                    }
//                }
//            );
//
//            // Update the label on a change in the methodName or methodArguments.
//            _methodArguments.Changed.MergeEvents(_methodName.Changed).Subscribe(_ =>
//            {
//                var newLabel = MethodName ?? "";
//                newLabel += "(";
//                foreach (var arg in MethodArguments)
//                {
//                    newLabel += arg + ",";
//                }
//
//                if (newLabel[newLabel.Length - 1] == ',')
//                {
//                    newLabel = newLabel.Substring(0, newLabel.Length - 1);
//                }
//
//                newLabel += ")";
//            });
//        }
    }
}