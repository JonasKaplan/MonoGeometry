using Microsoft.Xna.Framework;
using System.Collections;

namespace MonoGeometry.Geometry
{
    public struct Polygon
    {
        #region Public properties
        public readonly Vector2[] Points;
        #endregion
        #region Constructors
        public Polygon() : this(Array.Empty<Vector2>()) { }
        public Polygon(IEnumerable<Vector2> points)
        {
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
        internal readonly int[] TriangulatedIndices()
        {
            return new int[0];
        }
        #endregion
    }
}
