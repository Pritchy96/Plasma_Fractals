﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Plasma_Fractal
{
    public static class FractalCreator
    {
        private static Random rand = new Random();
        private static double roughness = 0;    //Increase to create smaller "islands"
        private static double screenSize = 0;   //Width + Height of screen.
        private static Bitmap shader;

        public static Bitmap MakeIsland(int width, int height, int Roughness = 13)
        {
            shader = MakeFractal(width, height, Roughness, false);

 
            return MakeFractal(width, height, Roughness, true);
        }

        private static Bitmap MakeFractal(int width, int height, int Roughness = 13, bool coloured = true)
        {
            screenSize = width + height;
            roughness = Roughness;

            Bitmap bitmap = new Bitmap(width, height);
            
            //Calculate corner values (c1, c2, c3, c4).
            double c1 = rand.NextDouble();
            double c2 = rand.NextDouble();
            double c3 = rand.NextDouble();
            double c4 = rand.NextDouble();

            //Call Divide, begin the iteration.
            Divide(coloured, bitmap, 0, 0, width, height, c1, c2, c3, c4);

            return bitmap;
        }

        private static void Divide(bool coloured, Bitmap bitmap, double x, double y, double width, double height, double c1, double c2, double c3, double c4)
        {
            //X and Y are the old c1 coordinates from the last recursive iteration.

            double middle, mid1, mid2, mid3, mid4;

            //calculate width and hight of new rectangle by halving the last.
            double newWidth = width / 2;
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
                Divide(coloured, bitmap, x, y, newWidth, newHeight, c1, mid1, mid2, middle);
                Divide(coloured, bitmap, x + newWidth, y, width - newWidth, newHeight, mid1, c2, middle, mid3);
                Divide(coloured, bitmap, x, y + newHeight, newWidth, height - newHeight, mid2, middle, c3, mid4);
                Divide(coloured, bitmap, x + newWidth, y + newHeight, width - newWidth, height - newHeight, middle, mid3, mid4, c4);
            }
            //If our rectangles are now 1px x 1px, we are ready to calculate final values and draw.
            else
            {
                //Average the points of the pixel sized rectangle down into a single number, which is that pixels final value.
                double finalVal = (c1 + c2 + c3 + c4) / 4;

                if (coloured)
                {
                    //places the current pixel we are working with to within the image.
                    Color colour = GenColour((int)(finalVal * 255));
                    Color shaderColor = shader.GetPixel((int)x, (int)y);

                    Color shadedColour = Color.FromArgb(
                        (int)(colour.R * ((float)shaderColor.R / (float)255)),
                        (int)(colour.G * ((float)shaderColor.G / (float)255)),
                        (int)(colour.B * ((float)shaderColor.B / (float)255)));


                    bitmap.SetPixel((int)x, (int)y, shadedColour);
                }
                else
                    //places the current pixel we are working with to within the image.
                    bitmap.SetPixel((int)x, (int)y, GenColourBWShader((int)(finalVal * 255)));
            }
        }

        private static Color GenColour(int finalVal)
        {
            //Snow Peak
            if (finalVal < 10)
            {
                return Color.FromArgb(189, 196, 190);
            }
            //High Mountains
            else if (finalVal < 25)
            {
                return Color.FromArgb(139, 121, 120);
            }
            //Low Mountains
            else if (finalVal < 50)
            {
                return Color.FromArgb(92, 92, 89);
            }
            //Dark grass
            else if (finalVal < 70)
            {
                return Color.FromArgb(30, 69, 38);
            }
            //Light Grass
            else if (finalVal < 145)
            {
                return Color.FromArgb(38, 85, 50);
            }
            //Shore 1 - Inner Light Sand
            else if (finalVal < 150)
            {
                return Color.FromArgb(130, 153, 100);
            }
            //Shore 2 - Outer Dark Sand
            else if (finalVal < 148)
            {
                return Color.FromArgb(217, 217, 0);
            }
            //Shore 3 - Water
            else if (finalVal < 155)
            {
                return Color.FromArgb(50, 86, 112);
            }
            //Reef
            else if (finalVal < 170)
            {
                return Color.FromArgb(28, 61, 85);
            }
            //Sea
            else if (finalVal < 200)
            {
                return Color.FromArgb(28, 50, 63);
            }
            //Deep Sea
            else
            {
                return Color.FromArgb(30, 49, 68);
            }
        }

        private static Color GenColourOld(int finalVal)
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
                return Color.FromArgb(8, 128, 0);
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

        private static Color GenColourBW(int finalVal)
        {
            //Snow Peak
            if (finalVal < 10)
            {
                return Color.FromArgb(250, 250, 250);
            }
            //High Mountains
            else if (finalVal < 25)
            {
                return Color.FromArgb(230, 230, 230);
            }
            //Low Mountains
            else if (finalVal < 50)
            {
                return Color.FromArgb(200, 200, 200);
            }
            //Dark grass
            else if (finalVal < 70)
            {
                return Color.FromArgb(170, 170, 170);
            }
            //Light Grass
            else if (finalVal < 145)
            {
                return Color.FromArgb(140, 140, 140);
            }
            //Shore 1 - Inner Light Sand
            else if (finalVal < 150)
            {
                return Color.FromArgb(140, 140, 140);
            }
            //Shore 2 - Outer Dark Sand
            // else if (finalVal < 148)
            // {
            //     return Color.FromArgb(217, 217, 0);
            //}
            //Shore 3 - Water
            else if (finalVal < 155)
            {
                return Color.FromArgb(110, 110, 110);
            }
            //Reef
            else if (finalVal < 170)
            {
                return Color.FromArgb(80, 80, 80);
            }
            //Sea
            else if (finalVal < 200)
            {
                return Color.FromArgb(50, 50, 50);
            }
            //Deep Sea
            else
            {
                return Color.FromArgb(20, 20, 20);
            }
        }

        private static Color GenColourBWShader(int finalVal)
        {
            int transparency = 100; //Val between 0 and 255.

            //Snow Peak
            if (finalVal < 10)
            {
                return Color.FromArgb(transparency, 250, 250, 250);
            }
            //High Mountains
            else if (finalVal < 25)
            {
                return Color.FromArgb(transparency, 240, 240, 240);
            }
            //Low Mountains
            else if (finalVal < 50)
            {
                return Color.FromArgb(transparency, 230, 230, 230);
            }
            //Dark grass
            else if (finalVal < 70)
            {
                return Color.FromArgb(transparency, 220, 220, 220);
            }
            //Light Grass
            else if (finalVal < 145)
            {
                return Color.FromArgb(transparency, 210, 210, 210);
            }
            //Shore 1 - Inner Light Sand
            else if (finalVal < 150)
            {
                return Color.FromArgb(transparency, 200, 200, 200);
            }
            //Shore 2 - Outer Dark Sand
            // else if (finalVal < 148)
            // {
            //     return Color.FromArgb(217, 217, 0);
            //}
            //Shore 3 - Water
            else if (finalVal < 155)
            {
                return Color.FromArgb(transparency, 190, 190, 190);
            }
            //Reef
            else if (finalVal < 170)
            {
                return Color.FromArgb(transparency, 180, 180, 180);
            }
            //Sea
            else if (finalVal < 200)
            {
                return Color.FromArgb(transparency, 170, 170, 170);
            }
            //Deep Sea
            else
            {
                return Color.FromArgb(transparency, 160, 160, 160);
            }
        }

        //Makes sure values stay within limits.
        private static double Round(double num)
        {
            if (num > 1)
                return 1;
            else if (num < 0)
                return 0;
            else
                return num;
        }

        //Displaces value a small amount.
        private static double Displace(double rectSize)
        {
            double Max = rectSize / screenSize * roughness;
            return (rand.NextDouble() - 0.5) * Max;
        }

    }

}


