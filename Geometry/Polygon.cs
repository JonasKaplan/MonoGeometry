using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MonoGeometry.Geometry
{
    public struct Polygon
    {
        #region Public properties
        public readonly Vector2[] Points;
        #endregion
        #region Internal properties
        internal readonly int[] TriangulatedIndices;
        #endregion
        #region Constructors
        public Polygon() : this(new[] { Vector2.Zero, Vector2.Zero, Vector2.Zero }) { }
        public Polygon(IEnumerable<Vector2> points)
        {
            if (points.Count() < 3) throw new ArgumentOutOfRangeException(nameof(points), "A polygon requires at least 3 points");
            this.Points = new Vector2[points.Count()];
            for (int i = 0; i < this.Points.Length; i++) this.Points[i] = points.ElementAt(i);

            float sum = 0;
            for (int i = 0; i < this.Points.Length; i++)
            {
                Vector2 a = this.Points[i];
                Vector2 b = this.Points[(i + 1) % this.Points.Length];
                sum += (b.X - a.X) * (b.Y + a.Y);
            }
            if (sum > 0) this.Points = this.Points.Reverse().ToArray();

            int[] indices = new int[3 * (this.Points.Length - 2)];
            int indexIndex = 0; //Behold: masterful naming
            List<Vector2> trianglePoints = this.Points.ToList();
            for (int i = 0; i < this.Points.Length - 3; i++)
            {
                for (int index1 = 0; index1 < trianglePoints.Count; index1++)
                {
                    int index2 = (index1 + 1) % trianglePoints.Count;
                    int index3 = (index1 + 2) % trianglePoints.Count;
                    Vector2 longEdge = trianglePoints[index3] - trianglePoints[index1];
                    Vector2 longEdgeNormal = new(longEdge.Y, -longEdge.X);
                    Vector2 shortEdge = trianglePoints[index2] - trianglePoints[index1];
                    float cosAngle = Vector2.Dot(longEdgeNormal, shortEdge) / (longEdgeNormal.Length() * shortEdge.Length());
                    if (cosAngle < 0) continue;

                    Triangle ear = new(trianglePoints[index1], trianglePoints[index2], trianglePoints[index3]);
                    bool earContainsPoint = false;
                    for (int j = 0; j < trianglePoints.Count; j++)
                    {
                        if ((j == index1) || (j == index2) || (j == index3)) continue;

                        if (ear.Contains(trianglePoints[j]))
                        {
                            earContainsPoint = true;
                            break;
                        }
                    }
                    if (earContainsPoint) continue;

                    indices[indexIndex++] = Array.IndexOf(this.Points, trianglePoints[index1]);
                    indices[indexIndex++] = Array.IndexOf(this.Points, trianglePoints[index2]);
                    indices[indexIndex++] = Array.IndexOf(this.Points, trianglePoints[index3]);
                    trianglePoints.RemoveAt(index2);
                    break;
                }
            }

            indices[indexIndex++] = Array.IndexOf(this.Points, trianglePoints[0]);
            indices[indexIndex++] = Array.IndexOf(this.Points, trianglePoints[1]);
            indices[indexIndex++] = Array.IndexOf(this.Points, trianglePoints[2]);

            this.TriangulatedIndices = indices;
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
