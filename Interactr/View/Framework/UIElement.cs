using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Interactr.Reactive;
using Interactr.Window;

namespace Interactr.View.Framework
{
    /// <summary>
    /// Parent class for views.
    /// </summary>
    public class UIElement
    {
        #region FocusedElement

        private static readonly ReactiveProperty<UIElement> _focusedElement = new ReactiveProperty<UIElement>();

        /// <summary>
        /// The UI element that currently has the keyboard focus.
        /// </summary>
        /// <remarks>
        /// The focused element is the first element to receive keyboard events.
        /// Use UIElement.Focus() to set an element as the focused element.
        /// </remarks>
        public static UIElement FocusedElement
        {
            get => _focusedElement.Value;
            private set => _focusedElement.Value = value;
        }

        public static IObservable<UIElement> FocusedElementChanged => _focusedElement.Changed;

        #endregion


        #region InputEvents

        #region Mouse

        private readonly Subject<MouseEventData> _mouseEventPreviewOccured = new Subject<MouseEventData>();
        public IObservable<MouseEventData> MouseEventPreviewOccured => _mouseEventPreviewOccured;

        private readonly Subject<MouseEventData> _mouseEventOccured = new Subject<MouseEventData>();
        public IObservable<MouseEventData> MouseEventOccured => _mouseEventOccured;

        #endregion

        #region Key

        private readonly Subject<KeyEventData> _keyEventPreviewOccurred = new Subject<KeyEventData>();
        public IObservable<KeyEventData> KeyEventPreviewOccurred => _keyEventPreviewOccurred;

        private readonly Subject<KeyEventData> _keyEventOccurred = new Subject<KeyEventData>();
        public IObservable<KeyEventData> KeyEventOccurred => _keyEventOccurred;

        #endregion

        #endregion

        #region MouseCapturingElement

        private static readonly ReactiveProperty<UIElement> _mouseCapturingElement = new ReactiveProperty<UIElement>();

        /// <summary>
        /// The UI element that has captured the mouse.
        /// </summary>
        /// <remarks>
        /// If an element captures the mouse, it will receive all mouse events, regardless of the current mouse position.
        /// Use UIElement.CaptureMouse() to capture the mouse.
        /// </remarks>
        public static UIElement MouseCapturingElement
        {
            get => _mouseCapturingElement.Value;
            private set => _mouseCapturingElement.Value = value;
        }

        public static IObservable<UIElement> MouseCapturingElementChanged => _mouseCapturingElement.Changed;

        #endregion

        /// <summary>
        /// The child-elements of this element in the view-tree.
        /// </summary>
        public ReactiveList<UIElement> Children { get; } = new ReactiveArrayList<UIElement>();

        #region Parent

        private readonly ReactiveProperty<UIElement> _parent = new ReactiveProperty<UIElement>();

        /// <summary>
        /// The parent element of this element in the view-tree.
        /// This property is set automatically when the element is added to another elements Children.
        /// </summary>
        public UIElement Parent
        {
            get => _parent.Value;
            private set => _parent.Value = value;
        }

        public IObservable<UIElement> ParentChanged => _parent.Changed;

        #endregion

        /// <summary>
        /// The properties that are attached to this element.
        /// </summary>
        public ReactiveDictionary<AttachedProperty, object> AttachedProperties { get; } =
            new ReactiveDictionary<AttachedProperty, object>();

        #region Position

        private readonly ReactiveProperty<Point> _position = new ReactiveProperty<Point>();

        /// <summary>
        /// The position of this element, relative to its parent
        /// </summary>
        public Point Position
        {
            get => _position.Value;
            set => _position.Value = value;
        }

        public IObservable<Point> PositionChanged => _position.Changed;

        #endregion

        public IObservable<Point> AbsolutePositionChanged { get; }

        #region Width

        private readonly ReactiveProperty<int> _width = new ReactiveProperty<int>();

        public int Width
        {
            get => _width.Value;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Invalid width value: " + value);
                }