/*
        private static void Divide(bool coloured, Bitmap bitmap, double x, double y, double width, double height, double c1, double c2, double c3, double c4)
        {
            //X and Y are the old c1 coordinates from the last recursive iteration.

            double middle, mid1, mid2, mid3, mid4;

            //calculate width and hight of new rectangle by halving the last.
            double newWidth = width / 2;
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
                Divide(coloured, bitmap, x, y, newWidth, newHeight, c1, mid1, mid2, middle);
                Divide(coloured, bitmap, x + newWidth, y, width - newWidth, newHeight, mid1, c2, middle, mid3);
                Divide(coloured, bitmap, x, y + newHeight, newWidth, height - newHeight, mid2, middle, c3, mid4);
                Divide(coloured, bitmap, x + newWidth, y + newHeight, width - newWidth, height - newHeight, middle, mid3, mid4, c4);
            }
            //If our rectangles are now 1px x 1px, we are ready to calculate final values and draw.
            else
            {
                //Average the points of the pixel sized rectangle down into a single number, which is that pixels final value.
                double finalVal = (c1 + c2 + c3 + c4) / 4;

                if (coloured)
                    //places the current pixel we are working with to within the image.
                    bitmap.SetPixel((int)x, (int)y, GenColour((int)(finalVal * 255)));
                else
                    //places the current pixel we are working with to within the image.
                    bitmap.SetPixel((int)x, (int)y, GenColourBWShader((int)(finalVal * 255)));
            }
        }*/