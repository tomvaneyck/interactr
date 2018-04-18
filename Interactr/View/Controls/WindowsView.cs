using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Interactr.Reactive;
using Interactr.View.Framework;
using Interactr.Window;
using Point = Interactr.View.Framework.Point;

namespace Interactr.View.Controls
{
    /// <summary>
    /// A layout UI element with draggable, resizable subwindows.
    /// </summary>
    public class WindowsView : DragPanel
    {
        #region BackgroundColor

        private readonly ReactiveProperty<Color> _backgroundColor = new ReactiveProperty<Color>();

        /// <summary>
        /// The color that is used to fill the background.
        /// </summary>
        public Color BackgroundColor
        {
            get => _backgroundColor.Value;
            set => _backgroundColor.Value = value;
        }

        public IObservable<Color> BackgroundColorChanged => _backgroundColor.Changed;

        #endregion

        private readonly Subject<Window> _windowCloseRequested = new Subject<Window>();
        public IObservable<Window> WindowCloseRequested => _windowCloseRequested;
        
        public WindowsView()
        {
            // Set default values.
            BackgroundColor = Color.FromArgb(0x33, 0x33, 0x33);

            // When a window (or a subchild) is clicked, bring the window to the front.
            FocusedElementChanged
                .Where(elem => elem != null)
                .Select(elem => elem.WalkToRoot().FirstOrDefault(n => n is Window))
                .Where(window => window != null)
                .Subscribe(window =>
                {
                    int curIndex = Children.IndexOf(window);
                    if (curIndex != Children.Count - 1)
                    {
                        Children.RemoveAt(curIndex);
                        Children.Add(window);
                    }
                });

            // When the close button of a window is clicked, notify WindowCloseRequested
            Children.ObserveWhere(window => ((Window)window).CloseButton.MouseEventOccured, elem => elem is Window).Subscribe(e =>
            {
                if (e.Value.Id == MouseEvent.MOUSE_RELEASED)
                {
                    _windowCloseRequested.OnNext((Window)e.Element);
                }
            });
        }

        public override void PaintElement(Graphics g)
        {
            // Draw background.
            using (Brush brush = new SolidBrush(BackgroundColor))
            {
                g.FillRectangle(brush, 0, 0, Width, Height);
            }
        }

        /// <summary>
        /// Add a new window to this view, with the specified element as its InnerElement.
        /// </summary>
        /// <param name="windowContents">The element to put inside the window.</param>
        /// <returns>The new window.</returns>
        public Window AddWindow(UIElement windowContents)
        {
            Window window = new Window(windowContents);
            // Give each window a different offset, like Windows does.
            int stacks = 7;
            int windowsPerStack = 10;
            int xOffset = 10;
            int yOffset = 10;
            window.Position = new Framework.Point(
                (Children.Count % (stacks * windowsPerStack)) * xOffset,
                (Children.Count % windowsPerStack) * yOffset
            );

            Children.Add(window);
            return window;
        }

        /// <summary>
        /// Remove the window that has the specified element as its InnerElement.
        /// </summary>
        /// <param name="elem">The element that is in the window.</param>
        /// <returns>True if a matching window was found and removed, else false.</returns>
        public bool RemoveWindowWith(UIElement elem)
        {
            Window window = Children.OfType<Window>().FirstOrDefault(w => w.InnerElement == elem);
            if (window == null)
            {
                return false;
            }

            Children.Remove(window);
            return true;
        }

        /// <summary>
        /// A window with a title and controls for resizing, closing...
        /// Used for the children of the WindowsView.
        /// </summary>
        public class Window : AnchorPanel
        {
            #region Title

            private readonly ReactiveProperty<String> _title = new ReactiveProperty<String>();
            
            /// <summary>
            /// The text that should be displayed in the titlebar of the window.
            /// </summary>
            public String Title
            {
                get => _title.Value;
                set => _title.Value = value;
            }

            public IObservable<String> TitleChanged => _title.Changed;

            #endregion
            
            /// <summary>
            /// The UIElement that is put inside this window
            /// </summary>
            public UIElement InnerElement { get; }

            /// <summary>
            /// The button to close the window.
            /// </summary>
            public Button CloseButton { get; }

