using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGeometry.Geometry;
using System.Diagnostics;

namespace MonoGeometry.Drawing
{
    //Based heavily on https://www.youtube.com/watch?v=ZqwfoMjJAO4
    public sealed class PrimitiveBatch : IDisposable
    {
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
        #region Public properties
        public Color FillColor { get; set; } = Color.Black;
        #endregion
        #region Constructors
        public PrimitiveBatch(GraphicsDevice graphicsDevice) : this(graphicsDevice, 2048) { }
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
            //TODO: this modifies the state of the passed GraphicsDevice, which may cause unexpected behaviour for the end user
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

                this._graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, this._vertices, 0, this._vertexCount, this._indices, 0, this._indexCount / 3);
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

            this._effect.Projection = transformation * Matrix.CreateOrthographicOffCenter(0f, this._graphicsDevice.Viewport.Width, this._graphicsDevice.Viewport.Height, 0f, 0f, 1f);

            this._isBatching = true;
        }
        public void End()
        {
            this.Flush();
            this._isBatching = false;
        }
        public void Triangle(Triangle triangle) => this.Triangle(triangle.P0.X, triangle.P0.Y, triangle.P1.X, triangle.P1.Y, triangle.P2.X, triangle.P2.Y);
        public void Triangle(Vector2 p0, Vector2 p1, Vector2 p2) => this.Triangle(p0.X, p0.Y, p1.X, p1.Y, p2.X, p2.Y);
        public void Triangle(float x0, float y0, float x1, float y1, float x2, float y2)
        {
            this.EnsureBatching();
            this.HandleOverflow(3, 3);

            this._indices[this._indexCount++] = 0 + this._vertexCount;
            this._indices[this._indexCount++] = 1 + this._vertexCount;
            this._indices[this._indexCount++] = 2 + this._vertexCount;

            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(x0, y0, 0f), this.FillColor);
            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(x1, y1, 0f), this.FillColor);
            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(x2, y2, 0f), this.FillColor);

            this._shapeCount++;
        }
        public void Rectangle(Rectangle rectangle) => this.Rectangle(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
        public void Rectangle(Vector2 topLeft, Vector2 bottomRight) => this.Rectangle(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
        public void Rectangle(float topLeftX, float topLeftY, float bottomRightX, float bottomRightY)
        {
            this.EnsureBatching();
            this.HandleOverflow(4, 6);

            this._indices[this._indexCount++] = 0 + this._vertexCount;
            this._indices[this._indexCount++] = 1 + this._vertexCount;
            this._indices[this._indexCount++] = 2 + this._vertexCount;

            this._indices[this._indexCount++] = 0 + this._vertexCount;
            this._indices[this._indexCount++] = 2 + this._vertexCount;
            this._indices[this._indexCount++] = 3 + this._vertexCount;

            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(topLeftX, topLeftY, 0f), this.FillColor);
            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(bottomRightX, topLeftY, 0f), this.FillColor);
            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(bottomRightX, bottomRightY, 0f), this.FillColor);
            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(topLeftX, bottomRightY, 0f), this.FillColor);

            this._shapeCount++;
        }
        public void RegularPolygon(Vector2 centerLocation, float centerToVertexDistance, int sideCount) =>
            this.RegularPolygon(centerLocation.X, centerLocation.Y, centerToVertexDistance, sideCount);
        public void RegularPolygon(float x, float y, float centerToVertexDistance, int sideCount)
        {
            this.EnsureBatching();
            if (sideCount < 3) throw new ArgumentOutOfRangeException(nameof(sideCount), "Regular polygons must have at least 3 sides");
            this.HandleOverflow(sideCount + 1, sideCount * 3);

            Vector3 center = new(x, y, 0f);
            Vector3 offsetVector = new(0f, centerToVertexDistance, 0f);
            Matrix rotation = Matrix.CreateRotationZ(MathHelper.Tau / sideCount);

            for (int i = 0; i < sideCount; i++)
            {
                this._indices[this._indexCount++] = sideCount + this._vertexCount;
                this._indices[this._indexCount++] = i + this._vertexCount;
                this._indices[this._indexCount++] = ((i + 1) % sideCount) + this._vertexCount;
            }
            
            for (int i = 0; i < sideCount; i++)
            {
                offsetVector = Vector3.Transform(offsetVector, rotation);
                this._vertices[this._vertexCount++] = new VertexPositionColor(center + offsetVector, this.FillColor);
            }
            this._vertices[this._vertexCount++] = new VertexPositionColor(center, this.FillColor);

            this._shapeCount++;
        }
        public void Circle(Vector2 centerLocation, float radius) => this.Circle(centerLocation.X, centerLocation.Y, radius);
        public void Circle(float x, float y, float radius) => this.Ellipse(x, y, radius, 1f);
        public void Ellipse(Vector2 location, float radius, float eccentricity) => this.Ellipse(location.X, location.Y, radius, eccentricity);
        public void Ellipse(float x, float y, float radius, float eccentricity)
        {
            this.EnsureBatching();
            if (eccentricity == 0) throw new ArgumentOutOfRangeException(nameof(eccentricity), "Eccentricity cannot be 0");
            int sideCount = (int)radius + 4;
            this.HandleOverflow(sideCount + 1, sideCount * 3);

            Vector3 center = new(x, y, 0f);
            Vector3 offsetVector = new(0f, radius, 0f);
            Matrix rotation = Matrix.CreateRotationZ(MathHelper.Tau / sideCount);
            Matrix scale = Matrix.CreateTranslation(-x, -y, 0f) * Matrix.CreateScale(1 / eccentricity, 1f, 1f) * Matrix.CreateTranslation(x, y, 0f);

            for (int i = 0; i < sideCount; i++)
            {
                this._indices[this._indexCount++] = sideCount + this._vertexCount;
                this._indices[this._indexCount++] = i + this._vertexCount;
                this._indices[this._indexCount++] = ((i + 1) % sideCount) + this._vertexCount;
            }
            
            for (int i = 0; i < sideCount; i++)
            {
                offsetVector = Vector3.Transform(offsetVector, rotation);
                this._vertices[this._vertexCount++] = new VertexPositionColor(Vector3.Transform(center + offsetVector, scale), this.FillColor);
            }
            this._vertices[this._vertexCount++] = new VertexPositionColor(center, this.FillColor);

            this._shapeCount++;
        }
        public void LineSegment(Vector2 p0, Vector2 p1, float width) => this.LineSegment(p0.X, p0.Y, p1.X, p1.Y, width);
        public void LineSegment(float p0X, float p0Y, float p1X, float p1Y, float width)
        {
            this.EnsureBatching();
            this.HandleOverflow(4, 6);

            Vector2 tail = new(p0X, p0Y);
            Vector2 head = new(p1X, p1Y);
            Vector2 offset = head - tail;
            offset.Normalize();
            offset = new Vector2(offset.Y, -offset.X) * (width / 2);

            this._indices[this._indexCount++] = 0 + this._vertexCount;
            this._indices[this._indexCount++] = 2 + this._vertexCount;
            this._indices[this._indexCount++] = 3 + this._vertexCount;

            this._indices[this._indexCount++] = 0 + this._vertexCount;
            this._indices[this._indexCount++] = 1 + this._vertexCount;
            this._indices[this._indexCount++] = 3 + this._vertexCount;

            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(tail + offset, 0f), this.FillColor);
            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(tail - offset, 0f), this.FillColor);
            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(head + offset, 0f), this.FillColor);
            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(head - offset, 0f), this.FillColor);

            this._shapeCount++;
        }
        public void Polygon(IEnumerable<Vector2> points) => this.Polygon(new Polygon(points)); //TODO: this makes me sad
        public void Polygon(Polygon polygon)
        {
            this.EnsureBatching();
            int[] indices = polygon.TriangulatedIndices;
            this.HandleOverflow(polygon.Points.Length, indices.Length);

            foreach (int index in indices) this._indices[this._indexCount++] = index + this._vertexCount;
            foreach (Vector2 point in polygon.Points) this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(point, 0f), this.FillColor);

            this._shapeCount++;
        }
        #endregion
    }
}
