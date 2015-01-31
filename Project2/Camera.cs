// Authors: Fei Tang, Kimple Ke, Ha Jin Song
// Last Modified: 20/10/2014
//Main file for game application
//Use SharpDX
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;
using SharpDX.Toolkit.Diagnostics;

namespace Project2
{
    /// <summary>
    /// Manage camera movement and rotation base on input from keyboard and mouse.
    /// </summary>
    public class Camera
    {
        private Project2Game game;
        private Matrix view;
        private Vector3 reference;
        private Vector3 position;
        private Vector3 target;
        private Vector3 up;
        private float moveSpeed;
        private float yaw = 0;     
        private HeightMap heightMap;
        
        /// <summary>
        /// Initialize a camera. 
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="heightMap">height map that used for restricting camera.</param>
        public Camera(Project2Game game)
        {
            this.game = game;
            heightMap = game.HeightMap;
            // Initialize View
            position = reference = new Vector3(0, 0.4f, -1.2f);
            target = position + Vector3.UnitZ;
            up = Vector3.UnitY;
            moveSpeed = heightMap.scale / 400;
        }

        /// <summary>
        /// View Matrix.
        /// </summary>
        public Matrix View { get { return view; } }

        /// <summary>
        /// Projection Matrix.
        /// </summary>
        public Matrix Projection
        {
            get
            {
                return Matrix.PerspectiveFovRH((float)Math.PI / 4.0f,
                    (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 1000.0f); ;
            }
        }

        public Vector3 Position { get { return position; } }

        public Vector3 Target { get { return target; } }

        public Vector3 Up { get { return up; } }

        /// <summary>
        /// Update the camera base on input.
        /// </summary>
        /// <param name="gameTime">Time of game.</param>
        public void Update(Vector3 playerPosition, float playerYaw)
        {
            Vector3 rotatedReference = Vector3.TransformCoordinate(reference, Matrix.RotationY(playerYaw));            
            position = playerPosition + rotatedReference;
            yaw = playerYaw;
            UpdateViewMatrix();

            //game.Page.DebugClear();
            //game.Page.Debug("Yaw: " + playerYaw);
        }

        /// <summary>
        /// Update the view matrix base on rotation matrix.
        /// </summary>
        private void UpdateViewMatrix()
        {
            Vector3 originalTarget = new Vector3(0, 0, 1);            
            Vector3 cameraRotatedTarget = Vector3.TransformCoordinate(originalTarget, Matrix.RotationY(yaw));
            target = position + cameraRotatedTarget;
            view = Matrix.LookAtRH(position, target, up);
        }
    }
}
