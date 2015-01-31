// Authors: Fei Tang, Kimple Ke, Ha Jin Song
// Last Modified: 20/10/2014
// Abstraction class for objects in the game
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
    abstract public class GameObject
    {   
        //Abstract class for all GameObjects
        // BasicEffect kept for terrain
        public BasicEffect basicEffect;
        public VertexInputLayout inputLayout;
        public Project2Game game;
        public Camera camera;
        public HeightMap heightMap;

        public GameObject(Project2Game game)
        {
            this.game = game;
            camera = game.Camera;
            heightMap = game.HeightMap;
        }

        public abstract void Update(GameTime gametime);
        public abstract void Draw(GameTime gametime);
    }
}