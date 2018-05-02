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
    /// A view that displays a line with an arrowhead at the end.
    /// </summary>
    public class ArrowView : UIElement
    {
        #region StartPoint

        private readonly ReactiveProperty<Point> _startPoint = new ReactiveProperty<Point>();

        /// <summary>
        /// The point where the line starts.
        /// </summary>
        public Point StartPoint
        {
            get => _startPoint.Value;
            set => _startPoint.Value = value;
        }

        public IObservable<Point> StartPointChanged => _startPoint.Changed;

        #endregion

        #region EndPoint

        private readonly ReactiveProperty<Point> _endPoint = new ReactiveProperty<Point>();

        /// <summary>
        /// The point where the line ends and an arrowhead is drawn.
        /// </summary>
        public Point EndPoint
        {
            get => _endPoint.Value;
            set => _endPoint.Value = value;
        }

        public IObservable<Point> EndPointChanged => _endPoint.Changed;

        #endregion

        #region ArrowHeadSize

        private readonly ReactiveProperty<float> _arrowHeadSize = new ReactiveProperty<float>();

        /// <summary>
        /// The size of the arrowhead at the end of the line.
        /// </summary>
        public float ArrowHeadSize
        {
            get => _arrowHeadSize.Value;
            set => _arrowHeadSize.Value = value;
        }

        public IObservable<float> ArrowHeadSizeChanged => _arrowHeadSize.Changed;

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

        public ArrowView()
        {
            CanBeFocused = false;

            // Default values.
            ArrowHeadSize = 10;
            LineThickness = 1;
            Color = Color.Black;

            // Repaint when a property changes.
            ReactiveExtensions.MergeEvents(
                StartPointChanged,
                EndPointChanged,
                ArrowHeadSizeChanged,
                LineThicknessChanged,
                ColorChanged
            ).Subscribe(_ => Repaint());
        }

        /// <see cref="PaintElement"/>
        public override void PaintElement(Graphics g)
        {
            // Draw line
            g.DrawLine(new Pen(Color, LineThickness),
                StartPoint.X,
                StartPoint.Y,
                EndPoint.X,
                EndPoint.Y);

            (Point pointL,Point pointR) = CalculateWingPoints();


            // Create a triangle with EndPoint and the 2 points we calculated above and fill it with Color.
            g.FillPolygon(new SolidBrush(Color), new[]
            {
                new System.Drawing.Point(EndPoint.X, EndPoint.Y),
                new System.Drawing.Point(pointL.X, pointL.Y),
                new System.Drawing.Point(pointR.X, pointR.Y)
            });
        }

        public (Point Point1, Point Point2) CalculateWingPoints()
        {
            // Draw arrowhead triangle.
            float radius = ArrowHeadSize;
            float wingAngle = (float)(Math.PI / 4d); // Angle between line and arrow wings.

            float xDiff = EndPoint.X - StartPoint.X; // Length of line projected on X-axis.
            float yDiff = EndPoint.Y - StartPoint.Y; // Length of line projected on Y-axis.

            // Angle between x-axis and line.
            float check = (float)Math.Asin(yDiff / Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2)));
            float angleCalc = (float)Math.Acos(xDiff / Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2)));
            float arrowAngle = check < 0 ? -1 * angleCalc : angleCalc;

            // If line has length 0, assume an angle of 0.
            arrowAngle = float.IsNaN(arrowAngle) ? 0 : arrowAngle;

            // Find points to use for drawing the wings of the arrow.
            // The wings will be drawn from EndPoint to each of these points.

            // Calculate point 1
            int x1 = (int)Math.Round(Math.Sin(arrowAngle - wingAngle) * radius) + EndPoint.X;
            int y1 = (int)Math.Round(-Math.Cos(arrowAngle - wingAngle) * radius) + EndPoint.Y;

            // Calculate point 2, on the other side of the line.
            int x2 = (int)Math.Round(-Math.Cos(arrowAngle - wingAngle) * radius) + EndPoint.X;
            int y2 = (int)Math.Round(-Math.Sin(arrowAngle - wingAngle) * radius) + EndPoint.Y;

            Point pointL = new Point(x1, y1);
            Point pointR = new Point(x2, y2);

            return (pointL, pointR);
        }
    }
}
