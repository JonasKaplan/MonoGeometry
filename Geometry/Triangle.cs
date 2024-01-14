using Microsoft.Xna.Framework;

namespace MonoGeometry.Geometry
{
    public struct Triangle : IEquatable<Triangle>, ITransformable
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
        public readonly double Perimeter
        {
            get
            {
                double d1 = Geometry.Distance(this.P1X, this.P1Y, this.P2X, this.P2Y);
                double d2 = Geometry.Distance(this.P2X, this.P2Y, this.P3X, this.P3Y);
                double d3 = Geometry.Distance(this.P3X, this.P3Y, this.P1X, this.P1Y);
                return d1 + d2 + d3;
            }
        }
        public readonly double Area => 0.5 * Math.Abs((this.P1X * (this.P2Y - this.P3Y)) + (this.P2X * (this.P3Y - this.P1Y)) + (this.P3X * (this.P1Y - this.P2Y)));
        public readonly Vector2 Center => new((this.P1X + this.P2X + this.P3X) / 3, (this.P1Y + this.P2Y + this.P3Y) / 3);
        #endregion
        #region Constructors
        public Triangle() : this(0f, 0f, 0f, 0f, 0f, 0f) { }
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
        public static bool operator ==(Triangle a, Triangle b) => a.P1X == b.P1X && a.P1Y == b.P1Y && a.P2X == b.P2X && a.P2Y == b.P2Y && a.P3X == b.P3X && a.P3Y == b.P3Y;
        public static bool operator !=(Triangle a, Triangle b) => !(a == b);
        #endregion
        #region Public methods
        public readonly override bool Equals(object? obj) => obj is Triangle triangle && this == triangle;
        public readonly bool Equals(Triangle other) => this == other;
        public readonly override int GetHashCode() => HashCode.Combine(this.P1X, this.P1Y, this.P2X, this.P2Y, this.P3X, this.P3Y);
        public readonly bool Contains(Vector2 point) => this.Contains(point.X, point.Y);
        public readonly bool Contains(float x, float y)
        {
            Vector2 point = new(x, y);
            Triangle t1 = new(point, this.P1, this.P2);
            Triangle t2 = new(point, this.P2, this.P3);
            Triangle t3 = new(point, this.P3, this.P1);
            return Math.Abs(t1.Area + t2.Area + t3.Area - this.Area) >= 0.0001;
        }
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
            this.P3 = Vector2.Transform(this.P3, transform);
        }
        #endregion
    }
}
