using System;
using System.Drawing;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Interactr.Reactive;
using Interactr.View.Framework;
using Point = Interactr.View.Framework.Point;

namespace Interactr.View.Controls
{
    /// <summary>
    /// A view that displays a line.
    /// </summary>
    public class LineView : UIElement
    {
        #region StartPoint

        private readonly ReactiveProperty<Point> _startPoint = new ReactiveProperty<Point>();

        /// <summary>
        /// The starting point of the line.
        /// </summary>
        public Point StartPoint
        {
            get => _startPoint.Value;
            set => _startPoint.Value = value;
        }

        /// <summary>
        /// Emit the new starting point when it changes.
        /// </summary>
        public IObservable<Point> StartPointChanged => _startPoint.Changed;

        #endregion

        #region EndPoint

        private readonly ReactiveProperty<Point> _endPoint = new ReactiveProperty<Point>();

        /// <summary>
        /// The endpoint of this line.
        /// </summary>
        public Point EndPoint
        {
            get => _endPoint.Value;
            set => _endPoint.Value = value;
        }

        /// <summary>
        /// Emit the new end point when it changes.
        /// </summary>
        public IObservable<Point> EndPointChanged => _endPoint.Changed;

        #endregion

        #region Color

        private readonly ReactiveProperty<Color> _color = new ReactiveProperty<Color>();

        /// <summary>
        /// The color of the line and arrowhead.
        /// </summary>
        public Color Color
        {
            get => _color.Value;
            set => _color.Value = value;
        }

        public IObservable<Color> ColorChanged => _color.Changed;

        #endregion

        #region LineThickness

        private readonly ReactiveProperty<float> _lineThickness = new ReactiveProperty<float>();

        /// <summary>
        /// The thickness of the line
        /// </summary>
        public float LineThickness
        {
            get => _lineThickness.Value;
            set => _lineThickness.Value = value;
        }

        public IObservable<float> LineThicknessChanged => _lineThickness.Changed;

        #endregion

        #region LineType

        private readonly ReactiveProperty<LineType> _style = new ReactiveProperty<LineType>();

        /// <summary>
        /// The style of displaying for the lime.
        /// </summary>
        public LineType Style
        {
            get => _style.Value;
            set => _style.Value = value;
        }

        public IObservable<LineType> StyleChanged => _style.Changed;

        #endregion

        public LineView()
        {
            // Default values.
            Color = Color.Black;
            LineThickness = 1;

            // Repaint on property change.
            Observable.Merge(
                ColorChanged.Select(_ => Unit.Default),
                LineThicknessChanged.Select(_ => Unit.Default),
                StartPointChanged.Select(_ => Unit.Default),
                EndPointChanged.Select(_ => Unit.Default)
            ).Subscribe(_ => Repaint());

            // Resize preferred size to fit line.
            Observable.CombineLatest(StartPointChanged, EndPointChanged).Subscribe(points =>
            {
                PreferredWidth = points.Max(p => p.X);
                PreferredHeight = points.Max(p => p.Y);
            });
        }

        /// <see cref="PaintElement"/>
        public override void PaintElement(Graphics g)
        {
            switch (Style)
            {
                case LineType.Solid:
                    g.DrawLine(new Pen(Color, LineThickness), StartPoint.X, StartPoint.Y, EndPoint.X, EndPoint.Y);
                    break;
                case LineType.Dotted:
                    g.DrawLine(
                        new Pen(Color, LineThickness)
                        {
                            DashPattern = new float[] {2, 2}
                        },
                        StartPoint.X, StartPoint.Y, EndPoint.X, EndPoint.Y);
                    break;
            }
        }

        /// <summary>
        /// The types of display for the line.
        /// </summary>
        public enum LineType
        {
            Solid,
            Dotted
        }
    }
}