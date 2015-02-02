// Authors: Fei Tang, Kimple Ke, Ha Jin Song
// Last Modified: 20/10/2014
// Shell file for shell object
//Use SharpDX

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;
using SharpDX.Toolkit.Graphics;
using SharpDX;

namespace Project2
{
    public class Shell : ModelGameObject
    {
        //Shell speed
        private float speed = 0.15f;
        public float distance;

        public Shell(Project2Game game, Tank tank, Vector3 shootPositon, Vector3 tankPositon, float yaw = 0)
            : base(game)
        {
            //load model and texture
            model = game.Content.Load<Model>("shell/shell");
            texture = game.Content.Load<Texture2D>("shell/shell_body");
            position = new Vector3(shootPositon.X,
                heightMap.GetHeight(tankPositon.X, tankPositon.Z) + 0.135f, shootPositon.Z);
            scale = 0.0005f;
            distance = 0;
            this.yaw = yaw - MathUtil.PiOverTwo;
            world = Matrix.Scaling(scale) * Matrix.RotationY(this.yaw) * Matrix.Translation(position);
            boundingSphere = TransformBoundingSphere(this, scale);
        }

        public override void Update(GameTime gameTime)
        {
            //Update shell's position every frame (gameTime)
            distance += speed;
            Vector3 speedRef = new Vector3(speed, 0, 0);
            Vector3 rotatedSpeedRef = Vector3.TransformCoordinate(speedRef, Matrix.RotationY(yaw));
            position += rotatedSpeedRef;
            world = Matrix.Scaling(scale) * Matrix.RotationY(this.yaw) * Matrix.Translation(position);
            boundingSphere = TransformBoundingSphere(this, scale / 0.55f * 0.7f);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        //Remove from game if out of range
        public bool OutOfScale() {
            float x = this.position.X;
            float y = this.position.Y;
            float z = this.position.Z;
            return (heightMap.GetHeight(x, z) > y || x < -heightMap.scale / 2 ||
                x > heightMap.scale / 2 || z < -heightMap.scale / 2 ||
                z > heightMap.scale / 2);
        }
    }
}
