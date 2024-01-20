using Microsoft.Xna.Framework;

namespace MonoGeometry.Geometry
{
    public struct LineSegment : IEquatable<LineSegment>
    {
        #region Public properties
        public Vector2 P0 { get; set; }
        public Vector2 P1 { get; set; }
        public readonly float Length => MathF.Sqrt(this.LengthSquared);
        public readonly float LengthSquared => ((this.P1.X - this.P0.X) * (this.P1.X - this.P0.X)) + ((this.P1.Y - this.P0.Y) * (this.P1.Y - this.P0.Y));
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
        public static LineSegment Transform(LineSegment lineSegment, Matrix matrix, Vector2 origin)
        {
            LineSegment returnLineSegment;
            returnLineSegment = Transform(lineSegment, Matrix.CreateTranslation(-origin.X, -origin.Y, 0f));
            returnLineSegment = Transform(returnLineSegment, matrix);
            returnLineSegment = Transform(returnLineSegment, Matrix.CreateTranslation(origin.X, origin.Y, 0f));
            return returnLineSegment;
        }
        public static LineSegment Transform(LineSegment triangle, Matrix matrix)
        {
            return new(
                Vector2.Transform(triangle.P0, matrix),
                Vector2.Transform(triangle.P1, matrix));
        }
        #endregion
    }
}
