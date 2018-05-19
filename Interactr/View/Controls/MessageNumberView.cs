using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Interactr.Reactive;

namespace Interactr.View.Controls
{
    public class MessageNumberView : RectangleView
    {
        #region Text

        private readonly ReactiveProperty<string> _messageNumber = new ReactiveProperty<string>();

        /// <summary>
        /// The text of the label.
        /// </summary>
        public string MessageNumber
        {
            get => _messageNumber.Value;
            set {
                if (value != null)
                {
                    _messageNumber.Value = value+":";
                } }
        }

        /// <summary>
        /// Emit the new text when the text changes.
        /// </summary>
        public IObservable<string> MessageNumberChanged => _messageNumber.Changed;

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

        public MessageNumberView()
        {
            // This element can never be focused or receive mouse events.
            CanBeFocused = false;
            IsVisibleToMouse = false;

            // Set the color and font.
            Color = Color.Black;
            Font = new Font("Arial", 11);

            // Set the preferred width and height of the messageNumberView by measuring
            // how much space it would take to fully render the string of the number.
            MessageNumberChanged.Subscribe(text =>
            {
                PreferredWidth = TextRenderer.MeasureText(text, Font).Width;
                PreferredHeight = TextRenderer.MeasureText(text, Font).Height;
            });
        }

        /// <see cref="PaintElement"/>
        public override void PaintElement(Graphics g)
        {
            // Draw the string.
            using (Brush brush = new SolidBrush(Color))
            {
                g.DrawString(MessageNumber, Font, brush, 0, 0);
            }

            using (Pen pen = new Pen(Color))
            {
                // Draw editing rectangle
                if (IsFocused)
                {
                    g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
                }
            }
        }
    }
}