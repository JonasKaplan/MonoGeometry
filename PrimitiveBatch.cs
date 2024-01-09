using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGeometry
{
    //Based heavily on https://www.youtube.com/watch?v=ZqwfoMjJAO4
    public sealed class PrimitiveBatch : IDisposable
    {
        #region Constants
        private const int _defaultMaxVertexCount = 2048;
        #endregion
        #region Private fields
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
        #endregion
        #region Constructors
        public PrimitiveBatch(GraphicsDevice graphicsDevice) : this(graphicsDevice, PrimitiveBatch._defaultMaxVertexCount) { }
        public PrimitiveBatch(GraphicsDevice graphicsDevice, int maxVertexCount)
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
        }
        #endregion
        #region Private methods
        private void EnsureBatching()
        {
            if (!this._isBatching) throw new InvalidOperationException("PrimitiveBatch has not yet begun");
        }
        private void Flush()
        {
            if (this._shapeCount == 0) return;
            this.EnsureBatching();

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
            if (this._vertexCount + shapeVertices > this._maxVertexCount || this._indexCount + shapeIndices > this._maxIndexCount) this.Flush();
        }
        #endregion
        #region Public methods
        public void Dispose()
        {
            if (this._isDisposed) return;
            this._effect?.Dispose();
            this._isDisposed = true;
        }
        public void Begin() => this.Begin(Matrix.Identity);
        public void Begin(Matrix transformation)
        {
            if (this._isBatching) throw new InvalidOperationException("PrimitiveBatch has already begun");

            this._effect.Projection = Matrix.CreateOrthographicOffCenter(0f, this._graphicsDevice.Viewport.Width, this._graphicsDevice.Viewport.Height, 0f, 0f, 1f) * transformation;

            this._isBatching = true;
        }
        public void End()
        {
            this.Flush();
            this._isBatching = false;
        }
        #region Triangle
        public void Triangle(Triangle triangle, Color color) => this.Triangle(triangle.P1X, triangle.P1Y, triangle.P2X, triangle.P2Y, triangle.P3X, triangle.P3Y, color);
        public void Triangle(Vector2 p1, Vector2 p2, Vector2 p3, Color color) => this.Triangle(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, color);
        public void Triangle(float x1, float y1, float x2, float y2, float x3, float y3, Color color)
        {
            this.EnsureBatching();
            this.HandleOverflow(3, 3);

            this._indices[this._indexCount++] = 0 + this._vertexCount;
            this._indices[this._indexCount++] = 1 + this._vertexCount;
            this._indices[this._indexCount++] = 2 + this._vertexCount;

            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(x1, y1, 0f), color);
            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(x2, y2, 0f), color);
            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(x3, y3, 0f), color);

            this._shapeCount++;
        }
        #endregion
        #region Rectangle
        public void Rectangle(Rectangle rectangle, Color color) => this.Rectangle(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, color);
        public void Rectangle(Vector2 topLeft, Vector2 bottomRight, Color color) => this.Rectangle(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y, color);
        public void Rectangle(float topLeftX, float topLeftY, float bottomRightX, float bottomRightY, Color color)
        {
            this.EnsureBatching();
            this.HandleOverflow(4, 6);

            this._indices[this._indexCount++] = 0 + this._vertexCount;
            this._indices[this._indexCount++] = 1 + this._vertexCount;
            this._indices[this._indexCount++] = 2 + this._vertexCount;

            this._indices[this._indexCount++] = 0 + this._vertexCount;
            this._indices[this._indexCount++] = 2 + this._vertexCount;
            this._indices[this._indexCount++] = 3 + this._vertexCount;

            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(topLeftX, topLeftY, 0f), color);
            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(bottomRightX, topLeftY, 0f), color);
            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(bottomRightX, bottomRightY, 0f), color);
            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(topLeftX, bottomRightY, 0f), color);

            this._shapeCount++;
        }
        #endregion
        #region Regular Polygon
        public void RegularPolygon(Vector2 centerLocation, float centerToVertexDistance, int sideCount, Color color) =>
            this.RegularPolygon(centerLocation.X, centerLocation.Y, centerToVertexDistance, sideCount, color);
        public void RegularPolygon(float x, float y, float centerToVertexDistance, int sideCount, Color color)
        {
            this.EnsureBatching();
            if (sideCount < 3) throw new ArgumentOutOfRangeException(nameof(sideCount), "Regular polygons must have at least 3 sides");
            this.HandleOverflow(sideCount + 1, sideCount * 3);

            for (int i = 0; i < sideCount; i++)
            {
                this._indices[this._indexCount++] = 0 + this._vertexCount;
                this._indices[this._indexCount++] = 1 + i + this._vertexCount;
                this._indices[this._indexCount++] = (2 + i > sideCount ? 1 : 2 + i) + this._vertexCount; //again
            }

            Vector3 center = new(x, y, 0f);
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
        public void Circle(Circle circle, Color color) => this.Circle(circle.X, circle.Y, circle.Radius, color);
        public void Circle(Vector2 centerLocation, float radius, Color color) => this.Circle(centerLocation.X, centerLocation.Y, radius, color);
        public void Circle(float x, float y, float radius, Color color) => this.RegularPolygon(x, y, radius, (int)(radius / 2) + 4, color);
        #endregion
        #endregion
    }
}
