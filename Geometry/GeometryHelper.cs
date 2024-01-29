using Microsoft.Xna.Framework;

namespace MonoGeometry.Geometry
{
    public sealed class GeometryHelper
    {
        public static T Translate<T>(ITransformable<T> shape, Vector2 delta) => shape.Transform(Matrix.CreateTranslation(new Vector3(delta, 0f)));
        public static T Rotate<T>(ITransformable<T> shape, float radians) => GeometryHelper.Rotate(shape, radians, shape.Center);
        public static T Rotate<T>(ITransformable<T> shape, float radians, Vector2 origin) => shape.Transform(Matrix.CreateRotationZ(radians), origin);
        public static T Scale<T>(ITransformable<T> shape, float factor) => GeometryHelper.Scale(shape, factor, shape.Center);
        public static T Scale<T>(ITransformable<T> shape, float factor, Vector2 origin) => shape.Transform(Matrix.CreateScale(factor, factor, 1f), origin);
        public static T Scale<T>(ITransformable<T> shape, Vector2 factor) => GeometryHelper.Scale(shape, factor, shape.Center);
        public static T Scale<T>(ITransformable<T> shape, Vector2 factor, Vector2 origin) => shape.Transform(Matrix.CreateScale(factor.X, factor.Y, 1f), origin);
        public static float DistanceSquared(Vector2 p0, Vector2 p1) => GeometryHelper.DistanceSquared(p0.X, p0.Y, p1.X, p1.Y);
        public static float DistanceSquared(float p0X, float p0Y, float p1X, float p1Y) => ((p1X - p0X) * (p1X - p0X)) + ((p1Y - p0Y) * (p1Y - p0Y));
        public static float Distance(Vector2 p0, Vector2 p1) => GeometryHelper.DistanceSquared(p0.X, p0.Y, p1.X, p1.Y);
        public static float Distance(float p0X, float p0Y, float p1X, float p1Y) => MathF.Sqrt(GeometryHelper.DistanceSquared(p0X, p0Y, p1X, p1Y));
    }
}
