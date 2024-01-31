using Microsoft.Xna.Framework;

namespace MonoGeometry.Geometry
{
    public sealed class GeometryHelper
    {
        /// <summary>
        /// Creates a new <c>T</c> instance translated by the x and y values of delta
        /// </summary>
        /// <typeparam name="T">A struct that implements the <see cref="ITransformable{T}"/> interface</typeparam>
        /// <param name="shape">The instance to be translated</param>
        /// <param name="delta">The x and y valued to add to the coordinates of the shape</param>
        /// <returns>A <c>T</c> instance translated by the given delta</returns>
        public static T Translate<T>(ITransformable<T> shape, Vector2 delta) => shape.Transform(Matrix.CreateTranslation(new Vector3(delta, 0f)));
        /// <summary>
        /// Creates a new <c>T</c> instance rotated around its center by the provided radian value
        /// </summary>
        /// <typeparam name="T">A struct that implements the <see cref="ITransformable{T}"/> interface</typeparam>
        /// <param name="shape">The instance to be rotated</param>
        /// <param name="radians">The number of radians by which to rotate the shape</param>
        /// <returns>A <c>T</c> instance rotated by the given number of radians</returns>
        public static T Rotate<T>(ITransformable<T> shape, float radians) => GeometryHelper.Rotate(shape, radians, shape.Center);
        /// <summary>
        /// Creates a new <c>T</c> instance rotated around a specified origin by the provided radian value
        /// </summary>
        /// <typeparam name="T">A struct that implements the <see cref="ITransformable{T}"/> interface</typeparam>
        /// <param name="shape">The instance to be rotated</param>
        /// <param name="radians">The number of radians by which to rotate the shape</param>
        /// <param name="origin">The origin about which to rotate the shape</param>
        /// <returns>A <c>T</c> instance rotated by the given number of radians, about the given origin</returns>
        public static T Rotate<T>(ITransformable<T> shape, float radians, Vector2 origin) => shape.Transform(Matrix.CreateRotationZ(radians), origin);
        /// <summary>
        /// Creates a new <c>T</c> instance scaled about its center by a given factor
        /// </summary>
        /// <typeparam name="T">A struct that implements the <see cref="ITransformable{T}"/> interface</typeparam>
        /// <param name="shape">The instance to be scaled</param>
        /// <param name="factor">The factor by which to scale the shape</param>
        /// <returns>A <c>T</c> instance scaled by the given factor</returns>
        public static T Scale<T>(ITransformable<T> shape, float factor) => GeometryHelper.Scale(shape, factor, shape.Center);
        /// <summary>
        /// Creates a new <c>T</c> instance scaled about a given origin by a given factor
        /// </summary>
        /// <typeparam name="T">A struct that implements the <see cref="ITransformable{T}"/> interface</typeparam>
        /// <param name="shape">The instance to be scaled</param>
        /// <param name="factor">The factor by which to scale the shape</param>
        /// <param name="origin">The origin about which to scale the shape</param>
        /// <returns>A <c>T</c> instance scaled by the given factor, about the given origin</returns>
        public static T Scale<T>(ITransformable<T> shape, float factor, Vector2 origin) => shape.Transform(Matrix.CreateScale(factor, factor, 1f), origin);
        /// <summary>
        /// Creates a new <c>T</c> instance scaled about its center by a given factor
        /// </summary>
        /// <typeparam name="T">A struct that implements the <see cref="ITransformable{T}"/> interface</typeparam>
        /// <param name="shape">The instance to be scaled</param>
        /// <param name="factor">The factor by which to scale the shape</param>
        /// <returns>A <c>T</c> instance scaled by the given factor</returns>
        public static T Scale<T>(ITransformable<T> shape, Vector2 factor) => GeometryHelper.Scale(shape, factor, shape.Center);
        /// <summary>
        /// Creates a new <c>T</c> instance scaled about a given origin by a given factor
        /// </summary>
        /// <typeparam name="T">A struct that implements the <see cref="ITransformable{T}"/> interface</typeparam>
        /// <param name="shape">The instance to be scaled</param>
        /// <param name="factor">The factor by which to scale the shape</param>
        /// <param name="origin">The origin about which to scale the shape</param>
        /// <returns>A <c>T</c> instance scaled by the given factor, about the given origin</returns>
        public static T Scale<T>(ITransformable<T> shape, Vector2 factor, Vector2 origin) => shape.Transform(Matrix.CreateScale(factor.X, factor.Y, 1f), origin);
        /// <summary>
        /// Finds the square of the distance between two <see cref="Vector2"/>s in 2D space
        /// </summary>
        /// <param name="p0">The first <see cref="Vector2"/></param>
        /// <param name="p1">The second <see cref="Vector2"/></param>
        /// <returns>The square of the distance between the two provided <see cref="Vector2"/>s</returns>
        public static float DistanceSquared(Vector2 p0, Vector2 p1) => (p1 - p0).LengthSquared();
        /// <summary>
        /// Finds the square of the distance between two points in 2D space, defined by the given values
        /// </summary>
        /// <param name="p0X">The x component of the first point</param>
        /// <param name="p0Y">The y component of the first point</param>
        /// <param name="p1X">The x component of the second point</param>
        /// <param name="p1Y">The y component of the second point</param>
        /// <returns>The square of the distance between the two provided points</returns>
        public static float DistanceSquared(float p0X, float p0Y, float p1X, float p1Y) => ((p1X - p0X) * (p1X - p0X)) + ((p1Y - p0Y) * (p1Y - p0Y));
        /// <summary>
        /// Finds the distance between two <see cref="Vector2"/>s in 2D space
        /// </summary>
        /// <param name="p0">The first <see cref="Vector2"/></param>
        /// <param name="p1">The second <see cref="Vector2"/></param>
        /// <returns>The distance between the two provided <see cref="Vector2"/>s</returns>
        public static float Distance(Vector2 p0, Vector2 p1) => (p1 - p0).Length();
        /// <summary>
        /// Finds the distance between two points in 2D space, defined by the given values
        /// </summary>
        /// <param name="p0X">The x component of the first point</param>
        /// <param name="p0Y">The y component of the first point</param>
        /// <param name="p1X">The x component of the second point</param>
        /// <param name="p1Y">The y component of the second point</param>
        /// <returns>The distance between the two provided points</returns>
        public static float Distance(float p0X, float p0Y, float p1X, float p1Y) => MathF.Sqrt(GeometryHelper.DistanceSquared(p0X, p0Y, p1X, p1Y));
    }
}
