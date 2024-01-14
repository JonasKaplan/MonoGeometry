using Microsoft.Xna.Framework;

namespace MonoGeometry.Geometry
{
    public struct LineSegment : IEquatable<LineSegment>, ITransformable
    {
        #region Public fields
        public float P1X;
        public float P1Y;
        public float P2X;
        public float P2Y;
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
        public readonly double Length => Geometry.Distance(this.P1X, this.P1Y, this.P2X, this.P2Y);
        public readonly double LengthSquared => Geometry.DistanceSquared(this.P1X, this.P1Y, this.P2X, this.P2Y);
        public readonly double Angle => Math.Atan2(P2Y - P1Y, P2X - P1X);
        #endregion
        #region Constructors
        public LineSegment() : this(0f, 0f, 0f, 0f) { }
        public LineSegment(Vector2 p1, Vector2 p2) : this(p1.X, p1.Y, p2.X, p2.Y) { }
        public LineSegment(float p1X, float p1Y, float p2X, float p2Y)
        {
            this.P1X = p1X;
            this.P1Y = p1Y;
            this.P2X = p2X;
            this.P2Y = p2Y;
        }
        #endregion
        #region Operators
        public static bool operator ==(LineSegment a, LineSegment b) => a.P1X == b.P1X && a.P1Y == b.P1Y && a.P2X == b.P2X && a.P2Y == b.P2Y;
        public static bool operator !=(LineSegment a, LineSegment b) => !(a == b);
        #endregion
        #region Public methods
        public readonly override bool Equals(object? obj) => obj is LineSegment lineSegment && this == lineSegment;
        public readonly bool Equals(LineSegment other) => this == other;
        public readonly override int GetHashCode() => HashCode.Combine(this.P1X, this.P1Y, this.P2X, this.P2Y);
        public void Transform(Matrix transform, Vector2 origin) => this.Transform(transform, origin.X, origin.Y);
        public void Transform(Matrix transform, float originX, float originY)
        {
            this.Transform(Matrix.CreateTranslation(-originX, -originY, 0f));
            this.Transform(transform);
            this.Transform(Matrix.CreateTranslation(originX, originY, 0f));
        }
        public void Transform(Matrix transform)
        {
            this.P1 = Vector2.Transform(this.P1, transform);
            this.P2 = Vector2.Transform(this.P2, transform);
        }
        public static bool Intersects(LineSegment a, LineSegment b)
        {
            //Algorithm stolen from https://stackoverflow.com/questions/3838329/how-can-i-check-if-two-segments-intersect
            if (a.P1 == b.P1 || a.P2 == b.P2) return true;
            if (Math.Max(a.P1X, a.P2X) < Math.Min(b.P1X, b.P2X)) return false;

            //TODO: fix division by zero here
            float aSlope = (a.P2Y - a.P1Y) / (a.P2X - a.P1X);
            float bSlope = (b.P2Y - b.P1Y) / (b.P2X - b.P1X);
            if (aSlope == bSlope) return false;

            float aYIntercept = a.P1Y - (aSlope * a.P1X);
            float bYIntercept = b.P1Y - (bSlope * b.P1X);

            float x = (bYIntercept - aYIntercept) / (bSlope - aSlope);
            return x < Math.Max(Math.Min(a.P1X, a.P2X), Math.Min(b.P1X, b.P2X)) || x > Math.Min(Math.Max(a.P1X, a.P2X), Math.Max(b.P1X, b.P2X));
        }
        #endregion
    }
}
