using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Interactr.Window
{
    /// <summary>
    /// A window for custom drawing.
    /// 
    /// To use this class, create a subclass, say MyCanvasWindow, that overrides methods
    /// <see cref="Paint"/>, <see cref="HandleMouseEvent"/>, and <see cref="HandleKeyEvent"/> 
    /// and then launch it from your main method as follows:
    /// 
    /// <code>
    /// [STAThread]
    /// static void Main()
    /// {
    ///     Application.EnableVisualStyles();
    ///     Application.SetCompatibleTextRenderingDefault(false);
    ///     new CanvasWindow("My Canvas Window").Show();
    /// }
    /// </code>
    /// </summary>
    public class CanvasWindow
    {
        public string Title { get; set; }

        public Size Size
        {
            get => _form.Size;
            set => _form.Size = value;
        }

        private readonly Form _form = new Form();
        private CanvasWindowRecording _recording;
        private string _recordingPath;
        private InputMapper _inputMapper;

        private readonly Panel _contentPanel = new DoubleBufferPanel();

        /// <summary>
        /// Initializes a CanvasWindow object.
        /// </summary>
        /// <param name="title">Window title</param>
        public CanvasWindow(string title)
        {
            Title = title;

            _contentPanel.Dock = DockStyle.Fill;
            _form.Controls.Add(_contentPanel);

            SetupFormEvents();
        }

        private void SetupFormEvents()
        {
            _form.BackColor = Color.White;
            _form.Closed += (sender, args) => _recording?.Save(_recordingPath);
            _contentPanel.Paint += (sender, args) =>
            {
                if (_recording == null)
                {
                    Paint(args.Graphics);
                }
                else
                {
                    Bitmap img = CaptureImage();
                    args.Graphics.DrawImage(img, Point.Empty);
                    _recording.Items.Add(new PaintItem(img));
                    UpdateFrameTitle();
                }
            };

            //Input events
            _form.KeyPreview = true;
            _inputMapper = new InputMapper(_form, _contentPanel);
            _inputMapper.OnMouseEvent += (s, e) =>
            {
                _recording?.Items.Add(e);
                HandleMouseEvent(e.Id, e.X, e.Y, e.ClickCount);
            };

            _inputMapper.OnKeyEvent += (s, e) =>
            {
                _recording?.Items.Add(e);
                HandleKeyEvent(e.Id, e.KeyCode, e.KeyChar);
            };
        }

        void UpdateFrameTitle()
        {
            _form.Text = _recording == null ? Title : Title + $" - Recording: {_recording.Items.Count} items recorded";
        }

        public void RecordSession(string path)
        {
            _recording = new CanvasWindowRecording();
            _recordingPath = path;
        }

        /// <summary>
        /// Call this method if the canvas is out of date and needs to be repainted.
        /// This function will invoke <see cref="Paint"/>, unless <see cref="Show"/> has not been called yet, or the window has been closed.
        /// The invocation of <see cref="Paint"/> will occur on the UI thread, regardless of which thread calls this function.
        /// The repaint is always handled synchronously. (This function returns after the repaint has occured.)
        /// </summary>
        public void Repaint()
        {
            if (_form.IsHandleCreated)
            {
                if (_form.InvokeRequired)
                {
                    _form.Invoke((Action)(() => _form.Refresh()));
                }
                else
                {
                    _form.Refresh();
                }
            }
        }

        /// <summary>
        /// Called to allow you to paint on the canvas.
        /// You should not use the Graphics object after you return from this method.
        /// </summary>
        /// <param name="g">This object offers the methods that allow you to paint on the canvas.</param>
        public virtual void Paint(Graphics g)
        {}

        /// <summary>
        /// Called when the user presses (id == MouseEvent.MOUSE_PRESSED), releases (id == MouseEvent.MOUSE_RELEASED), or drags (id == MouseEvent.MOUSE_DRAGGED) the mouse.
        /// </summary>
        /// <param name="id">The type of event that occurred</param>
        /// <param name="x">The x-coordinate of the event</param>
        /// <param name="y">The y-coordinate of the event</param>
        /// <param name="clickCount">The amount of mouse button clicks</param>
        public virtual void HandleMouseEvent(int id, int x, int y, int clickCount)
        {
            String idStr = "";
            switch (id)
            {
                case MouseEvent.MOUSE_PRESSED: idStr = "MOUSE_PRESSED"; break;
                case MouseEvent.MOUSE_CLICKED: idStr = "MOUSE_CLICKED"; break;
                case MouseEvent.MOUSE_RELEASED: idStr = "MOUSE_RELEASED"; break;
                case MouseEvent.MOUSE_DRAGGED: idStr = "MOUSE_DRAGGED"; break;
            }
            Debug.WriteLine(idStr + " " + x + " " + y + " " + clickCount);
        }

        /// <summary>
        /// Called when the user presses a key (id == KeyEvent.KEY_PRESSED) or enters a character (id == KeyEvent.KEY_TYPED).
        /// </summary>
        /// <param name="id">The type of event that occurred</param>
        /// <param name="keyCode">The unique code for the keyboard button that triggered the event</param>
        /// <param name="keyChar">The text character associated with the the keyboard button</param>
        public virtual void HandleKeyEvent(int id, int keyCode, char keyChar)
        {
            String idStr = "";
            switch (id)
            {
                case KeyEvent.KEY_PRESSED: idStr = "KEY_PRESSED"; break;
                case KeyEvent.KEY_TYPED: idStr = "KEY_TYPED"; break;
            }
            Debug.WriteLine($"{idStr} {keyCode} {(int)keyChar}");
        }

        public Bitmap CaptureImage()
        {
            Bitmap bitmap = new Bitmap(_form.Width, _form.Height, PixelFormat.Format32bppArgb);
            _form.DrawToBitmap(bitmap, new Rectangle(0, 0, _form.Width, _form.Height));
            return bitmap;
        }

        public void Show()
        {
            UpdateFrameTitle();
            Application.Run(_form);
        }

        public static void ReplayRecording(string path, CanvasWindow window)
        {
            new CanvasWindowRecording(path).Replay(window);
        }
    }

    class DoubleBufferPanel : Panel
    {
        public DoubleBufferPanel()
        {
            this.DoubleBuffered = true;
        }
    }
}
