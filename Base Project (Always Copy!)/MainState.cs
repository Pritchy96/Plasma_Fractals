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
    class MainState
    {
        DBPanel display;
        Bitmap island, colourIsland, shadedIsland;
        int width, height;
        bool coloured, shaded;

        public MainState(int width, int height, DBPanel display)
        {
            this.display = display;
            this.width = width;
            this.height = height;
            display.Size = new Size(width, height);
            MakeIsland(width, height);
        }

        public void MakeIsland(int width, int height)
        {
            //24 is a good setting here for anything under 2000x2000 (ish)
            Bitmap shader = FractalCreator.MakeFractal(width, height, 24);
            shader = FractalCreator.ColourBitmapBW(shader, null, false, 255);

            //18 is a good setting here for anything under 2000x2000 (ish)
            island = FractalCreator.MakeFractal(width, height, 18);
            colourIsland = FractalCreator.ColourBitmap(island, shader, true, 255);
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

            coloured = !coloured;
        }


        public void Redraw(PaintEventArgs e)
        {
            if (coloured)
            {
                //Draws 'compiled' Image, starting at 0, 0, of course.
                e.Graphics.DrawImage(colourIsland, Point.Empty);
            }
            else if (shaded)
                //Draws 'compiled' Image, starting at 0, 0, of course.
                e.Graphics.DrawImage(shadedIsland, Point.Empty);
        }
        }
    }

