using System;
using Interactr.Model;
using Interactr.Reactive;

namespace Interactr.ViewModel
{
    /// <summary>
    /// The ViewModel for MessageView.
    /// </summary>
    public class SequenceDiagramMessageViewModel
    {
        /// <summary>
        /// Reference to the Message model.
        /// </summary>
        public Message Message { get; }
        
        #region Tick

        private readonly ReactiveProperty<int> _tick = new ReactiveProperty<int>();

        /// <summary>
        /// This marks the position of this call in the sequence of messages.
        /// </summary>
        public int Tick
        {
            get => _tick.Value;
            set => _tick.Value = value;
        }

        public IObservable<int> TickChanged => _tick.Changed;

        #endregion

        #region SenderActivationBar

        private readonly ReactiveProperty<ActivationBarViewModel> _senderActivationBar = new ReactiveProperty<ActivationBarViewModel>();
        
        /// <summary>
        /// The activation bar from which this message is sent.
        /// </summary>
        public ActivationBarViewModel SenderActivationBar
        {
            get => _senderActivationBar.Value;
            set => _senderActivationBar.Value = value;
        }

        public IObservable<ActivationBarViewModel> SenderActivationBarChanged => _senderActivationBar.Changed;

        #endregion

        #region ReceiverActivationBar

        private readonly ReactiveProperty<ActivationBarViewModel> _receiverActivationBar = new ReactiveProperty<ActivationBarViewModel>();

        /// <summary>
        /// The activation bar that receives this message.
        /// </summary>
        public ActivationBarViewModel ReceiverActivationBar
        {
            get => _receiverActivationBar.Value;
            set => _receiverActivationBar.Value = value;
        }

        public IObservable<ActivationBarViewModel> ReceiverActivationBarChanged => _receiverActivationBar.Changed;

        #endregion

        #region Label

        private readonly ReactiveProperty<string> _label = new ReactiveProperty<string>();

        /// <summary>
        /// The Label stored in message view model.
        /// </summary>
        /// <remarks>This should not necessarily be the same as the label in the message model.
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
        public Message.MessageType MessageType => Message.Type;

        #endregion

        public SequenceDiagramMessageViewModel(Message message, int tick)
        {
            Message = message;
            Tick = tick;

            // Propagate changes in the model to the viewmodel.
            message.LabelChanged.Subscribe(newLabel => Label = newLabel);
        }
    }

    /// <summary>
    /// Viewmodel for a sequence diagram message that is currently being edited/created.
    /// </summary>
    public class PendingMessageViewModel
    {
        #region SenderActivationBar

        private readonly ReactiveProperty<ActivationBarViewModel> _senderActivationBar = new ReactiveProperty<ActivationBarViewModel>();

        /// <summary>
        /// The activation bar from which this message is sent.
        /// </summary>
        public ActivationBarViewModel SenderActivationBar
        {
            get => _senderActivationBar.Value;
            set => _senderActivationBar.Value = value;
        }

        public IObservable<ActivationBarViewModel> SenderActivationBarChanged => _senderActivationBar.Changed;

        #endregion

        #region Receiver

        private readonly ReactiveProperty<Party> _receiver = new ReactiveProperty<Party>();

        /// <summary>
        /// The party that receives the message.
        /// </summary>
        public Party Receiver
        {
            get => _receiver.Value;
            set => _receiver.Value = value;
        }

        public IObservable<Party> ReceiverChanged => _receiver.Changed;

        #endregion

        #region Label

        private readonly ReactiveProperty<string> _label = new ReactiveProperty<string>();
        
        /// <summary>
        /// The text associated with this message.
        /// </summary>
        public string Label
        {
            get => _label.Value;
            set => _label.Value = value;
        }

        public IObservable<string> LabelChanged => _label.Changed;

        #endregion

        #region Type

        private readonly ReactiveProperty<Message.MessageType> _type = new ReactiveProperty<Message.MessageType>();
        
        /// <summary>
        /// The type of message.
        /// </summary>
        public Message.MessageType Type
        {
            get => _type.Value;
            set => _type.Value = value;
        }

        public IObservable<Message.MessageType> TypeChanged => _type.Changed;

        #endregion
        
        #region Tick

        private readonly ReactiveProperty<int> _tick = new ReactiveProperty<int>();

        /// <summary>
        /// The timestamp at which this message is sent.
        /// </summary>
        public int Tick
        {
            get => _tick.Value;
            set => _tick.Value = value;
        }

        public IObservable<int> TickChanged => _tick.Changed;

        #endregion
    }
}