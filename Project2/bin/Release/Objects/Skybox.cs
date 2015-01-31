// Authors: Fei Tang, Kimple Ke, Ha Jin Song
// Last Modified: 20/10/2014
// Skybox for the game
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
    class Skybox : GameObject
    {
        //Texture in Cubic from and custome effect
        private TextureCube skyboxTexture;
        private Effect skyboxEffect;
        //vertices for cube
        public Buffer<VertexPositionColor> vertices;

        public Skybox(Project2Game game, float density)
            : base(game)
        {
            //Cube for skybox based from lab works, modified to resize
            Vector3 frontBottomLeft = new Vector3(-density, -density, -density);
            Vector3 frontTopLeft = new Vector3(-density, density, -density);
            Vector3 frontTopRight = new Vector3(density, density, -density);
            Vector3 frontBottomRight = new Vector3(density, -density, -density);
            Vector3 backBottomLeft = new Vector3(-density, -density, density);
            Vector3 backBottomRight = new Vector3(density, -density, density);
            Vector3 backTopLeft = new Vector3(-density, density, density);
            Vector3 backTopRight = new Vector3(density, density, density);

            Vector3 frontBottomLeftNormal = new Vector3(-0.333f, -0.333f, -0.333f);
            Vector3 frontTopLeftNormal = new Vector3(-0.333f, 0.333f, -0.333f);
            Vector3 frontTopRightNormal = new Vector3(0.333f, 0.333f, -0.333f);
            Vector3 frontBottomRightNormal = new Vector3(0.333f, -0.333f, -0.333f);
            Vector3 backBottomLeftNormal = new Vector3(-0.333f, -0.333f, 0.333f);
            Vector3 backBottomRightNormal = new Vector3(0.333f, -0.333f, 0.333f);
            Vector3 backTopLeftNormal = new Vector3(-0.333f, 0.333f, 0.333f);
            Vector3 backTopRightNormal = new Vector3(0.333f, 0.333f, 0.333f);

            vertices = Buffer.Vertex.New(
               game.GraphicsDevice,
               new[]
                    {
                    new VertexPositionColor(frontBottomLeft, Color.Orange), // Front
                    new VertexPositionColor(frontTopLeft, Color.Orange),
                    new VertexPositionColor(frontTopRight, Color.Orange),
                    new VertexPositionColor(frontBottomLeft, Color.Orange),
                    new VertexPositionColor(frontTopRight, Color.Orange),
                    new VertexPositionColor(frontBottomRight, Color.Orange),
                    new VertexPositionColor(backBottomLeft, Color.Orange), // BACK
                    new VertexPositionColor(backTopRight, Color.Orange),
                    new VertexPositionColor(backTopLeft, Color.Orange),
                    new VertexPositionColor(backBottomLeft, Color.Orange),
                    new VertexPositionColor(backBottomRight, Color.Orange),
                    new VertexPositionColor(backTopRight, Color.Orange),
                    new VertexPositionColor(frontTopLeft, Color.OrangeRed), // Top
                    new VertexPositionColor(backTopLeft, Color.OrangeRed),
                    new VertexPositionColor(backTopRight, Color.OrangeRed),
                    new VertexPositionColor(frontTopLeft, Color.OrangeRed),
                    new VertexPositionColor(backTopRight, Color.OrangeRed),
                    new VertexPositionColor(frontTopRight, Color.OrangeRed),
                    new VertexPositionColor(frontBottomLeft, Color.OrangeRed), // Bottom
                    new VertexPositionColor(backBottomRight, Color.OrangeRed),
                    new VertexPositionColor(backBottomLeft, Color.OrangeRed),
                    new VertexPositionColor(frontBottomLeft, Color.OrangeRed),
                    new VertexPositionColor(frontBottomRight, Color.OrangeRed),
                    new VertexPositionColor(backBottomRight, Color.OrangeRed),
                    new VertexPositionColor(frontBottomLeft, Color.DarkOrange), // Left
                    new VertexPositionColor(backBottomLeft, Color.DarkOrange),
                    new VertexPositionColor(backTopLeft, Color.DarkOrange),
                    new VertexPositionColor(frontBottomLeft, Color.DarkOrange),
                    new VertexPositionColor(backTopLeft, Color.DarkOrange),
                    new VertexPositionColor(frontTopLeft, Color.DarkOrange),
                    new VertexPositionColor(frontBottomRight, Color.DarkOrange), // Right
                    new VertexPositionColor(backTopRight, Color.DarkOrange),
                    new VertexPositionColor(backBottomRight, Color.DarkOrange),
                    new VertexPositionColor(frontBottomRight, Color.DarkOrange),
                    new VertexPositionColor(frontTopRight, Color.DarkOrange),
                    new VertexPositionColor(backTopRight, Color.DarkOrange),
                });






            //Load texture and effect
            skyboxTexture = game.Content.Load<TextureCube>("skybox/skybox");
            inputLayout = VertexInputLayout.FromBuffer(0, vertices);
            skyboxEffect = game.Content.Load<Effect>("skybox/SkyboxEffect");
        }
        public override void Update(GameTime gametime)
        {
        }
        public override void Draw(GameTime gametime)
        {

            //Set effect and draw
            skyboxEffect.Parameters["World"].SetValue(Matrix.Identity);
            skyboxEffect.Parameters["View"].SetValue(camera.View);
            skyboxEffect.Parameters["Projection"].SetValue(camera.Projection);
            skyboxEffect.Parameters["SkyBoxText"].SetResource(skyboxTexture);
            skyboxEffect.Parameters["CameraPosition"].SetValue(camera.Position);
            game.GraphicsDevice.SetVertexBuffer(vertices);
            game.GraphicsDevice.SetVertexInputLayout(inputLayout);
            skyboxEffect.CurrentTechnique.Passes[0].Apply();
            game.GraphicsDevice.Draw(PrimitiveType.TriangleList, vertices.ElementCount);

        }
    }
}
