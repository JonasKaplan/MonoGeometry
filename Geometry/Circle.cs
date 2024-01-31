using Microsoft.Xna.Framework;

namespace MonoGeometry.Geometry
{
    /// <summary>
    /// Describes a 2D circle
    /// </summary>
    public struct Circle : IEquatable<Circle>
    {
        #region Public properties
        /// <summary>
        /// The center of this <see cref="Circle"/>
        /// </summary>
        public Vector2 Center { get; set; }
        /// <summary>
        /// The radius of this <see cref="Circle"/>
        /// </summary>
        public float Radius { get; set; }
        /// <summary>
        /// The perimeter of this <see cref="Circle"/>
        /// </summary>
        public readonly float Perimeter => 2 * MathHelper.Pi * this.Radius;
        /// <summary>
        /// The area of this <see cref="Circle"/>
        /// </summary>
        public readonly float Area => MathHelper.Pi * this.Radius * this.Radius;
        #endregion
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Circle"/> instance, with a <see cref="this.Center"/> at the origin and a <see cref="this.Radius"/> of zero
        /// </summary>
        public Circle() : this(Vector2.Zero, 0f) { }
        /// <summary>
        /// Creates a new <see cref="Circle"/> instance, with the specified <see cref="this.Center"/> and <see cref="this.Radius"/>
        /// </summary>
        /// <param name="center">The center of the created <see cref="Circle"/></param>
        /// <param name="radius">The radius of the created <see cref="Circle"/></param>
        public Circle(Vector2 center, float radius)
        {
            this.Center = center;
            this.Radius = radius;
        }
        #endregion
        #region Operators
        /// <summary>
        /// Compares weather two <see cref="Circle"/> instances are equal
        /// </summary>
        /// <param name="a"><see cref="Circle"/> instance on the left of the equal sign</param>
        /// <param name="b"><see cref="Circle"/> instance on the right of the equal sign</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise</returns>
        public static bool operator ==(Circle a, Circle b) => (a.Center == b.Center) && (a.Radius == b.Radius);
        /// <summary>
        /// Compares weather two <see cref="Circle"/> instances are not equal
        /// </summary>
        /// <param name="a"><see cref="Circle"/> instance on the left of the not equal sign</param>
        /// <param name="b"><see cref="Circle"/> instance on the right of the not equal sign</param>
        /// <returns><c>true</c> if the instances are not equal; <c>false</c> otherwise</returns>
        public static bool operator !=(Circle a, Circle b) => !(a == b);
        #endregion
        #region Public methods
        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="object"/>
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise</returns>
        public override readonly bool Equals(object? obj) => (obj is Circle circle) && (this == circle);
        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="Circle"/>
        /// </summary>
        /// <param name="other">The <see cref="Circle"/> to compare</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise</returns>
        public readonly bool Equals(Circle other) => this == other;
        /// <summary>
        /// Gets the hash code of this <see cref="Circle"/>
        /// </summary>
        /// <returns>Hash code of this <see cref="Circle"/></returns>
        public override readonly int GetHashCode() => HashCode.Combine(this.Center, this.Radius);
        /// <summary>
        /// Returns a <see cref="string"/> representation of this <see cref="Circle"/> in the format:
        /// {Center: <see cref="this.Center"/>Radius: <see cref="this.Radius"/>}
        /// </summary>
        /// <returns><see cref="string"/> representation of this <see cref="Circle"/></returns>
        public override readonly string ToString() => "{Center:" + this.Center.ToString() + "Radius:" + this.Radius.ToString() + "}";
        /// <summary>
        /// Gets weather or not the specified <see cref="Vector2"/> lies within the bounds of this <see cref="Circle"/>
        /// </summary>
        /// <param name="point">The coordinates to check for inclusion in this <see cref="Circle"/></param>
        /// <returns><c>true</c> if the provided <see cref="Vector2"/> lies inside this <see cref="Circle"/>; <c>false</c> otherwise</returns>
        public readonly bool Contains(Vector2 point) => GeometryHelper.DistanceSquared(point, this.Center) <= this.Radius * this.Radius;
        /// <summary>
        /// Gets whether or not the other <see cref="Circle"/> intersects with this <see cref="Circle"/>
        /// </summary>
        /// <param name="other">The other <see cref="Circle"/> for testing</param>
        /// <returns><c>true</c> if the provided <see cref="Circle"/> intersects this <see cref="Circle"/>; <c>false</c> otherwise</returns>
        public readonly bool Intersects(Circle other) => GeometryHelper.DistanceSquared(this.Center, other.Center) <= (this.Radius + other.Radius) * (this.Radius + other.Radius);
        /// <summary>
        /// Creates a <see cref="Circle"/> with a <see cref="this.Center"/> offset by the provided values
        /// </summary>
        /// <param name="delta">The x and y components to offset the created <see cref="Circle"/> by</param>
        /// <returns>A new <see cref="Circle"/> instance with modified <see cref="this.Center"/></returns>
        public readonly Circle Translate(Vector2 delta)
        {
            Circle translated = this;
            translated.Center += delta;
            return translated;
        }
        /// <summary>
        /// Creates a <see cref="Circle"/> with a <see cref="this.Radius"/> multiplied by the provided value
        /// </summary>
        /// <param name="factor">The factor by which to scale the created <see cref="Circle"/> by</param>
        /// <returns>A new <see cref="Circle"/> instance with modified <see cref="this.Radius"/></returns>
        public readonly Circle Scale(float factor)
        {
            Circle scaled = this;
            scaled.Radius *= factor;
            return scaled;
        }
        #endregion
    }
}
