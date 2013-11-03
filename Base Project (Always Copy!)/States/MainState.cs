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
        double[,] points;

        double roughness = 10;
        double screenSize = 0;
        Random rand = new Random();


        public void Begin()
        {
            //Set screenSize.
            screenSize = width + height;
            //Create array to be filled out at the end.
            //It is the size of the pixel size of the area we are generating a Fractal for.
            points = new double[width, height];
            points[0, 2] = 0.8;
            points[0, 3] = 0.8;
            points[0, 4] = 0.8;
            points[0, 5] = 0.8;
            //Calculate corner values (c1, c2, c3, c4).
            double c1 = rand.NextDouble();
            double c2 = rand.NextDouble();
            double c3 = rand.NextDouble();
            double c4 = rand.NextDouble();

            //Call Divide, begin the iteration.
            Divide(points, 0, 0, width, height, c1, c2, c3, c4);
        }

        //X and Y are the old c1 coordinates from the last recursive iteration.
        void Divide(double[,] points, double x, double y, double width, double height, double c1, double c2, double c3, double c4)
        {

            double middle, mid1, mid2, mid3, mid4;

            //calculate width and hight of new rectangle by halving the last.
            double newWidth =  width / 2;
            double newHeight = height / 2;

            //If our rectangles are bigger than 1px x 1px.
            if (width > 1 && height > 1)
            {
                //Square Step.
                //Calculate middle Point by averaging corners and then adding a random displacement.
                middle = Round(((c1 + c2 + c3 + c4) / 4 + Displace(newWidth + newHeight)));

                //Diamond Step.
                //Calculating the edge points in order for the 4 points of each rectangle to all have values.
                //this is just the average of the two points it bisects.
                //POSSIBLY WRONG.
                mid1 = Round((c1 + c2) / 2);
                mid2 = Round((c1 + c3) / 2);
                mid3 = Round((c2 + c4) / 2);
                mid4 = Round((c3 + c4) / 2);

                //Call divide to calculate the middle of the new rectangles.
                Divide(points, x, y, newWidth, newHeight, c1, mid1, mid2, middle);
                Divide(points, x + newWidth, y, width - newWidth, newHeight, mid1, c2, middle, mid3);
                Divide(points, x, y + newHeight, newWidth, height - newHeight, mid2, middle, c3, mid4);
                Divide(points, x + newWidth, y + newHeight, width - newWidth, height - newHeight, middle, mid3, mid4, c4);
            }
            //If our rectangles are now 1px x 1px, we are ready to calculate final values and draw.
            else
            {
                //Average the points of the pixel sized rectangle down into a single number, which is that pixels final value.
                double finalVal = (c1 + c2 + c3 + c4) / 4;
                //Use X, Y as it's a single pixel.
                points[(int)(x), (int)(y)] = finalVal;
            }
        }

        public MainState(int width, int height)
        {
            this.width = width;
            this.height = height;
            Begin();
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
        //I don't entirely understand this. What does it achieve?
        public double Displace(double rectSize)
        {
            double Max = rectSize / screenSize * roughness;
            return (rand.NextDouble() - 0.5) * Max;
        }


        public void Update()
        {
        }

        public void MouseMoved(MouseEventArgs e)
        {
        }

        public void MouseClicked(MouseEventArgs e)
        {
        }

        public void Redraw(PaintEventArgs e)
        {
            for (int i = 0; i < points.GetLength(0); i++)
            {
                for (int j = 0; j < points.GetLength(1); j++)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb((int)(points[i, j] * 255), (int)(points[i, j] * 255), (int)(points[i, j] * 255))), new Rectangle(i, j, 1, 1));
                }
            }
        }
    }
}
