using Microsoft.Xna.Framework;

namespace MonoGeometry.Geometry
{
    public sealed class Geometry
    {
        public static float Distance(Point p1, Point p2) => Distance(p1.X, p1.Y, p2.X, p2.Y);
        public static float Distance(Vector2 p1, Vector2 p2) => Distance(p1.X, p1.Y, p2.X, p2.Y);
        public static float Distance(float x1, float y1, float x2, float y2) => MathF.Sqrt(DistanceSquared(x1, y1, x2, y2));
        public static int DistanceSquared(Point p1, Point p2) => DistanceSquared(p1.X, p1.Y, p2.X, p2.Y);
        public static float DistanceSquared(Vector2 p1, Vector2 p2) => DistanceSquared(p1.X, p1.Y, p2.X, p2.Y);
        public static int DistanceSquared(int x1, int y1, int x2, int y2) => ((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1));
        public static float DistanceSquared(float x1, float y1, float x2, float y2) => ((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1));
        public static void Rotate(ITransformable figure, float radians) => Rotate(figure, radians, 0f, 0f);
        public static void Rotate(ITransformable figure, float radians, Vector2 origin) => Rotate(figure, radians, origin.X, origin.Y);
        public static void Rotate(ITransformable figure, float radians, float originX, float originY) => figure.Transform(Matrix.CreateRotationZ(radians), originX, originY);
        public static void Translate(ITransformable figure, Vector2 delta) => Translate(figure, delta.X, delta.Y);
        public static void Translate(ITransformable figure, float deltaX, float deltaY) => figure.Transform(Matrix.CreateTranslation(deltaX, deltaY, 0f));
        public static void Scale(ITransformable figure, float factor) => Scale(figure, factor, factor);
        public static void Scale(ITransformable figure, Vector2 factor) => Scale(figure, factor.X, factor.Y);
        public static void Scale(ITransformable figure, float factorX, float factorY) => Scale(figure, factorX, factorY, 0f, 0f);
        public static void Scale(ITransformable figure, Vector2 factor, Vector2 origin) => Scale(figure, factor.X, factor.Y, origin.X, origin.Y);
        public static void Scale(ITransformable figure, float factorX, float factorY, Vector2 origin) => Scale(figure, factorX, factorY, origin.X, origin.Y);
        public static void Scale(ITransformable figure, Vector2 factor, float originX, float originY) => Scale(figure, factor.X, factor.Y, originX, originY);
        public static void Scale(ITransformable figure, float factorX, float factorY, float originX, float originY) => figure.Transform(Matrix.CreateScale(factorX, factorY, 1f), originX, originY);
    }
}
