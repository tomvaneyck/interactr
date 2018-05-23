using System.Drawing;
using Interactr.Reactive;
using System.Reactive.Linq;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Interactr.View.Controls
{
    class TextBox : EditableText
    {
        public TextBox()
        {
        }

        /// <see href="EditableText.UpdateLayout"/>
        protected override void UpdateLayout()
        {
            PreferredWidth = Math.Max(100, TextRenderer.MeasureText(Text, Font).Width) + 2;
            PreferredHeight = Font.Height + 1;
        }

        public override void PaintElement(Graphics g)
        {
            // Draw background.
            g.FillRectangle(Brushes.White, 0, 0, Width, Height);

            // Draw text box border.
            g.DrawLine(Pens.Black, 0, 0, 0, Height - 2);
            g.DrawLine(Pens.Black, 0, 0, Width - 2, 0);

            g.DrawLine(Pens.White, Width - 1, 0, Width - 1, Height - 2);
            g.DrawLine(Pens.White, 0, Height - 1, Width - 1, Height - 1);

            base.PaintElement(g);
        }
    }
}
