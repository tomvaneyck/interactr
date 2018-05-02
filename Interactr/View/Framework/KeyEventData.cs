using System;
using System.Reactive.Linq;
using Interactr.Reactive;

namespace Interactr.View.Framework
{
    /// <summary>
    /// Information about a keyboard input event.
    /// </summary>
    public class KeyEventData
    {
        /// <summary>
        /// The type of key event.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// The key identifier.
        /// </summary>
        public int KeyCode { get; }

        /// <summary>
        /// The character associated with this key.
        /// </summary>
        public char KeyChar { get; }

        #region IsCancelled

        private ReactiveProperty<bool> _isCancelled = new ReactiveProperty<bool>();

        /// <summary>
        /// True when the propagation of an event has to be stopped.
        /// This enable stopping event propagation in observables.
        /// </summary>
        public bool IsCancelled
        {
            get => _isCancelled.Value;
            set => _isCancelled.Value = value;
        }

        /// <summary>
        /// Observe the IsCancelledProperty.
        /// </summary>
        public IObservable<bool> IsCancelledChanged => _isCancelled.Changed;

        #endregion

        public KeyEventData(int id, int keyCode, char keyChar)
        {
            Id = id;
            KeyCode = keyCode;
            KeyChar = keyChar;
        }
    }
}