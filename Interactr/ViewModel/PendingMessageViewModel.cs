using System;
using Interactr.Model;
using Interactr.Reactive;

namespace Interactr.ViewModel
{
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