                _width.Value = value;
            }
        }

        public IObservable<int> WidthChanged => _width.Changed;

        #endregion

        #region Height

        private readonly ReactiveProperty<int> _height = new ReactiveProperty<int>();

        public int Height
        {
            get => _height.Value;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Invalid height value: " + value);
                }

                _height.Value = value;
            }
        }

        public IObservable<int> HeightChanged => _height.Changed;

        #endregion

        #region PreferredWidth

        private readonly ReactiveProperty<int> _preferredWidth = new ReactiveProperty<int>();

        public int PreferredWidth
        {
            get => _preferredWidth.Value;
            set => _preferredWidth.Value = value;
        }

        public IObservable<int> PreferredWidthChanged => _preferredWidth.Changed;

        #endregion

        #region PreferredHeight

        private readonly ReactiveProperty<int> _preferredHeight = new ReactiveProperty<int>();

        public int PreferredHeight
        {
            get => _preferredHeight.Value;
            set => _preferredHeight.Value = value;
        }

        public IObservable<int> PreferredHeightChanged => _preferredHeight.Changed;

        #endregion

        #region IsVisible

        private readonly ReactiveProperty<bool> _isVisible = new ReactiveProperty<bool>();

        public bool IsVisible
        {
            get => _isVisible.Value;
            set => _isVisible.Value = value;
        }

        public IObservable<bool> IsVisibleChanged => _isVisible.Changed;

        #endregion

        #region IsVisibleToMouse

        private readonly ReactiveProperty<bool> _isVisibleToMouse = new ReactiveProperty<bool>();

        /// <summary>
        /// If this property is false, this element will be ignored when searching the mouseover element.
        /// Both IsVisible and IsVisibleToMouse must be true for this element to receive mouse events.
        /// True by default.
        /// </summary>
        public bool IsVisibleToMouse
        {
            get => _isVisibleToMouse.Value;
            set => _isVisibleToMouse.Value = value;
        }

        public IObservable<bool> IsVisibleToMouseChanged => _isVisibleToMouse.Changed;

        #endregion

        #region Focus

        public bool IsFocused => FocusedElement == this;
        private readonly Subject<bool> _focusChanged = new Subject<bool>();
        public IObservable<bool> FocusChanged => _focusChanged.StartWith(IsFocused);

        public bool CanLoseFocus { get; set; } = true;

        #endregion

        #region RepaintRequested

        private readonly Subject<Unit> _repaintRequested = new Subject<Unit>();
        public IObservable<Unit> RepaintRequested => _repaintRequested;

        #endregion


        public bool CanBeFocused { get; protected set; } = true;

        public UIElement()
        {
            this.IsVisible = true;
            this.IsVisibleToMouse = true;

            //Trigger AbsolutePositionChanged when this elements position or an ancestors position changes.
            //Don't fire the event if the position relative to the root doesn't change. (because the changes cancel out.)
            AbsolutePositionChanged = ReactiveExtensions.MergeEvents(
                PositionChanged,
                ParentChanged.ObserveNested(parent => parent.AbsolutePositionChanged)
            ).Select(_ => GetPositionRelativeToRoot()).DistinctUntilChanged();

            ReactiveExtensions.MergeEvents(
                PositionChanged,
                WidthChanged,
                HeightChanged,
                IsVisibleChanged,
                Children.OnDelete,
                Children.OnAdd
            ).Subscribe(_ => Repaint());

            SetupParentChildRelationship();
        }

        private void SetupParentChildRelationship()
        {
            // Set parent-child relationship on child add
            Children.OnAdd.Subscribe(e =>
            {
                UIElement newChild = e.Element;
                if (newChild.Parent != null)
                {
                    throw new Exception("UIElement already has a parent.");
                }

                newChild.Parent = this;
            });

            // Remove parent-child relationship on child remove
            Children.OnDelete.Subscribe(e =>
            {
                e.Element.Parent = null;
                if (FocusedElement == e.Element || (FocusedElement?.WalkToRoot().Contains(e.Element) ?? false))
                {
                    // Focused element will be removed from the visual tree, focus the first ancestor.
                    this.Focus();
                }
            });

            // When a child requests a repaint, pass the request upwards so the canvaswindow on top can do the redraw.
            Children.ObserveEach(child => child.RepaintRequested).Subscribe(_ => Repaint());
        }

        /// <summary>
        /// Marks this element as the focused element.
        /// See UIElement.FocusedElement for more information.
        /// </summary>
        public void Focus()
        {
            if (this == FocusedElement || (!FocusedElement?.CanLoseFocus ?? false))
            {
                // This is already focused
                return;
            }

            // Set focused element, then emit events for previous and current focused elements.
            UIElement previouslyFocusedElement = FocusedElement;
            FocusedElement = this;

            // Previously focused element is now unfocused
            previouslyFocusedElement?._focusChanged.OnNext(false);

            // This element is now focused
            this._focusChanged.OnNext(true);
        }

        #region Keyboard events

        /// <summary>
        /// Emit a keyboard event.
        /// </summary>
        /// <param name="eventData">Details about this event.</param>
        public static void HandleKeyEvent(KeyEventData eventData)
        {
            TunnelDownKeyEventPreview(eventData);
            if (eventData.IsHandled)
            {
                //Event was handled
                return;
            }

            //Bubble up event from FocusedElement to root
            FocusedElement.BubbleUpKeyEvent(eventData);
        }

        /// <summary>
        /// Take a key event, call OnKeyEventPreview on every element from the root down until the event is canceled or FocusedElement is reached.
        /// </summary>
        /// <remarks>
        /// Only the ancestors of FocusedElement will receive the event.
        /// </remarks>
        /// <param name="eventData">Details about this event.</param>
        private static void TunnelDownKeyEventPreview(KeyEventData eventData)
        {
            foreach (UIElement element in UIElement.FocusedElement.WalkToRoot().Reverse())
            {
                element.OnKeyEventPreview(eventData);
                if (eventData.IsHandled)
                {
                    return;
                }

                element._keyEventPreviewOccurred.OnNext(eventData);
                if (eventData.IsHandled)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Take a key event, call OnKeyEvent and pass the event to the parent element until an element handles it.
        /// </summary>
        /// <param name="eventData">Details about this event.</param>
        private void BubbleUpKeyEvent(KeyEventData eventData)
        {
            OnKeyEvent(eventData);
            if (eventData.IsHandled)
            {
                return;
            }

            _keyEventOccurred.OnNext(eventData);
            if (eventData.IsHandled)
            {
                return;
            }

            Parent?.BubbleUpKeyEvent(eventData);
        }

        /// <summary>
        /// Is called when keyboard events are triggered.
        /// Can be overriden to intercept key events before they reach the FocusedElement.
        /// Set evenData.IsCancelled to true to indicate that the event has been handled and should not be pushed to more ui elements.
        /// </summary>
        /// <param name="eventData">Details about this event.</param>
        protected virtual void OnKeyEventPreview(KeyEventData eventData)
        {
        }

        /// <summary>
        /// Is called when keyboard events are triggered.
        /// Can be overridden to handle keyboard events.
        /// Set eventData.IsCancelled to true indicate that the event has been handled and should not be pushed to more ui elements.
        /// </summary>
        /// <param name="eventData">Details about this event.</param>
        /// <returns>True if this element has handled the event</returns>
        protected virtual void OnKeyEvent(KeyEventData eventData)
        {
        }

        #endregion

        #region Mouse events

        /// <summary>
        /// Emit a mouse event.
        /// </summary>
        /// <param name="rootElement">The element at the top of the view-tree</param>
        /// <param name="eventData">Details about this event. Should be relative to rootElement.</param>
        public static void HandleMouseEvent(UIElement rootElement, MouseEventData eventData)
        {
            UIElement targetElement = MouseCapturingElement ?? rootElement.FindElementAt(eventData.MousePosition);

            TunnelDownMouseEventPreview(rootElement, targetElement, eventData);
            if (eventData.IsHandled)
            {
                // Event was handled.
                return;
            }

            // Bubble up event from FocusedElement to root.
            Point relativeMousePos = rootElement.TranslatePointTo(targetElement, eventData.MousePosition);
            targetElement.BubbleUpMouseEvent(new MouseEventData(eventData.Id, relativeMousePos, eventData.ClickCount));
        }

        /// <summary>
        /// Take a mouse event, call OnMouseEventPreview on every element from the root down until the evenData
        /// is cancelled or mouseover-element is reached.
        /// Only the ancestors of the mouseover-element will receive the event.
        /// </summary>
        /// <param name="rootElement">The element at the top of the view-tree.</param>
        /// <param name="mouseoverElement">The element the mouse is over.</param>
        /// <param name="eventData">Details about this event. Should be relative to rootElement.</param>
        private static void TunnelDownMouseEventPreview(UIElement rootElement, UIElement mouseoverElement,
            MouseEventData eventData)
        {
            foreach (UIElement element in mouseoverElement.WalkToRoot().Reverse())
            {
                Point relativeMousePos = rootElement.TranslatePointTo(element, eventData.MousePosition);
                var newMouseEventData = new MouseEventData(eventData.Id, relativeMousePos, eventData.ClickCount);

                element.OnMouseEventPreview(newMouseEventData);
                if (newMouseEventData.IsHandled)
                {
                    // Stop event propagation.
                    eventData.IsHandled = true;
                    return;
                }

                element._mouseEventOccured.OnNext(newMouseEventData);
                if (newMouseEventData.IsHandled)
                {
                    // Stop event propagation.
                    eventData.IsHandled = true;
                    return;
                }
            }
        }

        /// <summary>
        /// Take a mouse event, call OnMouseEvent and keep passing the event to the parent element until an element handles it.
        /// </summary>
        /// <param name="eventData">Details about this event. Should be relative to this element.</param>
        private void BubbleUpMouseEvent(MouseEventData eventData)
        {
            OnMouseEvent(eventData);
            if (eventData.IsHandled)
            {
                return;
            }

            _mouseEventOccured.OnNext(eventData);
            if (eventData.IsHandled)
            {
                return;
            }

            // Return at root.
            if (Parent == null)
            {
                return;
            }

            Point relativeMousePos = TranslatePointTo(Parent, eventData.MousePosition);
            Parent.BubbleUpMouseEvent(new MouseEventData(eventData.Id, relativeMousePos, eventData.ClickCount));
        }

        /// <summary>
        /// Is called when mouse events are triggered.
        /// Override this method to intercept mouse events before they reach the element the mouse is over.
        /// Set eventData.IsCancelled to true to indicate that the event has been handled and should not be pushed
        /// to more UI elements.
        /// </summary>
        /// <param name="eventData">Details about this event. Should be relative to this element.</param>
        protected virtual void OnMouseEventPreview(MouseEventData eventData)
        {
        }

        /// <summary>
        /// Is called when mouse events are triggered.
        /// Can be overriden to handle mouse events.
        /// Set eventData.IsCancelled to true to indicate that the event has been handled and should not be pushed to more ui elements.
        /// </summary>
        /// <param name="eventData">Details about this event. Should be relative to this element.</param>
        protected virtual void OnMouseEvent(MouseEventData eventData)
        {
            // Only focus on mouseclick.
            if (eventData.Id == MouseEvent.MOUSE_PRESSED && CanBeFocused)
            {
                Focus();
                eventData.IsHandled = true;
            }
        }

        public void CaptureMouse()
        {
            MouseCapturingElement = this;
        }

        public void ReleaseMouseCapture()
        {
            if (MouseCapturingElement == this)
            {
                MouseCapturingElement = null;
            }
        }

        #endregion

        #region Painting

        /// <summary>
        /// Paint this element and its children.
        /// </summary>
        /// <param name="g">The graphics object</param>
        public void Paint(Graphics g)
        {
            // Create drawing stack
            Stack<PaintingStackFrame> stack = new Stack<PaintingStackFrame>();

            // Put this element on the stack
            stack.Push(new PaintingStackFrame
            {
                Element = this,
                Origin = new Point(0, 0),
                RenderHeight = Height,
                RenderWidth = Width
            });

            // Save current transform and clip
            Matrix defaultTransform = g.Transform;
            Region defaultClip = g.Clip;

            // Draw elements
            while (stack.Count > 0)
            {
                // Get current stack frame
                PaintingStackFrame f = stack.Pop();
                UIElement curElement = f.Element;

                // Push visible children of curElement onto stack
                // We use Children.Reverse() because the usage of the stack reverses.
                foreach (UIElement child in curElement.Children.Reverse().Where(c => c.IsVisible))
                {
                    // Check if the element is completely outside the available space.
                    if (child.Position.X > f.RenderWidth || child.Position.Y > f.RenderHeight ||
                        child.Position.X + child.Width < 0 || child.Position.Y + child.Height < 0)
                    {
                        // Child is out of bounds, don't render.
                        continue;
                    }

                    stack.Push(new PaintingStackFrame
                    {
                        Element = child,
                        Origin = f.Origin + child.Position,
                        RenderWidth = Math.Min(child.Width, f.RenderWidth - child.Position.X),
                        RenderHeight = Math.Min(child.Height, f.RenderHeight - child.Position.Y)
                    });
                }

                g.SetClip(new RectangleF(f.Origin.X, f.Origin.Y, f.RenderWidth, f.RenderHeight));
                // Map x, y coordinates to child space (relative to child origin)
                g.TranslateTransform(f.Origin.X, f.Origin.Y);
                // Set max drawing width and height
                //g.SetClip(new RectangleF(0, 0, f.RenderWidth, f.RenderHeight));

                // Paint
                curElement.PaintElement(g);

                // Reset clip and transform
                g.Transform = defaultTransform;
                g.Clip = defaultClip;
            }
        }

        private class PaintingStackFrame
        {
            // Element to be painted.
            public UIElement Element { get; set; }

            // Origin point of this element.
            public Point Origin { get; set; }

            // Actual width in pixels to render the Element at.
            public int RenderWidth { get; set; }

            // Actual height in pixels to render the Element at.
            public int RenderHeight { get; set; }
        }

        /// <summary>
        /// Emit a request for repainting.
        /// </summary>
        /// <remarks>
        /// This method will trigger a RepaintRequested event, which will be bubbled upwards
        /// to the top-level where it can be handled by the window.
        /// </remarks>
        public void Repaint()
        {
            _repaintRequested.OnNext(Unit.Default);
        }

        /// <summary>
        /// Render this element to the screen.
        /// </summary>
        /// <param name="g">The graphics object that can be used to draw</param>
        public virtual void PaintElement(Graphics g)
        {
        }

        #endregion

        /// <summary>
        /// Transform input point p to coördinates relative to the given UIElement.
        /// </summary>
        /// <param name="e">UIElement to transform input point to</param>
        /// <param name="p">Input point relative to this</param>
        /// <returns>Transformed point</returns>
        public Point TranslatePointTo(UIElement e, Point p)
        {
            Point pRoot = GetPositionRelativeToRoot(p);

            Point uiElementPosition = e.GetPositionRelativeToRoot();

            return new Point(
                pRoot.X - uiElementPosition.X,
                pRoot.Y - uiElementPosition.Y
            );
        }

        /// <summary>
        /// Find the UIElement that is visible at this point on the screen.
        /// This function searches downward in the tree for the bottom-most node that contains the specified point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public UIElement FindElementAt(Point point)
        {
            UIElement childContainingPoint = Children
                .Reverse()
                .FirstOrDefault(child =>
                    child.IsVisible &&
                    child.IsVisibleToMouse &&
                    point.X >= child.Position.X && point.Y >= child.Position.Y &&
                    point.X < (child.Position.X + child.Width) &&
                    point.Y < (child.Position.Y + child.Height)
                );

            if (childContainingPoint == null)
            {
                //No child of this element contains 'point', so this is the deepest element that does.
                return this;
            }

            Point relativePosition = this.TranslatePointTo(childContainingPoint, point);
            return childContainingPoint.FindElementAt(relativePosition);
        }

        /// <summary>
        /// Return coördinates relative to the root of this UIElement.
        /// </summary>
        /// <returns>Given point relative to the root.</returns>
        private Point GetPositionRelativeToRoot(Point p = default(Point))
        {
            return WalkToRoot().Select(c => c.Position).Aggregate((p1, p2) => p1 + p2) + p;
        }

        /// <summary>
        /// Return this element, this elements parent, and so on until the root element is reached.
        /// </summary>
        /// <returns>This element, every element between this element and the root element, and the root element</returns>
        public IEnumerable<UIElement> WalkToRoot()
        {
            UIElement ancestor = this;
            yield return ancestor;
            while (ancestor.Parent != null)
            {
                ancestor = ancestor.Parent;
                yield return ancestor;
            }
        }

        /// <summary>
        /// Return all the decendants of this element.
        /// </summary>
        /// <returns>All decendants of this element.</returns>
        public IEnumerable<UIElement> GetDecendants()
        {
            foreach (var child in Children)
            {
                yield return child;
            }

            foreach (var child in Children)
            {
                foreach (var subChild in child.GetDecendants())
                {
                    yield return subChild;
                }
            }
        }

        /// <summary>
        /// Walk through the children of the UIElement to check if one of them is in focus.
        /// </summary>
        /// <returns>Wether the UIElement has a child that is currently in focus.</returns>
        public bool HasChildInFocus()
        {
            return GetDecendants().Any(d => d.IsFocused);
        }
    }
}