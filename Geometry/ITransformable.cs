using Microsoft.Xna.Framework;

namespace MonoGeometry.Geometry
{
    public interface ITransformable
    {
        public void Transform(Matrix transform, Vector2 origin);
        public void Transform(Matrix transform, float originX, float originY);
        public void Transform(Matrix transform);
    }
}
