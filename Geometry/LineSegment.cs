using Microsoft.Xna.Framework;

namespace MonoGeometry.Geometry
{
    public struct LineSegment : IEquatable<LineSegment>, ITransformable<LineSegment>
    {
        #region Public properties
        public Vector2 P0 { get; set; }
        public Vector2 P1 { get; set; }
        public readonly float Length => MathF.Sqrt(this.LengthSquared);
        public readonly float LengthSquared => ((this.P1.X - this.P0.X) * (this.P1.X - this.P0.X)) + ((this.P1.Y - this.P0.Y) * (this.P1.Y - this.P0.Y));
        public readonly Vector2 Center => 0.5f * (this.P0 + this.P1);
        public readonly Vector2 Direction => this.P1 - this.P0;
        #endregion
        #region Constructors
        public LineSegment() : this(Vector2.Zero, Vector2.Zero) { }
        public LineSegment(Vector2 P0, Vector2 P1)
        {
            this.P0 = P0;
            this.P1 = P1;
        }
        #endregion
        #region Operators
        public static bool operator ==(LineSegment a, LineSegment b) => (a.P0 == b.P0) && (a.P1 == b.P1);
        public static bool operator !=(LineSegment a, LineSegment b) => !(a == b);
        #endregion
        #region Public methods
        public override readonly bool Equals(object? obj) => (obj is LineSegment lineSegment) && (this == lineSegment);
        public readonly bool Equals(LineSegment other) => this == other;
        public override readonly int GetHashCode() => HashCode.Combine(this.P0, this.P1);
        public override readonly string ToString() => "{" + this.P0.ToString() + ", " + this.P1.ToString() + "}";
        public readonly bool BoundingBoxContains(Vector2 point) =>
            (point.X <= MathF.Max(this.P0.X, this.P1.X)) && (point.X >= MathF.Min(this.P0.X, this.P1.X)) && (point.Y <= MathF.Max(this.P0.Y, this.P1.Y)) && (point.Y >= MathF.Min(this.P0.Y, this.P1.Y));
        public readonly bool Contains(Vector2 point)
        {
            if (!this.BoundingBoxContains(point)) return false;
            float lambdaX = (point.X - this.P0.X) / this.Direction.X;
            float lambdaY = (point.Y - this.P0.Y) / this.Direction.Y;
            return MathF.Abs(lambdaX - lambdaY) <= 0.00001f;
        }
        public static bool AreParallel(LineSegment a, LineSegment b) => MathF.Abs((a.Direction.X / b.Direction.X) - (a.Direction.Y / b.Direction.Y)) <= 0.00001f;
        public static bool AreCollinear(LineSegment a, LineSegment b)
        {
            bool b0OnLine = MathF.Abs(((b.P0.X - a.P0.X) / a.Direction.X) - ((b.P0.Y - a.P0.Y) / a.Direction.Y)) <= 0.00001f;
            bool b1OnLine = MathF.Abs(((b.P1.X - a.P0.X) / a.Direction.X) - ((b.P1.Y - a.P0.Y) / a.Direction.Y)) <= 0.00001f;
            return b0OnLine && b1OnLine;
        }
        public static Vector2? Intersection(LineSegment a, LineSegment b)
        {
            if ((a.P0 == b.P0) && (a.P1 != b.P1)) return a.P0;
            if ((a.P1 == b.P1) && (a.P0 != b.P0)) return a.P1;
            if ((a.P1 == b.P0) && (a.P0 != b.P1)) return a.P1;
            if ((a.P0 == b.P1) && (a.P1 != b.P0)) return a.P0;

            if (LineSegment.AreParallel(a, b)) return null;

            //Algorithm derived using linear algebra
            Vector2 offset = b.P0 - a.P0;
            float beta = ((a.Direction.X * offset.Y) - (a.Direction.Y * offset.X)) / ((a.Direction.Y * b.Direction.X) - (a.Direction.X * b.Direction.Y));
            float alpha = (offset.X + (beta * b.Direction.X)) / a.Direction.X;

            return ((Math.Clamp(alpha, 0.0f, 1.0f) == alpha) && (Math.Clamp(beta, 0.0f, 1.0f) == beta)) ? b.P0 + (beta * b.Direction) : null;
        }
        public readonly LineSegment Transform(Matrix matrix, Vector2 origin)
        {
            Matrix transform = Matrix.CreateTranslation(-origin.X, -origin.Y, 0f) * matrix * Matrix.CreateTranslation(origin.X, origin.Y, 0f);
            return this.Transform(transform);
        }
        public readonly LineSegment Transform(Matrix matrix)
        {
            return new()
            {
                P0 = Vector2.Transform(this.P0, matrix),
                P1 = Vector2.Transform(this.P1, matrix)
            };
        }
        #endregion
    }
}
