// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

// Authors: Fei Tang, Kimple Ke, Ha Jin Song
// Last Modified: 20/10/2014
//Main file for game application
//Use SharpDX

using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;
using SharpDX.Toolkit.Graphics;
using Windows.UI.Core;
using Windows.Devices.Sensors;
using SharpDX.Toolkit.Audio;

namespace Project2
{

    public class Project2Game : Game
    {
        public GraphicsDeviceManager graphicsDeviceManager;
        // Add input managers and input states.
        // Add accelerometer
        public KeyboardManager keyboardManager;
        public AudioManager audioManager;
        public KeyboardState keyboardState;
        public Accelerometer accel;
        public AccelerometerReading accel_val;

        //Models used for the game
        private Landscape landscape;
        public Player player;
        public List<Enemy> enemies;
        public List<Shell> shells;
        public List<Item> items;
        private Skybox skybox;
        public ParticleSystem particleSystem;
        //Number of enemys per game
        public int MAX_ENEMIES = 4;
        //index for chance to get shield and first-aid
        public int shield_index = 3;
        public int firstaid_index = 2;
        public int barrel_index = 4;
        //difficulty level for the game
        public int difficulty;
        //Random used to randomly place enemies on the plane
        public Random random;

        //Game State
        public bool started = false;
        public bool paused = false;
        public GameTime gameTime;

        public int score;
        //Audio files
        public SoundEffect explosionSound;
        public SoundEffect pickUpItemSound;
        public SoundEffect fireSound;
        public SoundEffect hitSound;
        public SoundEffect backgroundMusic;
        public SoundEffect victorySound;
        public SoundEffect defeatSound;
        SoundEffectInstance backgroundMusicInstance;

        public MainPage Page { get; set; }

        public HeightMap HeightMap { get; set; }

