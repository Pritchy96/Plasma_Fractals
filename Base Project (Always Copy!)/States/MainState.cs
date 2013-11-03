﻿using System;
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
        Color[,] points;

        double roughness = 8;
        double screenSize = 0;
        Random rand = new Random();

        public void Begin()
        {
            //Set screenSize.
            screenSize = width + height;
            //Create array to be filled out at the end.
            //It is the size of the pixel size of the area we are generating a Fractal for.
            points = new Color[width, height];
            //Calculate corner values (c1, c2, c3, c4).
            double c1 = rand.NextDouble();
            double c2 = rand.NextDouble();
            double c3 = rand.NextDouble();
            double c4 = rand.NextDouble();

            //Call Divide, begin the iteration.
            Divide(points, 0, 0, width, height, c1, c2, c3, c4);
        }

        //X and Y are the old c1 coordinates from the last recursive iteration.
        void Divide(Color[,] points, double x, double y, double width, double height, double c1, double c2, double c3, double c4)
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
                points[(int)(x), (int)(y)] = GenColour(finalVal * 255);
            }
        }

        public Color GenColour (double finalVal)
        {
            //High Mountains
            if (finalVal < 10)
            {
                return Color.WhiteSmoke;
            }
            else if (finalVal < 25)
            {
                return Color.DarkGray;
            }
            //Low Mountains
            else if (finalVal < 50)
            {
                return Color.Gray;
            }
            //Dark grass
            else if (finalVal < 70)
            {
                return Color.DarkGreen;
            }
            //Light Grass
            else if (finalVal < 145)
            {
                return Color.Green;
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
                    e.Graphics.FillRectangle(new SolidBrush(points[i, j]), new Rectangle(i, j, 1, 1));
                }
            }
        }
    }
}
