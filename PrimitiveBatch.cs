using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace MGPrimitives
{
    //Based heavily on https://www.youtube.com/watch?v=ZqwfoMjJAO4
    public sealed class PrimitiveBatch : IDisposable
    {
        private bool _isDisposed;

        private readonly GraphicsDevice _graphicsDevice;
        private readonly VertexPositionColor[] _vertices;
        private readonly int[] _indices;
        private readonly int _maxVertexCount;
        private readonly int _maxIndexCount;
        private int _vertexCount;
        private int _indexCount;
        private int _shapeCount;

        private readonly BasicEffect _effect;

        private bool _isBatching;

        public Color DefaultColor;
        public PrimitiveBatch(GraphicsDevice graphicsDevice, int maxVertexCount = 2048)
        {
            this._isDisposed = false;

            this._graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
            this._maxVertexCount = maxVertexCount;
            this._maxIndexCount = this._maxVertexCount * 3;
            this._vertices = new VertexPositionColor[this._maxVertexCount];
            this._indices = new int[this._maxIndexCount];
            this._vertexCount = 0;
            this._indexCount = 0;
            this._shapeCount = 0;

            this._effect = new(this._graphicsDevice)
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
            this._graphicsDevice.RasterizerState = new RasterizerState() { CullMode = CullMode.None };

            this.DefaultColor = Color.Black;
        }
        public void Dispose()
        {
            if (this._isDisposed) return;
            this._effect?.Dispose();
            this._isDisposed = true;
        }
        private void EnsureBatching()
        {
            if (!this._isBatching) throw new InvalidOperationException("PrimitiveBatch has not yet begun");
        }
        public void Begin()
        {
            if (this._isBatching) throw new InvalidOperationException("PrimitiveBatch has already begun");

            this._effect.Projection = Matrix.CreateOrthographicOffCenter(0f, this._graphicsDevice.Viewport.Width, this._graphicsDevice.Viewport.Height, 0f, 0f, 1f);

            this._isBatching = true;
        }
        public void End()
        {
            Flush();
            this._isBatching = false;
        }
        private void Flush()
        {
            if (this._shapeCount == 0) return;
            EnsureBatching();

            foreach (EffectPass pass in this._effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                this._graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                    PrimitiveType.TriangleList,
                    this._vertices,
                    0,
                    this._vertexCount,
                    this._indices,
                    0,
                    this._indexCount / 3);
            }

            this._vertexCount = 0;
            this._indexCount = 0;
            this._shapeCount = 0;
        }
        private void HandleOverflow(int shapeVertices, int shapeIndices)
        {
            if (shapeVertices > this._maxVertexCount) throw new ArgumentOutOfRangeException(nameof(shapeVertices), "Shape exceeds maximum vertex count of " + this._maxVertexCount);
            if (shapeIndices > this._maxIndexCount) throw new ArgumentOutOfRangeException(nameof(shapeIndices), "Shape exceeds maximum index count of " + this._maxIndexCount);
            if (this._vertexCount + shapeVertices > this._maxVertexCount || this._indexCount + shapeIndices > this._maxIndexCount) Flush();
        }
        #region Triangle
        public void Triangle(Point p1, Point p2, Point p3)
        {
            EnsureBatching();
            HandleOverflow(3, 3);

            this._indices[this._indexCount++] = 0 + this._vertexCount;
            this._indices[this._indexCount++] = 1 + this._vertexCount;
            this._indices[this._indexCount++] = 2 + this._vertexCount;

            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(p1.ToVector2(), 0f), this.DefaultColor);
            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(p2.ToVector2(), 0f), this.DefaultColor);
            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(p3.ToVector2(), 0f), this.DefaultColor);

            this._shapeCount++;
        }
        public void Triangle(Point p1, Point p2, Point p3, Color color)
        {
            EnsureBatching();
            HandleOverflow(3, 3);

            this._indices[this._indexCount++] = 0 + this._vertexCount;
            this._indices[this._indexCount++] = 1 + this._vertexCount;
            this._indices[this._indexCount++] = 2 + this._vertexCount;

            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(p1.ToVector2(), 0f), color);
            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(p2.ToVector2(), 0f), color);
            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(p3.ToVector2(), 0f), color);

            this._shapeCount++;
        }
        #endregion
        #region Rectangle
        public void Rectangle(Rectangle rectangle)
        {
            EnsureBatching();
            HandleOverflow(4, 6);

            this._indices[this._indexCount++] = 0 + this._vertexCount;
            this._indices[this._indexCount++] = 1 + this._vertexCount;
            this._indices[this._indexCount++] = 2 + this._vertexCount;

            this._indices[this._indexCount++] = 0 + this._vertexCount;
            this._indices[this._indexCount++] = 2 + this._vertexCount;
            this._indices[this._indexCount++] = 3 + this._vertexCount;

            Vector3 v0 = new(rectangle.Left, rectangle.Top, 0f);
            Vector3 v1 = new(rectangle.Right, rectangle.Top, 0f);
            Vector3 v2 = new(rectangle.Right, rectangle.Bottom, 0f);
            Vector3 v3 = new(rectangle.Left, rectangle.Bottom, 0f);

            this._vertices[this._vertexCount++] = new VertexPositionColor(v0, this.DefaultColor);
            this._vertices[this._vertexCount++] = new VertexPositionColor(v1, this.DefaultColor);
            this._vertices[this._vertexCount++] = new VertexPositionColor(v2, this.DefaultColor);
            this._vertices[this._vertexCount++] = new VertexPositionColor(v3, this.DefaultColor);

            this._shapeCount++;
        }
        public void Rectangle(Rectangle rectangle, Color color)
        {
            EnsureBatching();
            HandleOverflow(4, 6);

            this._indices[this._indexCount++] = 0 + this._vertexCount;
            this._indices[this._indexCount++] = 1 + this._vertexCount;
            this._indices[this._indexCount++] = 2 + this._vertexCount;

            this._indices[this._indexCount++] = 0 + this._vertexCount;
            this._indices[this._indexCount++] = 2 + this._vertexCount;
            this._indices[this._indexCount++] = 3 + this._vertexCount;

            Vector3 v0 = new(rectangle.Left, rectangle.Top, 0f);
            Vector3 v1 = new(rectangle.Right, rectangle.Top, 0f);
            Vector3 v2 = new(rectangle.Right, rectangle.Bottom, 0f);
            Vector3 v3 = new(rectangle.Left, rectangle.Bottom, 0f);

            this._vertices[this._vertexCount++] = new VertexPositionColor(v0, color);
            this._vertices[this._vertexCount++] = new VertexPositionColor(v1, color);
            this._vertices[this._vertexCount++] = new VertexPositionColor(v2, color);
            this._vertices[this._vertexCount++] = new VertexPositionColor(v3, color);

            this._shapeCount++;
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
                this._indices[this._indexCount++] = 0 + this._vertexCount;
                this._indices[this._indexCount++] = 1 + i + this._vertexCount;
                this._indices[this._indexCount++] = (2 + i > sideCount ? 1 : 2 + i) + this._vertexCount; //this is kinda eww
            }

            Vector3 center = new(centerLocation.ToVector2(), 0f);
            Vector3 offsetVector = new(0f, centerToVertexDistance, 0f);
            Matrix rotation = Matrix.CreateRotationZ(MathHelper.Tau / sideCount);

            this._vertices[this._vertexCount++] = new VertexPositionColor(center, this.DefaultColor);
            for (int i = 0; i < sideCount; i++)
            {
                offsetVector = Vector3.Transform(offsetVector, rotation);
                this._vertices[this._vertexCount++] = new VertexPositionColor(center + offsetVector, this.DefaultColor);
            }

            this._shapeCount++;
        }
        public void RegularPolygon(Point centerLocation, float centerToVertexDistance, int sideCount, Color color)
        {
            EnsureBatching();
            if (sideCount < 3) throw new ArgumentOutOfRangeException(nameof(sideCount), "Regular polygons must have at least 3 sides");
            HandleOverflow(sideCount + 1, sideCount * 3);

            for (int i = 0; i < sideCount; i++)
            {
                this._indices[this._indexCount++] = 0 + this._vertexCount;
                this._indices[this._indexCount++] = 1 + i + this._vertexCount;
                this._indices[this._indexCount++] = (2 + i > sideCount ? 1 : 2 + i) + this._vertexCount; //again
            }

            Vector3 center = new(centerLocation.ToVector2(), 0f);
            Vector3 offsetVector = new(0f, centerToVertexDistance, 0f);
            Matrix rotation = Matrix.CreateRotationZ(MathHelper.Tau / sideCount);

            this._vertices[this._vertexCount++] = new VertexPositionColor(center, color);
            for (int i = 0; i < sideCount; i++)
            {
                offsetVector = Vector3.Transform(offsetVector, rotation);
                this._vertices[this._vertexCount++] = new VertexPositionColor(center + offsetVector, color);
            }

            this._shapeCount++;
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
