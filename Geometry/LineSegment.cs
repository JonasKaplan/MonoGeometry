using Microsoft.Xna.Framework;

namespace MonoGeometry.Geometry
{
    /// <summary>
    /// Describes a 2D line segment
    /// </summary>
    public struct LineSegment : IEquatable<LineSegment>, ITransformable<LineSegment>
    {
        #region Public properties
        /// <summary>
        /// The start of this <see cref="LineSegment"/>
        /// </summary>
        public Vector2 P0 { get; set; }
        /// <summary>
        /// The end of this <see cref="LineSegment"/>
        /// </summary>
        public Vector2 P1 { get; set; }
        /// <summary>
        /// The length of this <see cref="LineSegment"/>
        /// </summary>
        public readonly float Length => MathF.Sqrt(this.LengthSquared);
        /// <summary>
        /// The length of this <see cref="LineSegment"/> squared
        /// </summary>
        public readonly float LengthSquared => ((this.P1.X - this.P0.X) * (this.P1.X - this.P0.X)) + ((this.P1.Y - this.P0.Y) * (this.P1.Y - this.P0.Y));
        /// <summary>
        /// The center of this <see cref="LineSegment"/>
        /// </summary>
        public readonly Vector2 Center => 0.5f * (this.P0 + this.P1);
        /// <summary>
        /// The <see cref="Vector2"/> defining the direction in which this <see cref="LineSegment"/> points
        /// </summary>
        public readonly Vector2 Direction => this.P1 - this.P0;
        #endregion
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="LineSegment"/> instance, with both points at the origin
        /// </summary>
        public LineSegment() : this(Vector2.Zero, Vector2.Zero) { }
        /// <summary>
        /// Creates a new <see cref="LineSegment"/>, with a defined start and end point
        /// </summary>
        /// <param name="p0">The start point of this <see cref="LineSegment"/></param>
        /// <param name="p1">The end point of this <see cref="LineSegment"/></param>
        public LineSegment(Vector2 p0, Vector2 p1)
        {
            this.P0 = p0;
            this.P1 = p1;
        }
        #endregion
        #region Operators
        /// <summary>
        /// Compares weather two <see cref="LineSegment"/> instances are equal
        /// </summary>
        /// <param name="a"><see cref="LineSegment"/> instance on the left of the equal sign</param>
        /// <param name="b"><see cref="LineSegment"/> instance on the right of the equal sign</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise</returns>
        public static bool operator ==(LineSegment a, LineSegment b) => (a.P0 == b.P0) && (a.P1 == b.P1);
        /// <summary>
        /// Compares weather two <see cref="LineSegment"/> instances are not equal
        /// </summary>
        /// <param name="a"><see cref="LineSegment"/> instance on the left of the not equal sign</param>
        /// <param name="b"><see cref="LineSegment"/> instance on the right of the not equal sign</param>
        /// <returns><c>true</c> if the instances are not equal; <c>false</c> otherwise</returns>
        public static bool operator !=(LineSegment a, LineSegment b) => !(a == b);
        #endregion
        #region Public methods
        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="object"/>
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise</returns>
        public override readonly bool Equals(object? obj) => (obj is LineSegment lineSegment) && (this == lineSegment);
        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="LineSegment"/>
        /// </summary>
        /// <param name="other">The <see cref="LineSegment"/> to compare</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise</returns>
        public readonly bool Equals(LineSegment other) => this == other;
        /// <summary>
        /// Gets the hash code of this <see cref="LineSegment"/>
        /// </summary>
        /// <returns>Hash code of this <see cref="LineSegment"/></returns>
        public override readonly int GetHashCode() => HashCode.Combine(this.P0, this.P1);
        /// <summary>
        /// Returns a <see cref="string"/> representation of this <see cref="LineSegment"/> in the format:
        /// {<see cref="this.P0"/>, <see cref="this.P1"/>}
        /// </summary>
        /// <returns><see cref="string"/> representation of this <see cref="LineSegment"/></returns>
        public override readonly string ToString() => "{" + this.P0.ToString() + ", " + this.P1.ToString() + "}";
        /// <summary>
        /// Checks if the bounding box of this <see cref="LineSegment"/> contains the specified point
        /// </summary>
        /// <param name="point">The <see cref="Vector2"/> to be checked for inclusion</param>
        /// <returns><c>true</c> if the point is contained in the bounding box; <c>false</c> otherwise</returns>
        public readonly bool BoundingBoxContains(Vector2 point) =>
            (point.X <= MathF.Max(this.P0.X, this.P1.X)) && (point.X >= MathF.Min(this.P0.X, this.P1.X)) && (point.Y <= MathF.Max(this.P0.Y, this.P1.Y)) && (point.Y >= MathF.Min(this.P0.Y, this.P1.Y));
        /// <summary>
        /// Checks if the specified point falls on this <see cref="LineSegment"/>
        /// </summary>
        /// <param name="point">The <see cref="Vector2"/> to be checked for inclusion</param>
        /// <returns><c>true</c> if the point lies on this <see cref="LineSegment"/>; <c>false</c> otherwise</returns>
        public readonly bool Contains(Vector2 point)
        {
            if (!this.BoundingBoxContains(point)) return false;
            float lambdaX = (point.X - this.P0.X) / this.Direction.X;
            float lambdaY = (point.Y - this.P0.Y) / this.Direction.Y;
            return MathF.Abs(lambdaX - lambdaY) <= 0.00001f;
        }
        /// <summary>
        /// Checks if two <see cref="LineSegment"/> instances are parallel
        /// </summary>
        /// <param name="a">The first <see cref="LineSegment"/> instance</param>
        /// <param name="b">The second <see cref="LineSegment"/> instance</param>
        /// <returns><c>true</c> if the two <see cref="LineSegment"/>s are parallel; <c>false</c> otherwise</returns>
        public static bool AreParallel(LineSegment a, LineSegment b) => MathF.Abs((a.Direction.X / b.Direction.X) - (a.Direction.Y / b.Direction.Y)) <= 0.00001f;
        /// <summary>
        /// Checks if two <see cref="LineSegment"/> instances are collinear
        /// </summary>
        /// <param name="a">The first <see cref="LineSegment"/> instance</param>
        /// <param name="b">The second <see cref="LineSegment"/> instance</param>
        /// <returns><c>true</c> if the two <see cref="LineSegment"/>s are collinear; <c>false</c> otherwise</returns>
        public static bool AreCollinear(LineSegment a, LineSegment b)
        {
            bool b0OnLine = MathF.Abs(((b.P0.X - a.P0.X) / a.Direction.X) - ((b.P0.Y - a.P0.Y) / a.Direction.Y)) <= 0.00001f;
            bool b1OnLine = MathF.Abs(((b.P1.X - a.P0.X) / a.Direction.X) - ((b.P1.Y - a.P0.Y) / a.Direction.Y)) <= 0.00001f;
            return b0OnLine && b1OnLine;
        }
        /// <summary>
        /// Finds the intersection point between two <see cref="LineSegment"/> instances
        /// </summary>
        /// <param name="a">The first <see cref="LineSegment"/> instance</param>
        /// <param name="b">The second <see cref="LineSegment"/> instance</param>
        /// <returns>The <see cref="Vector2"/> representing the intersection point between the two <see cref="LineSegment"/> instances; <c>null</c> if they do not intersect or are colliner and intersecting</returns>
        public static Vector2? Intersection(LineSegment a, LineSegment b)
        {
            if ((a.P0 == b.P0) && (a.P1 != b.P1)) return a.P0;
            if ((a.P1 == b.P1) && (a.P0 != b.P0)) return a.P1;
            if ((a.P1 == b.P0) && (a.P0 != b.P1)) return a.P1;
            if ((a.P0 == b.P1) && (a.P1 != b.P0)) return a.P0;

            if (LineSegment.AreParallel(a, b)) return null;

            //Algorithm derived using linear algebra
            Vector2 offset = b.P0 - a.P0;
            float beta = ((a.Direction.X * offset.Y) - (a.Direction.Y * offset.X)) / ((a.Direction.Y * b.Direction.X) - (a.Direction.X * b.Direction.Y));
            float alpha = (offset.X + (beta * b.Direction.X)) / a.Direction.X;

            return ((Math.Clamp(alpha, 0.0f, 1.0f) == alpha) && (Math.Clamp(beta, 0.0f, 1.0f) == beta)) ? b.P0 + (beta * b.Direction) : null;
        }
        /// <summary>
        /// Creates a new <see cref="LineSegment"/> instance, transformed about a given origin by a given matrix
        /// </summary>
        /// <param name="matrix">The transformation to be applied to the <see cref="LineSegment"/></param>
        /// <param name="origin">The origin about which to apply the transformation</param>
        /// <returns>A new <see cref="LineSegment"/> instance, transformed based on the given parameters</returns>
        public readonly LineSegment Transform(Matrix matrix, Vector2 origin)
        {
            Matrix transform = Matrix.CreateTranslation(-origin.X, -origin.Y, 0f) * matrix * Matrix.CreateTranslation(origin.X, origin.Y, 0f);
            return this.Transform(transform);
        }
        /// <summary>
        /// Creates a new <see cref="LineSegment"/> instance, transformed by a given matrix
        /// </summary>
        /// <param name="matrix">The transformation to be applied to the <see cref="LineSegment"/></param>
        /// <returns>A new <see cref="LineSegment"/> instance, transformed based on the given parameters</returns>
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
