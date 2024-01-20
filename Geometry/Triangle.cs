using Microsoft.Xna.Framework;

namespace MonoGeometry.Geometry
{
    public struct Triangle : IEquatable<Triangle>
    {
        #region Public properties
        public Vector2 P0 { get; set; }
        public Vector2 P1 { get; set; }
        public Vector2 P2 { get; set; }
        public readonly Vector2 Center => new((this.P0.X + this.P1.X + this.P2.X) / 3, (this.P0.Y + this.P1.Y + this.P2.Y) / 3);
        public readonly float Perimeter => (this.P0 - this.P1).Length() + (this.P1 - this.P2).Length() + (this.P2 - this.P0).Length();
        //https://www.cuemath.com/geometry/area-of-triangle-in-coordinate-geometry/
        public readonly float Area => 0.5f * MathF.Abs((this.P0.X * (this.P1.Y - this.P2.Y)) + (this.P1.X * (this.P2.Y - this.P0.Y)) + (this.P2.X * (this.P0.Y - this.P1.Y)));
        #endregion
        #region Constructors
        public Triangle() : this(Vector2.Zero, Vector2.Zero, Vector2.Zero) { }
        public Triangle(Vector2 p0, Vector2 p1, Vector2 p2)
        {
            this.P0 = p0;
            this.P1 = p1;
            this.P2 = p2;
        }
        #endregion
        #region Operators
        public static bool operator ==(Triangle a, Triangle b) => (a.P0 == b.P0) && (a.P1 == b.P1) && (a.P2 == b.P2);
        public static bool operator !=(Triangle a, Triangle b) => !(a == b);
        #endregion
        #region Public methods
        public override readonly bool Equals(object? obj) => (obj is Triangle triangle) && (this == triangle);
        public readonly bool Equals(Triangle other) => this == other;
        public override readonly int GetHashCode() => HashCode.Combine(this.P0, this.P1, this.P2);
        public override readonly string ToString() => "{" + this.P0.ToString() + ", " + this.P1.ToString() + ", " + this.P2.ToString() + "}";
        public readonly bool Contains(Vector2 point)
        {
            Triangle t1 = new(point, this.P0, this.P1);
            Triangle t2 = new(point, this.P1, this.P2);
            Triangle t3 = new(point, this.P2, this.P0);
            return (t1.Area + t2.Area + t3.Area - this.Area) <= 0.0001f;
        }
        public readonly Polygon ToPolygon() => new(new Vector2[] { this.P0, this.P1, this.P2 });
        public static Triangle Transform(Triangle triangle, Matrix matrix, Vector2 origin)
        {
            Triangle returnTriangle;
            returnTriangle = Transform(triangle, Matrix.CreateTranslation(-origin.X, -origin.Y, 0f));
            returnTriangle = Transform(returnTriangle, matrix);
            returnTriangle = Transform(returnTriangle, Matrix.CreateTranslation(origin.X, origin.Y, 0f));
            return returnTriangle;
        }
        public static Triangle Transform(Triangle triangle, Matrix matrix)
        {
            return new(
                Vector2.Transform(triangle.P0, matrix),
                Vector2.Transform(triangle.P1, matrix),
                Vector2.Transform(triangle.P2, matrix));
        }
        #endregion
    }
}
