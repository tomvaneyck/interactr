using Interactr.Constants;
using Interactr.Reactive;
using Interactr.View.Framework;
using Interactr.Window;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Reactive.Linq;
using System.Windows.Forms;

namespace Interactr.View.Controls
{
    /// <summary>
    /// Provides a system for displaying and editing text.
    /// </summary>
    public abstract class EditableText : UIElement
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
            FocusChanged
                .Where(isFocused => !IsFocused)
                .Subscribe(_ => HandleFocusChange());
        }

        /// <summary>
        /// Handles the loss of focus.
        /// </summary>
        /// <remarks>
        /// Defers the implementation to derived classes with the template pattern.
        /// </remarks>
        protected virtual void HandleFocusChange()
        {
            IsInEditMode = false;
        }

        /// <summary>
        /// Update the preferred height and preferred width.
        /// </summary>
        protected virtual void UpdateLayout()
        {
            // Set the preferred width and height of the labelView by measuring
            // how much space it would take to fully render the string.
            PreferredWidth = Text.Length > 0 ? TextRenderer.MeasureText(Text, Font).Width : 5;
            PreferredHeight = Font.Height;
        }

        public override void PaintElement(Graphics g)
        {
            UpdateLayout();

            // Draw the string.
            using (Brush brush = new SolidBrush(Color))
            {
                g.DrawString(Text, Font, brush, 1, 0);
            }

            using (Pen pen = new Pen(Color))
            {
                // Draw cursor.
                if (CursorIsVisible)
                {
                    int cursorPosition = Text.Length > 0
                        ? TextRenderer.MeasureText(Text, Font).Width - 3
                        : 3;
                    g.DrawLine(pen, cursorPosition, 0, cursorPosition, PreferredHeight);
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
                    // If Keychar is a letter or a colon.
                    else if (char.IsLetterOrDigit(eventData.KeyChar) ||
                             eventData.KeyChar == HexaDecimalKeyChars.Colon ||
                             eventData.KeyChar == HexaDecimalKeyChars.OpeningParenthesis ||
                             eventData.KeyChar == HexaDecimalKeyChars.ClosingParenthesis ||
                             eventData.KeyChar == HexaDecimalKeyChars.Comma)
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
            if (IsFocused && eventData.Id == MouseEvent.MOUSE_CLICKED)
            {
                IsInEditMode = true;
                eventData.IsHandled = true;
            }

            base.OnMouseEvent(eventData);
        }
    }
}