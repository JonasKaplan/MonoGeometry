using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGeometry.Geometry
{
    /// <summary>
    /// Describes a 2D triangle
    /// </summary>
    public struct Triangle : IEquatable<Triangle>, ITransformable<Triangle>
    {
        #region Public properties
        /// <summary>
        /// The first point of this <see cref="Triangle"/>
        /// </summary>
        public Vector2 P0 { get; set; }
        /// <summary>
        /// The second point of this <see cref="Triangle"/>
        /// </summary>
        public Vector2 P1 { get; set; }
        /// <summary>
        /// The third point of this <see cref="Triangle"/>
        /// </summary>
        public Vector2 P2 { get; set; }
        /// <summary>
        /// The center of this <see cref="Triangle"/>, defined as the averade of all of its x and y coordinates
        /// </summary>
        public readonly Vector2 Center => new((this.P0.X + this.P1.X + this.P2.X) / 3, (this.P0.Y + this.P1.Y + this.P2.Y) / 3);
        /// <summary>
        /// The perimeter of this <see cref="Triangle"/>
        /// </summary>
        public readonly float Perimeter => (this.P0 - this.P1).Length() + (this.P1 - this.P2).Length() + (this.P2 - this.P0).Length();
        /// <summary>
        /// The area of this <see cref="Triangle"/>
        /// </summary>
        //https://www.cuemath.com/geometry/area-of-triangle-in-coordinate-geometry/
        public readonly float Area => 0.5f * MathF.Abs((this.P0.X * (this.P1.Y - this.P2.Y)) + (this.P1.X * (this.P2.Y - this.P0.Y)) + (this.P2.X * (this.P0.Y - this.P1.Y)));
        #endregion
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Triangle"/> instance, with all points at the origin
        /// </summary>
        public Triangle() : this(Vector2.Zero, Vector2.Zero, Vector2.Zero) { }
        /// <summary>
        /// Creates a new <see cref="Triangle"/>, with defined points
        /// </summary>
        /// <param name="p0">The first point of this <see cref="Triangle"/></param>
        /// <param name="p1">The second point of this <see cref="Triangle"/></param>
        /// <param name="p2">The third point of this <see cref="Triangle"/></param>
        public Triangle(Vector2 p0, Vector2 p1, Vector2 p2)
        {
            this.P0 = p0;
            this.P1 = p1;
            this.P2 = p2;
        }
        #endregion
        #region Operators
        /// <summary>
        /// Compares weather two <see cref="Triangle"/> instances are equal
        /// </summary>
        /// <param name="a"><see cref="Triangle"/> instance on the left of the equal sign</param>
        /// <param name="b"><see cref="Triangle"/> instance on the right of the equal sign</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise</returns>
        public static bool operator ==(Triangle a, Triangle b) => (a.P0 == b.P0) && (a.P1 == b.P1) && (a.P2 == b.P2);
        /// <summary>
        /// Compares weather two <see cref="Triangle"/> instances are not equal
        /// </summary>
        /// <param name="a"><see cref="Triangle"/> instance on the left of the not equal sign</param>
        /// <param name="b"><see cref="Triangle"/> instance on the right of the not equal sign</param>
        /// <returns><c>true</c> if the instances are not equal; <c>false</c> otherwise</returns>
        public static bool operator !=(Triangle a, Triangle b) => !(a == b);
        #endregion
        #region Public methods
        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="object"/>
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise</returns>
        public override readonly bool Equals(object? obj) => (obj is Triangle triangle) && (this == triangle);
        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="Triangle"/>
        /// </summary>
        /// <param name="other">The <see cref="Triangle"/> to compare</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise</returns>
        public readonly bool Equals(Triangle other) => this == other;
        /// <summary>
        /// Gets the hash code of this <see cref="Triangle"/>
        /// </summary>                            
        /// <returns>Hash code of this <see cref="Triangle"/></returns>
        public override readonly int GetHashCode() => HashCode.Combine(this.P0, this.P1, this.P2);
        /// <summary>
        /// Returns a <see cref="string"/> representation of this <see cref="Triangle"/> in the format:
        /// {<see cref="this.P0"/>, <see cref="this.P1"/>, <see cref="this.P2"/>}
        /// </summary>
        /// <returns><see cref="string"/> representation of this <see cref="Triangle"/></returns>
        public override readonly string ToString() => "{" + this.P0.ToString() + ", " + this.P1.ToString() + ", " + this.P2.ToString() + "}";
        /// <summary>
        /// Checks if the specified point falls inside this <see cref="Triangle"/>
        /// </summary>
        /// <param name="point">The <see cref="Vector2"/> to be checked for inclusion</param>
        /// <returns><c>true</c> if the point lies inside this <see cref="Triangle"/>; <c>false</c> otherwise</returns>
        public readonly bool Contains(Vector2 point)
        {
            Triangle t1 = new(point, this.P0, this.P1);
            Triangle t2 = new(point, this.P1, this.P2);
            Triangle t3 = new(point, this.P2, this.P0);
            return (t1.Area + t2.Area + t3.Area - this.Area) <= 0.000001f;
        }
        /// <summary>
        /// Creates a <see cref="Polygon"/> instance with identical points to this <see cref="Triangle"/>
        /// </summary>
        /// <returns>A <see cref="Polygon"/> instance with identical points to this <see cref="Triangle"/></returns>
        public readonly Polygon ToPolygon() => new(new Vector2[] { this.P0, this.P1, this.P2 });
        /// <summary>
        /// Creates a new <see cref="Triangle"/> instance, transformed about a given origin by a given matrix
        /// </summary>
        /// <param name="matrix">The transformation to be applied to the <see cref="Triangle"/></param>
        /// <param name="origin">The origin about which to apply the transformation</param>
        /// <returns>A new <see cref="Triangle"/> instance, transformed based on the given parameters</returns>
        public Triangle Transform(Matrix matrix, Vector2 origin)
        {
            Matrix transform = Matrix.CreateTranslation(-origin.X, -origin.Y, 0f) * matrix * Matrix.CreateTranslation(origin.X, origin.Y, 0f);
            return this.Transform(transform);
        }
        /// <summary>
        /// Creates a new <see cref="Triangle"/> instance, transformed by a given matrix
        /// </summary>
        /// <param name="matrix">The transformation to be applied to the <see cref="Triangle"/></param>
        /// <returns>A new <see cref="Triangle"/> instance, transformed based on the given parameters</returns>
        public Triangle Transform(Matrix matrix)
        {
            return new()
            {
                P0 = Vector2.Transform(this.P0, matrix),
                P1 = Vector2.Transform(this.P1, matrix),
                P2 = Vector2.Transform(this.P2, matrix)
            };
        }
        #endregion
    }
}
