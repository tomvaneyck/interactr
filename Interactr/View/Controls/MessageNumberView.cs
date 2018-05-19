using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Interactr.Reactive;

namespace Interactr.View.Controls
{
    public class MessageNumberView : LabelView
    {
        #region Text

        private readonly ReactiveProperty<string> _text = new ReactiveProperty<string>();

        public override string Text
        {
            get => _text.Value;
            set
            {
                if (value != null)
                {
                    _text.Value = value + ":";
                }
            }
        }

        ///<see cref="TextChanged"/>
        public override IObservable<string> TextChanged => _text.Changed;

        #endregion

        public MessageNumberView()
        {
            // This element can never be focused or receive mouse events.
            CanBeFocused = false;
            IsVisibleToMouse = false;
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
                // Draw editing rectangle
                if (IsFocused)
                {
                    g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
                }
            }
        }
    }
}