using System;
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

        /// <summary> A label.
        /// <example> instance_name;class_name </example>
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

        private readonly ReactiveProperty<string> _label = new ReactiveProperty<string>();

        /// <summary>
        /// The label to be displayed, includes the messageNumber and the labelText.
        /// </summary>
        public string Label
        {
            get => _label.Value;
            private set => _label.Value = value;
        }

        public IObservable<string> LabelChanged => _label.Changed;

        private readonly ReactiveProperty<string> _labelText = new ReactiveProperty<string>();

        /// <summary>
        /// The text of the Label stored in message view model.
        /// </summary>
        /// <remarks>This should not necessarily be the same as the label in the message model.
        /// If the changes of viewModel are not propogated to the model for example.
        /// Any changes to the model are however immediately propagated to the viewmodel.
        /// </remarks>
        public string LabelText
        {
            get => _labelText.Value;
            protected set => _labelText.Value = value;
        }

        public IObservable<string> LabelTextChanged => _labelText.Changed;

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
            message.LabelTextChanged.Subscribe(newLabelText => { LabelText = newLabelText; });
            message.LabelChanged.Subscribe(newLabel => Label = newLabel);

            // Change the label value on a change of the messageNumber or the LabelText.
            Observable.Merge(MessageNumberChanged, LabelTextChanged).Subscribe(
                _ => Label = MessageNumber + ":" + LabelText);
        }
    }
}