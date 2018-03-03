using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Interactr.Reactive;

namespace Interactr.View.Controls
{
    /// <summary>
    /// Parent class for views.
    /// </summary>
    public class UIElement
    {
        /// <summary>
        /// The UI element that currently has the keyboard focus.
        /// The focused element is the first element to receive keyboard events.
        /// Use UIElement.Focus() to set an element as the focused element.
        /// </summary>
        public static UIElement FocusedElement { get; private set; }

        #region Children
        /// <summary>
        /// The child-elements of this element in the view-tree.
        /// </summary>
        public IList<UIElement> Children => _children;
        private readonly ReactiveList<UIElement> _children = new ReactiveList<UIElement>();
        #endregion

        /// <summary>
        /// The parent element of this element in the view-tree.
        /// </summary>
        public UIElement Parent { get; private set; }

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
        #endregion

        #region RepaintRequested
        private readonly Subject<Unit> _repaintRequested = new Subject<Unit>();
        public IObservable<Unit> RepaintRequested => _repaintRequested;
        #endregion

        public UIElement()
        {
            this.IsVisible = true;

            this.PositionChanged.Subscribe(_ => Repaint());
            this.WidthChanged.Subscribe(_ => Repaint());
            this.HeightChanged.Subscribe(_ => Repaint());
            this.IsVisibleChanged.Subscribe(_ => Repaint());

            SetupParentChildRelationship();
        }

        private void SetupParentChildRelationship()
        {
            //Set parent-child relationship on child add
            _children.OnAdd.Subscribe(newChild =>
            {
                if (newChild.Parent != null)
                {
                    throw new Exception("UIElement already has a parent.");
                }

                newChild.Parent = this;
            });

            //Remove parent-child relationship on child remove
            _children.OnDelete.Subscribe(child =>
            {
                child.Parent = null;
            });

            //When a child requests a repaint, pass the request upwards so the canvaswindow on top can do the redraw.
            _children.OnAdd.Subscribe(newChild =>
            {
                IDisposable subscription = newChild.RepaintRequested.Subscribe(_ => Repaint());
                //When the child is removed, also unsubscribe from its RepaintRequested observable
                _children.OnDelete.Where(c => c == newChild).Take(1).Subscribe(_ =>
                {
                    subscription.Dispose();
                });
            });
        }
        
        /// <summary>
        /// Marks this element as the focused element.
        /// See UIElement.FocusedElement for more information.
        /// </summary>
        public void Focus()
        {
            if (this == FocusedElement)
            {
                //this is already focused
                return;
            }

            //Previously focused element is now unfocused
            FocusedElement?._focusChanged.OnNext(false);

            FocusedElement = this;

            //This element is now focused
            this._focusChanged.OnNext(true);
        }

        #region Keyboard events
        /// <summary>
        /// Emits a keyboard event
        /// </summary>
        /// <param name="id">The type of key event</param>
        /// <param name="keyCode">The key identifier</param>
        /// <param name="keyChar">The character associated with this key</param>
        /// <returns>True if the event was handled by an element</returns>
        public static bool HandleKeyEvent(int id, int keyCode, char keyChar)
        {
            if (TunnelDownKeyEventPreview(id, keyCode, keyChar))
            {
                //Event was handled
                return true;
            }

            //Bubble up event from FocusedElement to root
            return FocusedElement.BubbleUpKeyEvent(id, keyCode, keyChar);
        }

