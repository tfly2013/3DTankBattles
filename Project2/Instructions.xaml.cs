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
using SharpDX;
using System;
using System.IO;
namespace Project2
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    // TASK 4: Instructions Page
    public sealed partial class Instructions
    {
        private MainPage parent;
        public Instructions(MainPage parent)
        {
            InitializeComponent();
            this.parent = parent;
            //Set instruction text
            txtInstructions.Text += "You are surrounded by enemy tank units.\r\n";
            txtInstructions.Text += "There is no way out, you must fight on...\r\n";
            txtInstructions.Text += "Tablet users: \r\n";
            txtInstructions.Text += "\tUse controller on screen or accelerometer to control tank.\r\n";
            txtInstructions.Text += "\tPress screen to shoot.\r\n";
            txtInstructions.Text += "PC users: \r\n";
            txtInstructions.Text += "\tw - forward               a - turn left\r\n";
            txtInstructions.Text += "\ts - backward              d - turn right\r\n";
            txtInstructions.Text += "\tp/left click on mouse - fire (fire range apply)\r\n";
            txtInstructions.Text += "\tu,i,o,j,k,l buttons - apply effects of items in inventory\r\n";
            txtInstructions.Text += "Use radar to detect enemy movements:\r\n";
            txtInstructions.Text += "\tRed - You are in enemies' range and are in danger.\r\n";
            txtInstructions.Text += "\tBlue - You are detected, enemies will fire.\r\n";
            txtInstructions.Text += "\tGreen - Not within enemies' range, you are safe.\r\n";
            txtInstructions.Text += "Good luck!";
        }

        //Event for Back button, go back to main menu
        private void GoBack(object sender, RoutedEventArgs e)
        {
            parent.Children.Add(parent.mainMenu);
            parent.Children.Remove(this);
        }
    }
}
