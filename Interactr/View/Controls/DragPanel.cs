using Interactr.View.Framework;
using Interactr.Window;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Interactr.Reactive;

namespace Interactr.View.Controls
{
    /// <summary>
    /// A panel that enables some elements to be dragged around.
    /// </summary>
    public class DragPanel : UIElement
    {
        private Point _previousCursorPosition;
        private UIElement _childBeingDragged;

        #region OnDragStart
        private readonly Subject<UIElement> _onDragStart = new Subject<UIElement>();
        
        /// <summary>
        /// This observable provides the child being dragged when the drag starts.
        /// </summary>
        public IObservable<UIElement> OnDragStart => _onDragStart;
        #endregion

        #region OnDragFinished
        private readonly Subject<UIElement> _onDragFinished = new Subject<UIElement>();

        /// <summary>
        /// This observable provides the child being dragged when the drag ends.
        /// </summary>
        public IObservable<UIElement> OnDragFinished => _onDragFinished;
        #endregion

        public DragPanel()
        {
            // Update layout when the width or height is changed.
            ReactiveExtensions.MergeEvents(
                WidthChanged,
                HeightChanged
            ).Subscribe(_ => UpdateLayout());

            // Update layout when a child changes its preferred width or height.
            ReactiveExtensions.MergeEvents(
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
            else if (eventData.Id == MouseEvent.MOUSE_RELEASED)
            {
                _childBeingDragged?.ReleaseMouseCapture();
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
            UIElement newDragElement = FocusedElement.WalkToRoot().FirstOrDefault((element) => element.Parent == this);

            // If the element was not being dragged before, trigger OnDragStart
            if (newDragElement != _childBeingDragged)
            {
                _childBeingDragged = newDragElement;
                _onDragStart.OnNext(_childBeingDragged);
            }

            if (_childBeingDragged != null)
            {
                _childBeingDragged.CaptureMouse();

                Point newPosition = new Point(
                    (int) (_childBeingDragged.Position.X + dragEventData.DeltaX),
                    (int) (_childBeingDragged.Position.Y + dragEventData.DeltaY)
                );

                if (IsValidPosition(_childBeingDragged, newPosition))
                {
                    _childBeingDragged.Position = newPosition;
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
