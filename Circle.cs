using Microsoft.Xna.Framework;

namespace MonoGeometry
{
    public struct Circle : IEquatable<Circle>
    {
        #region Public fields
        public float X;
        public float Y;
        public float Radius;
        #endregion
        #region Public properties
        public Vector2 Location
        {
            readonly get => new(this.X, this.Y);
            set
            {
                this.X = value.X;
                this.Y = value.Y;
            }
        }
        #endregion
        #region Constructors
        public Circle() : this(0f, 0f, 0f) { }
        public Circle(Vector2 location, float radius) : this(location.X, location.Y, radius) { }
        public Circle(float x, float y, float radius)
        {
            this.X = x;
            this.Y = y;
            this.Radius = radius;
        }
        #endregion
        #region Operators
        public static bool operator ==(Circle a, Circle b) => (a.Radius == b.Radius) && (a.Location == b.Location);
        public static bool operator !=(Circle a, Circle b) => !(a == b);
        #endregion
        #region Public methods
        public readonly override bool Equals(object? obj) => (obj is Circle circle) && (this == circle);
        public readonly bool Equals(Circle other) => this == other;
        public readonly override int GetHashCode() => HashCode.Combine(this.X, this.Y, this.Radius);
        public readonly bool Contains(Vector2 point) => this.Contains(point.X, point.Y);
        public readonly bool Contains(float x, float y) =>Geometry.DistanceSquared(x, y, this.X, this.Y) <= this.Radius * this.Radius;
        public readonly bool Intersects(Circle circle) =>
            (this.Radius + circle.Radius) * (this.Radius + circle.Radius) >= Geometry.DistanceSquared(circle.X, circle.Y, this.X, this.Y);
        public void Translate(float deltaX, float deltaY)
        {
            this.X += deltaX;
            this.Y += deltaY;
        }
        public void Translate(Vector2 delta) => this.Translate(delta.X, delta.Y);
        public void Scale(float factor) => this.Radius *= factor;
        public readonly override string ToString() => "{X:" + this.X + " Y:" + this.Y + " Radius:" + this.Radius + "}";
        #endregion
    }
}
