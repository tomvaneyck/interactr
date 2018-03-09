using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactr.View.Framework
{
    /// <summary>
    /// Information about a mouse drag event.
    /// </summary>
    class MouseDragEventData
    {
        /// <summary>
        /// The horizontal distance over which the mouse is dragged.
        /// </summary>
        public double DeltaX { get; }
        
        /// <summary>
        /// The vertical distance over which the mouse is dragged.
        /// </summary>
        public double DeltaY { get; }

        public MouseDragEventData(int deltaX, int deltaY)
        {
            DeltaX = deltaX;
            DeltaY = deltaY;
        }
    }
}
