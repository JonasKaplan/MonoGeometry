using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;

namespace MonoGeometry.Geometry
{
    /// <summary>
    /// Describes a 2D polygon. Does not allow for self-intersection
    /// </summary>
    public struct Polygon : ITransformable<Polygon>
    {
        #region Private fields
        /// <summary>
        /// The points defining this <see cref="Polygon"/>
        /// </summary>
        private Vector2[] _points;
        /// <summary>
        /// The indices, within <see cref="this._points"/>, of the points that form triangles for drawing
        /// </summary>
        private int[] _triangulatedIndices;
        #endregion
        #region Public properties
        /// <summary>
        /// The points defining this <see cref="Polygon"/>
        /// </summary>
        public Vector2[] Points
        {
            readonly get => (Vector2[])this._points.Clone();
            set
            {
                this._points = value;
                this.Triangulate();
            }
        }
        /// <summary>
        /// The center of this <see cref="Polygon"/>, defined as the average of all x and y coordinates
        /// </summary>
        public readonly Vector2 Center
        {
            get
            {
                Vector2 totalVector = Vector2.Zero;
                foreach (Vector2 point in this._points) totalVector += point;
                return totalVector / this._points.Length;
            }
        }
        /// <summary>
        /// The perimeter of this <see cref="Polygon"/>
        /// </summary>
        public readonly float Perimeter
        {
            get
            {
                float perimeter = 0;
                for (int i = 0; i < this._points.Length; i++) perimeter += (this._points[(i + 1) % this._points.Length] - this._points[i]).Length();
                return perimeter;
            }
        }
        /// <summary>
        /// The area of this <see cref="Polygon"/>
        /// </summary>
        public readonly float Area
        {
            get
            {
                float area = 0;
                for (int i = 0; i < this.TriangulatedIndices.Length; i += 3) area += new Triangle(this._points[i], this._points[i + 1], this._points[i + 2]).Area;
                return area;
            }
        }
        #endregion
        #region Internal properties
        /// <summary>
        /// The indices, within <see cref="this.Points"/>, of the points that form triangles for drawing
        /// </summary>
        internal readonly int[] TriangulatedIndices => (int[])this._triangulatedIndices.Clone();
        #endregion
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Polygon"/> instance with three points, all at the origin
        /// </summary>
        public Polygon() : this(new[] { Vector2.Zero, Vector2.Zero, Vector2.Zero }) { }
        /// <summary>
        /// Creates a new <see cref="Polygon"/> instance, with a defined set of points. Points are always stored internally in a counter-clockwise winding order, regardless of how they are passed to this constructor
        /// </summary>
        /// <param name="points">The set of points used to define this <see cref="Polygon"/></param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the size of <c>points</c> is less than 3</exception>
        public Polygon(IEnumerable<Vector2> points)
        {
            if (points.Count() < 3) throw new ArgumentOutOfRangeException(nameof(points), "A polygon requires at least 3 points");
            this._points = new Vector2[points.Count()];
            this._triangulatedIndices = Array.Empty<int>();
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
        /// <summary>
        /// Compares weather two <see cref="Polygon"/> instances are equal
        /// </summary>
        /// <param name="a"><see cref="Polygon"/> instance on the left of the equal sign</param>
        /// <param name="b"><see cref="Polygon"/> instance on the right of the equal sign</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise</returns>
        public static bool operator ==(Polygon a, Polygon b)
        {
            if (a._points.Length == b._points.Length) return false;
            for (int i = 0; i < a._points.Length; i++) if (a._points[i] != b._points[i]) return false;
            return true;
        }
        /// <summary>
        /// Compares weather two <see cref="Polygon"/> instances are not equal
        /// </summary>
        /// <param name="a"><see cref="Polygon"/> instance on the left of the not equal sign</param>
        /// <param name="b"><see cref="Polygon"/> instance on the right of the not equal sign</param>
        /// <returns><c>true</c> if the instances are not equal; <c>false</c> otherwise</returns>
        public static bool operator !=(Polygon a, Polygon b) => !(a == b);
        #endregion
        #region Private methods
        /// <summary>
        /// Sets the value of <see cref="this._triangulatedIndices"/> based on the current state of <see cref="this._points"/>
        /// </summary>
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

            this._triangulatedIndices = indices;
        }
        #endregion
        #region Public methods
        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="object"/>
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise</returns>
        public override readonly bool Equals(object? obj) => (obj is Polygon polygon) && (this == polygon);
        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="Polygon"/>
        /// </summary>
        /// <param name="other">The <see cref="Polygont"/> to compare</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise</returns>
        public readonly bool Equals(Polygon other) => this == other;
        /// <summary>
        /// Gets the hash code of this <see cref="Polygon"/>
        /// </summary>
        /// <returns>Hash code of this <see cref="Polygon"/></returns>
        public override readonly int GetHashCode() => StructuralComparisons.StructuralEqualityComparer.GetHashCode(this._points);
        /// <summary>
        /// Returns a <see cref="string"/> representation of this <see cref="Polygon"/> in the format:
        /// {{P1}, {P2}, ... {Pn}}
        /// </summary>
        /// <returns><see cref="string"/> representation of this <see cref="Polygon"/></returns>
        public override readonly string ToString()
        {
            string returnString = "{";
            foreach (Vector2 point in this._points) returnString += point.ToString() + ", ";
            return returnString.Remove(returnString.Length - 3) + "}";
        }
        /// <summary>
        /// Creates a new <see cref="Polygon"/> instance, transformed about a given origin by a given matrix
        /// </summary>
        /// <param name="matrix">The transformation to be applied to the <see cref="Polygon"/></param>
        /// <param name="origin">The origin about which to apply the transformation</param>
        /// <returns>A new <see cref="Polygon"/> instance, transformed based on the given parameters</returns>
        public readonly Polygon Transform(Matrix matrix, Vector2 origin)
        {
            Matrix transform = Matrix.CreateTranslation(-origin.X, -origin.Y, 0f) * matrix * Matrix.CreateTranslation(origin.X, origin.Y, 0f);
            return this.Transform(transform);
        }
        /// <summary>
        /// Creates a new <see cref="Polygon"/> instance, transformed by a given matrix
        /// </summary>
        /// <param name="matrix">The transformation to be applied to the <see cref="Polygon"/></param>
        /// <returns>A new <see cref="Polygon"/> instance, transformed based on the given parameters</returns>
        public readonly Polygon Transform(Matrix matrix)
        {
            Vector2[] newPoints = new Vector2[this._points.Length];
            for (int i = 0; i < this._points.Length; i++) newPoints[i] = Vector2.Transform(this._points[i], matrix);
            return new Polygon(newPoints);
        }
        #endregion
    }
}
