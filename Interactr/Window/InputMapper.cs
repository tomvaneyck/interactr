using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Interactr;

namespace Interactr.Window
{
    /// <summary>
    /// This class maps Windows Forms input events onto CanvasWindow events.
    /// 
    /// Known issues:
    ///  - A KEY_PRESSED event sometimes has the wrong character due to a simplified KeyToChar implementation
    /// </summary>
    public class InputMapper
    {
        public event EventHandler<MouseEventItem> OnMouseEvent;
        public event EventHandler<KeyEventItem> OnKeyEvent;
        
        private bool _isMouseDown;
        private bool _isDragging;
        private int _clicks;
        private Stopwatch _stopwatch = new Stopwatch();
        private Point _lastClickPos;

        public InputMapper(Form form, Panel panel)
        {
            _stopwatch.Start();

            panel.MouseDown += (sender, args) =>
            {
                _isMouseDown = true;
                bool inDoubleClickInterval = _stopwatch.ElapsedMilliseconds < SystemInformation.DoubleClickTime;
                bool inDoubleClickDistance = Math.Abs(_lastClickPos.X - args.X) < SystemInformation.DoubleClickSize.Width &&
                                             Math.Abs(_lastClickPos.Y - args.Y) < SystemInformation.DoubleClickSize.Height;
                if (inDoubleClickInterval && inDoubleClickDistance)
                {
                    _clicks++;
                }
                else
                {
                    _clicks = 1;
                }
                _stopwatch.Restart();
                _lastClickPos = args.Location;
                OnMouseEvent?.Invoke(sender, new MouseEventItem(MouseEvent.MOUSE_PRESSED, args.X, args.Y, _clicks));
            };
            panel.MouseUp += (sender, args) =>
            {
                OnMouseEvent?.Invoke(sender, new MouseEventItem(MouseEvent.MOUSE_RELEASED, args.X, args.Y, _clicks));
                if (!_isDragging)
                {
                    OnMouseEvent?.Invoke(sender, new MouseEventItem(MouseEvent.MOUSE_CLICKED, args.X, args.Y, _clicks));
                }
                _isMouseDown = false;
                _isDragging = false;
            };
            panel.MouseMove += (sender, args) =>
            {
                if (_isMouseDown)
                {
                    _isDragging = true;
                    OnMouseEvent?.Invoke(sender, new MouseEventItem(MouseEvent.MOUSE_DRAGGED, args.X, args.Y, 0));
                }
            };
            form.KeyDown += (sender, args) =>
            {
                OnKeyEvent?.Invoke(sender, new KeyEventItem(KeyEvent.KEY_PRESSED, (int)args.KeyCode, KeyToChar(args)));
            };
            form.KeyPress += (sender, args) =>
            {
                OnKeyEvent?.Invoke(sender, new KeyEventItem(KeyEvent.KEY_TYPED, KeyEvent.VK_UNDEFINED, args.KeyChar));
            };
            form.KeyUp += (sender, args) =>
            {
                OnKeyEvent?.Invoke(sender, new KeyEventItem(KeyEvent.KEY_RELEASED, (int)args.KeyCode, KeyToChar(args)));
            };
        }

        private readonly Dictionary<Keys, char> _alphanumericKeyMap = new Dictionary<Keys, char>
        {
            {Keys.A, 'A'},
            {Keys.B, 'B'},
            {Keys.C, 'C'},
            {Keys.D, 'D'},
            {Keys.E, 'E'},
            {Keys.F, 'F'},
            {Keys.G, 'G'},
            {Keys.H, 'H'},
            {Keys.I, 'I'},
            {Keys.J, 'J'},
            {Keys.K, 'K'},
            {Keys.L, 'L'},
            {Keys.M, 'M'},
            {Keys.N, 'N'},
            {Keys.O, 'O'},
            {Keys.P, 'P'},
            {Keys.Q, 'Q'},
            {Keys.R, 'R'},
            {Keys.S, 'S'},
            {Keys.T, 'T'},
            {Keys.U, 'U'},
            {Keys.V, 'V'},
            {Keys.W, 'W'},
            {Keys.X, 'X'},
            {Keys.Y, 'Y'},
            {Keys.Z, 'Z'},
            {Keys.D0, '0'},
            {Keys.D1, '1'},
            {Keys.D2, '2'},
            {Keys.D3, '3'},
            {Keys.D4, '4'},
            {Keys.D5, '5'},
            {Keys.D6, '6'},
            {Keys.D7, '7'},
            {Keys.D8, '8'},
            {Keys.D9, '9'}
        };

        private readonly Dictionary<Keys, char> _numpadKeyMap = new Dictionary<Keys, char>
        {
            {Keys.NumPad0, '0'},
            {Keys.NumPad1, '1'},
            {Keys.NumPad2, '2'},
            {Keys.NumPad3, '3'},
            {Keys.NumPad4, '4'},
            {Keys.NumPad5, '5'},
            {Keys.NumPad6, '6'},
            {Keys.NumPad7, '7'},
            {Keys.NumPad8, '8'},
            {Keys.NumPad9, '9'}
        };

        // Does not match 100% due to keyboard layout irregularities, but works well enough for simple alphanumerical keys
        private char KeyToChar(KeyEventArgs e)
        {
            char c;
            if (Control.IsKeyLocked(Keys.NumLock) && _numpadKeyMap.TryGetValue(e.KeyCode, out c))
            {
                return c;
            }

            if (_alphanumericKeyMap.TryGetValue(e.KeyCode, out c))
            {
                bool uppercase = e.Shift || Control.IsKeyLocked(Keys.CapsLock);
                c = uppercase ? Char.ToUpper(c) : Char.ToLower(c);
                return c;
            }

            return (char)KeyEvent.CHAR_UNDEFINED;
        }
    }
}
