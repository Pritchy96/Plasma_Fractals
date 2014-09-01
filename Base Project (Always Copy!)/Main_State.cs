using System;
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
        Bitmap island, colourIsland, shadedIsland, shader;
        int width, height;
        bool colour, shade, clouds;
        Island_Display islandDisplay;
        Options_Menu optionsMenu;

        public Main_State(int width, int height, Island_Display islandDisplay)
        {
            this.islandDisplay = islandDisplay;
            this.width = width;
            this.height = height;
            
            islandDisplay.DrawScreen.Size = new Size(width, height);    //Set size of the screen to be drawn to (holding the bitmap)
            Size border = islandDisplay.Size - islandDisplay.ClientSize;    //Size of window borders.   
            islandDisplay.MaximumSize = new Size(width, height) + border;   //Sets max form size (so user can't make it bigger than the map, leading to ugly white borders.

            MakeIsland(width, height);

            optionsMenu = new Options_Menu(this); 
            optionsMenu.Show();
            optionsMenu.StartPosition = FormStartPosition.Manual;   //Setting StartPosition to Location
            optionsMenu.Location = new Point(islandDisplay.Size.Width + 5, 0);
        }

        public void MakeIsland(int width, int height)
        {
            //24 is a good setting here for anything under 2000x2000 (ish)
            shader = Fractal_Creator.MakeFractal(width, height, 24);
            shader = Fractal_Creator.ColourBitmapBW(shader, null, false, 255);

            //18 is a good setting here for anything under 2000x2000 (ish)
            island = Fractal_Creator.MakeFractal(width, height, 18);
            colourIsland = Fractal_Creator.ColourBitmap(island, shader, true, 255);

            #region Setting up Clouds
            shader.RotateFlip(RotateFlipType.Rotate270FlipX);
            for (int i = 0; i < shader.Width; i++)
            {
                for (int j = 0; j < shader.Height; j++)
                {
                    shader.SetPixel(i, j, Color.FromArgb(100, shader.GetPixel(i, j)));
                }
            }
            #endregion
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
                //  Bitmap image = new Bitmap();



                colourIsland.Save("C:/Users/Pritchy/Desktop/Island.png");
                MessageBox.Show("Image Saved to Desktop!");
            }
            else
            {
                MakeIsland(width, height);
            }
        }

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

        public void Redraw(PaintEventArgs e)
        {
            //Draws 'compiled' Image, starting at 0, 0, of course.
            e.Graphics.DrawImage(colourIsland, Point.Empty);

            if (clouds)
            {
                e.Graphics.DrawImage(shader, Point.Empty);
            }
        }
    }
}

