using System;
using System.Drawing;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Interactr.Reactive;
using Interactr.View.Framework;

namespace Interactr.View.Controls
{
    /// <summary>
    /// A view that displays rectangles.
    /// </summary>
    public class RectangleView : UIElement
    {
        #region BorderColor

        private readonly ReactiveProperty<Color> _borderColor = new ReactiveProperty<Color>();

        /// <summary>
        /// The color of the border of the rectangle.
        /// </summary>
        public Color BorderColor
        {
            get => _borderColor.Value;
            set => _borderColor.Value = value;
        }

        public IObservable<Color> BorderColorChanged => _borderColor.Changed;

        #endregion

        #region BackgroundColor

        private readonly ReactiveProperty<Color> _backgroundColor = new ReactiveProperty<Color>();

        /// <summary>
        /// The color that is used to fill the background of the rectangle.
        /// </summary>
        public Color BackgroundColor
        {
            get => _backgroundColor.Value;
            set => _backgroundColor.Value = value;
        }

        public IObservable<Color> BackgroundColorChanged => _backgroundColor.Changed;

        #endregion

        #region BorderWidth

        private readonly ReactiveProperty<float> _borderWidth = new ReactiveProperty<float>();

        /// <summary>
        /// The width of the border in pixels.
        /// </summary>
        public float BorderWidth
        {
            get => _borderWidth.Value;
            set => _borderWidth.Value = value;
        }

        public IObservable<float> BorderWidthChanged => _borderWidth.Changed;

        #endregion

        public RectangleView()
        {
            // Default values.
            BorderColor = Color.Black;
            BackgroundColor = Color.Transparent;
            BorderWidth = 1;
            CanBeFocused = false;

            // When a property changes, repaint.
            ReactiveExtensions.MergeEvents(
                BorderColorChanged,
                BackgroundColorChanged,
                BorderWidthChanged
            ).Subscribe(_ => Repaint());
        }

        /// <see cref="PaintElement"/>
        public override void PaintElement(Graphics g)
        {
            // Fill rectangle with BackgroundColor.
            g.FillRectangle(
                new SolidBrush(BackgroundColor),
                0, 0,
                Width, Height
            );

            // Draw border.
            g.DrawRectangle(
                new Pen(BorderColor, BorderWidth),
                BorderWidth / 2, BorderWidth / 2,
                Width - BorderWidth, Height - BorderWidth
            );
        }
    }
}