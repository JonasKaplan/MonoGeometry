using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MGPrimitives
{
    //Based heavily on https://www.youtube.com/watch?v=ZqwfoMjJAO4
    public sealed class PrimitiveBatch : IDisposable
    {
        private bool isDisposed;

        private readonly GraphicsDevice graphicsDevice;
        private readonly VertexPositionColor[] vertices;
        private readonly int[] indices;
        private readonly int maxVertexCount;
        private readonly int maxIndexCount;
        private int vertexCount;
        private int indexCount;
        private int shapeCount;

        private readonly BasicEffect effect;

        private bool isBatching;

        public Color DefaultColor;
        public PrimitiveBatch(GraphicsDevice graphicsDevice, int maxVertexCount = 2048)
        {
            this.isDisposed = false;

            this.graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
            this.maxVertexCount = maxVertexCount;
            this.maxIndexCount = this.maxVertexCount * 3;
            this.vertices = new VertexPositionColor[this.maxVertexCount];
            this.indices = new int[this.maxIndexCount];
            this.vertexCount = 0;
            this.indexCount = 0;
            this.shapeCount = 0;

            this.effect = new(this.graphicsDevice)
            {
                TextureEnabled = false,
                FogEnabled = false,
                LightingEnabled = false,
                VertexColorEnabled = true,
                World = Matrix.Identity,
                View = Matrix.Identity,
                Projection = Matrix.Identity
                
            };

            //Prevents annoyances accociated with triangle cycle order
            this.graphicsDevice.RasterizerState = new RasterizerState() { CullMode = CullMode.None };

            this.DefaultColor = Color.Black;
        }
        public void Dispose()
        {
            if (this.isDisposed) return;
            this.effect?.Dispose();
            this.isDisposed = true;
        }
        private void EnsureBatching()
        {
            if (!this.isBatching) throw new InvalidOperationException("PrimitiveBatch has not yet begun");
        }
        public void Begin()
        {
            if (this.isBatching) throw new InvalidOperationException("PrimitiveBatch has already begun");

            this.effect.Projection = Matrix.CreateOrthographicOffCenter(0f, this.graphicsDevice.Viewport.Width, this.graphicsDevice.Viewport.Height, 0f, 0f, 1f);

            this.isBatching = true;
        }
        public void End()
        {
            Flush();
            this.isBatching = false;
        }
        private void Flush()
        {
            if (this.shapeCount == 0) return;
            EnsureBatching();

            foreach (EffectPass pass in this.effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                this.graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                    PrimitiveType.TriangleList,
                    this.vertices,
                    0,
                    this.vertexCount,
                    this.indices,
                    0,
                    this.indexCount / 3);
            }

            this.vertexCount = 0;
            this.indexCount = 0;
            this.shapeCount = 0;
        }
        private void HandleOverflow(int shapeVertices, int shapeIndices)
        {
            if (shapeVertices > this.maxVertexCount) throw new ArgumentOutOfRangeException(nameof(shapeVertices), "Shape exceeds maximum vertex count of " + this.maxVertexCount);
            if (shapeIndices > this.maxIndexCount) throw new ArgumentOutOfRangeException(nameof(shapeIndices), "Shape exceeds maximum index count of " + this.maxIndexCount);
            if (this.vertexCount + shapeVertices > this.maxVertexCount || this.indexCount + shapeIndices > this.maxIndexCount) Flush();
        }
        #region Triangle
        public void Triangle(Point p1, Point p2, Point p3)
        {
            EnsureBatching();
            HandleOverflow(3, 3);

            this.indices[this.indexCount++] = 0 + this.vertexCount;
            this.indices[this.indexCount++] = 1 + this.vertexCount;
            this.indices[this.indexCount++] = 2 + this.vertexCount;

            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(p1.ToVector2(), 0f), this.DefaultColor);
            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(p2.ToVector2(), 0f), this.DefaultColor);
            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(p3.ToVector2(), 0f), this.DefaultColor);

            this.shapeCount++;
        }
        public void Triangle(Point p1, Point p2, Point p3, Color color)
        {
            EnsureBatching();
            HandleOverflow(3, 3);

            this.indices[this.indexCount++] = 0 + this.vertexCount;
            this.indices[this.indexCount++] = 1 + this.vertexCount;
            this.indices[this.indexCount++] = 2 + this.vertexCount;

            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(p1.ToVector2(), 0f), color);
            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(p2.ToVector2(), 0f), color);
            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(p3.ToVector2(), 0f), color);

            this.shapeCount++;
        }
        #endregion
        #region Rectangle
        public void Rectangle(Rectangle rectangle)
        {
            EnsureBatching();
            HandleOverflow(4, 6);

            this.indices[this.indexCount++] = 0 + this.vertexCount;
            this.indices[this.indexCount++] = 1 + this.vertexCount;
            this.indices[this.indexCount++] = 2 + this.vertexCount;

            this.indices[this.indexCount++] = 0 + this.vertexCount;
            this.indices[this.indexCount++] = 2 + this.vertexCount;
            this.indices[this.indexCount++] = 3 + this.vertexCount;

            Vector3 v0 = new(rectangle.Left, rectangle.Top, 0f);
            Vector3 v1 = new(rectangle.Right, rectangle.Top, 0f);
            Vector3 v2 = new(rectangle.Right, rectangle.Bottom, 0f);
            Vector3 v3 = new(rectangle.Left, rectangle.Bottom, 0f);

            this.vertices[this.vertexCount++] = new VertexPositionColor(v0, this.DefaultColor);
            this.vertices[this.vertexCount++] = new VertexPositionColor(v1, this.DefaultColor);
            this.vertices[this.vertexCount++] = new VertexPositionColor(v2, this.DefaultColor);
            this.vertices[this.vertexCount++] = new VertexPositionColor(v3, this.DefaultColor);

            this.shapeCount++;
        }
        public void Rectangle(Rectangle rectangle, Color color)
        {
            EnsureBatching();
            HandleOverflow(4, 6);

            this.indices[this.indexCount++] = 0 + this.vertexCount;
            this.indices[this.indexCount++] = 1 + this.vertexCount;
            this.indices[this.indexCount++] = 2 + this.vertexCount;

            this.indices[this.indexCount++] = 0 + this.vertexCount;
            this.indices[this.indexCount++] = 2 + this.vertexCount;
            this.indices[this.indexCount++] = 3 + this.vertexCount;

            Vector3 v0 = new(rectangle.Left, rectangle.Top, 0f);
            Vector3 v1 = new(rectangle.Right, rectangle.Top, 0f);
            Vector3 v2 = new(rectangle.Right, rectangle.Bottom, 0f);
            Vector3 v3 = new(rectangle.Left, rectangle.Bottom, 0f);

            this.vertices[this.vertexCount++] = new VertexPositionColor(v0, color);
            this.vertices[this.vertexCount++] = new VertexPositionColor(v1, color);
            this.vertices[this.vertexCount++] = new VertexPositionColor(v2, color);
            this.vertices[this.vertexCount++] = new VertexPositionColor(v3, color);

            this.shapeCount++;
        }
        #endregion
        #region Regular Polygon
        public void RegularPolygon(Point centerLocation, float centerToVertexDistance, int sideCount)
        {
            EnsureBatching();
            if (sideCount < 3) throw new ArgumentOutOfRangeException(nameof(sideCount), "Regular polygons must have at least 3 sides");
            HandleOverflow(sideCount + 1, sideCount * 3);

            for (int i = 0; i < sideCount; i++)
            {
                this.indices[this.indexCount++] = 0 + this.vertexCount;
                this.indices[this.indexCount++] = 1 + i + this.vertexCount;
                this.indices[this.indexCount++] = (2 + i > sideCount ? 1 : 2 + i) + this.vertexCount; //this is kinda eww
            }

            Vector3 center = new(centerLocation.ToVector2(), 0f);
            Vector3 offsetVector = new(0f, centerToVertexDistance, 0f);
            Matrix rotation = Matrix.CreateRotationZ(MathHelper.Tau / sideCount);

            this.vertices[this.vertexCount++] = new VertexPositionColor(center, this.DefaultColor);
            for (int i = 0; i < sideCount; i++)
            {
                offsetVector = Vector3.Transform(offsetVector, rotation);
                this.vertices[this.vertexCount++] = new VertexPositionColor(center + offsetVector, this.DefaultColor);
            }

            this.shapeCount++;
        }
        public void RegularPolygon(Point centerLocation, float centerToVertexDistance, int sideCount, Color color)
        {
            EnsureBatching();
            if (sideCount < 3) throw new ArgumentOutOfRangeException(nameof(sideCount), "Regular polygons must have at least 3 sides");
            HandleOverflow(sideCount + 1, sideCount * 3);

            for (int i = 0; i < sideCount; i++)
            {
                this.indices[this.indexCount++] = 0 + this.vertexCount;
                this.indices[this.indexCount++] = 1 + i + this.vertexCount;
                this.indices[this.indexCount++] = (2 + i > sideCount ? 1 : 2 + i) + this.vertexCount; //again
            }

            Vector3 center = new(centerLocation.ToVector2(), 0f);
            Vector3 offsetVector = new(0f, centerToVertexDistance, 0f);
            Matrix rotation = Matrix.CreateRotationZ(MathHelper.Tau / sideCount);

            this.vertices[this.vertexCount++] = new VertexPositionColor(center, color);
            for (int i = 0; i < sideCount; i++)
            {
                offsetVector = Vector3.Transform(offsetVector, rotation);
                this.vertices[this.vertexCount++] = new VertexPositionColor(center + offsetVector, color);
            }

            this.shapeCount++;
        }
        #endregion
        #region Circle
        public void Circle(Point centerLocation, float radius)
        {
            //using radius as the number of sides scales level of detail as radius grows. Adding 4 garuntees no weirdness for small radii
            RegularPolygon(centerLocation, radius, (int)(radius / 2) + 4);
        }
        public void Circle(Point centerLocation, float radius, Color color)
        {
            RegularPolygon(centerLocation, radius, (int)(radius / 2) + 4, color);
        }
        #endregion
        #region Ellipse
        public void Ellipse(Point focus1, Point focus2, float radius)
        {
            EnsureBatching();
            
        }
        #endregion
    }
}
