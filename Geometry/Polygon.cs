using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;

namespace MonoGeometry.Geometry
{
    public struct Polygon : ITransformable
    {
        #region Private fields
        private Vector2[] _points;
        #endregion
        #region Public properties
        public Vector2[] Points
        {
            readonly get => (Vector2[])_points.Clone();
            set
            {
                this._points = value;
                this.Triangulate();
            }
        }
        public readonly Vector2 Center
        {
            get
            {
                Vector2 totalVector = Vector2.Zero;
                foreach (Vector2 point in this._points) totalVector += point;
                return totalVector / this._points.Length;
            }
        }
        #endregion
        #region Internal properties
        internal int[] TriangulatedIndices;
        #endregion
        #region Constructors
        public Polygon() : this(new[] { Vector2.Zero, Vector2.Zero, Vector2.Zero }) { }
        public Polygon(IEnumerable<Vector2> points)
        {
            if (points.Count() < 3) throw new ArgumentOutOfRangeException(nameof(points), "A polygon requires at least 3 points");
            this._points = new Vector2[points.Count()];
            this.TriangulatedIndices = Array.Empty<int>();
            for (int i = 0; i < this._points.Length; i++) this._points[i] = points.ElementAt(i);

            //Guarantee that the points are stored with a clockwise winding order
            float sum = 0;
            for (int i = 0; i < this._points.Length; i++)
            {
                Vector2 a = this._points[i];
                Vector2 b = this._points[(i + 1) % this._points.Length];
                sum += (b.X - a.X) * (b.Y + a.Y);
            }
            if (sum > 0) this._points = this._points.Reverse().ToArray();

            this.Triangulate();
        }
        #endregion
        #region Operators
        public static bool operator ==(Polygon a, Polygon b)
        {
            if (a._points.Length == b._points.Length) return false;
            for (int i = 0; i < a._points.Length; i++) if (a._points[i] != b._points[i]) return false;
            return true;
        }
        public static bool operator !=(Polygon a, Polygon b) => !(a == b);
        #endregion
        #region Private methods
        private void Triangulate()
        {
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
        #region Public methods
        public override readonly bool Equals(object? obj) => (obj is Polygon polygon) && (this == polygon);
        public readonly bool Equals(Polygon other) => this == other;
        public override readonly int GetHashCode() => StructuralComparisons.StructuralEqualityComparer.GetHashCode(this._points);
        public override readonly string ToString()
        {
            string returnString = "{";
            foreach (Vector2 point in this._points) returnString += point.ToString() + ", ";
            return returnString.Remove(returnString.Length - 3) + "}";
        }
        internal void Transform(Matrix matrix, Vector2 origin)
        {
            this.Transform(Matrix.CreateTranslation(-origin.X, -origin.Y, 0f));
            this.Transform(matrix);
            this.Transform(Matrix.CreateTranslation(origin.X, origin.Y, 0f));
        }
        internal void Transform(Matrix matrix)
        {
            Vector2[] newPoints = new Vector2[this._points.Length];
            for (int i = 0; i < this._points.Length; i++) newPoints[i] = Vector2.Transform(this._points[i], matrix);
            this._points = newPoints;
            this.Triangulate();
        }
        #endregion
    }
}
