using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MGPrimitives
{
    public struct IntCircle : IEquatable<IntCircle>
    {
        #region Public fields
        public int X;

        public int Y;

        public int Radius;
        #endregion
        #region Public properties
        public Point Location
        {
            readonly get => new(this.X, this.Y);
            set
            {
                this.X = value.X;
                this.Y = value.Y;
            }
        }

        public readonly Rectangle BoundingRectangle => new(this.X - this.Radius, this.Y - this.Radius, this.Radius * 2, this.Radius * 2);
        #endregion
        #region Constructors
        public IntCircle()
        {
            this.X = 0;
            this.Y = 0;
            this.Radius = 0;
        }
        public IntCircle(Point location, int radius)
        {
            this.X = location.X;
            this.Y = location.Y;
            this.Radius = radius;
        }
        public IntCircle(int x, int y, int radius)
        {
            this.X = x;
            this.Y = y;
            this.Radius = radius;
        }
        #endregion
        #region Operators
        public static bool operator ==(IntCircle a, IntCircle b) => (a.Radius == b.Radius) && (a.Location == b.Location);
        public static bool operator !=(IntCircle a, IntCircle b) => !(a == b);
        #endregion
        #region Private methods
        private static double Distance(int x1, int y1, int x2, int y2) => Math.Sqrt(((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1)));
        #endregion
        #region Public methods
        public readonly override bool Equals(object? obj) => (obj is IntCircle circle) && (this == circle);
        public readonly bool Equals(IntCircle other) => this == other;
        public readonly override int GetHashCode() => HashCode.Combine(this.X, this.Y, this.Radius);
        public readonly bool Contains(float x, float y) => Math.Sqrt(((this.X - x) * (this.X - x)) + ((this.Y - y) * (this.Y - y))) <= this.Radius;
        public readonly bool Contains(int x, int y) => IntCircle.Distance(x, y, this.X, this.Y) <= this.Radius;
        public readonly bool Contains(Vector2 point) => this.Contains(point.X, point.Y);
        public readonly bool Contains(Point point) => this.Contains(point.X, point.Y);
        public readonly bool Intersects(IntCircle circle) => (this.Radius + circle.Radius) >= IntCircle.Distance(circle.X, circle.Y, this.X, this.Y);
        public void Translate(int deltaX, int deltaY)
        {
            this.X += deltaX;
            this.Y += deltaY;
        }
        public void Translate(Point delta) => this.Translate(delta.X, delta.Y);
        public readonly FloatCircle ToFloatCircle() => new(this.X, this.Y, this.Radius);
        public readonly override string ToString() => "{X:" + this.X + " Y:" + this.Y + " Radius:" + this.Radius + "}";
        #endregion
    }
}
