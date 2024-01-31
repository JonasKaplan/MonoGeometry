using Microsoft.Xna.Framework;

namespace MonoGeometry.Geometry
{
    /// <summary>
    /// Interface for transformable 2D shapes
    /// </summary>
    /// <typeparam name="T">Must be the same as the struct or class thast implements this interface</typeparam>
    public interface ITransformable<T>
    {
        /// <summary>
        /// The Center of this shape in 2D space. Usually, the average of all x and y positions. Used as the center for matrix transformations.
        /// </summary>
        public Vector2 Center { get; }
        /// <summary>
        /// Creates a new <c>T</c> instance, transformed about a given origin by a given matrix
        /// </summary>
        /// <param name="matrix">The transformation to be applied to the <see cref="ITransformable{T}"/></param>
        /// <param name="origin">The origin about which to apply the transformation</param>
        /// <returns>A new <c>T</c> instance, transformed based on the given parameters</returns>
        public T Transform(Matrix matrix, Vector2 origin);
        /// <summary>
        /// Creates a new <c>T</c> instance, transformed by a given matrix
        /// </summary>
        /// <param name="matrix">The transformation to be applied to the <see cref="ITransformable{T}"/></param>
        /// <returns>A new <c>T</c> instance, transformed based on the given parameters</returns>
        public T Transform(Matrix matrix);
    }
}
