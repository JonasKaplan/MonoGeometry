using Microsoft.Xna.Framework;

namespace MonoGeometry.Geometry
{
    public sealed class GeometryHelper
    {
        public static void Transform(ITransformable shape, Matrix transformation) => GeometryHelper.Transform(shape, transformation, shape.Center);
        public static void Transform(ITransformable shape, Matrix transformation, Vector2 origin) => shape.Transform(transformation, origin);
        public static void Translate(ITransformable shape, Vector2 delta) => shape.Transform(Matrix.CreateTranslation(new Vector3(delta, 0f)));
        public static void Rotate(ITransformable shape, float radians) => GeometryHelper.Rotate(shape, radians, shape.Center);
        public static void Rotate(ITransformable shape, float radians, Vector2 origin) => shape.Transform(Matrix.CreateRotationZ(radians), origin);
        public static void Scale(ITransformable shape, float factor) => GeometryHelper.Scale(shape, factor, shape.Center);
        public static void Scale(ITransformable shape, float factor, Vector2 origin) => shape.Transform(Matrix.CreateScale(factor, factor, 1f), origin);
        public static void Scale(ITransformable shape, Vector2 factor) => GeometryHelper.Scale(shape, factor, shape.Center);
        public static void Scale(ITransformable shape, Vector2 factor, Vector2 origin) => shape.Transform(Matrix.CreateScale(factor.X, factor.Y, 1f), origin);

    }
}
