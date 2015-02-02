// Authors: Fei Tang, Kimple Ke, Ha Jin Song
// Last Modified: 20/10/2014
// ModelGameObject extends GameObject Abstract class
// class for objects that uses model (.fbx) file
//Use SharpDX
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2
{
    public class ModelGameObject: GameObject
    {
        public Model model;
        public Matrix world;
        public Vector3 position;
        public float scale;
        public float yaw;
        public BoundingSphere boundingSphere;
        public Matrix worldInversedTransposed;
        public Effect effect;
        public Texture2D texture;
        //For sound (spatial sound is used)
        public Matrix SoundMatrix
        {
            get
            {
                Vector3 reference = Vector3.TransformCoordinate(new Vector3(0, 0, 1), Matrix.RotationY(yaw));
                Vector3 target = position + reference;
                Vector3 up = Vector3.UnitY * position.Y;
                return Matrix.LookAtRH(position, target, up);
            }
        }

        public ModelGameObject(Project2Game game)
            : base(game) 
        {            
            position = Vector3.Zero;
            world = Matrix.Identity;
            worldInversedTransposed = Matrix.Transpose(Matrix.Invert(world));
            scale = 1;
            //Load shader
            effect = game.Content.Load<Effect>("ObjectShader");
            
        }

        public override void Update(GameTime gameTime)
        {
            //Glowing effect as you move around the field
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {

                    part.Effect = effect;
                    effect.Parameters["updateX"].SetValue(position.X/2);
                    effect.Parameters["updateZ"].SetValue(position.Z/2) ;

                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            //Go through Meshes in model and apply shading
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                    effect.Parameters["World"].SetValue(world);
                    effect.Parameters["Projection"].SetValue(camera.Projection);
                    effect.Parameters["View"].SetValue(camera.View);
                    effect.Parameters["cameraPos"].SetValue(camera.Position);
                    effect.Parameters["worldInvTrp"].SetValue(worldInversedTransposed);
                    effect.Parameters["ModelTexture"].SetResource(texture);
                }
                //Draw mesh individually
                //Matrix[] transforms = new Matrix[mesh.MeshParts.Count];
                mesh.Draw(game.GraphicsDevice, effect);
            }
        }

        //Set hitbox
        public static BoundingSphere TransformBoundingSphere(ModelGameObject obj, float scale)
        {
            Vector3 worldCenter = obj.position;
            float r = obj.model.CalculateBounds().Radius * scale * 0.55f;
            return new BoundingSphere(worldCenter, r);
        }
    }
}
