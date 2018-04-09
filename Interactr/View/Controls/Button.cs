using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactr.Reactive;
using Interactr.View.Framework;
using Interactr.Window;

namespace Interactr.View.Controls
{
    public class Button : UIElement
    {
        #region Label

        private readonly ReactiveProperty<String> _label = new ReactiveProperty<String>();
        
        public String Label
        {
            get => _label.Value;
            set => _label.Value = value;
        }

        public IObservable<String> LabelChanged => _label.Changed;

        #endregion

        #region LabelFont

        private readonly ReactiveProperty<Font> _labelFont = new ReactiveProperty<Font>();

        public Font LabelFont
        {
            get => _labelFont.Value;
            set => _labelFont.Value = value;
        }

        public IObservable<Font> LabelFontChanged => _labelFont.Changed;

        #endregion
        
        public Button()
        {
            Label = "";
            LabelFont = new Font("Arial", 8f);

            LabelChanged.Subscribe(_ => Repaint());
        }

        private bool isPressed;

        protected override bool OnMouseEvent(MouseEventData eventData)
        {
            if (eventData.Id == MouseEvent.MOUSE_PRESSED)
            {
                isPressed = true;
                Repaint();
            }
            else if (eventData.Id == MouseEvent.MOUSE_RELEASED)
            {
                isPressed = false;
                Repaint();
            }
            return true;
        }

        public override void PaintElement(Graphics g)
        {
            var labelSize = g.MeasureString(Label, LabelFont);
            PreferredWidth = (int)labelSize.Width + 2;
            PreferredHeight = (int)labelSize.Height + 2;

            if (!isPressed)
            {
                using (Brush brush = new SolidBrush(Color.FromArgb(195, 199, 203)))
                {
                    g.FillRectangle(brush, 1, 1, Width - 3, Height - 3);
                }

                g.DrawLine(Pens.White, 0, 0, 0, Height - 2);
                g.DrawLine(Pens.White, 0, 0, Width - 2, 0);

                g.DrawLine(Pens.Black, Width - 1, 0, Width - 1, Height - 2);
                g.DrawLine(Pens.Black, 0, Height - 1, Width - 1, Height - 1);

                using (Pen pen = new Pen(Color.FromArgb(134, 138, 142)))
                {
                    g.DrawLine(pen, Width - 2, 1, Width - 2, Height - 2);
                    g.DrawLine(pen, 1, Height - 2, Width - 2, Height - 2);
                }

                g.DrawString(Label, LabelFont, Brushes.Black, 0, 1);
            }
            else
            {
                using (Brush brush = new SolidBrush(Color.FromArgb(195, 199, 203)))
                {
                    g.FillRectangle(brush, 1, 1, Width - 3, Height - 3);
                }

                g.DrawLine(Pens.Black, 0, 0, 0, Height - 2);
                g.DrawLine(Pens.Black, 0, 0, Width - 2, 0);

                g.DrawLine(Pens.White, Width - 1, 0, Width - 1, Height - 2);
                g.DrawLine(Pens.White, 0, Height - 1, Width - 1, Height - 1);

                using (Pen pen = new Pen(Color.FromArgb(134, 138, 142)))
                {
                    g.DrawLine(pen, 1, 1, Width - 3, 1);
                    g.DrawLine(pen, 1, 1, 1, Height - 3);
                }

                g.DrawString(Label, LabelFont, Brushes.Black, 1, 2);
            }
        }
    }
}
