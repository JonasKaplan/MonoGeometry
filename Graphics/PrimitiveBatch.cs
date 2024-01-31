using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGeometry.Geometry;
using System.Diagnostics;

namespace MonoGeometry.Drawing
{
    //Based heavily on https://www.youtube.com/watch?v=ZqwfoMjJAO4
    /// <summary>
    /// Helper class for drawing primitive shapes
    /// </summary>
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
        /// <summary>
        /// The <see cref="Color"/> to fill all drawn shapes with
        /// </summary>
        public Color FillColor { get; set; } = Color.Black;
        #endregion
        #region Constructors
        /// <summary>
        /// Constructs a <see cref="PrimitiveBatch"/>, with a maximum vertex count for a single shape of 2048
        /// </summary>
        /// <param name="graphicsDevice">The <see cref="GraphicsDevice"/> that will be used to draw primitives</param>
        /// <exception cref="ArgumentNullException">Thrown if the passed <see cref="GraphicsDevice"/> is <c>null</c></exception>
        public PrimitiveBatch(GraphicsDevice graphicsDevice) : this(graphicsDevice, 2048) { }
        /// <summary>
        /// Constructs a <see cref="PrimitiveBatch"/>
        /// </summary>
        /// <param name="graphicsDevice">The <see cref="GraphicsDevice"/> that will be used to draw primitives</param>
        /// <param name="maxVertexCount">The maximum number of vertices for a single shape</param>
        /// <exception cref="ArgumentNullException">Thrown if the passed <see cref="GraphicsDevice"/> is <c>null</c></exception>
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
        /// <summary>
        /// Ensured that batching has begun before adding anything to be drawn
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if batching has not yet begun</exception>
        private void EnsureBatching()
        {
            if (!this._isBatching) throw new InvalidOperationException("PrimitiveBatch has not yet begun");
        }
        /// <summary>
        /// Draws the current contents of this <see cref="PrimitiveBatch"/> to the stored <see cref="GraphicsDevice"/>
        /// </summary>
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
        /// <summary>
        /// Draws the current contents of this <see cref="PrimitiveBatch"/> to the screen if the shape being added would cause an overflow in the vertex or index arrays, or throws an error if the shape is too large
        /// </summary>
        /// <param name="shapeVertices">The number of vertices in the shape to be drawn</param>
        /// <param name="shapeIndices">The number of indices in the shape to be drawn</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the number of vertices or indices exceeds their maximum allowable values</exception>
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
        /// <summary>
        /// Begins a new primitive batch
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if this <see cref="PrimitiveBatch"/> is already batching</exception>
        public void Begin() => this.Begin(Matrix.Identity);
        /// <summary>
        /// Begins a new primitive batch, with a specified transformation
        /// </summary>
        /// <param name="transformation">The transformation to be applied to all drawn shapes</param>
        /// <exception cref="InvalidOperationException">Thrown if this <see cref="PrimitiveBatch"/> is already batching</exception>
        public void Begin(Matrix transformation)
        {
            if (this._isBatching) throw new InvalidOperationException("PrimitiveBatch has already begun");

            this._effect.Projection = transformation * Matrix.CreateOrthographicOffCenter(0f, this._graphicsDevice.Viewport.Width, this._graphicsDevice.Viewport.Height, 0f, 0f, 1f);

            this._isBatching = true;
        }
        /// <summary>
        /// Flushes the current contents of this <see cref="PrimitiveBatch"/> to the stored <see cref="GraphicsDevice"/>
        /// </summary>
        public void End()
        {
            this.Flush();
            this._isBatching = false;
        }
        /// <summary>
        /// Submits a <see cref="Geometry.Triangle"/> for drawing in the current batch
        /// </summary>
        /// <param name="triangle">The <see cref="Geometry.Triangle"/> to be added</param>
        public void Triangle(Triangle triangle) => this.Triangle(triangle.P0, triangle.P1, triangle.P2);
        /// <summary>
        /// Submits a triangle for drawing in the current batch
        /// </summary>
        /// <param name="p0">The first point in the triangle to be added</param>
        /// <param name="p1">The second point in the triangle to be added</param>
        /// <param name="p2">The third point in the triangle to be added</param>
        public void Triangle(Vector2 p0, Vector2 p1, Vector2 p2)
        {
            this.EnsureBatching();
            this.HandleOverflow(3, 3);

            this._indices[this._indexCount++] = 0 + this._vertexCount;
            this._indices[this._indexCount++] = 1 + this._vertexCount;
            this._indices[this._indexCount++] = 2 + this._vertexCount;

            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(p0, 0f), this.FillColor);
            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(p1, 0f), this.FillColor);
            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(p2, 0f), this.FillColor);

            this._shapeCount++;
        }
        /// <summary>
        /// Submits a <see cref="Microsoft.Xna.Framework.Rectangle"/> for drawing in the current batch
        /// </summary>
        /// <param name="rectangle">The <see cref="Microsoft.Xna.Framework.Rectangle"/> to be added</param>
        public void Rectangle(Rectangle rectangle) => this.Rectangle(rectangle.Location.ToVector2(), (rectangle.Location + rectangle.Size).ToVector2());
        /// <summary>
        /// Submits a rectangle for drawing in the current batch
        /// </summary>
        /// <param name="topLeft">The x and y coordinates of the top left corner of the rectangle to be added</param>
        /// <param name="bottomRight">The x and y coordinates of the bottom right corner of the rectangle to be added</param>
        public void Rectangle(Vector2 topLeft, Vector2 bottomRight)
        {
            this.EnsureBatching();
            this.HandleOverflow(4, 6);

            this._indices[this._indexCount++] = 0 + this._vertexCount;
            this._indices[this._indexCount++] = 1 + this._vertexCount;
            this._indices[this._indexCount++] = 2 + this._vertexCount;

            this._indices[this._indexCount++] = 0 + this._vertexCount;
            this._indices[this._indexCount++] = 2 + this._vertexCount;
            this._indices[this._indexCount++] = 3 + this._vertexCount;

            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(topLeft.X, topLeft.Y, 0f), this.FillColor);
            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(bottomRight.X, topLeft.Y, 0f), this.FillColor);
            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(bottomRight.X, bottomRight.Y, 0f), this.FillColor);
            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(topLeft.X, bottomRight.Y, 0f), this.FillColor);

            this._shapeCount++;
        }
        /// <summary>
        /// Submits a regular polygon for drawing in the current batch
        /// </summary>
        /// <param name="centerLocation">The x and y coordinates of the center of the regular polygon to be added</param>
        /// <param name="centerToVertexDistance">The distance from the center to a vertex in the recular polygon to be added</param>
        /// <param name="sideCount">The number of sides of the current regular polygon to be added</param>
        public void RegularPolygon(Vector2 centerLocation, float centerToVertexDistance, int sideCount)
        {
            this.EnsureBatching();
            if (sideCount < 3) throw new ArgumentOutOfRangeException(nameof(sideCount), "Regular polygons must have at least 3 sides");
            this.HandleOverflow(sideCount + 1, sideCount * 3);

            Vector3 center = new(centerLocation, 0f);
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
        /// <summary>
        /// Submits a <see cref="Geometry.Circle"/> for drawing in the current batch
        /// </summary>
        /// <param name="circle">The <see cref="Geometry.Circle"/> to be added</param>
        public void Circle(Circle circle) => this.Circle(circle.Center, circle.Radius);
        /// <summary>
        /// Submits a circle for drawing in the current batch
        /// </summary>
        /// <param name="centerLocation">The x and y coordinates of the circle to be added</param>
        /// <param name="radius">The radius of the circle to be added</param>
        public void Circle(Vector2 centerLocation, float radius) => this.Ellipse(centerLocation, radius, 1f);
        /// <summary>
        /// Submits an ellipse for drawing in the current batch
        /// </summary>
        /// <param name="centerLocation">The x and y coordinates of the ellipse to be added</param>
        /// <param name="radius">The distance from the center to the nearest point on the edge of the ellipse to be added</param>
        /// <param name="eccentricity">The eccentricity of the ellipse to be added</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if eccentricity is 0</exception>
        public void Ellipse(Vector2 centerLocation, float radius, float eccentricity)
        {
            this.EnsureBatching();
            if (eccentricity == 0) throw new ArgumentOutOfRangeException(nameof(eccentricity), "Eccentricity cannot be 0");
            int sideCount = (int)radius + 4;
            this.HandleOverflow(sideCount + 1, sideCount * 3);

            Vector3 center = new(centerLocation, 0f);
            Vector3 offsetVector = new(0f, radius, 0f);
            Matrix rotation = Matrix.CreateRotationZ(MathHelper.Tau / sideCount);
            Matrix scale = Matrix.CreateTranslation(-centerLocation.X, -centerLocation.Y, 0f) * Matrix.CreateScale(1 / eccentricity, 1f, 1f) * Matrix.CreateTranslation(centerLocation.X, centerLocation.Y, 0f);

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
        /// <summary>
        /// Submits a <see cref="Geometry.LineSegment"/> for drawing in the current batch
        /// </summary>
        /// <param name="lineSegment">The <see cref="Geometry.LineSegment"/> to be added</param>
        /// <param name="width">The width of the <see cref="Geometry.LineSegment"/> to be added</param>
        public void LineSegment(LineSegment lineSegment, float width) => this.LineSegment(lineSegment.P0, lineSegment.P1, width);
        /// <summary>
        /// Submits a line segment for drawing in the current batch
        /// </summary>
        /// <param name="p0">The tail of the line segment to be added</param>
        /// <param name="p1">The head of the line segment to be added</param>
        /// <param name="width">The width of the line segment to be added</param>
        public void LineSegment(Vector2 p0, Vector2 p1, float width)
        {
            this.EnsureBatching();
            this.HandleOverflow(4, 6);

            Vector2 offset = p1 - p0;
            offset.Normalize();
            offset = new Vector2(offset.Y, -offset.X) * (width / 2);

            this._indices[this._indexCount++] = 0 + this._vertexCount;
            this._indices[this._indexCount++] = 2 + this._vertexCount;
            this._indices[this._indexCount++] = 3 + this._vertexCount;

            this._indices[this._indexCount++] = 0 + this._vertexCount;
            this._indices[this._indexCount++] = 1 + this._vertexCount;
            this._indices[this._indexCount++] = 3 + this._vertexCount;

            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(p0 + offset, 0f), this.FillColor);
            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(p0 - offset, 0f), this.FillColor);
            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(p1 + offset, 0f), this.FillColor);
            this._vertices[this._vertexCount++] = new VertexPositionColor(new Vector3(p1 - offset, 0f), this.FillColor);

            this._shapeCount++;
        }
        /// <summary>
        /// Submits a polygon for drawing in the current batch
        /// </summary>
        /// <param name="points">The ordered set of points defining the polygon to be added. The shape cannot self-intersect</param>
        public void Polygon(IEnumerable<Vector2> points) => this.Polygon(new Polygon(points)); //TODO: this makes me sad
        /// <summary>
        /// Submits a <see cref="Geometry.Polygon"/> for drawing in the current batch
        /// </summary>
        /// <param name="polygon">The polygon to be added</param>
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