        public Camera Camera { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Project2Game" /> class.
        /// </summary>
        public Project2Game(MainPage page)
        {
            // Creates a graphics manager. This is mandatory.
            graphicsDeviceManager = new GraphicsDeviceManager(this);

            // Setup the relative directory to the executable directory
            // for loading contents with the ContentManager
            Content.RootDirectory = "Content";

            // Create the keyboard manager
            keyboardManager = new KeyboardManager(this);

            // Create audio manager, audio volume needs to be dependent on distance between source and listener
            audioManager = new AudioManager(this);
            audioManager.EnableMasterVolumeLimiter();
            audioManager.EnableSpatialAudio();

            this.Page = page;
            random = new Random();
        }

        protected override void LoadContent()
        {
            //Generate terrain of size 512 by 512
            HeightMap = new HeightMap(513, 1.0f, 60.0f);
            Random r = new Random();
            Camera = new Camera(this);
            landscape = new Landscape(this);
            //Skybox, Cube containing the terrain, same size as terrain
            skybox = new Skybox(this, 513.0f);
            player = new Player(this);
            shells = new List<Shell>();
            enemies = new List<Enemy>();
            items = new List<Item>();

            //initialized score
            score = 0;
            //Get accelerometer
            accel = Accelerometer.GetDefault();
            //Set particle system
            particleSystem = new ParticleSystem(this);

            //Load sounds
            explosionSound = Content.Load<SoundEffect>("sound/explosion");
            pickUpItemSound = Content.Load<SoundEffect>("sound/pickUpItem");
            fireSound = Content.Load<SoundEffect>("sound/fire");
            hitSound = Content.Load<SoundEffect>("sound/hit");
            victorySound = Content.Load<SoundEffect>("sound/Victory");
            defeatSound = Content.Load<SoundEffect>("sound/Defeat");
            backgroundMusic = Content.Load<SoundEffect>("sound/background_music");
            backgroundMusicInstance = backgroundMusic.Create();
            base.LoadContent();
        }

        protected override void Initialize()
        {
            Window.Title = "Project 2";

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;
            //Update if and only if game is started and is not paused
            if (started && !paused)
            {
                // intialize input states
                keyboardState = keyboardManager.GetState();
                //Get accelerometer if device supports it
                if (accel != null)
                {
                    accel_val = accel.GetCurrentReading();
                }
                if (!backgroundMusicInstance.State.Equals(SoundState.Playing))
                {
                    backgroundMusicInstance.IsLooped = true;
                    backgroundMusicInstance.Volume = 0.1f;
                    backgroundMusicInstance.Play();
                }

                //provide different difficulty levels accordingly
                if (Settings.difficulty != difficulty)
                {
                    difficulty = Settings.difficulty;
                    if (Settings.difficulty == 0)
                        MAX_ENEMIES = 4;
                    else if (Settings.difficulty == 1)
                        MAX_ENEMIES = 8;
                    else if (Settings.difficulty == 2)
                        MAX_ENEMIES = 12;
                    else if (Settings.difficulty == 3)
                        MAX_ENEMIES = 16;
                    else if (Settings.difficulty == 4)
                        MAX_ENEMIES = 20;
                    else if (Settings.difficulty == 5)
                        MAX_ENEMIES = 24;
                    else if (Settings.difficulty == 6)
                        MAX_ENEMIES = 28;
                    else if (Settings.difficulty == 7)
                        MAX_ENEMIES = 32;
                    else if (Settings.difficulty == 8)
                        MAX_ENEMIES = 36;
                    else
                        MAX_ENEMIES = 40;
                    Reset();
                }

                //Update terrain, player and particles that are already ceated
                landscape.Update(gameTime);
                player.Update(gameTime);
                particleSystem.Update(gameTime);
                //Update shells fired, remove if out of range
                for (int i = 0; i < shells.Count; i++)
                {
                    if (shells[i].OutOfScale() || shells[i].distance > 10)
                    {
                        shells.RemoveAt(i);
                        i--;
                    }
                    else
                        shells[i].Update(gameTime);
                }

                //Update enemies on field, remove if health = 0
                for (int i = 0; i < enemies.Count; i++)
                {
                    if (enemies[i].health <= 0)
                    {
                        //add item
                        int ran = random.Next(0, 10);
                        if (ran <= firstaid_index)
                            items.Add(new Firstaid(this, enemies[i].position.X,
                                enemies[i].position.Z, enemies[i].yaw));
                        else if (ran <= shield_index && ran > firstaid_index)
                            items.Add(new Shield(this, enemies[i].position.X,
                                enemies[i].position.Z, enemies[i].yaw));
                        else if (ran <= barrel_index && ran > shield_index)
                            items.Add(new OilBarrel(this, enemies[i].position.X,
                                enemies[i].position.Z, enemies[i].yaw));

                        // Explosion particle
                        for (int j = 0; j < 200; j++)
                        {
                            Vector3 explosionPos = enemies[i].position;
                            Vector3 velocity = Vector3.Zero;
                            float horizontalVelocity = MathUtil.Lerp(0.2f, 0.3f, (float)random.NextDouble());
                            double horizontalAngle = random.NextDouble() * MathUtil.TwoPi;
                            velocity.X += horizontalVelocity * (float)Math.Cos(horizontalAngle);
                            velocity.Z += horizontalVelocity * (float)Math.Sin(horizontalAngle);
                            velocity.Y += MathUtil.Lerp(-0.2f, 0.2f, (float)random.NextDouble());
                            particleSystem.Spawn(explosionPos, velocity, 0.2f, 0.5f, 1f);
                        }

                        // Explosion Sound
                        SoundEffectInstance explosionSoundInstance = explosionSound.Create();
                        explosionSoundInstance.Apply3D(player.SoundMatrix, Vector3.Zero, enemies[i].SoundMatrix, Vector3.Zero);
                        explosionSoundInstance.Play();
                        enemies.RemoveAt(i);
                        score += 10;
                        i--;
                    }
                    else
                    {
                        enemies[i].Update(gameTime);
                    }
                }

                //Check item collision with player in the game
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].isCollidedWith(player))
                    {
                        // Pick-Up-Item Sound
                        SoundEffectInstance pickUpItemSoundInstance = pickUpItemSound.Create();
                        pickUpItemSoundInstance.Apply3D(player.SoundMatrix, Vector3.Zero, enemies[i].SoundMatrix, Vector3.Zero);
                        pickUpItemSoundInstance.Play();
                        for (int j = 0; j < player.inventory.Count; j++)
                        {
                            if (player.inventory[j] is EmptyItem)
                            {
                                player.inventory[j] = items[i];
                                Page.UpdateInventory(j, true);
                                break;
                            }
                        }
                        items.RemoveAt(i);
                        i--;
                    }
                    else if (items[i].timeRemained <= 0)
                    {
                        items.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        items[i].Update(gameTime);
                    }
                }

