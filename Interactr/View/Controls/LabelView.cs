using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactr.Reactive;
using Interactr.View.Framework;

namespace Interactr.View.Controls
{
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

        public LabelView()
        {
            Font = new Font("Arial", 11);

            Observable.Merge(
                TextChanged.Select(_ => Unit.Default),
                FontChanged.Select(_ => Unit.Default)
            ).Subscribe(_ => Repaint());
        }

        public override void PaintElement(Graphics g)
        {
            var preferredSize = g.MeasureString(Text, Font);
            PreferredWidth = (int)Math.Ceiling(preferredSize.Width);
            PreferredHeight = (int)Math.Ceiling(preferredSize.Height);

            g.DrawString(Text, Font, Brushes.Black, 0, 0);
        }
    }
}
