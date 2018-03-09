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
using Point = Interactr.View.Framework.Point;

namespace Interactr.View.Controls
{
    /// <summary>
    /// A view that displays a line.
    /// </summary>
    public class LineView : UIElement
    {
        #region Pen
        /// <summary>
        /// The pen that is used to draw the line.
        /// Changing the color or thickness of the line can be done with this property.
        /// </summary>
        public Pen Pen
        {
            get => _pen.Value;
            set => _pen.Value = value;
        }
        private readonly ReactiveProperty<Pen> _pen = new ReactiveProperty<Pen>();
        public IObservable<Pen> PenChanged => _pen.Changed;
        #endregion

        #region PointA
        /// <summary>
        /// The starting point of the line.
        /// </summary>
        public Point PointA
        {
            get => _pointA.Value;
            set => _pointA.Value = value;
        }
        private readonly ReactiveProperty<Point> _pointA = new ReactiveProperty<Point>();
        public IObservable<Point> PointAChanged => _pointA.Changed;
        #endregion

        #region PointB
        /// <summary>
        /// The endpoint of this line.
        /// </summary>
        public Point PointB
        {
            get => _pointB.Value;
            set => _pointB.Value = value;
        }
        private readonly ReactiveProperty<Point> _pointB = new ReactiveProperty<Point>();
        public IObservable<Point> PointBChanged => _pointB.Changed;
        #endregion

        public LineView()
        {
            // Default values.
            this.Pen = Pens.Black;
            this.PointA = new Point();
            this.PointB = new Point(10, 10);

            SetupObservables();
        }

        private void SetupObservables()
        {
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

        public override void PaintElement(Graphics g)
        {
            g.DrawLine(Pen, PointA.X, PointA.Y, PointB.X, PointB.Y);
        }
    }
}
