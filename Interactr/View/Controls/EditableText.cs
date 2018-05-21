using Interactr.Constants;
using Interactr.Reactive;
using Interactr.View.Framework;
using Interactr.Window;
using System;
using System.Drawing;
using System.Reactive.Linq;
using System.Windows.Forms;

namespace Interactr.View.Controls
{
    public class EditableText : UIElement
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

        protected bool CursorIsVisible { get; set; } = false;

        public EditableText()
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
                    CursorIsVisible = false;
                    return Observable.Empty<long>();
                }
            }).Switch().Subscribe(_ =>
            {
                CursorIsVisible = !CursorIsVisible;
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
        }

        public override void PaintElement(Graphics g)
        {
            // Draw the string.
            using (Brush brush = new SolidBrush(Color))
            {
                g.DrawString(Text, Font, brush, 0, 0);
            }

            using (Pen pen = new Pen(Color))
            {
                // Draw cursor.
                if (CursorIsVisible)
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
                if (eventData.Id == KeyEvent.KEY_TYPED)
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
            HandleMouseEvent(eventData);

            if (!eventData.IsHandled)
            {
                base.OnMouseEvent(eventData);
            }
        }

        /// <summary>
        /// Handle the mouse event.
        /// </summary>
        /// <remarks>
        /// Implemented following the template pattern. This methods serves to defer the variant
        /// implementation of OnMouseEvent to each sub class.
        /// </remarks>
        /// <param name="eventData">Details of the event.</param>
        protected virtual void HandleMouseEvent(MouseEventData eventData)
        {
            if (IsFocused && eventData.Id == MouseEvent.MOUSE_CLICKED)
            {
                IsInEditMode = true;
                eventData.IsHandled = true;
            }
        }
    }
}
