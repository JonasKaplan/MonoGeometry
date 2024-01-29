using Microsoft.Xna.Framework;

namespace MonoGeometry.Geometry
{
    public interface ITransformable<T>
    {
        public Vector2 Center { get; }
        public T Transform(Matrix matrix, Vector2 origin);
        public T Transform(Matrix matrix);
    }
}
