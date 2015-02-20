// Authors: Fei Tang, Kimple Ke, Ha Jin Song
// Last Modified: 28/12/2014
// Item might drop when an enemy dies, item include first-aid, shield, oil barrel
//Use SharpDX
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Project2
{
    public class OilBarrel : Item
    {
        public OilBarrel(Project2Game game, float posX, float posZ, float yaw)
            : base(game, posX, posZ)
        {
            model = game.Content.Load<Model>("items/oil-barrel/barrel");
            texture = game.Content.Load<Texture2D>("items/oil-barrel/barrel_diffuse");

            this.yaw = yaw + (float)Math.PI/2;
            scale = 0.25f;
            world = Matrix.Scaling(scale) * Matrix.RotationY(this.yaw) * Matrix.Translation(position);
            boundingSphere = TransformBoundingSphere(this, scale);
        }
    }
}