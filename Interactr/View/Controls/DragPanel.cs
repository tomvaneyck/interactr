using Interactr.View.Framework;
using Interactr.Window;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactr.View.Controls
{
    /// <summary>
    /// A panel that enables some elements to be dragged around.
    /// </summary>
    public class DragPanel : UIElement
    {
        private Point _previousCursorPosition;

        public DragPanel()
        {
            // Update layout when the width or height is changed.
            Observable.Merge(
                WidthChanged.Select(_ => Unit.Default),
                HeightChanged.Select(_ => Unit.Default)
            ).Subscribe(_ => UpdateLayout());

            // Update layout when a child changes its preferred width or height.
            Observable.Merge(
                Children.ObserveEach(child => child.PreferredWidthChanged),
                Children.ObserveEach(child => child.PreferredHeightChanged)
            ).Subscribe(_ => UpdateLayout());
        }

        /// <see cref="UIElement.OnMouseEvent(MouseEventData)"/>
        protected override bool OnMouseEvent(MouseEventData eventData)
        {
            if (eventData.Id == MouseEvent.MOUSE_DRAGGED && FocusedElement.CanLoseFocus)
            {
                MouseDragEventData dragEventData = new MouseDragEventData(
                    eventData.MousePosition.X - _previousCursorPosition.X,
                    eventData.MousePosition.Y - _previousCursorPosition.Y
                );
                ApplyDragToFocusedElement(dragEventData);
                _previousCursorPosition = eventData.MousePosition;
                return true;
            }

            return base.OnMouseEvent(eventData);
        }

        /// <see cref="UIElement.OnMouseEventPreview(MouseEventData)"/>
        protected override bool OnMouseEventPreview(MouseEventData eventData)
        {
            if (eventData.Id == MouseEvent.MOUSE_PRESSED)
            {
                _previousCursorPosition = eventData.MousePosition;
            }

            return base.OnMouseEventPreview(eventData);
        }

        /// <summary>
        /// Apply the drag data to the focused element.
        /// </summary>
        /// <remarks>
        /// Only apply the data if the focused element is a descendant.
        /// </remarks>
        /// <param name="dragEventData">The drag data.</param>
        private void ApplyDragToFocusedElement(MouseDragEventData dragEventData)
        {
            // Only drag the direct descendents of this DragPanel.
            UIElement dragElement = FocusedElement.WalkToRoot().FirstOrDefault((element) => element.Parent == this);

            if (dragElement != null)
            {
                Point newPosition = new Point(
                    (int) (dragElement.Position.X + dragEventData.DeltaX),
                    (int) (dragElement.Position.Y + dragEventData.DeltaY)
                );

                if (IsValidPosition(dragElement, newPosition))
                {
                    dragElement.Position = newPosition;
                }
            }
        }

        /// <summary>
        /// Check if the given position + width of given element is inside this dragPanel.
        /// </summary>
        /// <param name="element">The given element.</param>
        /// <param name="position">The given position.</param>
        /// <returns>True if position is a valid position for the given elementt.</returns>
        private bool IsValidPosition(UIElement element, Point position)
        {
            return (
                position.X + element.Width < Width &&
                position.X >= 0 &&
                position.Y + element.Height < Height &&
                position.Y >= 0
            );
        }

        /// <summary>
        /// Update the layout of all the children.
        /// </summary>
        private void UpdateLayout()
        {
            foreach (UIElement child in Children)
            {
                UpdateLayout(child);
            }
        }

        /// <summary>
        /// Update the layout of the given child.
        /// </summary>
        /// <remarks>
        /// Change the width of the child if it wants to.
        /// </remarks>
        /// <param name="child">The given child.</param>
        private void UpdateLayout(UIElement child)
        {
            child.Width = child.PreferredWidth;
            child.Height = child.PreferredHeight;
        }
    }
}