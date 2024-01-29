using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        public static bool Intersect(LineSegment a, LineSegment b)
        {
            Triangle t0 = new(a.P0, b.P0, b.P1);
            Triangle t1 = new(a.P1, b.P0, b.P1);
            Triangle t2 = new(a.P0, a.P1, b.P0);
            Triangle t3 = new(a.P0, a.P1, b.P1);
            return MathF.Abs(t0.Area + t1.Area - t2.Area - t3.Area) <= 0.00001f;
        }
        public readonly LineSegment Transform(Matrix matrix, Vector2 origin)
        {
            LineSegment transformed = this.Transform(Matrix.CreateTranslation(-origin.X, -origin.Y, 0f));
            transformed = transformed.Transform(matrix);
            transformed = transformed.Transform(Matrix.CreateTranslation(origin.X, origin.Y, 0f));
            return transformed;
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
