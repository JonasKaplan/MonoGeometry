using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics.CodeAnalysis;

namespace MGPrimitives
{
    public struct Circle : IEquatable<Circle>
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
        public Circle()
        {
            this.X = 0;
            this.Y = 0;
            this.Radius = 0;
        }
        public Circle(Point location, int radius)
        {
            this.X = location.X;
            this.Y = location.Y;
            this.Radius = radius;
        }
        public Circle(int x, int y, int radius)
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
        #region Private methods
        private static double Distance(double x1, double y1, int x2, int y2) => Math.Sqrt(((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1)));
        private static double Distance(int x1, int y1, int x2, int y2) => Math.Sqrt(((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1)));
        #endregion
        #region Public methods
        public readonly override bool Equals(object? obj) => (obj is Circle circle) && (this == circle);
        public readonly bool Equals(Circle other) => this == other;
        public readonly override int GetHashCode() => HashCode.Combine(this.X, this.Y, this.Radius);
        public readonly bool Contains(double x, double y) => Distance(x, y, this.X, this.Y) <= this.Radius;
        public readonly bool Contains(float x, float y) => Distance(x, y, this.X, this.Y) <= this.Radius;
        public readonly bool Contains(int x, int y) => Distance(x, y, this.X, this.Y) <= this.Radius;
        public readonly bool Contains(Point point) => this.Contains(point.X, point.Y);
        public readonly bool Intersects(Circle circle) => (this.Radius + circle.Radius) >= Distance(circle.X, circle.Y, this.X, this.Y);
        public void Translate(int deltaX, int deltaY)
        {
            this.X += deltaX;
            this.Y += deltaY;
        }
        public void Translate(Point delta) => this.Translate(delta.X, delta.Y);
        public readonly override string ToString() => "{X:" + this.X + " Y:" + this.Y + " Radius:" + this.Radius + "}";
        #endregion
    }
}