        /// <summary>
        /// Takes a key event, calls OnKeyEventPreview on every element from the root down until an element returns true or FocusedElement is reached.
        /// Only the ancestors of FocusedElement will receive the event.
        /// </summary>
        /// <param name="id">The type of key event</param>
        /// <param name="keyCode">The key identifier</param>
        /// <param name="keyChar">The character associated with this key</param>
        /// <returns>True if the event was handled by an element</returns>
        private static bool TunnelDownKeyEventPreview(int id, int keyCode, char keyChar)
        {
            foreach (UIElement element in UIElement.FocusedElement.WalkToRoot().Reverse())
            {
                if (element.OnKeyEventPreview(id, keyCode, keyChar))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Takes a key event, calls OnKeyEvent and passes the event to the parent element until an element handles it.
        /// </summary>
        /// <param name="id">The type of key event</param>
        /// <param name="keyCode">The key identifier</param>
        /// <param name="keyChar">The character associated with this key</param>
        /// <returns>True if this element or one of its ancestors has handled the event</returns>
        private bool BubbleUpKeyEvent(int id, int keyCode, char keyChar)
        {
            if (this.OnKeyEvent(id, keyCode, keyChar))
            {
                return true;
            }

            return Parent?.BubbleUpKeyEvent(id, keyCode, keyChar) ?? false;
        }

        /// <summary>
        /// This function is called when keyboard events are triggered.
        /// Use this function to intercept key events before they reach the FocusedElement.
        /// Return true to indicate that the event has been handled and should not be pushed to more ui elements.
        /// </summary>
        /// <param name="id">The type of key event</param>
        /// <param name="keyCode">The key identifier</param>
        /// <param name="keyChar">The character associated with this key</param>
        /// <returns>True if this element has handled the event</returns>
        protected virtual bool OnKeyEventPreview(int id, int keyCode, char keyChar)
        {
            return false;
        }

        /// <summary>
        /// This function is called when keyboard events are triggered.
        /// Return true to indicate that the event has been handled and should not be pushed to more ui elements.
        /// </summary>
        /// <param name="id">The type of key event</param>
        /// <param name="keyCode">The key identifier</param>
        /// <param name="keyChar">The character associated with this key</param>
        /// <returns>True if this element has handled the event</returns>
        protected virtual bool OnKeyEvent(int id, int keyCode, char keyChar)
        {
            return false;
        }
        #endregion

        #region Mouse events
        /// <summary>
        /// Emits a mouse event
        /// </summary>
        /// <param name="rootElement">The element at the top of the view-tree</param>
        /// <param name="id">The type of mouse event</param>
        /// <param name="mousePos">The current position of the mouse relative to the root element</param>
        /// <param name="clickCount">Amount of times the mouse button was pressed</param>
        /// <returns>True if the event was handled by an element</returns>
        public static bool HandleMouseEvent(UIElement rootElement, int id, Point mousePos, int clickCount)
        {
            UIElement mouseoverElement = rootElement.FindElementAt(mousePos);

            if (TunnelDownMouseEventPreview(rootElement, mouseoverElement, id, mousePos, clickCount))
            {
                //Event was handled
                return true;
            }

            //Bubble up event from FocusedElement to root
            Point relativeMousePos = rootElement.TranslatePointTo(mouseoverElement, mousePos);
            return mouseoverElement.BubbleUpMouseEvent(id, mousePos, clickCount);
        }

        /// <summary>
        /// Takes a mouse event, calls OnMouseEventPreview on every element from the root down until an element returns true or mouseover-element is reached.
        /// Only the ancestors of the mouseover-element will receive the event.
        /// </summary>
        /// <param name="rootElement">The element at the top of the view-tree</param>
        /// <param name="mouseoverElement">The element the mouse is over</param>
        /// <param name="id">The type of mouse event</param>
        /// <param name="mousePos">The current position of the mouse relative to the root element</param>
        /// <param name="clickCount">Amount of times the mouse button was pressed</param>
        /// <returns>True if the event was handled by an element</returns>
        private static bool TunnelDownMouseEventPreview(UIElement rootElement, UIElement mouseoverElement, int id, Point mousePos, int clickCount)
        {
            foreach (UIElement element in mouseoverElement.WalkToRoot().Reverse())
            {
                Point relativeMousePos = rootElement.TranslatePointTo(element, mousePos);
                if (element.OnMouseEventPreview(id, relativeMousePos, clickCount))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Takes a mouse event, calls OnMouseEvent and passes the event to the parent element until an element handles it.
        /// </summary>
        /// <param name="id">The type of mouse event</param>
        /// <param name="mousePos">The current position of the mouse relative to the this element</param>
        /// <param name="clickCount">Amount of times the mouse button was pressed</param>
        /// <returns>True if the event was handled by an element</returns>
        private bool BubbleUpMouseEvent(int id, Point mousePos, int clickCount)
        {
            if (this.OnMouseEvent(id, mousePos, clickCount))
            {
                return true;
            }

            if (Parent == null)
            {
                return false;
            }

            Point relativeMousePos = this.TranslatePointTo(Parent, mousePos);
            return Parent.BubbleUpMouseEvent(id, relativeMousePos, clickCount);
        }

        /// <summary>
        /// This function is called when mouse events are triggered.
        /// Use this method to intercept mouse events before they reach the element the mouse is over.
        /// Return true to indicate that the event has been handled and should not be pushed to more ui elements.
        /// </summary>
        /// <param name="id">The type of mouse event</param>
        /// <param name="mousePos">The current position of the mouse relative to this element</param>
        /// <param name="clickCount">Amount of times the mouse button was pressed</param>
        /// <returns>True if this element has handled the event</returns>
        protected virtual bool OnMouseEventPreview(int id, Point mousePos, int clickCount)
        {
            return false;
        }

        /// <summary>
        /// This function is called when mouse events are triggered.
        /// Return true to indicate that the event has been handled and should not be pushed to more ui elements.
        /// </summary>
        /// <param name="id">The type of mouse event</param>
        /// <param name="mousePos">The current position of the mouse relative to this element</param>
        /// <param name="clickCount">Amount of times the mouse button was pressed</param>
        /// <returns>True if this element has handled the event</returns>
        protected virtual bool OnMouseEvent(int id, Point mousePos, int clickCount)
        {
            this.Focus();
            return true;
        }
        #endregion

        #region Painting
        /// <summary>
        /// Paints this element and its children.
        /// </summary>
        /// <param name="g">The graphics object</param>
        public void Paint(Graphics g)
        {
            if (IsVisible)
            {
                PaintElement(g);
                PaintChildren(g);
            }
        }

        public void Repaint()
        {
            _repaintRequested.OnNext(Unit.Default);
        }

        /// <summary>
        /// Draws the children of this element
        /// </summary>
        private void PaintChildren(Graphics g)
        {
            //Render first to last, so last element is on top
            foreach (UIElement child in Children)
            {
                //save current transform and clip
                Matrix currentTransform = g.Transform;
                Region currentClip = g.Clip;

                //map x, y coordinates to child space (relative to child origin)
                g.TranslateTransform(child.Position.X, child.Position.Y);
                g.SetClip(new RectangleF(0, 0, child.Width+1, child.Height+1));

                //paint
                child.Paint(g);

                //Reset clip and transform
                g.Transform = currentTransform;
                g.Clip = currentClip;
            }
        }

        /// <summary>
        /// This function is called to render this element to the screen.
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
            Point pRoot = RelativeToRoot(p);

            Point uiElementPosition = e.RelativeToRoot();

            return new Point(
                pRoot.X - uiElementPosition.X,
                pRoot.Y - uiElementPosition.Y
            );
        }

        /// <summary>
        /// Returns the UIElement that is visible at this point.
        /// This function searches downward in the tree for the bottom-most node that contains the specified point
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public UIElement FindElementAt(Point point)
        {
            var childContainingPoint = Children.FirstOrDefault(child => point.X >= child.Position.X && point.Y >= child.Position.Y &&
                                                                        point.X < (child.Position.X + child.Width) &&
                                                                        point.Y < (child.Position.Y + child.Height));
            if (childContainingPoint == null)
            {
                return this;
            }

            Point relativePosition = this.TranslatePointTo(childContainingPoint, point);
            return childContainingPoint.FindElementAt(relativePosition);
        }

        /// <summary>
        /// Return coördinates relative to the root of this UIElement.
        /// </summary>
        /// <returns>Given point relative to the root.</returns>
        private Point RelativeToRoot(Point p = default(Point))
        {
            return WalkToRoot().Select(c => c.Position).Aggregate((p1, p2) => p1 + p2) + p;
        }

        /// <summary>
        /// Returns this element, this elements parent, and so on until the root element is reached.
        /// </summary>
        /// <returns>This element, every element between this element and the root element, and the root element</returns>
        private IEnumerable<UIElement> WalkToRoot()
        {
            UIElement ancestor = this;
            yield return ancestor;
            while (ancestor.Parent != null)
            {
                ancestor = ancestor.Parent;
                yield return ancestor;
            }
        }
    }
}
