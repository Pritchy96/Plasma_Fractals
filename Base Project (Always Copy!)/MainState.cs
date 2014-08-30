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
        Bitmap islandBaseColour;
        int width, height;

        public MainState(int width, int height, DBPanel display)
        {
            this.width = width;
            this.height = height;
            this.display = display;
            MakeIsland();
        }

        public void MakeIsland()
        {
            islandBaseColour = FractalCreator.MakeIsland(width, height, 22);
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



                islandBaseColour.Save("C:/Users/Pritchy/Desktop/Island.png");
                MessageBox.Show("Image Saved to Desktop!");
            }
            else
            {
                MakeIsland();
            }
        }

        public void Redraw(PaintEventArgs e)
        {
            //Draws 'compiled' Image, starting at 0, 0, of course.
            e.Graphics.DrawImage(islandBaseColour, Point.Empty);
        }
    }
}
