using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using Interactr.Model;
using Interactr.Reactive;

namespace Interactr.ViewModel
{
    /// <summary>
    /// The ViewModel for MessageView.
    /// </summary>
    public class MessageViewModel
    {
        #region message and tick

        /// <summary>
        /// Refernce to the Message model.
        /// </summary>
        public Message Message { get; }

        /// <summary>
        /// Indicator of the call sequence of the message.
        /// </summary>
        public int Tick { get; set; }

        #endregion

        #region Sender

        private readonly ReactiveProperty<Party> _sender = new ReactiveProperty<Party>();

        /// <summary>
        /// The sender stored in message view model.
        /// </summary>
        /// <remarks>This should not necessarily be the same as the sender in the message model.
        /// If the changes of viewModel are not propogated to the model for example.
        /// Any changes to the model are however immediately propagated to the viewmodel.
        /// </remarks>
        public Party Sender
        {
            get => _sender.Value;
            private set => _sender.Value = value;
        }

        public IObservable<Party> SenderChanged => _sender.Changed;

        #endregion

        #region Receiver

        /// <summary>
        /// The sender stored in message view model.
        /// </summary>
        /// <remarks>This should not necessarily be the same as the sender in the message model.
        /// If the changes of viewModel are not propogated to the model for example.
        /// Any changes to the model are however immediately propagated to the viewmodel.
        /// </remarks>
        private readonly ReactiveProperty<Party> _receiver = new ReactiveProperty<Party>();

        /// <summary>
        /// The Receiver stored in message view model.
        /// </summary>
        /// <remarks>This should not necessarily be the same as the sender in the message model.
        /// If the changes of viewModel are not propogated to the model for example.
        /// Any changes to the model are however immediately propagated to the viewmodel.
        /// </remarks>
        public Party Receiver
        {
            get => _receiver.Value;
            private set => _receiver.Value = value;
        }

        public IObservable<Party> ReceiverChanged => _receiver.Changed;

        #endregion

        #region Label

        private readonly ReactiveProperty<string> _label = new ReactiveProperty<string>();

        /// <summary>
        /// The Label stored in message view model.
        /// </summary>
        /// <remarks>This should not necessarily be the same as the sender in the message model.
        /// If the changes of viewModel are not propogated to the model for example.
        /// Any changes to the model are however immediately propagated to the viewmodel.
        /// </remarks>
        public string Label
        {
            get => _label.Value;
            private set => _label.Value = value;
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
        public Message.MessageType MessageType
        {
            get => Message.Type;
        }

        #endregion

        //TODO reference to activation bars

        public MessageViewModel(Message message, int tick)
        {
            Message = message;
            Tick = tick;

            // Propogate changes in the model to the viewmodel.
            message.SenderChanged.Subscribe(newSender => Sender = newSender);
            message.ReceiverChanged.Subscribe(newReceiver => Receiver = newReceiver);
            message.LabelChanged.Subscribe(newLabel => Label = newLabel);
        }
    }
}