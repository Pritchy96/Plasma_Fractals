﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Plasma_Fractal
{
    public class Main_State
    {
        Bitmap islandFractal, islandColoured, shader, heightMap;
        Island_Display islandDisplay;
        Gen_Menu optionsMenu;
        Random rand = new Random();

        public Main_State(int width, int height, Island_Display islandDisplay)
        {
            this.islandDisplay = islandDisplay;

            optionsMenu = new Gen_Menu(this);
            optionsMenu.Show();
            optionsMenu.Owner = islandDisplay;
            optionsMenu.StartPosition = FormStartPosition.Manual;   //Setting StartPosition to Location
        }

        public void MakeIsland(int width, int height, bool coloured = true, bool shaded = true, bool noise = true, int baseRoughness = 24, int shaderRoughness = 18)
        {
            islandDisplay.DrawScreen.Size = new Size(width, height);    //Set size of the screen to be drawn to (holding the bitmap)
            Size border = islandDisplay.Size - islandDisplay.ClientSize;    //Size of window borders.
            islandDisplay.MaximumSize = new Size(width, height) + border;   //Sets max form size (so user can't make it bigger than the map, leading to ugly white borders.

            shader = Fractal_Creator.MakeFractal(width, height, shaderRoughness);
            shader = Fractal_Creator.ColourBitmapBW(shader, null, false, 255);

            islandFractal = Fractal_Creator.MakeFractal(width, height, baseRoughness);

            #region Specifying Image parameters (Colour, shade etc)
            if (coloured)   //Colour the island.
            {
                #region Shade
                if (shaded)
                {
                    #region Noise
                    if (noise)
                    {
                        islandColoured = Fractal_Creator.ColourBitmap(islandFractal, shader, true, 255);
                    }
                    else
                    {
                        islandColoured = Fractal_Creator.ColourBitmap(islandFractal, shader, false, 255);
                    }
                    #endregion
                }
                else
                {
                    #region Noise
                    if (noise)
                    {
                        islandColoured = Fractal_Creator.ColourBitmap(islandFractal, null, true, 255);
                    }
                    else
                    {
                        islandColoured = Fractal_Creator.ColourBitmap(islandFractal, null, false, 255);
                    }
                    #endregion
                }
                #endregion
            }
            else   //Keep the island Black and White
            {
                #region Shade
                if (shaded)
                {
                    #region Noise
                    if (noise)
                    {
                        islandColoured = Fractal_Creator.ColourBitmapBW(islandFractal, shader, true, 255);
                    }
                    else
                    {
                        islandColoured = Fractal_Creator.ColourBitmapBW(islandFractal, shader, false, 255);
                    }
                    #endregion
                }
                else
                {
                    #region Noise
                    if (noise)
                    {
                        islandColoured = Fractal_Creator.ColourBitmapBW(islandFractal, null, true, 255);
                    }
                    else
                    {
                        islandColoured = Fractal_Creator.ColourBitmapBW(islandFractal, null, false, 255);
                    }
                    #endregion
                }
                #endregion
            }
            #endregion

            if (coloured)
            {
                for (int i = 0; i < 10; i++)
                    AddRiver();
            }
            heightMap = Fractal_Creator.ColourBitmapHeightMapBW(islandFractal, null, false, 255);   
        }

        public void AddRiver()
        {
            Point currentPixel = new Point();
            Color riverColour = Color.DarkBlue;
            
            Point bestDirectionPoint = new Point();
            int bestValue = 255;

            do
            {
                currentPixel.X = rand.Next(10, islandFractal.Width - 10);
                currentPixel.Y = rand.Next(10, islandFractal.Height - 10);
            }
            while (islandFractal.GetPixel(currentPixel.X, currentPixel.Y).B > 150);  //Choose a position which is on high land. (blue heightmap)

            islandColoured.SetPixel(currentPixel.X, currentPixel.Y, riverColour);   //Colour first pixel  

            while (islandFractal.GetPixel(currentPixel.X, currentPixel.Y).B < 155)  
            {

                bestValue = 0;

                if (islandFractal.GetPixel(currentPixel.X, currentPixel.Y - 1).B > bestValue && islandColoured.GetPixel(currentPixel.X, currentPixel.Y - 1).ToArgb() != Color.DarkBlue.ToArgb()) 
                {
                    bestValue = islandFractal.GetPixel(currentPixel.X, currentPixel.Y - 1).B;
                    bestDirectionPoint = new Point(currentPixel.X, currentPixel.Y - 1);
                }

                if (islandFractal.GetPixel(currentPixel.X, currentPixel.Y + 1).B > bestValue && islandColoured.GetPixel(currentPixel.X, currentPixel.Y + 1).ToArgb() != Color.DarkBlue.ToArgb())
                {
                    bestValue = islandFractal.GetPixel(currentPixel.X, currentPixel.Y + 1).B;
                    bestDirectionPoint = new Point(currentPixel.X, currentPixel.Y + 1);
                }
                if (islandFractal.GetPixel(currentPixel.X - 1, currentPixel.Y).B > bestValue && islandColoured.GetPixel(currentPixel.X - 1, currentPixel.Y).ToArgb() != Color.DarkBlue.ToArgb())
                {
                    bestValue = islandFractal.GetPixel(currentPixel.X - 1, currentPixel.Y).B;
                    bestDirectionPoint = new Point(currentPixel.X - 1, currentPixel.Y);
                }
                if (islandFractal.GetPixel(currentPixel.X + 1, currentPixel.Y).B > bestValue && islandColoured.GetPixel(currentPixel.X + 1, currentPixel.Y).ToArgb() != Color.DarkBlue.ToArgb())
                {
                    bestValue = islandFractal.GetPixel(currentPixel.X + 1, currentPixel.Y).B;
                    bestDirectionPoint = new Point(currentPixel.X + 1, currentPixel.Y);
                }

                if (bestValue == 0)
                {
                    islandColoured.SetPixel(bestDirectionPoint.X, bestDirectionPoint.Y, riverColour);   //Colour first pixel  
                    bestDirectionPoint.X += 2;
                }

                islandColoured.SetPixel(bestDirectionPoint.X, bestDirectionPoint.Y, riverColour);   //Colour first pixel  
                currentPixel = bestDirectionPoint;
            }
        
        }

        public void Update()
        {

        }

        public void MouseMoved(MouseEventArgs e)
        {
        }

        public void MouseClicked(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                islandFractal.Save("C:/Users/Pritchy/Desktop/HeightMap.png");
                islandColoured.Save("C:/Users/Pritchy/Desktop/Island.png");
                MessageBox.Show("Image Saved to Desktop!");
            }
            else
            {
                // MakeIsland(width, height);
            }
        }

        public void ScreenResized()
        {
            if (islandDisplay.Bounds.Right + optionsMenu.Width + 5 < Screen.PrimaryScreen.Bounds.Width)
            {
                optionsMenu.Location = new Point(islandDisplay.Bounds.Right + 5, islandDisplay.Bounds.Top);
            }
            else
            {
                optionsMenu.Location = new Point(Screen.PrimaryScreen.Bounds.Right - optionsMenu.Width, islandDisplay.Bounds.Top);
            }

        }

        public void Redraw(PaintEventArgs e)
        {
            //Draws 'compiled' Image, starting at 0, 0, of course.
            e.Graphics.DrawImage(islandColoured, Point.Empty);
        }
    }
}

/*

        public void Colour_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            colour = cb.Checked;
        }

        public void Shade_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            shade = cb.Checked;
        }

        public void Clouds_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            clouds = cb.Checked;
        }

*/

/*
        public void Colour_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            colour = cb.Checked;
        }

        public void Shade_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            shade = cb.Checked;
        }

        public void Clouds_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            clouds = cb.Checked;
        }

*/