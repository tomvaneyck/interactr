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
    public class RectangleView : UIElement
    {
        #region BorderColor
        private readonly ReactiveProperty<Color> _borderColor = new ReactiveProperty<Color>();
        public Color BorderColor
        {
            get => _borderColor.Value;
            set => _borderColor.Value = value;
        }
        public IObservable<Color> BorderColorChanged => _borderColor.Changed;
        #endregion

        #region BackgroundColor
        private readonly ReactiveProperty<Color> _backgroundColor = new ReactiveProperty<Color>();
        public Color BackgroundColor
        {
            get => _backgroundColor.Value;
            set => _backgroundColor.Value = value;
        }
        public IObservable<Color> BackgroundColorChanged => _backgroundColor.Changed;
        #endregion
        
        #region BorderWidth
        private readonly ReactiveProperty<float> _borderWidth = new ReactiveProperty<float>();
        public float BorderWidth
        {
            get => _borderWidth.Value;
            set => _borderWidth.Value = value;
        }
        public IObservable<float> BorderWidthChanged => _borderWidth.Changed;
        #endregion

        public RectangleView()
        {
            BorderColor = Color.Black;
            BackgroundColor = Color.Transparent;
            BorderWidth = 1;

            Observable.Merge(
                BorderColorChanged.Select(_ => Unit.Default),
                BackgroundColorChanged.Select(_ => Unit.Default),
                BorderWidthChanged.Select(_ => Unit.Default)
            ).Subscribe(_ => Repaint());
        }
        
        public override void PaintElement(Graphics g)
        {
            g.DrawRectangle(
                new Pen(BorderColor, BorderWidth), 
                BorderWidth / 2, BorderWidth / 2, 
                Width - BorderWidth, Height - BorderWidth
            );
        }
    }
}
