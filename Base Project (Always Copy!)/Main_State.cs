using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Plasma_Fractal
{
    public class Main_State
    {
        Bitmap islandFractal, islandShape, temperateFractal, heightFractal, rainFractal, islandColoured, gradientMap;
        Island_Display islandDisplay;
        Gen_Menu optionsMenu;
        Random rand = new Random();

        public Main_State(int width, int height, Island_Display islandDisplay)
        {
            this.islandDisplay = islandDisplay;

            /*
            optionsMenu = new Gen_Menu(this);
            optionsMenu.Show();
            optionsMenu.Owner = islandDisplay;
            optionsMenu.StartPosition = FormStartPosition.Manual;   //Setting StartPosition to Location
             * */
        }

        public void MakeIsland(int width, int height, bool coloured = true, bool shaded = true, bool noise = true, int rivers = 0, int baseRoughness = 4, int shaderRoughness = 18)
        {
            islandDisplay.DrawScreen.Size = new Size(width, height);    //Set size of the screen to be drawn to (holding the bitmap)
            Size border = islandDisplay.Size - islandDisplay.ClientSize;    //Size of window borders.
            islandDisplay.MaximumSize = new Size(width, height) + border;   //Sets max form size (so user can't make it bigger than the map, leading to ugly white borders.

            islandFractal = Fractal_Creator.MakeFractal(width, height, 12); 
            islandShape = Fractal_Creator.ShapeIsland(islandFractal);

            temperateFractal = Fractal_Creator.MakeFractal(width, height, 4, 100, 0);
            
            rainFractal = Fractal_Creator.MakeFractal(width, height, 4, 100, 0);
            heightFractal = Fractal_Creator.MakeFractal(width, height, 12);
            gradientMap = Fractal_Creator.CalculateGradient(width, height, 255, 0);


            heightFractal = Fractal_Creator.InterpolateBitmaps(heightFractal, gradientMap, 0.3, 1);
            temperateFractal = Fractal_Creator.InterpolateBitmaps(temperateFractal, gradientMap, 1, 1, 0);
            islandColoured = Fractal_Creator.CalculateBiomes(islandFractal, islandShape, heightFractal, temperateFractal, rainFractal);
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
                DirectoryInfo dir = Directory.CreateDirectory(System.IO.Path.Combine(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Island")));

                //dir.FullName
                islandFractal.Save(System.IO.Path.Combine(dir.FullName, "islandFractal.bmp"), ImageFormat.Bmp);
                islandShape.Save(System.IO.Path.Combine(dir.FullName, "islandShape.bmp"), ImageFormat.Bmp);
                islandColoured.Save(System.IO.Path.Combine(dir.FullName, "islandColoured.bmp"), ImageFormat.Bmp);
                temperateFractal.Save(System.IO.Path.Combine(dir.FullName, "temperateFractal.bmp"), ImageFormat.Bmp);
                heightFractal.Save(System.IO.Path.Combine(dir.FullName, "heightFractal.bmp"), ImageFormat.Bmp);
                rainFractal.Save(System.IO.Path.Combine(dir.FullName, "rainFractal.bmp"), ImageFormat.Bmp);
                MessageBox.Show("Image Saved to Desktop!");
            }
        }

        public void ScreenResized()
        {
            /*
            if (islandDisplay.Bounds.Right + optionsMenu.Width + 5 < Screen.PrimaryScreen.Bounds.Width)
            {
                optionsMenu.Location = new Point(islandDisplay.Bounds.Right + 5, islandDisplay.Bounds.Top);
            }
            else
            {
                optionsMenu.Location = new Point(Screen.PrimaryScreen.Bounds.Right - optionsMenu.Width, islandDisplay.Bounds.Top);
            }
             * */

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