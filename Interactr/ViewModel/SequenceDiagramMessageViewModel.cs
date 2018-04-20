﻿using System;
using Interactr.Model;
using Interactr.Reactive;

namespace Interactr.ViewModel
{
    /// <summary>
    /// The ViewModel for MessageView for a sequence diagram model.
    /// </summary>
    public class SequenceDiagramMessageViewModel : MessageViewModel
    {
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

        private readonly ReactiveProperty<ActivationBarViewModel> _senderActivationBar =
            new ReactiveProperty<ActivationBarViewModel>();

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

        private readonly ReactiveProperty<ActivationBarViewModel> _receiverActivationBar =
            new ReactiveProperty<ActivationBarViewModel>();

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

        public SequenceDiagramMessageViewModel(Message message, int tick) : base(message)
        {
            Tick = tick;
        }
    }
}