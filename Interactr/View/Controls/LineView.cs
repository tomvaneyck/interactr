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
        #region Pen

        private readonly ReactiveProperty<Pen> _pen = new ReactiveProperty<Pen>();

        /// <summary>
        /// The pen that is used to draw the line.
        /// Changing the color or thickness of the line can be done with this property.
        /// </summary>
        public Pen Pen
        {
            get => _pen.Value;
            set => _pen.Value = value;
        }

        /// <summary>
        /// Emit the new pen when the pen changes.
        /// </summary>
        public IObservable<Pen> PenChanged => _pen.Changed;

        #endregion

        #region PointA

        private readonly ReactiveProperty<Point> _pointA = new ReactiveProperty<Point>();

        /// <summary>
        /// The starting point of the line.
        /// </summary>
        public Point PointA
        {
            get => _pointA.Value;
            set => _pointA.Value = value;
        }

        /// <summary>
        /// Emit the new point A when it changes.
        /// </summary>
        public IObservable<Point> PointAChanged => _pointA.Changed;

        #endregion

        #region PointB

        private readonly ReactiveProperty<Point> _pointB = new ReactiveProperty<Point>();

        /// <summary>
        /// The endpoint of this line.
        /// </summary>
        public Point PointB
        {
            get => _pointB.Value;
            set => _pointB.Value = value;
        }

        /// <summary>
        /// Emit the new point B when it changes.
        /// </summary>
        public IObservable<Point> PointBChanged => _pointB.Changed;

        #endregion

        public LineView()
        {
            // Default values.
            Pen = Pens.Black;
            PointA = new Point();
            PointB = new Point(10, 10);

            // Repaint on property change.
            Observable.Merge(
                PenChanged.Select(_ => Unit.Default),
                PointAChanged.Select(_ => Unit.Default),
                PointBChanged.Select(_ => Unit.Default)
            ).Subscribe(_ => Repaint());

            // Resize preferred size to fit line.
            Observable.CombineLatest(PointAChanged, PointBChanged).Subscribe(points =>
            {
                PreferredWidth = points.Max(p => p.X);
                PreferredHeight = points.Max(p => p.Y);
            });
        }

        /// <see cref="PaintElement"/>
        public override void PaintElement(Graphics g)
        {
            g.DrawLine(Pen, PointA.X, PointA.Y, PointB.X, PointB.Y);
        }
    }
}