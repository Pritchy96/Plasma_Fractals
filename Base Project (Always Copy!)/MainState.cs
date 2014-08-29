using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Plasma_Fractal
{
    class MainState
    {
        DBPanel display;
        Bitmap islandBitmap;
        int width, height;

        public MainState(int width, int height, DBPanel display)
        {
            this.width = width;
            this.height = height;
            this.display = display;
            islandBitmap = FractalCreator.MakeFractal(width, height, 22);
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
                islandBitmap.Save("C:/Users/Pritchy/Desktop/Island.png");
                MessageBox.Show("Image Saved to Desktop!");
            }
            else
            {
                islandBitmap = FractalCreator.MakeFractal(width, height, 22);
            }
        }

        public void Redraw(PaintEventArgs e)
        {
            //Draws 'compiled' Image, starting at 0, 0, of course.
            e.Graphics.DrawImage(islandBitmap, Point.Empty);
            
        }
    }
}
