using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MGPrimitives
{
    public struct FloatCircle : IEquatable<FloatCircle>
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

        public readonly Rectangle BoundingRectangle => new((int)(this.X - this.Radius), (int)(this.Y - this.Radius), (int)(this.Radius * 2), (int)(this.Radius * 2));
        #endregion
        #region Constructors
        public FloatCircle()
        {
            this.X = 0f;
            this.Y = 0f;
            this.Radius = 0f;
        }
        public FloatCircle(Vector2 location, float radius)
        {
            this.X = location.X;
            this.Y = location.Y;
            this.Radius = radius;
        }
        public FloatCircle(Point location, float radius)
        {
            this.X = location.X;
            this.Y = location.Y;
            this.Radius = radius;
        }
        public FloatCircle(float x, float y, float radius)
        {
            this.X = x;
            this.Y = y;
            this.Radius = radius;
        }
        #endregion
        #region Operators
        public static bool operator ==(FloatCircle a, FloatCircle b) => (a.Radius == b.Radius) && (a.Location == b.Location);
        public static bool operator !=(FloatCircle a, FloatCircle b) => !(a == b);
        #endregion
        #region Private methods
        private static double Distance(float x1, float y1, float x2, float y2) => Math.Sqrt(((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1)));
        #endregion
        #region Public methods
        public readonly override bool Equals(object? obj) => (obj is FloatCircle circle) && (this == circle);
        public readonly bool Equals(FloatCircle other) => this == other;
        public readonly override int GetHashCode() => HashCode.Combine(this.X, this.Y, this.Radius);
        public readonly bool Contains(float x, float y) => FloatCircle.Distance(x, y, this.X, this.Y) <= this.Radius;
        public readonly bool Contains(int x, int y) => FloatCircle.Distance(x, y, this.X, this.Y) <= this.Radius;
        public readonly bool Contains(Vector2 point) => this.Contains(point.X, point.Y);
        public readonly bool Contains(Point point) => this.Contains(point.X, point.Y);
        public readonly bool Intersects(FloatCircle circle) => (this.Radius + circle.Radius) >= FloatCircle.Distance(circle.X, circle.Y, this.X, this.Y);
        public void Translate(float deltaX, float deltaY)
        {
            this.X += deltaX;
            this.Y += deltaY;
        }
        public void Translate(Vector2 delta) => this.Translate(delta.X, delta.Y);
        public readonly IntCircle ToIntCircle(bool roundUp)
        {
            return roundUp
                ? new((int)Math.Ceiling(this.X), (int)Math.Ceiling(this.Y), (int)Math.Ceiling(this.Radius))
                : new((int)Math.Floor(this.X), (int)Math.Floor(this.Y), (int)Math.Floor(this.Radius));
        }
        public readonly IntCircle ToIntCircle() => this.ToIntCircle(false);
        public readonly override string ToString() => "{X:" + this.X + " Y:" + this.Y + " Radius:" + this.Radius + "}";
        #endregion
    }
}
