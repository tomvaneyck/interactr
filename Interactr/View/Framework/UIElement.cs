using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        /// <summary>
        /// The UI element that currently has the keyboard focus.
        /// </summary>
        /// <remarks>
        /// The focused element is the first element to receive keyboard events.
        /// Use UIElement.Focus() to set an element as the focused element.
        /// </remarks>
        public static UIElement FocusedElement { get; private set; }

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
            set => _width.Value = value;
        }

        public IObservable<int> WidthChanged => _width.Changed;

        #endregion

        #region Height

        private readonly ReactiveProperty<int> _height = new ReactiveProperty<int>();

        public int Height
        {
            get => _height.Value;
            set => _height.Value = value;
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

        #region Input event observables

        private readonly Subject<MouseEventData> _mouseEventOccured = new Subject<MouseEventData>();
        public IObservable<MouseEventData> MouseEventOccured => _mouseEventOccured;

        private readonly Subject<KeyEventData> _keyEventOccurred = new Subject<KeyEventData>();
        public IObservable<KeyEventData> KeyEventOccurred => _keyEventOccurred;

        #endregion

        public bool CanBeFocused { get; protected set; } = true;

        public UIElement()
        {
            this.IsVisible = true;

            //Trigger AbsolutePositionChanged when this elements position or an ancestors position changes.
            //Don't fire the event if the position relative to the root doesn't change. (because the changes cancel out.)
            AbsolutePositionChanged = Observable.Merge(
                PositionChanged,
                ParentChanged.ObserveNested(parent => parent.AbsolutePositionChanged)
            ).Select(_ => GetPositionRelativeToRoot()).DistinctUntilChanged();

            this.PositionChanged.Subscribe(_ => Repaint());
            this.WidthChanged.Subscribe(_ => Repaint());
            this.HeightChanged.Subscribe(_ => Repaint());
            this.IsVisibleChanged.Subscribe(_ => Repaint());
            this.Children.OnDelete.Subscribe(_ => Repaint());
            this.Children.OnAdd.Subscribe(_ => Repaint());

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
            Children.OnDelete.Subscribe(e => { e.Element.Parent = null; });

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
        /// <returns>True if the event was handled by an element.</returns>
        public static bool HandleKeyEvent(KeyEventData eventData)
        {
            if (TunnelDownKeyEventPreview(eventData))
            {
                //Event was handled
                return true;
            }

            //Bubble up event from FocusedElement to root
            return FocusedElement.BubbleUpKeyEvent(eventData);
        }

        /// <summary>
        /// Take a key event, call OnKeyEventPreview on every element from the root down until an element returns true or FocusedElement is reached.
        /// </summary>
        /// <remarks>
        /// Only the ancestors of FocusedElement will receive the event.
        /// </remarks>
        /// <param name="eventData">Details about this event.</param>
        /// <returns>True if the event was handled by an element</returns>
        private static bool TunnelDownKeyEventPreview(KeyEventData eventData)
        {
            foreach (UIElement element in UIElement.FocusedElement.WalkToRoot().Reverse())
            {
                if (element.OnKeyEventPreview(eventData))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Take a key event, call OnKeyEvent and pass the event to the parent element until an element handles it.
        /// </summary>
        /// <param name="eventData">Details about this event.</param>
        /// <returns>True if this element or one of its ancestors has handled the event</returns>
        private bool BubbleUpKeyEvent(KeyEventData eventData)
        {
            _keyEventOccurred.OnNext(eventData);
            if (this.OnKeyEvent(eventData))
            {
                return true;
            }

            return Parent?.BubbleUpKeyEvent(eventData) ?? false;
        }

        /// <summary>
        /// Is called when keyboard events are triggered.
        /// Can be overriden to intercept key events before they reach the FocusedElement.
        /// Return true to indicate that the event has been handled and should not be pushed to more ui elements.
        /// </summary>
        /// <param name="eventData">Details about this event.</param>
        /// <returns>True if this element has handled the event</returns>
        protected virtual bool OnKeyEventPreview(KeyEventData eventData)
        {
            return false;
        }

        /// <summary>
        /// Is called when keyboard events are triggered.
        /// Can be overridden to handle keyboard events.
        /// Return true to indicate that the event has been handled and should not be pushed to more ui elements.
        /// </summary>
        /// <param name="eventData">Details about this event.</param>
        /// <returns>True if this element has handled the event</returns>
        protected virtual bool OnKeyEvent(KeyEventData eventData)
        {
            return false;
        }

        #endregion

        #region Mouse events

        /// <summary>
        /// Emit a mouse event.
        /// </summary>
        /// <param name="rootElement">The element at the top of the view-tree</param>
        /// <param name="eventData">Details about this event. Should be relative to rootElement.</param>
        /// <returns>True if the event was handled by an element.</returns>
        public static bool HandleMouseEvent(UIElement rootElement, MouseEventData eventData)
        {
            UIElement mouseoverElement = rootElement.FindElementAt(eventData.MousePosition);

            if (TunnelDownMouseEventPreview(rootElement, mouseoverElement, eventData))
            {
                // Event was handled.
                return true;
            }

            // Bubble up event from FocusedElement to root.
            Point relativeMousePos = rootElement.TranslatePointTo(mouseoverElement, eventData.MousePosition);
            return mouseoverElement.BubbleUpMouseEvent(new MouseEventData(eventData.Id, relativeMousePos,
                eventData.ClickCount));
        }

        /// <summary>
        /// Take a mouse event, call OnMouseEventPreview on every element from the root down until an element returns true or mouseover-element is reached.
        /// Only the ancestors of the mouseover-element will receive the event.
        /// </summary>
        /// <param name="rootElement">The element at the top of the view-tree.</param>
        /// <param name="mouseoverElement">The element the mouse is over.</param>
        /// <param name="eventData">Details about this event. Should be relative to rootElement.</param>
        /// <returns>True if the event was handled by an element.</returns>
        private static bool TunnelDownMouseEventPreview(UIElement rootElement, UIElement mouseoverElement,
            MouseEventData eventData)
        {
            foreach (UIElement element in mouseoverElement.WalkToRoot().Reverse())
            {
                Point relativeMousePos = rootElement.TranslatePointTo(element, eventData.MousePosition);
                if (element.OnMouseEventPreview(
                    new MouseEventData(eventData.Id, relativeMousePos, eventData.ClickCount)))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Take a mouse event, call OnMouseEvent and keep passing the event to the parent element until an element handles it.
        /// </summary>
        /// <param name="eventData">Details about this event. Should be relative to this element.</param>
        /// <returns>True if the event was handled by an element.</returns>
        private bool BubbleUpMouseEvent(MouseEventData eventData)
        {
            _mouseEventOccured.OnNext(eventData);
            if (this.OnMouseEvent(eventData))
            {
                return true;
            }

            if (Parent == null)
            {
                return false;
            }

            Point relativeMousePos = this.TranslatePointTo(Parent, eventData.MousePosition);
            return Parent.BubbleUpMouseEvent(new MouseEventData(eventData.Id, relativeMousePos, eventData.ClickCount));
        }

        /// <summary>
        /// Is called when mouse events are triggered.
        /// Override this method to intercept mouse events before they reach the element the mouse is over.
        /// Return true to indicate that the event has been handled and should not be pushed to more ui elements.
        /// </summary>
        /// <param name="eventData">Details about this event. Should be relative to this element.</param>
        /// <returns>True if this element has handled the event</returns>
        protected virtual bool OnMouseEventPreview(MouseEventData eventData)
        {
            return false;
        }

        /// <summary>
        /// Is called when mouse events are triggered.
        /// Can be overriden to handle mouse events.
        /// Return true to indicate that the event has been handled and should not be pushed to more ui elements.
        /// </summary>
        /// <param name="eventData">Details about this event. Should be relative to this element.</param>
        /// <returns>True if this element has handled the event</returns>
        protected virtual bool OnMouseEvent(MouseEventData eventData)
        {
            // Only focus on mouseclick.
            if (eventData.Id == MouseEvent.MOUSE_PRESSED && CanBeFocused)
            {
                this.Focus();
                return true;
            }

            return false;
        }

        #endregion

        #region Painting

        /// <summary>
        /// Paint this element and its children.
        /// </summary>
        /// <param name="g">The graphics object</param>
        public void Paint(Graphics g)
        {
            if (IsVisible)
            {
                ValidateLayout();
                PaintElement(g);
                PaintChildren(g);
            }
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
        /// Draw the children of this element
        /// </summary>
        private void PaintChildren(Graphics g)
        {
            // Render first to last, so last element is on top
            foreach (UIElement child in Children)
            {
                // Save current transform and clip
                Matrix currentTransform = g.Transform;
                Region currentClip = g.Clip;

                // Map x, y coordinates to child space (relative to child origin)
                g.TranslateTransform(child.Position.X, child.Position.Y);
                g.SetClip(new RectangleF(0, 0, child.Width, child.Height));

                // Paint
                child.Paint(g);

                // Reset clip and transform
                g.Transform = currentTransform;
                g.Clip = currentClip;
            }
        }

        protected void ValidateLayout()
        {
            foreach (UIElement child in Children)
            {
                int availableWidth = this.Width - child.Position.X;
                if (availableWidth < child.Width)
                {
                    child.Width = availableWidth;
                }

                int availableHeight = this.Height - child.Position.Y;
                if (availableHeight < child.Height)
                {
                    child.Height = availableHeight;
                }
            }
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
            var childContainingPoint = Children.FirstOrDefault(child =>
                child.IsVisible &&
                point.X >= child.Position.X && point.Y >= child.Position.Y &&
                point.X < (child.Position.X + child.Width) &&
                point.Y < (child.Position.Y + child.Height));

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
        public ReactiveList<UIElement> GetDecendants()
        {
            ReactiveList<UIElement> result = new ReactiveArrayList<UIElement>();
            result.OnAdd.Subscribe(child => result.AddRange(child.Element.Children));
            result.AddRange(Children);

            return result;
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