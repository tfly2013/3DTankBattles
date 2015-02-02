// Authors: Fei Tang, Kimple Ke, Ha Jin Song
// Last Modified: 28/12/2014
// Item might drop when an enemy dies, item include first-aid, shield
//Use SharpDX
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Audio;

namespace Project2
{
    /// <summary>
    /// The Item class.
    /// </summary>
    public class Item : ModelGameObject
    {
        protected const float HEIGHTOFFSET = 0.2f;
        public float timeRemained;

        /// <summary>
        /// Initialize the item
        /// </summary>
        /// <param name="game">The game.</param>
        public Item(Project2Game game, float posX, float posZ) 
            : base(game)
        {
            timeRemained = 10000;
            position = new Vector3(posX,
                heightMap.GetHeight(posX, posZ) + HEIGHTOFFSET, posZ);
        }

        //check if player has reached the item
        public Boolean isCollidedWith(Player player)
        {
            return this.boundingSphere.Intersects(player.boundingSphere);
        }

        public override void Update(GameTime gameTime)
        {
            //Update cool down every seconds
            if (timeRemained > 0)
                timeRemained -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            
            base.Update(gameTime);
        }
    }
}
