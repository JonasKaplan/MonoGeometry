using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGeometry.Geometry
{
    public struct LineSegment : IEquatable<LineSegment>, ITransformable
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
        internal void Transform(Matrix matrix, Vector2 origin)
        {
            this.Transform(Matrix.CreateTranslation(-origin.X, -origin.Y, 0f));
            this.Transform(matrix);
            this.Transform(Matrix.CreateTranslation(origin.X, origin.Y, 0f));
        }
        internal void Transform(Matrix matrix)
        {
            this.P0 = Vector2.Transform(this.P0, matrix);
            this.P1 = Vector2.Transform(this.P1, matrix);
        }
        #endregion
    }
}
