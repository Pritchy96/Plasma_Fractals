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
        int width, height;
        double roughness = 13;
        double screenSize = 0;
        Random rand = new Random();
        DBPanel display;
        Bitmap finishedImage;

        public MainState(int width, int height, DBPanel display)
        {
            this.width = width;
            this.height = height;
            this.display = display;
            finishedImage = new Bitmap(width, height);
            Begin();
        }

        public void Update()
        {
        }

        public void Begin()
        {
            //Set screenSize.
            screenSize = width + height;
            //Calculate corner values (c1, c2, c3, c4).
            double c1 = rand.NextDouble();
            double c2 = rand.NextDouble();
            double c3 = rand.NextDouble();
            double c4 = rand.NextDouble();

            //Call Divide, begin the iteration.
            Divide(0, 0, width, height, c1, c2, c3, c4);
        }

        //X and Y are the old c1 coordinates from the last recursive iteration.
        void Divide(double x, double y, double width, double height, double c1, double c2, double c3, double c4)
        {
            double middle, mid1, mid2, mid3, mid4;

            //calculate width and hight of new rectangle by halving the last.
            double newWidth =  width / 2;
            double newHeight = height / 2;

            //If our rectangles are bigger than 1px x 1px.
            if (width > 1 || height > 1)
            {
                //Square Step.
                //Calculate middle Point by averaging corners and then adding a random displacement.
                middle = Round(((c1 + c2 + c3 + c4) / 4 + Displace(newWidth + newHeight)));

                //Diamond Step.
                //Calculating the edge points in order for the 4 points of each rectangle to all have values.
                //this is just the average of the two points it bisects.
                mid1 = Round((c1 + c2) / 2);
                mid2 = Round((c1 + c3) / 2);
                mid3 = Round((c2 + c4) / 2);
                mid4 = Round((c3 + c4) / 2);

                //Call divide to calculate the middle of the new rectangles.
                Divide(x, y, newWidth, newHeight, c1, mid1, mid2, middle);
                Divide(x + newWidth, y, width - newWidth, newHeight, mid1, c2, middle, mid3);
                Divide(x, y + newHeight, newWidth, height - newHeight, mid2, middle, c3, mid4);
                Divide(x + newWidth, y + newHeight, width - newWidth, height - newHeight, middle, mid3, mid4, c4);
            }
            //If our rectangles are now 1px x 1px, we are ready to calculate final values and draw.
            else
            {
                //Average the points of the pixel sized rectangle down into a single number, which is that pixels final value.
                double finalVal = (c1 + c2 + c3 + c4) / 4;

                //places the current pixel we are working with to within the image.
                finishedImage.SetPixel((int)x, (int)y, GenColour((int)(finalVal * 255)));
            }
        }

        public Color GenColour (int finalVal)
        {
            //Snow Peak
            if (finalVal < 10)
            {
                return Color.FromArgb(245, 245, 245);
            }
            //High Mountains
            else if (finalVal < 25)
            {
                return Color.FromArgb(169, 169, 169);
            }
            //Low Mountains
            else if (finalVal < 50)
            {
                return Color.FromArgb(128, 128, 128);
            }
            //Dark grass
            else if (finalVal < 70)
            {
                return Color.FromArgb(0, 100, 0);
            }
            //Light Grass
            else if (finalVal < 145)
            {
                return Color.FromArgb(0, 128, 0);
            }
            //Shore 1 - Inner Light Sand
            else if (finalVal < 150)
            {
                return Color.FromArgb(227, 227, 69);
            }
            //Shore 2 - Outer Dark Sand
            else if (finalVal < 148)
            {
                return Color.FromArgb(217, 217, 0);
            }
            //Shore 3 - Water
            else if (finalVal < 155)
            {
                return Color.FromArgb(0, 178, 178);
            }
            //Reef
            else if (finalVal < 170)
            {
                return Color.FromArgb(0, 163, 217);
            }
            //Sea
            else if (finalVal < 200)
            {
                return Color.FromArgb(0, 133, 178);
            }
            //Deep Sea
            else
            {
                return Color.FromArgb(0, 105, 140);
            }
        }

        //Makes sure values stay within limits.
        public double Round(double num)
        {
            if (num > 1)
                return 1;
            else if (num < 0)
                return 0;
            else
                return num;
        }

        //Displaces value a small amount.
        public double Displace(double rectSize)
        {
            double Max = rectSize / screenSize * roughness;
            return (rand.NextDouble() - 0.5) * Max;
        }

        public void MouseMoved(MouseEventArgs e)
        {
        }

        public void MouseClicked(MouseEventArgs e)
        {
            finishedImage.Save("C:/Users/Pritchy/Desktop/Island.png");
        }

        public void Redraw(PaintEventArgs e)
        {
            //Draws 'compiled' Image, starting at 0, 0, of course.
            e.Graphics.DrawImage(finishedImage, Point.Empty);
        }
    }
}
