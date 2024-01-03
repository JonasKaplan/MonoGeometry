using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MGPrimitives
{
    public class GPUPrimitiveDrawer
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly BasicEffect _effect;
        private readonly List<VertexPositionColor[]> _polygons;

        public GPUPrimitiveDrawer(GraphicsDevice graphicsDevice)
        {
            this._graphicsDevice = graphicsDevice;
            this._effect = new BasicEffect(this._graphicsDevice);
            this._effect.TextureEnabled = false;
            this._effect.FogEnabled = false;
            this._effect.LightingEnabled = false;
            this._effect.VertexColorEnabled = true;
            this._polygons = new List<VertexPositionColor[]>();
        }
        public void DrawTemp()
        {
            foreach (EffectPass p in this._effect.CurrentTechnique.Passes)
            {
                p.Apply();

                VertexPositionColor[] vpc = new VertexPositionColor[]
                {
                    new(new(0, 0, 0), Color.Green),
                    new(new(0.5f, 0.5f, 0), Color.Red),
                    new(new(0.5f, 0, 0), Color.Blue)
                };

                this._graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vpc, 0, 1);
            }
        }
    }
}
