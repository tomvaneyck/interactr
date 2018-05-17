using System;
using System.Drawing;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Forms;
using Interactr.Constants;
using Interactr.Reactive;
using Interactr.View.Framework;
using Interactr.Window;

namespace Interactr.View.Controls
{
    /// <summary>
    /// A view for displaying and editing the text label.
    /// </summary>
    public class LabelView : UIElement
    {
        public static AttachedProperty<LabelView> LabelBeingEdited { get; } = new AttachedProperty<LabelView>(null);

        #region Text

        private readonly ReactiveProperty<string> _text = new ReactiveProperty<string>();

        /// <summary>
        /// The text of the label.
        /// </summary>
        public string Text
        {
            get => _text.Value;
            set => _text.Value = value;
        }

        /// <summary>
        /// Emit the new text when the text changes.
        /// </summary>
        public IObservable<string> TextChanged => _text.Changed;

        #endregion

        #region Font

        private readonly ReactiveProperty<Font> _font = new ReactiveProperty<Font>();

        /// <summary>
        /// The font for displaying the label text.
        /// </summary>
        public Font Font
        {
            get => _font.Value;
            set => _font.Value = value;
        }

        /// <summary>
        /// Emit the new Font when the font changes.
        /// </summary>
        public IObservable<Font> FontChanged => _font.Changed;

        #endregion

        #region Color

        private readonly ReactiveProperty<Color> _color = new ReactiveProperty<Color>();

        /// <summary>
        /// The color of the label text.
        /// </summary>
        public Color Color
        {
            get => _color.Value;
            set => _color.Value = value;
        }

        /// <summary>
        /// Emit the new Color when it changes.
        /// </summary>
        public IObservable<Color> ColorChanged => _color.Changed;

        #endregion

        #region IsInEditMode

        private readonly ReactiveProperty<bool> _isInEditMode = new ReactiveProperty<bool>();

        /// <summary>
        /// Indicate if the label is in edit mode.
        /// </summary>
        public bool IsInEditMode
        {
            get => _isInEditMode.Value;
            set => _isInEditMode.Value = value;
        }

        /// <summary>
        /// Emit the new editMode when edit mode changes.
        /// </summary>
        public IObservable<bool> EditModeChanged => _isInEditMode.Changed;

        #endregion

        #region CanLeaveEditMode

        private readonly ReactiveProperty<bool> _canLeaveEditMode = new ReactiveProperty<bool>();

        /// <summary>
        /// Indicate if leaving edit mode is allowed.
        /// </summary>
        public bool CanLeaveEditMode
        {
            get => _canLeaveEditMode.Value;
            set => _canLeaveEditMode.Value = value;
        }

        /// <summary>
        /// Emit the new canLeaceEditMode value when it changes.
        /// </summary>
        public IObservable<bool> CanLeaveEditModeChanged => _canLeaveEditMode.Changed;

        #endregion

        #region CursorIsVisible

        private bool _cursorIsVisible;

        #endregion

        private bool _isFocusing;

        public LabelView()
        {
            // Set the text to empty string
            Text = "";

            // Set the default font.
            Font = new Font("Arial", 11);
            Color = Color.Black;

            // When a property changes, repaint.
            ReactiveExtensions.MergeEvents(
                TextChanged,
                FontChanged,
                ColorChanged
            ).Subscribe(_ => Repaint());

            // Set the preferred width and height of the labelView by measuring
            // how much space it would take to fully render the string.
            TextChanged.Subscribe(text =>
            {
                PreferredWidth = TextRenderer.MeasureText(text, Font).Width;
                PreferredHeight = TextRenderer.MeasureText(text, Font).Height;
            });

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
                    // Repainting to get rid of cursor.
                    Repaint();
                    return Observable.Empty<long>();
                }
            }).Switch().Subscribe(_ =>
            {
                _cursorIsVisible = !_cursorIsVisible;
                Repaint();
            });

            // Set attached property on window when edit mode changed.
            EditModeChanged.Subscribe(editMode =>
            {
                WindowsView.Window window = WalkToRoot().OfType<WindowsView.Window>().FirstOrDefault();

                if (window != null)
                {
                    if (editMode)
                    {
                        LabelBeingEdited.SetValue(window, this);
                    }
                    else
                    {
                        LabelBeingEdited.SetValue(window, null);
                    }
                }
            });

            // Exit edit mode when focus is lost and the label can leave edit mode.
            FocusChanged.Where(_ => !_).Subscribe(_ =>
            {
                if (CanLeaveEditMode)
                {
                    IsInEditMode = false;
                }
            });

            // Ignore mouse clicked when just received focus.
            FocusChanged.Where(v => v).Subscribe(_ => _isFocusing = true);

            CanLeaveEditMode = true;
        }

        /// <see cref="PaintElement"/>
        public override void PaintElement(Graphics g)
        {
            // Draw the string.
            using (Brush brush = new SolidBrush(Color))
            {
                g.DrawString(Text, Font, brush, 0, 0);
            }

            using (Pen pen = new Pen(Color))
            {
                // Draw editing rectangle.
                if (IsInEditMode)
                {
                    using (Pen borderPen = new Pen(Color.DodgerBlue))
                    {
                        g.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);
                    }
                
                    // Draw cursor.
                    if (_cursorIsVisible)
                    {
                        g.DrawLine(pen, PreferredWidth - 5, 0, PreferredWidth - 5, PreferredHeight);
                    }
                }
                // Draw focusing rectangle.
                else if (IsFocused)
                {
                    g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
                }
            }
        }

        /// <see cref="OnKeyEvent"/>
        protected override void OnKeyEvent(KeyEventData eventData)
        {
            if (IsInEditMode)
            {
                if (eventData.Id == KeyEvent.KEY_RELEASED &&
                    eventData.KeyCode == KeyEvent.VK_ESCAPE &&
                    CanLeaveEditMode)
                {
                    IsInEditMode = false;
                }
                else if (eventData.Id == KeyEvent.KEY_TYPED)
                {
                    // If the keyChar is backspace.
                    if (eventData.KeyChar == HexaDecimalKeyChars.BackSpace)
                    {
                        if (Text.Length > 0)
                        {
                            Text = Text.Substring(0, Text.Length - 1);
                        }
                    }
                    // If Keychar is not escape.
                    else if (char.IsLetterOrDigit(eventData.KeyChar) || eventData.KeyChar == HexaDecimalKeyChars.Colon)
                    {
                        Text += eventData.KeyChar;
                    }
                }

                // Cancel event propagation.
                eventData.IsHandled = true;
            }
        }

        /// <see cref="OnMouseEvent"/>
        protected override void OnMouseEvent(MouseEventData eventData)
        {
            if (_isFocusing && eventData.Id == MouseEvent.MOUSE_CLICKED)
            {
                _isFocusing = false;
                eventData.IsHandled = true;
                return;
            }
            if (IsFocused && eventData.Id == MouseEvent.MOUSE_CLICKED)
            {
                IsInEditMode = true;
                eventData.IsHandled = true;
                return;
            }

            base.OnMouseEvent(eventData);
        }
    }
}