            public Window(UIElement innerElement)
            {
                // Title
                Title = "New Window";
                TitleChanged.Subscribe(_ => Repaint());

                // Inner element
                InnerElement = innerElement;
                MarginsProperty.SetValue(InnerElement, new Margins(2, 30, 2, 2));
                Children.Add(innerElement);

                // Close button
                CloseButton = new Button
                {
                    LabelFont = new Font("Wingdings 2", 8, FontStyle.Bold),
                    Label = "\u00ce" //X symbol
                };
                AnchorsProperty.SetValue(CloseButton, Anchors.Top | Anchors.Right);
                MarginsProperty.SetValue(CloseButton, new Margins(top: 7, right: 7, bottom: 7));
                this.Children.Add(CloseButton);

                //Focus child if this window is clicked.
                this.FocusChanged.Subscribe(focused =>
                {
                    if (focused && InnerElement.CanBeFocused)
                    {
                        InnerElement.Focus();
                    }
                });
            }

            [Flags]
            private enum ResizeMode
            {
                None = 0,
                Left = 1,
                Top = 2,
                Right = 4,
                Bottom = 8
            }

            private ResizeMode _resizeMode;
            protected override bool OnMouseEvent(MouseEventData eventData)
            {
                if (eventData.Id == MouseEvent.MOUSE_PRESSED)
                {
                    _resizeMode = ResizeMode.None;
                    if (eventData.MousePosition.X < 5)
                    {
                        _resizeMode |= ResizeMode.Left;
                    }
		    else if (eventData.MousePosition.X > Width - 5)
                    {
                        _resizeMode |= ResizeMode.Right;
                    }

                    if (eventData.MousePosition.Y < 5)
                    {
                        _resizeMode |= ResizeMode.Top;
                    }
		    else if (eventData.MousePosition.Y > Height - 5)
                    {
                        _resizeMode |= ResizeMode.Bottom;
                    }

                    if (_resizeMode != ResizeMode.None)
                    {
                        CaptureMouse();
                        return true;
                    }
                }
                else if (eventData.Id == MouseEvent.MOUSE_RELEASED && _resizeMode != ResizeMode.None)
                {
                    ReleaseMouseCapture();
                    return true;
                }
                else if(_resizeMode != ResizeMode.None)
                {
                    if ((_resizeMode & ResizeMode.Left) != 0)
                    {
                        this.Width -= eventData.MousePosition.X;
                        this.Position += new Point(eventData.MousePosition.X, 0);
                    }
                    if ((_resizeMode & ResizeMode.Top) != 0)
                    {
                        this.Height -= eventData.MousePosition.Y;
                        this.Position += new Point(0, eventData.MousePosition.Y);
                    }
                    if ((_resizeMode & ResizeMode.Right) != 0)
                    {
                        this.Width = eventData.MousePosition.X;
                    }
                    if ((_resizeMode & ResizeMode.Bottom) != 0)
                    {
                        this.Height = eventData.MousePosition.Y;
                    }

                    return true;
                }

                return base.OnMouseEvent(eventData);
            }

            public override void PaintElement(Graphics g)
            {
                // Draw background.
                using (Brush brush = new SolidBrush(Color.FromArgb(195, 199, 203)))
                {
                    g.FillRectangle(brush, 0, 0, Width, Height);
                }

                // Draw borders.
                g.DrawLine(Pens.Black, Width - 1, 0, Width - 1, Height - 1);
                g.DrawLine(Pens.Black, 0, Height - 1, Width - 1, Height - 1);

                g.DrawLine(Pens.White, 1, 1, Width - 3, 1);
                g.DrawLine(Pens.White, 1, 1, 1, Height - 3);
                
                using (Pen pen = new Pen(Color.FromArgb(134, 138, 142)))
                {
                    g.DrawLine(pen, 1, Height - 2, Width - 2, Height - 2);
                    g.DrawLine(pen, Width - 2, 1, Width - 2, Height - 2);
                }

                using (Brush brush = new SolidBrush(Color.FromArgb(0, 0, 170)))
                {
                    g.FillRectangle(brush, 4, 4, Width-8, 18);
                }

                // Draw title.
                g.DrawString(Title, new Font("Arial", 9f, FontStyle.Bold), Brushes.White, 5, 5);
            }
        }
    }
}
