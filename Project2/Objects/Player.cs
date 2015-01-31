// Authors: Fei Tang, Kimple Ke, Ha Jin Song
// Last Modified: 20/10/2014
// Player file for main tank of the player
//Use SharpDX
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2
{
    public class Player : Tank
    {
        public bool Firing { get; set; }

        public Player(Project2Game game)
            : base(game)
        {
            //Load model and texture
            model = game.Content.Load<Model>("tank/tiger/tiger");
            texture = game.Content.Load<Texture2D>("tank/tiger/tank_body");
            scale = 0.05f;
            health = 100;
            Firing = false;            
            // Set hitbox
            boundingSphere = TransformBoundingSphere(this, scale);
            CalculateSurroundings();
            camera.Update(position, yaw);
        }

        public override void Update(GameTime gameTime)
        {
            //Handles player input
            float move = 0;
            float rotate = 0;

            //Input from screen touch
            if (Settings.onScreenControl)
            {
                float moveZ = -Convert.ToSingle(game.Page.ControlY) / 2000;
                move = moveZ;
                rotate = -Convert.ToSingle(game.Page.ControlX) / 5000;
            }
            //input from accelerometer
            else
            {
                float moveZ = (float)game.accel_val.AccelerationY/10;
                move = moveZ;
                rotate = -(float)game.accel_val.AccelerationX/50;
            }

            float moveSpeed = 0.05f;
            if (game.keyboardState.IsKeyDown(Keys.W))
                move += moveSpeed;
            if (game.keyboardState.IsKeyDown(Keys.S))
                move -= moveSpeed;
            if (game.keyboardState.IsKeyDown(Keys.A))
                rotate = 0.02f;
            if (game.keyboardState.IsKeyDown(Keys.D))
                rotate = -0.02f;

            if ((move > 0 && isBlockedOnFrontByAny()) || (move < 0 && isBlockedOnBackByAny()))
                move = 0;
            if ((rotate > 0 && isBlockedOnLeftByAny()) || (rotate < 0 && isBlockedOnRightByAny()))
                rotate = 0;

            if (move != 0 || rotate != 0)
            {
                Move(Vector3.UnitZ * move, rotate);
                camera.Update(position, yaw);
            }

            if (game.keyboardState.IsKeyDown(Keys.P) || Firing)
                Fire();
            base.Update(gameTime);
        }

        //Collision with other models/object
        //Front, Back and Side
        protected Boolean isBlockedOnFrontByAny()
        {
            foreach (Tank enemy in game.enemies)
            {
                if (this.isCollidedWith(enemy))
                {
                    if (this.isBlockedOnFront(enemy))
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        protected Boolean isBlockedOnBackByAny()
        {
            foreach (Tank enemy in game.enemies)
            {
                if (this.isCollidedWith(enemy))
                {
                    if (this.isBlockedOnBack(enemy))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected Boolean isBlockedOnRightByAny()
        {
            foreach (Tank enemy in game.enemies)
            {
                if (this.isCollidedWith(enemy))
                {
                    if (this.isBlockedOnRight(enemy))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected Boolean isBlockedOnLeftByAny()
        {
            foreach (Tank enemy in game.enemies)
            {
                if (this.isCollidedWith(enemy))
                {
                    if (this.isBlockedOnLeft(enemy))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
