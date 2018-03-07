namespace Interactr.View.Controls
{
    /// <summary>
    /// Hold the size of the margins around the 4 sides.
    /// </summary>
    public struct Margins
    {
        public int Left { get; }
        public int Top { get; }
        public int Right { get; }
        public int Bottom { get; }

        public Margins(int left = 0, int top = 0, int right = 0, int bottom = 0)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }

        /// <summary>
        /// See if the two given margins have different values.
        /// </summary>
        /// <param name="m1">First margin</param>
        /// <param name="m2">Second margin</param>
        /// <returns>True if the margins are not equal, otherwise false.</returns>
        public static bool operator !=(Margins m1, Margins m2)
        {
            return !(m1 == m2);
        }

        /// <summary>
        /// See if the two given margins have the same values.
        /// </summary>
        /// <param name="m1">First margin</param>
        /// <param name="m2">Second margin</param>
        /// <returns>True if the margins are equal, otherwise false.</returns>
        public static bool operator ==(Margins m1, Margins m2)
        {
            return m1.Left == m2.Left &&
                   m1.Top == m2.Top &&
                   m1.Right == m2.Right &&
                   m1.Bottom == m2.Bottom;
        }

        /// <summary>
        /// Convert margin to readable format.
        /// </summary>
        /// <returns>The string formatted for human reading.</returns>
        public override string ToString()
        {
            return $"({this.Left}, {this.Top}, {this.Right}, {this.Bottom})";
        }

        /// <summary>
        /// See if given object is the same as this.
        /// </summary>
        /// <param name="obj">The given object.</param>
        /// <returns>True if the given object and this are equal, otherwise false.</returns>
        public override bool Equals(object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            Margins m = (Margins)obj;
            return this == m;
        }

        /// <summary>
        /// Return the hashcode for this object.
        /// </summary>
        /// <returns>The hashcode for this object.</returns>
        public override int GetHashCode()
        {
            return Left ^ Top ^ Right ^ Bottom;
        }
    }
}