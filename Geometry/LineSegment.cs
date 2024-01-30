using Microsoft.Xna.Framework;

namespace MonoGeometry.Geometry
{
    public struct LineSegment : IEquatable<LineSegment>, ITransformable<LineSegment>
    {
        #region Constants
        private enum TripletType
        {
            Clockwise,
            Counterclockwise,
            Collinear
        }
        #endregion
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
        public readonly bool Contains(Vector2 point) =>
            (point.X <= MathF.Max(this.P0.X, this.P1.X)) && (point.X >= MathF.Min(this.P0.X, this.P1.X)) && (point.Y <= MathF.Max(this.P0.Y, this.P1.Y)) && (point.Y >= MathF.Min(this.P0.Y, this.P1.Y));
        private static TripletType Type(Vector2 a, Vector2 b, Vector2 c)
        {
            float check = ((b.Y - a.Y) * (c.X - b.X)) - ((b.X - a.X) * (c.Y - b.Y));
            if (MathF.Abs(check) <= 0.00001f) return TripletType.Collinear;
            return check > 0 ? TripletType.Clockwise : TripletType.Counterclockwise;
        }
        public static bool Intersect(LineSegment a, LineSegment b)
        {
            //Algorithm: https://www.geeksforgeeks.org/check-if-two-given-line-segments-intersect/
            TripletType t0 = Type(a.P0, a.P1, b.P0);
            TripletType t1 = Type(a.P0, a.P1, b.P1);
            TripletType t2 = Type(b.P0, b.P1, a.P0);
            TripletType t3 = Type(b.P0, b.P1, a.P1);

            if ((t0 != t1) && (t2 != t3)) return true;

            if ((t0 == TripletType.Collinear) && a.Contains(b.P0)) return true;
            if ((t1 == TripletType.Collinear) && a.Contains(b.P1)) return true;
            if ((t2 == TripletType.Collinear) && b.Contains(a.P0)) return true;
            if ((t3 == TripletType.Collinear) && b.Contains(a.P1)) return true;

            return false;
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
