namespace Interactr.View.Framework
{
    /// <summary>
    /// Represents a position in 2D space.
    /// </summary>
    public struct Point
    {
        public int X { get; }
        public int Y { get; }

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public static Point operator+(Point p1, Point p2)
        {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Point operator-(Point p1, Point p2)
        {
            return new Point(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static bool operator!=(Point p1, Point p2)
        {
            return !(p1 == p2);
        }

        public static bool operator==(Point p1, Point p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y;
        }

        public override string ToString()
        {
            return $"({this.X}, {this.Y})";
        }
        
        public override bool Equals(object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            Point p = (Point)obj;
            return this == p;
        }

        public override int GetHashCode()
        {
            return X ^ Y;
        }
    }
}
