using System.Drawing;

namespace MGPrimitives
{
    public struct IntTriangle : IEquatable<IntTriangle>
    {
        #region Public fields
        public int P1X;
        public int P1Y;
        public int P2X;
        public int P2Y;
        public int P3X;
        public int P3Y;
        #endregion
        #region Public properties
        public Point P1
        {
            readonly get => new(this.P1X, this.P1Y);
            set
            {
                this.P1X = value.X;
                this.P1Y = value.Y;
            }
        }
        public Point P2
        {
            readonly get => new(this.P2X, this.P2Y);
            set
            {
                this.P2X = value.X;
                this.P2Y = value.Y;
            }
        }
        public Point P3
        {
            readonly get => new(this.P3X, this.P3Y);
            set
            {
                this.P3X = value.X;
                this.P3Y = value.Y;
            }
        }
        public readonly Rectangle BoundingRectangle
        {
            get
            {
                int x1 = Math.Min(this.P1X, Math.Min(this.P2X, this.P3X));
                int y1 = Math.Min(this.P1Y, Math.Min(this.P2Y, this.P3Y));
                int x2 = Math.Max(this.P1X, Math.Max(this.P2X, this.P3X));
                int y2 = Math.Max(this.P1Y, Math.Max(this.P2Y, this.P3Y));
                return new(x1, y1, x2 - x1, y2 - y1);
            }
        }
        public readonly double Perimeter
        {
            get
            {
                double d1 = IntTriangle.Distance(this.P1X, this.P1Y, this.P2X, this.P2Y);
                double d2 = IntTriangle.Distance(this.P2X, this.P2Y, this.P3X, this.P3Y);
                double d3 = IntTriangle.Distance(this.P3X, this.P3Y, this.P1X, this.P1Y);
                return d1 + d2 + d3;
            }
        }
        public readonly double Area => 0.5 * Math.Abs((this.P1X * (this.P2Y - this.P3Y)) + (this.P2X * (this.P3Y - this.P1Y)) + (this.P3X * (this.P1Y - this.P2Y)));
        #endregion
        #region Constructors
        public IntTriangle()
        {
            this.P1X = 0;
            this.P1Y = 0;
            this.P2X = 0;
            this.P2Y = 0;
            this.P3X = 0;
            this.P3Y = 0;
        }
        public IntTriangle(Point p1, Point p2, Point p3)
        {
            this.P1X = p1.X;
            this.P1Y = p1.Y;
            this.P2X = p2.X;
            this.P2Y = p2.Y;
            this.P3X = p3.X;
            this.P3Y = p3.Y;
        }
        public IntTriangle(int p1X, int p1Y, int p2X, int p2Y, int p3X, int p3Y)
        {
            this.P1X = p1X;
            this.P1Y = p1Y;
            this.P2X = p2X;
            this.P2Y = p2Y;
            this.P3X = p3X;
            this.P3Y = p3Y;
        }
        #endregion
        #region Operators
        public static bool operator ==(IntTriangle a, IntTriangle b) =>
            (a.P1X == b.P1X) && (a.P1Y == b.P1Y) && (a.P2X == b.P2X) && (a.P2Y == b.P2Y) && (a.P3X == b.P3X) && (a.P3Y == b.P3Y);
        public static bool operator !=(IntTriangle a, IntTriangle b) => !(a == b);
        #endregion
        #region Private methods
        private static double Distance(int x1, int y1, int x2, int y2) => Math.Sqrt(((x2 - x1) * (x2 - x1)) +  ((y2 - y1) * (y2 - y1)));
        #endregion
        #region Public methods
        public readonly override bool Equals(object? obj) => (obj is IntTriangle triangle) && (this == triangle);
        public readonly bool Equals(IntTriangle other) => this == other;
        public readonly override int GetHashCode() => HashCode.Combine(this.P1X, this.P1Y, this.P2X, this.P2Y, this.P3X, this.P3Y);
        #endregion
    }
}
