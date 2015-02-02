// Authors: Fei Tang, Kimple Ke, Ha Jin Song
// Last Modified: 28/12/2014
// Item might drop when an enemy dies, item include first-aid, shield
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
    public class EmptyItem : Item
    {
        public EmptyItem(Project2Game game, float posX, float posZ)
            : base(game, posX, posZ)
        {
            type = "empty";
        }
    }
}