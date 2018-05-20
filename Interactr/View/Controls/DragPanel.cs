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

        /// <summary>
        /// On which axes should the children be movable?
        /// </summary>
        public Orientation DraggableOrientations { get; set; } = Orientation.Horizontal | Orientation.Vertical;

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

            Children.ObserveEach(c => c.MouseEventOccured).Subscribe(e =>
            {
                if (e.Value.Id == MouseEvent.MOUSE_PRESSED)
                {
                    // Start drag
                    StartDrag(e.Element, e.Element.TranslatePointTo(this, e.Value.MousePosition));
                }
                else if (e.Value.Id == MouseEvent.MOUSE_DRAGGED)
                {
                    // Apply drag
                    var mousePosition = e.Element.TranslatePointTo(this, e.Value.MousePosition);
                    double deltaX = mousePosition.X - _previousCursorPosition.X;
                    double deltaY = mousePosition.Y - _previousCursorPosition.Y;
                    ApplyDrag(e.Element, deltaX, deltaY);
                    _previousCursorPosition = mousePosition;
                    
                    e.Value.IsHandled = true;
                }
                else if (e.Value.Id == MouseEvent.MOUSE_RELEASED)
                {
                    if (_childBeingDragged != null)
                    {
                        // Finish drag
                        _childBeingDragged.ReleaseMouseCapture();
                        _onDragFinished.OnNext(_childBeingDragged);
                        _childBeingDragged = null;

                        e.Value.IsHandled = true;
                    }
                }
            });
        }

        /// <see cref="UIElement.OnMouseEventPreview(MouseEventData)"/>
        protected override void OnMouseEventPreview(MouseEventData eventData)
        {
            if (eventData.Id == MouseEvent.MOUSE_PRESSED)
            {
                _previousCursorPosition = eventData.MousePosition;
            }

            base.OnMouseEventPreview(eventData);
        }

        /// <summary>
        /// Start the drag of a child of this panel.
        /// </summary>
        /// <param name="child">The child to start dragging.</param>
        /// <param name="mousePos">The current position of the mouse</param>
        public void StartDrag(UIElement child, Point mousePos)
        {
            _previousCursorPosition = mousePos;
            ApplyDrag(child, 0, 0);
        }

        /// <summary>
        /// Apply the drag data to the specified child of this element.
        /// </summary>
        /// <param name="target">The element to be dragged.</param>
        /// <param name="deltaX">The x-axis distance to move the target element. Can be negative.</param>
        /// <param name="deltaY">The y-axis distance to move the target element. Can be negative.</param>
        private void ApplyDrag(UIElement target, double deltaX, double deltaY)
        {
            // If the element was not being dragged before, trigger OnDragStart
            if (target != _childBeingDragged)
            {
                _childBeingDragged = target;
                _onDragStart.OnNext(_childBeingDragged);
            }

            if (_childBeingDragged != null)
            {
                _childBeingDragged.CaptureMouse();

                int newX = _childBeingDragged.Position.X;
                if (DraggableOrientations.HasFlag(Orientation.Horizontal))
                {
                    newX += (int)deltaX;
                }

                int newY = _childBeingDragged.Position.Y;
                if (DraggableOrientations.HasFlag(Orientation.Vertical))
                {
                    newY += (int)deltaY;
                }

                Point newPosition = new Point(newX, newY);
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
        /// <returns>True if position is a valid position for the given element.</returns>
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
