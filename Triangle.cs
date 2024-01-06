using Microsoft.Xna.Framework;

namespace MGPrimitives
{
    public struct Triangle : IEquatable<Triangle>
    {
        #region Public fields
        public float P1X;
        public float P1Y;
        public float P2X;
        public float P2Y;
        public float P3X;
        public float P3Y;
        #endregion
        #region Public properties
        public Vector2 P1
        {
            readonly get => new(this.P1X, this.P1Y);
            set
            {
                this.P1X = value.X;
                this.P1Y = value.Y;
            }
        }
        public Vector2 P2
        {
            readonly get => new(this.P2X, this.P2Y);
            set
            {
                this.P2X = value.X;
                this.P2Y = value.Y;
            }
        }
        public Vector2 P3
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
                int x1 = (int)Math.Min(this.P1X, Math.Min(this.P2X, this.P3X));
                int y1 = (int)Math.Min(this.P1Y, Math.Min(this.P2Y, this.P3Y));
                int x2 = (int)Math.Max(this.P1X, Math.Max(this.P2X, this.P3X));
                int y2 = (int)Math.Max(this.P1Y, Math.Max(this.P2Y, this.P3Y));
                return new(x1, y1, x2 - x1, y2 - y1);
            }
        }
        public readonly double Perimeter
        {
            get
            {
                double d1 = Triangle.Distance(this.P1X, this.P1Y, this.P2X, this.P2Y);
                double d2 = Triangle.Distance(this.P2X, this.P2Y, this.P3X, this.P3Y);
                double d3 = Triangle.Distance(this.P3X, this.P3Y, this.P1X, this.P1Y);
                return d1 + d2 + d3;
            }
        }
        public readonly double Area => 0.5 * Math.Abs((this.P1X * (this.P2Y - this.P3Y)) + (this.P2X * (this.P3Y - this.P1Y)) + (this.P3X * (this.P1Y - this.P2Y)));
        #endregion
        #region Constructors
        public Triangle() : this(0f, 0f, 0f, 0f, 0f, 0f) { }
        public Triangle(Point p1, Point p2, Point p3) : this(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y) { }
        public Triangle(Vector2 p1, Vector2 p2, Vector2 p3) : this(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y) { }
        public Triangle(float p1X, float p1Y, float p2X, float p2Y, float p3X, float p3Y)
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
        public static bool operator ==(Triangle a, Triangle b) =>
            (a.P1X == b.P1X) && (a.P1Y == b.P1Y) && (a.P2X == b.P2X) && (a.P2Y == b.P2Y) && (a.P3X == b.P3X) && (a.P3Y == b.P3Y);
        public static bool operator !=(Triangle a, Triangle b) => !(a == b);
        #endregion
        #region Private methods
        private static double Distance(float x1, float y1, float x2, float y2) => Math.Sqrt(((x2 - x1) * (x2 - x1)) +  ((y2 - y1) * (y2 - y1)));
        #endregion
        #region Public methods
        public readonly override bool Equals(object? obj) => (obj is Triangle triangle) && (this == triangle);
        public readonly bool Equals(Triangle other) => this == other;
        public readonly override int GetHashCode() => HashCode.Combine(this.P1X, this.P1Y, this.P2X, this.P2Y, this.P3X, this.P3Y);
        public readonly bool Contains(Point point) => this.Contains(point.X, point.Y);
        public readonly bool Contains(Vector2 point) => this.Contains(point.X, point.Y);
        public readonly bool Contains(float x, float y)
        {
            Vector2 point = new(x, y);
            Triangle t1 = new(point, this.P1, this.P2);
            Triangle t2 = new(point, this.P2, this.P3);
            Triangle t3 = new(point, this.P3, this.P1);
            return Math.Abs(t1.Area + t2.Area + t3.Area - this.Area) >= 0.0001;
        }
        #endregion
    }
}
