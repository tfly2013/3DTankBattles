// Authors: Fei Tang, Kimple Ke, Ha Jin Song
// Last Modified: 20/10/2014
// File for game Landscape
//Use SharpDX
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Project2
{
    using SharpDX.Toolkit.Graphics;
    /// <summary>
    /// The Landscape class.
    /// </summary>
    class Landscape : GameObject
    {
        private Buffer<VertexPositionNormalTexture> vertices;
        /// <summary>
        /// Initialize the landscape.
        /// </summary>
        /// <param name="game">The game.</param>
        public Landscape(Project2Game game)
            : base(game)
        {
            VertexPositionNormalTexture[] landscape;
            landscape = new VertexPositionNormalTexture[heightMap.density * heightMap.density * 4];

            // Generate vertices using heightmap
            int count = 0;
            for (int i = 0; i < heightMap.density - 1; i++)
            {
                for (int j = 0; j < heightMap.density - 1; j++)
                {
                    float x = heightMap.unitLength * i - heightMap.scale / 2;
                    float z = heightMap.unitLength * j - heightMap.scale / 2;
                    // Calculate postions
                    Vector3 vertex1 = new Vector3(x, heightMap[i, j], z);
                    Vector3 vertex2 = new Vector3(x + heightMap.unitLength, heightMap[i + 1, j], z);
                    Vector3 vertex3 = new Vector3(x, heightMap[i, j + 1], z + heightMap.unitLength);
                    Vector3 vertex4 = new Vector3(x + heightMap.unitLength,
                        heightMap[i + 1, j + 1], z + heightMap.unitLength);
                    // Calculate normals
                    Vector3 normal1 = Vector3.Cross(vertex1 - vertex3, vertex1 - vertex4);
                    Vector3 normal2 = Vector3.Cross(vertex4 - vertex2, vertex4 - vertex1);
                    // Nomralize normals
                    normal1.Normalize();
                    normal2.Normalize();
                    // Add vertices to landscape
                    landscape[count] = new VertexPositionNormalTexture(vertex3,
                        normal1, new Vector2(heightMap.GetTexture(heightMap[i, j]), 1));
                    landscape[++count] = new VertexPositionNormalTexture(vertex1,
                        normal1, new Vector2(heightMap.GetTexture(heightMap[i, j]), 0));
                    landscape[++count] = new VertexPositionNormalTexture(vertex4,
                        normal1, new Vector2(heightMap.GetTexture(heightMap[i, j]) + 0.25f, 1));
                    landscape[++count] = new VertexPositionNormalTexture(vertex2,
                        normal2, new Vector2(heightMap.GetTexture(heightMap[i, j]) + 0.25f, 0));
                    count++;
                }
                // Add degenerate triangles
                landscape[count] = landscape[count - 1];
                if (i < heightMap.density - 2)
                {
                    landscape[++count] = new VertexPositionNormalTexture(
                        new Vector3(heightMap.unitLength * (i + 1) - heightMap.scale / 2,
                            heightMap[i + 1, 1], heightMap.unitLength - heightMap.scale / 2),
                            Vector3.Zero, Vector2.Zero);

                }
                count++;
            }
            //set Vertices and effect
            vertices = Buffer.Vertex.New(game.GraphicsDevice, landscape);
            basicEffect = new BasicEffect(game.GraphicsDevice)
            {
                VertexColorEnabled = false,
                TextureEnabled = true,
                //Set texture for Landscape
                Texture = game.Content.Load<Texture2D>("terrain_texture"),
                View = camera.View,
                Projection = camera.Projection,
                World = Matrix.Identity,
                // Set up primitive color for lighting
                AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f),
                DiffuseColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                SpecularColor = new Vector3(0.3f, 0.3f, 0.3f),
                SpecularPower = 2.0f,
                Alpha = 1.0f,
                LightingEnabled = true
            };
            if (basicEffect.LightingEnabled)
            {
                // Directional Light
                basicEffect.DirectionalLight0.Enabled = true;
                basicEffect.DirectionalLight0.DiffuseColor = new Vector3(0.8f, 0.8f, 0.8f);
                basicEffect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(-1, -1, 0));
                basicEffect.DirectionalLight0.SpecularColor = new Vector3(0.7f, 0.7f, 0.7f);
            }

            inputLayout = VertexInputLayout.FromBuffer(0, vertices);
        }

        /// <summary>
        /// Update the landscape.
        /// </summary>
        /// <param name="gameTime">Time of game.</param>
        public override void Update(GameTime gameTime)
        {
            bool rotateLight = false;
            basicEffect.View = camera.View;
            basicEffect.Projection = camera.Projection;
            // Rotate Directional Light around Z axis to simulate the effect of a sun
            if (rotateLight && basicEffect.DirectionalLight0.Enabled)
            {
                float lightRotateSpeed = 0.003f;
                Vector3 oldDir = basicEffect.DirectionalLight0.Direction;
                basicEffect.DirectionalLight0.Direction =
                    Vector3.Transform(oldDir, Quaternion.RotationAxis(Vector3.UnitZ, lightRotateSpeed));
            }
        }

        /// <summary>
        /// Draw the landscape.
        /// </summary>
        /// <param name="gameTime">Time of game.</param>
        public override void Draw(GameTime gameTime)
        {
            // Setup the vertices
            game.GraphicsDevice.SetVertexBuffer(vertices);
            game.GraphicsDevice.SetVertexInputLayout(inputLayout);

            // Apply the basic effect technique
            basicEffect.CurrentTechnique.Passes[0].Apply();
            game.GraphicsDevice.Draw(PrimitiveType.TriangleStrip, vertices.ElementCount);
        }
    }
}