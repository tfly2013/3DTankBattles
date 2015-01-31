// Authors: Fei Tang, Kimple Ke, Ha Jin Song
// Last Modified: 20/10/2014
// Enemy file for enemy objects
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
    //Enemy class for enemy tank AI
    public class Enemy : Tank
    {
        public string name;
        public float timer;
        public Enemy(Project2Game game, float x, float z, string name, int color)
            : base(game, x, z)
        {
            //each enemy has a name (used for recognition on player's radar)
            this.name = name;
            timer = 0;
            //Load model and texture
            model = game.Content.Load<Model>("tank/abrams/abrams");
            if (color == 0)
                texture = game.Content.Load<Texture2D>("tank/abrams/tank_body_g");
            else
                texture = game.Content.Load<Texture2D>("tank/abrams/tank_body_y");
            scale = 0.0003f;
            boundingSphere = TransformBoundingSphere(this, scale);
            CalculateSurroundings();
        }

        public override void Update(GameTime gameTime)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            timer += time;
            //reasonable movement speed for all enemies, roughly (float)5 per 10 seconds
            float moveSpeed = 0.015f;
            float amount = 0;

            float distx = this.position.X - game.player.position.X;
            float distz = this.position.Z - game.player.position.Z;
            float totalDist = (float)Math.Sqrt(distx * distx + distz * distz);

            //tracking player, target player
            float rotationSpeed = 0.02f;
            float rotate = 0;
            float theta = GetAngleInBetween(game.player);

            if (Math.Sin(theta + rotationSpeed/2) > 0)
                rotate = rotationSpeed;
            if (Math.Sin(theta - rotationSpeed/2) < 0)
                rotate = -rotationSpeed;

            if (timer < 2000)
            {
                //move to player
                amount = moveSpeed;
                //urgent situation, found player, fire immediately 
                if (totalDist < 1.0f)
                    Fire();
            }
            else
            {
                amount = 0;
                //stop, target player and fire                
                if (totalDist < 10.0f && !ShouldNotFire())
                    Fire();
            }
            if (timer > 4000)
                timer = 0;

            Enemy mate = GetNearMate();
            if (!this.Equals(mate) && !this.IsAheadOf(mate))
            {
                amount = 0;
            }

            Move(Vector3.Zero, rotate);
            if (!this.isBlocked())
                Move(new Vector3(0, 0, amount), 0);

            base.Update(gameTime);
        }

        protected Boolean isBlocked()
        {
            if (this.isCollidedWith(game.player)
               && this.isBlockedOnFront(game.player)
               || this.isBlockedOnLeft(game.player)
               || this.isBlockedOnRight(game.player))
                return true;

            //Check collision
            foreach (Tank enemy in game.enemies)
            {
                if (this.isCollidedWith(enemy) && !this.Equals(enemy)
                    && (this.isBlockedOnFront(enemy)
                    || this.isBlockedOnLeft(enemy)
                    || this.isBlockedOnRight(enemy)))
                    return true;
            }
            return false;
        }

        //implement advanced radar system into enemy so that it will not fire when 
        //any of its teammates are on its front
        protected bool ShouldNotFire()
        {
            foreach (Enemy enemy in game.enemies)
            {
                if (!this.Equals(enemy))
                {
                    double angleInBetween = GetAngleInBetween(this, enemy);

                    if (angleInBetween < 15 || angleInBetween > 345)
                    {
                        return !IsPlayerInBetween(this.GetDistance(enemy));
                    }
                }
            }
            return false;
        }

        //change if player is between two enemies 
        private bool IsPlayerInBetween(float enemydist)
        {
            double angle = GetAngleInBetween(this, game.player);
            return angle < 10 || angle > 350 && this.GetDistance(game.player) < enemydist;
        }

        //get angle of two vectors in degrees
        private double GetAngleInBetween(Tank tank1, Tank tank2)
        {
            return MathUtil.RadiansToDegrees(tank1.GetAngleInBetween(tank2));
        }

        private Enemy GetNearMate()
        {
            foreach (Enemy enemy in game.enemies)
            {
                if (!this.Equals(enemy) && enemy.GetDistance(this) < 3)
                    //enemy become too close to each other
                    return enemy;
            }
            return this;
        }

        //this enemy tank is ahead of its mate
        private bool IsAheadOf(Enemy mate)
        {
            if (Math.Cos(this.GetAngleInBetween(mate)) < 0)
                return true;
            return false;
        }
    }
}
