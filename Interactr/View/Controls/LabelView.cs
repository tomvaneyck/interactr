using System;
using System.Diagnostics;
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

        #region IsReadOnly

        private readonly ReactiveProperty<bool> _isReadOnly = new ReactiveProperty<bool>();

        /// <summary>
        /// Indicate if the label is readonly.
        /// </summary>
        public bool IsReadOnly
        {
            get => _isReadOnly.Value;
            set => _isReadOnly.Value = value;
        }

        /// <summary>
        /// Emit the new IsReadOnly values.
        /// </summary>
        public IObservable<bool> IsReadOnlyChanged => _isReadOnly.Changed;

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

        public LabelView()
        {
            // Set the text to empty string.
            Text = "";

            // Set the default font.
            Font = new Font("Arial", 11);
            Color = Color.Black;

            // When a property changes, repaint.
            ReactiveExtensions.MergeEvents(
                TextChanged,
                FontChanged,
                ColorChanged,
                EditModeChanged,
                FocusChanged
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

            // If the focus changed, exit edit mode unless CanLeaveEditMove is true.
            FocusChanged
                .Where(isFocused => !isFocused && CanLeaveEditMode)
                .Subscribe(_ => IsInEditMode = false);

            // Leave edit mode if ReadOnly is activated.
            IsReadOnlyChanged.Where(isReadOnly => isReadOnly).Subscribe(_ => IsInEditMode = false);

            // In edit mode, capture the mouse on the diagram editor scope.
            EditModeChanged.Subscribe(editMode =>
            {
                UIElement mouseCaptureScope =
                    WalkToRoot().OfType<DiagramEditorView>().FirstOrDefault() ?? WalkToRoot().Last();

                mouseCaptureScope.MouseCapturingElement = editMode ? this : null;
            });
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
                if (IsFocused)
                {
                    g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
                }

                // Draw cursor.
                if (_cursorIsVisible)
                {
                    g.DrawLine(pen, PreferredWidth - 5, 0, PreferredWidth - 5, PreferredHeight);
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
        protected override void OnMouseEvent(MouseEventData e)
        {
            // When the user clicks outside of the label bounds, try to exit edit mode.
            if (IsInEditMode)
            {
                bool eventOutOfLabelBounds = e.MousePosition.X < 0 || e.MousePosition.Y < 0 ||
                                             e.MousePosition.X >= Width || e.MousePosition.Y >= Height;

                if (e.Id == MouseEvent.MOUSE_PRESSED && eventOutOfLabelBounds && CanLeaveEditMode)
                {
                    IsInEditMode = false;
                }

                e.IsHandled = true;
            }
            // When the label is focused and the user clicks the label, enter edit mode.
            else if (IsFocused && e.Id == MouseEvent.MOUSE_PRESSED && !IsReadOnly)
            {
                IsInEditMode = true;
                e.IsHandled = true;
                return;
            }

            base.OnMouseEvent(e);
        }
    }
}