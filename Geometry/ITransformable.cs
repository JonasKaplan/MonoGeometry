using Microsoft.Xna.Framework;

namespace MonoGeometry.Geometry
{
    public interface ITransformable
    {
        public void Transform(Matrix matrix, Vector2 origin);
        public void Transform(Matrix matrix);
    }
}
