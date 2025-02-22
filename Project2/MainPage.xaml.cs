﻿// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
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

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Input;
using Windows.UI.Input;
using System;
using System.Windows;
using System.Linq;
using SharpDX.Toolkit;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Generic;

namespace Project2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        public Project2Game game;
        public MainMenu mainMenu;

        public double ControlX { get; set; }
        public double ControlY { get; set; }

        private uint draggerPointerId;
        private uint firePointerId;

        //private Dictionary<int, Image> inventoryList;

        public MainPage()
        {
            InitializeComponent();
            game = new Project2Game(this);
            game.Run(this);
            mainMenu = new MainMenu(this);
            this.Children.Add(mainMenu);
            HideGameUI();
            HideScoreboard();
        }

        public void StartGame()
        {
            this.Children.Remove(mainMenu);
            if (game.paused) game.paused = false;
            game.started = true;
            ShowGameUI();
            game.Reset();
        }

        #region GameInfoUpdate
        public void UpdateLevel()
        {
            int dif = game.difficulty + 1;
            level.Text = "Level " + dif.ToString();
        }

        public void DisplayLevelBoard()
        {
            UpdateLevel();
            LevelBoard.Begin();
        }

        public void UpdateScore(int score)
        {
            txtScore.Text = "Score: " + score.ToString();
        }

        public void UpdatePlayerHealth(int health)
        {
            lifeBar.Value = health;
        }
        #endregion

        #region Inventory
        public void ResetInventory()
        {
            inventory.Children.Clear();
        }

        public void UpdateInventory(int num, bool add)
        {
            Tuple<double, double>pos = InventoryCat(num);
            double left = pos.Item1;
            double top = pos.Item2;
            if (add)
                DrawInventory(left, top, game.player.inventory[num]);
            else 
            {
                for (int i = 0; i < game.player.inventory.Count; i++)
                {
                    Tuple<double, double> pos1 = InventoryCat(i);
                    double left1 = pos1.Item1;
                    double top1 = pos1.Item2;
                    DrawInventory(left1, top1, game.player.inventory[i]);
                }
            }
        }

        private Tuple<double, double> InventoryCat(int key)
        {
            if (key == 0)
                return new Tuple<double, double>(0, 0);
            else if (key == 1)
                return new Tuple<double, double>(63.5, 0);
            else if (key == 2)
                return new Tuple<double, double>(125, 0);
            else if (key == 3)
                return new Tuple<double, double>(0, 61.5);
            else if (key == 4)
                return new Tuple<double, double>(63.5, 61.5);
            else
                return new Tuple<double, double>(125, 61.5);
        }

        private void DrawInventory(double left, double top, Item item)
        {
            Image img = new Image { Width = 35, Height = 45 };
            if (item is Firstaid)
                img.Source = new BitmapImage(new Uri("ms-appx:///Assets/UI/firstaid.png", UriKind.Absolute));
            else if (item is Shield)
                img.Source = new BitmapImage(new Uri("ms-appx:///Assets/UI/shield.png", UriKind.Absolute));
            else if (item is OilBarrel)
                img.Source = new BitmapImage(new Uri("ms-appx:///Assets/UI/oil_barrel.png", UriKind.Absolute));
            else if (item is EmptyItem)
                img.Source = new BitmapImage(new Uri("ms-appx:///Assets/UI/EmptySlot.png", UriKind.Absolute));
            
            Canvas.SetLeft(img, left);
            Canvas.SetTop(img, top);
            inventory.Children.Add(img);
        }
        #endregion

        #region Radar
        public void UpdateRadar()
        {
            radar.Children.Clear();
            //update radar
            foreach (Enemy enemy in game.enemies)
            {
                if (game.player.GetDistance(enemy) <= 15)
                {
                    double angleInBetween = game.player.GetAngleInBetween(enemy);
                    float dist = game.player.GetDistance(enemy);
                    double left = 10 * (15 - (double)Math.Sin(angleInBetween) * dist);
                    double top = 10 * (15 - (double)Math.Cos(angleInBetween) * dist);
                    Windows.UI.Color color;
                    if (dist < 5)
                        color = Windows.UI.Color.FromArgb(255, 255, 0, 0);
                    else if (dist < 10)
                        color = Windows.UI.Color.FromArgb(255, 0, 255, 255);
                    else
                        color = Windows.UI.Color.FromArgb(255, 0, 255, 0);
                    DrawEnemyPoint(left, top, color);
                }
            }
        }

        public void DrawEnemyPoint(double left, double top,
                                    Windows.UI.Color col)
        {
            Ellipse enemyPoint = new Ellipse();
            enemyPoint.Height = 10;
            enemyPoint.Width = 10;
            enemyPoint.StrokeThickness = 5;
            Canvas.SetLeft(enemyPoint, left);
            Canvas.SetTop(enemyPoint, top);
            enemyPoint.Fill = new SolidColorBrush(col);
            radar.Children.Add(enemyPoint);
        }
        #endregion

        #region InGameMenu
        private void btnMenuClicked(object sender, RoutedEventArgs e)
        {
            ShowMenu();
        }

        private void ResumeClicked(object sender, RoutedEventArgs e)
        {
            HideMenu();
            if (game.paused) game.paused = false;
        }

        private void SettingsClicked(object sender, RoutedEventArgs e)
        {
            App.ShowSettingsFlyout();
        }

        private void RestartClicked(object sender, RoutedEventArgs e)
        {
            HideMenu();
            if (game.paused) game.paused = false;
            game.started = true;
            ShowGameUI();
            game.Reset();
        }

        private void EndGameClicked(object sender, RoutedEventArgs e)
        {
            HideMenu();
            HideGameUI();
            HideScoreboard();
            game.started = false;
            mainMenu = new MainMenu(this);
            this.Children.Add(mainMenu);
        }
        #endregion

        #region ScoreBoard
        public void Victory()
        {
            HideMenu();
            HideGameUI();
            game.paused = true;

            v_health.Text = "Player Health:    " + game.player.health.ToString();
            v_kills.Text = "Enemies Killed:  " + GetEnemiesKilled().ToString();
            v_time.Text = "Game Time:       " + GetGameTime() + "s";
            v_gamescore.Text = "Game Score:      " + CalculateScore();

            v_scoreboard.Visibility = Visibility.Visible;
            d_scoreboard.Visibility = Visibility.Collapsed;
        }

        private void VContinueClicked(object sender, RoutedEventArgs e)
        {
            HideScoreboard();
            if (game.difficulty < 9)
            {
                Settings.difficulty++;
                game.started = true;
                game.paused = false;
                ShowGameUI();
            }
            else
            {
                game.started = false;
                mainMenu = new MainMenu(this);
                this.Children.Add(mainMenu);
            }
            game.Reset();
        }

        public void Defeat()
        {
            HideMenu();
            HideGameUI();
            game.paused = true;

            if (game.player.health <= 0)
                game.player.health = 0;
            d_health.Text = "Player Health:    " + game.player.health.ToString();
            d_kills.Text = "Enemies Killed:  " + GetEnemiesKilled().ToString();
            d_time.Text = "Game Time:       " + GetGameTime() + "s";
            d_gamescore.Text = "Game Score:      " + CalculateScore();

            d_scoreboard.Visibility = Visibility.Visible;
            v_scoreboard.Visibility = Visibility.Collapsed;
        }

        private void DContinueClicked(object sender, RoutedEventArgs e)
        {
            HideScoreboard();
            Settings.difficulty = 0;
            game.started = false;

            mainMenu = new MainMenu(this);
            this.Children.Add(mainMenu);
        }

        private int GetEnemiesKilled()
        {
            int enemiesCount = 0;
            if (Settings.difficulty == 0)
                enemiesCount += 4 - game.enemies.Count;
            else if (Settings.difficulty == 1)
                enemiesCount += 8 - game.enemies.Count;
            else if (Settings.difficulty == 2)
                enemiesCount += 12 - game.enemies.Count;
            else if (Settings.difficulty == 3)
                enemiesCount += 16 - game.enemies.Count;
            else if (Settings.difficulty == 4)
                enemiesCount += 20 - game.enemies.Count;
            else if (Settings.difficulty == 5)
                enemiesCount += 24 - game.enemies.Count;
            else if (Settings.difficulty == 6)
                enemiesCount += 28 - game.enemies.Count;
            else if (Settings.difficulty == 7)
                enemiesCount += 32 - game.enemies.Count;
            else if (Settings.difficulty == 8)
                enemiesCount += 36 - game.enemies.Count;
            else
                enemiesCount += 40 - game.enemies.Count;

            return enemiesCount;
        }

        private int CalculateScore()
        {
            int score = game.score + game.player.health -
                    (int)game.gameTime.ElapsedGameTime.TotalMilliseconds;
            foreach (Item item in game.player.inventory)
            {
                if (!(item is EmptyItem))
                    score += 5;
            }
            if (score < 0)
                score = 0;

            return score;
        }

        private double GetGameTime()
        {
            return Math.Round((float)game.gameTime.ElapsedGameTime.TotalMilliseconds, 1,
                MidpointRounding.AwayFromZero);
        }
        #endregion

        #region Control
        private void MoveDragger(PointerPoint point)
        {
            draggerPointerId = point.PointerId;
            double controlRadius = control.Width / 2;
            double draggerRadius = dragger.Width / 2;
            double ringWidth = controlRadius * 4 / 25;
            double R = controlRadius - draggerRadius - ringWidth;
            double X = point.Position.X - controlRadius;
            double Y = point.Position.Y - controlRadius;
            if (X * X + Y * Y <= R * R)
            {
                // Pointer is inside control ring
                Canvas.SetLeft(dragger, point.Position.X - draggerRadius);
                Canvas.SetTop(dragger, point.Position.Y - draggerRadius);
                ControlX = X;
                ControlY = Y;
            }
            else
            {
                // Pointer is outside control ring
                double dirX = Math.Sqrt(R * R / (Y * Y / X / X + 1));
                dirX = X > 0 ? dirX : -dirX;
                double dirY = Math.Sqrt(R * R / (X * X / Y / Y + 1));
                dirY = Y > 0 ? dirY : -dirY;
                Canvas.SetLeft(dragger, dirX + controlRadius - draggerRadius);
                Canvas.SetTop(dragger, dirY + controlRadius - draggerRadius);
                ControlX = dirX;
                ControlY = dirY;
            }
        }

        private void PanelPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint point = e.GetCurrentPoint(control);
            double controlRadius = control.Width / 2;
            double X = point.Position.X - controlRadius;
            double Y = point.Position.Y - controlRadius;

            if (point.PointerId == draggerPointerId)
            {
                MoveDragger(point);
            }
            if (point.IsInContact && X * X + Y * Y < controlRadius * controlRadius)
            {
                draggerPointerId = point.PointerId;
                MoveDragger(point);
            }
        }

        private void PanelPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint point = e.GetCurrentPoint(control);
            double controlRadius = control.Width / 2;
            double X = point.Position.X - controlRadius;
            double Y = point.Position.Y - controlRadius;
            if (X * X + Y * Y > controlRadius * controlRadius)
            {
                game.player.Firing = true;
                firePointerId = e.Pointer.PointerId;

                //get touch input to operate inventory
                int num = -1;
                if (Y > -554 && Y < -507)
                {
                    if (X > 121 && X < 161)
                        num = 0;
                    else if (X > 186 && X < 225)
                        num = 1;
                    else if (X > 247 && X < 286)
                        num = 2;
                }
                else if (Y > -492 && Y < -445)
                {
                    if (X > 121 && X < 161)
                        num = 3;
                    else if (X > 186 && X < 225)
                        num = 4;
                    else if (X > 247 && X < 286)
                        num = 5;
                }
                if(num != -1)
                {
                    game.player.InventoryOperate(game.player.inventory[num], num);
                    UpdateInventory(num, false);
                }
            }
        }

        private void PanelPointerRealased(object sender, PointerRoutedEventArgs e)
        {
            ResetPointer(e);
        }

        private void PanelPointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            ResetPointer(e);
        }

        private void PanelPointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            ResetPointer(e);
        }

        private void ResetPointer(PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerId == draggerPointerId)
            {
                Canvas.SetLeft(dragger, 60);
                Canvas.SetTop(dragger, 60);
                draggerPointerId = 0;
                game.Page.ControlX = game.Page.ControlY = 0;
            }
            if (e.Pointer.PointerId == firePointerId)
            {
                firePointerId = 0;
                game.player.Firing = false;
            }
        }
        #endregion

        #region UI-Functions
        public void HideScoreboard()
        {
            d_scoreboard.Visibility = Visibility.Collapsed;
            v_scoreboard.Visibility = Visibility.Collapsed;
        }

        public void ShowGameUI()
        {
            txtScore.Visibility = Visibility.Visible;
            btnMenu.Visibility = Visibility.Visible;
            lifeBar.Visibility = Visibility.Visible;
            if (Settings.onScreenControl && HasTouch())
                control.Visibility = Visibility.Visible;
            else
                control.Visibility = Visibility.Collapsed;
            radar.Visibility = Visibility.Visible;
            character.Visibility = Visibility.Visible;
        }

        public void HideGameUI()
        {
            txtScore.Visibility = Visibility.Collapsed;
            btnMenu.Visibility = Visibility.Collapsed;
            lifeBar.Visibility = Visibility.Collapsed;
            control.Visibility = Visibility.Collapsed;
            radar.Visibility = Visibility.Collapsed;
            character.Visibility = Visibility.Collapsed;
        }

        public void ShowMenu()
        {
            if (game.started)
                game.paused = true;
            menu.Visibility = Visibility.Visible;
            btnMenu.IsEnabled = false;
        }

        public void HideMenu()
        {
            menu.Visibility = Visibility.Collapsed;
            btnMenu.IsEnabled = true;
        }

        public bool HasTouch()
        {
            return Windows.Devices.Input
                      .PointerDevice.GetPointerDevices()
                      .Any(p => p.PointerDeviceType ==
                          Windows.Devices.Input.PointerDeviceType.Touch);
        }
        #endregion

        #region debug
        public void Debug(string bug)
        {
            debug.Text += bug;
        }

        public void ClearDebug()
        {
            debug.Text = "";
        }
        #endregion
    }
}
