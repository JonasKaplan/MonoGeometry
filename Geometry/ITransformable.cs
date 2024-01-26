using Microsoft.Xna.Framework;

namespace MonoGeometry.Geometry
{
    public interface ITransformable
    {
        public Vector2 Center { get; }
        internal void Transform(Matrix matrix, Vector2 origin);
        internal void Transform(Matrix matrix);
    }
}
