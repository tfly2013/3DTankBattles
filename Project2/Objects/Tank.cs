// Authors: Fei Tang, Kimple Ke, Ha Jin Song
// Last Modified: 20/10/2014
// Tank file for tank objects (Both enemy and player extends this)
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
    /// The Tank class.
    /// </summary>
    public class Tank : ModelGameObject
    {
        //Heightoffset used to make them float off the ground a bit
        //Avoid actually creating whole physics of bending wheels
        protected const float HEIGHTOFFSET = 0.02f;
        protected float fireCooldown;
        public int health;

        public Vector3 front;
        public Vector3 back;
        public Vector3 left;
        public Vector3 right;

        protected float frontDistToTar;
        protected float backDistToTar;
        protected float rightDistToTar;
        protected float leftDistToTar;

        Random random;

        /// <summary>
        /// Initialize the tank.
        /// </summary>
        /// <param name="game">The game.</param>
        public Tank(Project2Game game, float positionX = 0, float positionZ = 0)
            : base(game)
        {
            random = new Random();
            health = 5;
            yaw = 0;
            scale = 0.05f;
            position = new Vector3(positionX,
                heightMap.GetHeight(positionX, positionZ) + HEIGHTOFFSET, positionZ);
            world = Matrix.Scaling(scale) * Matrix.RotationY(yaw) * Matrix.Translation(position);            
        }

        protected void Move(Vector3 move, float rotate)
        {
            //move tank given move and rotate
            yaw += rotate;
            Vector3 rotatedMove = Vector3.TransformCoordinate(move, Matrix.RotationY(yaw));
            Vector3 newPosition = position + rotatedMove;
            float edgeOffset = GetRadians();
            if (newPosition.X < heightMap.scale / 2 - edgeOffset &&
                newPosition.Z < heightMap.scale / 2 - edgeOffset &&
                newPosition.X > edgeOffset - heightMap.scale / 2 &&
                newPosition.Z > edgeOffset - heightMap.scale / 2)
            {
                position = newPosition;
                position.Y = heightMap.GetHeight(position.X, position.Z) + HEIGHTOFFSET;
                world = Matrix.Scaling(scale) * Matrix.RotationY(yaw) * Matrix.Translation(position);
                boundingSphere = TransformBoundingSphere(this, scale);
                CalculateSurroundings();
            }
        }

        public void Fire()
        {
            //Fire shell if cooldown is completed
            if (fireCooldown <= 0)
            {
                Vector3 shootRef = new Vector3(0, 0, GetRadians());
                Vector3 rotatedshootRef = Vector3.TransformCoordinate(shootRef, Matrix.RotationY(yaw));
                Shell shell = new Shell(game, this, position + rotatedshootRef, position, yaw);
                game.shells.Add(shell);
                fireCooldown = 600;
                SoundEffectInstance fireSoundInstance = game.fireSound.Create();
                fireSoundInstance.Apply3D(game.player.SoundMatrix, Vector3.Zero, shell.SoundMatrix, Vector3.UnitZ);
                fireSoundInstance.Play();
            }
        }

        public override void Update(GameTime gameTime)
        {
            //Update cool down every seconds
            if (fireCooldown > 0)
                fireCooldown -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            //Check collision with shells in the game
            for (int i = 0; i < game.shells.Count; i++)
            {
                //If collision, create particle for explosion and update them
                if (this.isHit(game.shells[i]))
                {
                    for (int j = 0; j < 200; j++)
                    {
                        Vector3 explosionPos = game.shells[i].position;
                        Vector3 velocity = Vector3.Zero;
                        float horizontalVelocity = MathUtil.Lerp(0.03f, 0.010f, (float)random.NextDouble());
                        double horizontalAngle = random.NextDouble() * MathUtil.TwoPi;
                        velocity.X += horizontalVelocity * (float)Math.Cos(horizontalAngle);
                        velocity.Z += horizontalVelocity * (float)Math.Sin(horizontalAngle);
                        velocity.Y += MathUtil.Lerp(-0.03f, 0.03f, (float)random.NextDouble());
                        game.particleSystem.Spawn(explosionPos, velocity, 0.02f, 0.05f, 0.5f, false, true);
                    }
                    //Create hit sound (spatial sound)
                    SoundEffectInstance hitSoundInstance = game.hitSound.Create();
                    hitSoundInstance.Apply3D(game.player.SoundMatrix, Vector3.Zero,
                        game.shells[i].SoundMatrix, Vector3.Zero);
                    hitSoundInstance.Play();
                    //remove shell from the game
                    game.shells.RemoveAt(i);
                    i--;
                    //remove health
                    this.health--;
                }
            }
            base.Update(gameTime);
        }

        protected float GetRadians()
        {
            return model.CalculateBounds().Radius * scale;
        }

        //Collision with other tanks, prevent them from going through one another
        protected Boolean isCollidedWith(Tank tank)
        {
            return this.boundingSphere.Intersects(tank.boundingSphere);
        }

        //Hit by missile with its hitbox
        protected Boolean isHit(Shell missile)
        {
            return this.boundingSphere.Intersects(missile.boundingSphere);
        }

        protected void CalculateSurroundings()
        {
            float x = this.position.X;
            float z = this.position.Z;
            float r = this.model.CalculateBounds().Radius * scale * 0.55f;
            front = new Vector3(x + (float)Math.Sin(yaw) * r,
                            heightMap.GetHeight(x, z), z + (float)Math.Cos(yaw) * r);
            back = new Vector3(x - (float)Math.Sin(yaw) * r,
                            heightMap.GetHeight(x, z), z - (float)Math.Cos(yaw) * r);
            right = new Vector3(x + (float)Math.Cos(yaw) * r,
                            heightMap.GetHeight(x, z), z - (float)Math.Sin(yaw) * r);
            left = new Vector3(x - (float)Math.Cos(yaw) * r,
                            heightMap.GetHeight(x, z), z + (float)Math.Sin(yaw) * r);
        }

        protected void CalculateSurroundingsToTar(Tank enemy)
        {
            Vector3 frontToTar = front - enemy.position;
            frontDistToTar = frontToTar.Length();
            Vector3 backToTar = back - enemy.position;
            backDistToTar = backToTar.Length();
            Vector3 rightToTar = right - enemy.position;
            rightDistToTar = rightToTar.Length();
            Vector3 leftToTar = left - enemy.position;
            leftDistToTar = leftToTar.Length();
        }

        protected Boolean isBlockedOnFront(Tank enemy)
        {
            CalculateSurroundingsToTar(enemy);
            return (frontDistToTar < backDistToTar &&
                frontDistToTar < rightDistToTar &&
                frontDistToTar < leftDistToTar);
        }

        protected Boolean isBlockedOnBack(Tank enemy)
        {
            CalculateSurroundingsToTar(enemy);
            return (backDistToTar < frontDistToTar &&
                backDistToTar < rightDistToTar &&
                backDistToTar < leftDistToTar);
        }

        protected Boolean isBlockedOnRight(Tank enemy)
        {
            CalculateSurroundingsToTar(enemy);
            return (rightDistToTar < frontDistToTar &&
                rightDistToTar < backDistToTar &&
                rightDistToTar < leftDistToTar);
        }

        protected Boolean isBlockedOnLeft(Tank enemy)
        {
            CalculateSurroundingsToTar(enemy);
            return (leftDistToTar < frontDistToTar &&
                leftDistToTar < rightDistToTar &&
                leftDistToTar < backDistToTar);
        }

        //get distance between two tanks
        public float GetDistance(Tank tank)
        {
            Vector3 vector = this.position - tank.position;
            return vector.Length();
        }

        //angle between two vectors
        public float GetAngleInBetween(Tank tank)
        {
            Vector3 vector1 = tank.position - this.position;
            Vector3 vector2 = this.front - this.position;
            Vector3 vector3 = this.right - this.position;
            Vector3 vector4 = this.left - this.position;
            return CalculateTrueAngleInBetween(
                                CalculateAngleInBetween(vector1, vector2),
                                CalculateAngleInBetween(vector1, vector3),
                                CalculateAngleInBetween(vector1, vector4));
        }

        //calculate the angle between two vectors
        private float CalculateAngleInBetween(Vector3 vector1, Vector3 vector2)
        {
            return Convert.ToSingle(Math.Acos((vector1.X * vector2.X + vector1.Z * vector2.Z) /
                   (Math.Sqrt(Math.Pow(vector1.X, 2) + Math.Pow(vector1.Z, 2)) *
                   Math.Sqrt(Math.Pow(vector2.X, 2) + Math.Pow(vector2.Z, 2)))));
        }

        //calculate the true angle between two vectors
        private float CalculateTrueAngleInBetween(float angleInBetween1,
                            float angleInBetween2, float angleInBetween3)
        {
            float angleInBetween = 0;
            if (angleInBetween2 < MathUtil.PiOverTwo && angleInBetween3 > MathUtil.PiOverTwo)
            {
                angleInBetween = angleInBetween1;
            }
            else if (angleInBetween2 > MathUtil.PiOverTwo && angleInBetween3 < MathUtil.PiOverTwo)
            {
                angleInBetween = MathUtil.TwoPi - angleInBetween1;
            }
            return angleInBetween;
        }
    }
}
