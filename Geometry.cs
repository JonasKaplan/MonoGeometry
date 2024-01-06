using Microsoft.Xna.Framework;

namespace MonoGeometry
{
    public sealed class Geometry
    {
        public static double Distance(Point p1, Point p2) => Geometry.Distance(p1.X, p1.Y, p2.X, p2.Y);
        public static double Distance(Vector2 p1, Vector2 p2) => Geometry.Distance(p1.X, p1.Y, p2.X, p2.Y);
        public static double Distance(float x1, float y1, float x2, float y2) => Math.Sqrt(Geometry.DistanceSquared(x1, y1, x2, y2));
        public static int DistanceSquared(Point p1, Point p2) => Geometry.DistanceSquared(p1.X, p1.Y, p2.X, p2.Y);
        public static double DistanceSquared(Vector2 p1, Vector2 p2) => Geometry.DistanceSquared(p1.X, p1.Y, p2.X, p2.Y);
        public static int DistanceSquared(int x1, int y1, int x2, int y2) => ((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1));
        public static double DistanceSquared(float x1, float y1, float x2, float y2) => ((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1));
    }
}
