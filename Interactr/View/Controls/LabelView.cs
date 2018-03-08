using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Interactr.Reactive;
using Interactr.View.Framework;
using Interactr.Window;

namespace Interactr.View.Controls
{
    /// <summary>
    /// A view for displaying and aediting the text label.
    /// </summary>
    public class LabelView : UIElement
    {
        #region Text

        private readonly ReactiveProperty<string> _text = new ReactiveProperty<string>();

        public string Text
        {
            get => _text.Value;
            set => _text.Value = value;
        }

        public IObservable<string> TextChanged => _text.Changed;

        #endregion

        #region Font

        private readonly ReactiveProperty<Font> _font = new ReactiveProperty<Font>();

        public Font Font
        {
            get => _font.Value;
            set => _font.Value = value;
        }

        public IObservable<Font> FontChanged => _font.Changed;

        #endregion

        #region IsInEditMode

        private readonly ReactiveProperty<bool> _isInEditMode = new ReactiveProperty<bool>();

        public bool IsInEditMode
        {
            get => _isInEditMode.Value;
            set => _isInEditMode.Value = value;
        }

        public IObservable<bool> EditModeChanged => _isInEditMode.Changed;

        #endregion

        #region CanLeaveEditMode

        private readonly ReactiveProperty<bool> _canLeaveEditMode = new ReactiveProperty<bool>();

        public bool CanLeaveEditMode
        {
            get => _canLeaveEditMode.Value;
            set => _canLeaveEditMode.Value = value;
        }

        public IObservable<bool> CanLeaveEditModeChanged => _canLeaveEditMode.Changed;

        #endregion

        #region CursorIsVisible

        private bool _cursorIsVisible;

        #endregion

        public LabelView()
        {
            // Set the text to empty string
            Text = "";
            
            // Set the default font.
            Font = new Font("Arial", 11);

            // When a property changes, repaint.
            TextChanged.Select(_ => Unit.Default).Merge(
                FontChanged.Select(_ => Unit.Default)
            ).Subscribe(_ => Repaint());

            // Blink cursor if label is in edit mode.
            EditModeChanged.Select(editMode =>
            {
                if (editMode)
                {
                    return Observable.Interval(TimeSpan.FromMilliseconds(SystemInformation.CaretBlinkTime))
                        .StartWith(0);
                }
                else
                {
                    _cursorIsVisible = false;
                    return Observable.Empty<long>();
                }
            }).Switch().Subscribe(_ =>
            {
                _cursorIsVisible = !_cursorIsVisible;
                Repaint();
            });

            // Leave edit mode if focus is lost and Repaint.
            FocusChanged.Subscribe(isFocused =>
            {
                if (!isFocused)
                {
                    IsInEditMode = false;
                }

                Repaint();
            });

            // Update canLoseFocus when the CanLeaveEditMode is changed.
            CanLeaveEditModeChanged.Subscribe(canLoseFocus => CanLoseFocus = canLoseFocus);
            CanLeaveEditMode = true;
        }

        /// <inheritdoc cref="PaintElement"/>
        public override void PaintElement(Graphics g)
        {
            // Measure how much space it would take to fully render the
            // the string. Must be done in this function because it requires
            // a Graphics object.
            var preferredSize = g.MeasureString(Text, Font);
            PreferredWidth = (int) Math.Ceiling(preferredSize.Width);
            PreferredHeight = (int) Math.Ceiling(preferredSize.Height);

            if (IsFocused)
            {
                g.DrawRectangle(Pens.Black, 0, 0, Width - 1, Height - 1);
            }

            // Draw the string.
            g.DrawString(Text, Font, Brushes.Black, 0, 0);

            // Draw cursor.
            if (_cursorIsVisible)
            {
                g.DrawLine(Pens.Black, PreferredWidth - 5, 0, PreferredWidth - 5, PreferredHeight);
            }
        }

        /// <inheritdoc cref="OnKeyEvent"/>
        protected override bool OnKeyEvent(KeyEventData eventData)
        {
            if (eventData.KeyCode == KeyEvent.VK_ESCAPE)
            {
                if (eventData.Id == KeyEvent.KEY_RELEASED && CanLeaveEditMode)
                {
                    IsInEditMode = false;
                }
                return true;
            }
            else if (eventData.Id == KeyEvent.KEY_TYPED && IsInEditMode)
            {
                // If the keyChar is backspace.
                if (eventData.KeyChar == '\b')
                {
                    if (Text.Length > 0)
                    {
                        Text = Text.Substring(0, Text.Length - 1);
                    }
                }
                // If Keychar is not escape.
                else if (eventData.KeyChar != '\x1b')
                {
                    Text += eventData.KeyChar;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc cref="OnMouseEvent"/>
        protected override bool OnMouseEvent(MouseEventData eventData)
        {
            if (IsFocused && eventData.Id == MouseEvent.MOUSE_CLICKED)
            {
                IsInEditMode = true;
                return true;
            }

            return base.OnMouseEvent(eventData);
        }
    }
}