using Microsoft.Xna.Framework;
using System.Collections;

namespace MonoGeometry.Geometry
{
    public struct Polygon
    {
        #region Public properties
        public readonly Vector2[] Points;
        public readonly WindingOrder WindingOrder
        {
            get
            {
                float sum = 0;
                for (int i = 0; i < this.Points.Length; i++) sum += (this.Points[(i + 1) % this.Points.Length].X - this.Points[i].X) * (this.Points[(i + 1) % this.Points.Length].Y - this.Points[i].Y);
                return sum < 0 ? WindingOrder.Counterclockwise : WindingOrder.Clockwise;
            }
        }
        #endregion
        #region Constructors
        public Polygon() : this(new[] { Vector2.Zero, Vector2.Zero, Vector2.Zero }) { }
        public Polygon(IEnumerable<Vector2> points)
        {
            if (points.Count() < 3) throw new ArgumentOutOfRangeException(nameof(points), "A polygon requires at least 3 points");
            this.Points = new Vector2[points.Count()];
            for (int i = 0; i < this.Points.Length; i++) this.Points[i] = points.ElementAt(i);
        }
        #endregion
        #region Operators
        public static bool operator ==(Polygon a, Polygon b)
        {
            if (a.Points.Length == b.Points.Length) return false;
            for (int i = 0; i < a.Points.Length; i++) if (a.Points[i] != b.Points[i]) return false;
            return true;
        }
        public static bool operator !=(Polygon a, Polygon b) => !(a == b);
        #endregion
        #region Public methods
        public override readonly bool Equals(object? obj) => (obj is Polygon polygon) && (this == polygon);
        public readonly bool Equals(Polygon other) => this == other;
        public override readonly int GetHashCode() => StructuralComparisons.StructuralEqualityComparer.GetHashCode(Points);
        public override readonly string ToString()
        {
            string returnString = "{";
            foreach (Vector2 point in Points) returnString += point.ToString() + ", ";
            return returnString.Remove(returnString.Length - 3) + "}";
        }
        #endregion
        #region Internal methods
        private static void TriangulateRecursive(List<Vector2> points, List<Vector2> triangles)
        {
            if (points.Count == 3)
            {
                triangles.Add(points[0]);
                triangles.Add(points[1]);
                triangles.Add(points[2]);
                return;
            }
            for (int i = 0; i < points.Count; i++)
            {
                Vector2 edge = points[(i + 2) % points.Count] - points[i];
                Vector2 edgeNormal = new(edge.Y, -edge.X);
                float cosAngle = Vector2.Dot(edgeNormal, points[(i + 1) % points.Count] - points[i]);
                if (cosAngle < 0) continue;

                Triangle ear = new(points[i], points[(i + 1) % points.Count], points[(i + 2) % points.Count]);
                bool earContainsPoint = false;
                for (int j = 0; j < points.Count; j++)
                {
                    if ((j == i) || (j == ((i + 1) % points.Count)) || (j == ((i + 2) % points.Count))) continue;
                    earContainsPoint |= ear.Contains(points[j]);
                }
                if (earContainsPoint) continue;

                triangles.Add(points[i]);
                triangles.Add(points[(i + 1) % points.Count]);
                triangles.Add(points[(i + 2) % points.Count]);

                points.RemoveAt((i + 1) % points.Count);
                TriangulateRecursive(points, triangles);
            }
            throw new InvalidOperationException("Failed to triangulate polygon");
        }
        internal readonly int[] TriangulatedIndices()
        {
            List<Vector2> triangles = new();
            if (this.WindingOrder == WindingOrder.Counterclockwise) TriangulateRecursive(this.Points.Reverse().ToList(), triangles);
            else TriangulateRecursive(this.Points.ToList(), triangles);
            int[] indices = new int[triangles.Count];
            for (int i = 0; i < indices.Length; i++) indices[i] = Array.IndexOf(this.Points, triangles[i]);
            return indices;
        }
        public static Polygon Transform(Polygon polygon, Matrix matrix, Vector2 origin)
        {
            Polygon returnPolygon;
            returnPolygon = Transform(polygon, Matrix.CreateTranslation(-origin.X, -origin.Y, 0f));
            returnPolygon = Transform(returnPolygon, matrix);
            returnPolygon = Transform(returnPolygon, Matrix.CreateTranslation(origin.X, origin.Y, 0f));
            return returnPolygon;
        }
        public static Polygon Transform(Polygon polygon, Matrix matrix)
        {
            Vector2[] newPoints = new Vector2[polygon.Points.Length];
            for (int i = 0; i < polygon.Points.Length; i++) newPoints[i] = Vector2.Transform(polygon.Points[i], matrix);
            return new Polygon(newPoints);
        }
        #endregion
    }
}