                //Update radar, dependent on enemies on field
                Page.UpdateRadar();

                //update score
                Page.UpdateScore(score);

                //update player health
                Page.UpdatePlayerHealth(player.health);

                if (keyboardState.IsKeyDown(Keys.Escape))
                    Page.ShowMenu();

                //Game ending scenario
                if (player.health <= 0 && enemies.Count > 0)
                {
                    // Explosion particle
                    for (int j = 0; j < 200; j++)
                    {
                        Vector3 explosionPos = player.position;
                        Vector3 velocity = Vector3.Zero;
                        float horizontalVelocity = MathUtil.Lerp(0.2f, 0.3f, (float)random.NextDouble());
                        double horizontalAngle = random.NextDouble() * MathUtil.TwoPi;
                        velocity.X += horizontalVelocity * (float)Math.Cos(horizontalAngle);
                        velocity.Z += horizontalVelocity * (float)Math.Sin(horizontalAngle);
                        velocity.Y += MathUtil.Lerp(-0.2f, 0.2f, (float)random.NextDouble());
                        particleSystem.Spawn(explosionPos, velocity, 0.2f, 0.5f, 1f);
                    }

                    // Explosion Sound
                    SoundEffectInstance explosionSoundInstance = explosionSound.Create();
                    explosionSoundInstance.Apply3D(player.SoundMatrix, Vector3.Zero, player.SoundMatrix, Vector3.Zero);
                    explosionSoundInstance.Play();

                    SoundEffectInstance defeatSoundInstance = defeatSound.Create();
                    defeatSoundInstance.Play();
                    Page.Defeat();

                }
                if (player.health > 0 && enemies.Count <= 0)
                {
                    SoundEffectInstance victorySoundInstance = victorySound.Create();
                    victorySoundInstance.Play();
                    Page.Victory();
                }
            }
            else
            {
                if (backgroundMusicInstance.State.Equals(SoundState.Playing))
                    backgroundMusicInstance.Pause();
            }

            // Handle base.Update
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //Draw models and object if started
            if (started)
            {
                // Clears the screen with the Color.CornflowerBlue
                GraphicsDevice.Clear(SharpDX.Color.CornflowerBlue);
                skybox.Draw(gameTime);
                landscape.Draw(gameTime);
                player.Draw(gameTime);
                foreach (Enemy enemy in enemies)
                    enemy.Draw(gameTime);
                foreach (Shell shell in shells)
                    shell.Draw(gameTime);
                foreach (Item item in items)
                    item.Draw(gameTime);
                particleSystem.Draw(gameTime);
            }
            else
            {
                GraphicsDevice.Clear(SharpDX.Color.Black);
            }
            // Handle base.Draw
            base.Draw(gameTime);
        }

        //Reset function resets the models and object of the game
        public void Reset()
        {
            this.ResetElapsedTime();

            //Reset things back to initial state by instancing them again
            HeightMap = new HeightMap(513, 1.0f, 60.0f);
            Random r = new Random();
            Camera = new Camera(this);
            landscape = new Landscape(this);
            player = new Player(this);
            shells = new List<Shell>();
            enemies = new List<Enemy>();
            items = new List<Item>();
            Page.ResetInventory();
            score = 0;
            for (int i = 0; i < MAX_ENEMIES; i++)
            {
                int color = 0;
                if (i > MAX_ENEMIES / 2)
                    color = 1;
                string name = "enemyPoint" + i.ToString();
                enemies.Add(new Enemy(this, r.NextFloat(-24.0f, 24.0f),
                    r.NextFloat(-24.0f, 24.0f), name, color));
            }
            particleSystem = new ParticleSystem(this);

            //difficulty level
            Page.DisplayLevelBoard();
        }
    }
}
