using System;
using Interactr.Reactive;

namespace Interactr.View.Framework
{
    /// <summary>
    /// Information about a mouse input event.
    /// </summary>
    public class MouseEventData
    {
        /// <summary>
        /// The type of mouse event.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// The current position of the mouse.
        /// </summary>
        public Point MousePosition { get; }

        /// <summary>
        /// Amount of times the mouse button was pressed.
        /// </summary>
        public int ClickCount { get; }

        /// <summary>
        /// True when the propagation of an event has to be stopped.
        /// This enable stopping event propagation in observables.
        /// </summary>
        public bool IsHandled { get; set; } = false;

        public MouseEventData(int id, Point mousePosition, int clickCount)
        {
            Id = id;
            MousePosition = mousePosition;
            ClickCount = clickCount;
        }
    }
}