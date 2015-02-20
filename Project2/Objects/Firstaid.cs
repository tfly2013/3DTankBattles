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
    public class Firstaid : Item
    {
        public Firstaid(Project2Game game, float posX, float posZ, float yaw)
            : base(game, posX, posZ)
        {
            model = game.Content.Load<Model>("items/first-aid/first_aid_kit");
            texture = game.Content.Load<Texture2D>("items/first-aid/firstaid_color");

            this.yaw = yaw;
            scale = 0.002f;
            world = Matrix.Scaling(scale) * Matrix.RotationY(this.yaw) * Matrix.Translation(position);
            boundingSphere = TransformBoundingSphere(this, scale);
        }
    }
}
