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

        public void UpdateLevel() 
        {
            int dif = game.difficulty + 1;
            level.Text = "Level " + dif.ToString();
        }

        public void UpdateScore(int score)
        {
            txtScore.Text = "Score: " + score.ToString();
        }

        public void UpdatePlayerHealth(int health)
        {
            lifeBar.Value = health;
        }

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

        public void StartGame()
        {
            game.Reset();
            this.Children.Remove(mainMenu);
            if (game.paused) game.paused = false;
            game.started = true;
            ShowGameUI();
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
            game.Reset();
            if (game.paused) game.paused = false;
            game.started = true;
            ShowGameUI();

        }

        public void Victory()
        {
            HideMenu();
            HideGameUI();
            game.paused = true;

            int score = game.score + game.player.health + (int)game.gameTime.TotalGameTime.TotalSeconds/3;
            v_health.Text = "Player Health:    " + game.player.health.ToString();
            int enemiesCount = 0;
            if (Settings.difficulty == 0)
                enemiesCount += 10 - game.enemies.Count;
            else if (Settings.difficulty == 1)
                enemiesCount += 20 - game.enemies.Count;
            else
                enemiesCount += 30 - game.enemies.Count;
            v_kills.Text = "Enemies Killed:  " + enemiesCount.ToString();
            v_time.Text = "Game Time:       " +
                Math.Round((float)game.gameTime.TotalGameTime.TotalSeconds, 1, MidpointRounding.AwayFromZero) + "s";
            v_gamescore.Text = "Game Score:      " + score;

            v_scoreboard.Visibility = Visibility.Visible;
            d_scoreboard.Visibility = Visibility.Collapsed;
        }

        private void VContinueClicked(object sender, RoutedEventArgs e)
        {
            HideScoreboard();
            game.Reset();
            if (game.difficulty < 2)
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
        }

        public void Defeat() 
        {
            HideMenu();
            HideGameUI();
            game.paused = true;
            int score = game.score + game.player.health + (int)game.gameTime.TotalGameTime.TotalSeconds/3;
            if (game.player.health <= 0)
                game.player.health = 0;
            d_health.Text = "Player Health:    " + game.player.health.ToString();
            int enemiesCount = 0;
            if (Settings.difficulty == 0)
                enemiesCount += 10 - game.enemies.Count;
            else if (Settings.difficulty == 1)
                enemiesCount += 18 - game.enemies.Count;
            else
                enemiesCount += 25 - game.enemies.Count;
            d_kills.Text = "Enemies Killed:  " + enemiesCount.ToString();
            d_time.Text = "Game Time:       " + 
                Math.Round((float)game.gameTime.TotalGameTime.TotalSeconds, 1, MidpointRounding.AwayFromZero) + "s";
            d_gamescore.Text = "Game Score:      " + score;
            d_scoreboard.Visibility = Visibility.Visible;
            v_scoreboard.Visibility = Visibility.Collapsed;
        }

        private void DContinueClicked(object sender, RoutedEventArgs e)
        {
            game.Reset();
            HideScoreboard();
            game.started = false;
            mainMenu = new MainMenu(this);
            this.Children.Add(mainMenu);
        }

        private void EndGameClicked(object sender, RoutedEventArgs e)
        {
            HideMenu();
            HideGameUI();
            HideScoreboard();
            game.Reset();
            game.started = false;
            mainMenu = new MainMenu(this);
            this.Children.Add(mainMenu);
        }

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
            level.Visibility = Visibility.Visible;
        }

        public void HideGameUI()
        {
            txtScore.Visibility = Visibility.Collapsed;
            btnMenu.Visibility = Visibility.Collapsed;
            lifeBar.Visibility = Visibility.Collapsed;
            control.Visibility = Visibility.Collapsed;
            radar.Visibility = Visibility.Collapsed;
            level.Visibility = Visibility.Collapsed;
        }

        private void btnMenuClicked(object sender, RoutedEventArgs e)
        {
            ShowMenu();
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
                      .Any(p => p.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch);
        }

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
                Canvas.SetLeft(dragger, 100);
                Canvas.SetTop(dragger, 100);
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

        public void DebugClear() 
        {
            txtDebug.Text = "";    
        }

        public void Debug(string s) 
        {
            txtDebug.Text += s;
        }
    }
}
