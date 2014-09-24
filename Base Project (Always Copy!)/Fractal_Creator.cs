using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Plasma_Fractal
{
    public static class Fractal_Creator
    {
        private static Random rand = new Random();
        private static double roughness = 0;    //Increase to create smaller "islands"
        private static double screenSize = 0;   //Width + Height of screen.

        public static Bitmap MakeFractal(int width, int height, int Roughness = 13)
        {
            screenSize = width + height;
            roughness = Roughness;

            Bitmap bitmap = new Bitmap(width, height);

            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            BitmapData mapBmpData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            // Get the address of the first line.
            IntPtr mapPtr = mapBmpData.Scan0;

            // Declare an array to hold the shaderBytes of the bitmap. 
            int mapBytes = Math.Abs(mapBmpData.Stride) * bitmap.Height;
            byte[] mapRgbValues = new byte[mapBytes];

            // Copy the RGB values into the array. This array just holds the RGB (BGR) values of each pixel in the format
            // B, G, R, B, G, R, B, G, R and so on.
            System.Runtime.InteropServices.Marshal.Copy(mapPtr, mapRgbValues, 0, mapBytes);

            //Calculate corner values (c1, c2, c3, c4).
            double c1 = rand.NextDouble();
            double c2 = rand.NextDouble();
            double c3 = rand.NextDouble();
            double c4 = rand.NextDouble();

            //Call Divide, begin the iteration.
            Divide(mapRgbValues, width, 0, 0, width, height, c1, c2, c3, c4);

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(mapRgbValues, 0, mapPtr, mapBytes);

            // Unlock the bits and return.
            bitmap.UnlockBits(mapBmpData);
            return bitmap;
        }

        /// <summary>
        /// </summary>
        /// <param name="mapRgbValues"> Array of BGR Bytes </param>
        /// <param name="bitmapWidth"> Width of the bitmap image </param>
        /// <param name="x"> X Pos </param>
        /// <param name="y"> Y Pos </param>
        /// <param name="width"> Width of current rectangle being worked on. </param>
        /// <param name="height"> Height of current rectangle being worked on. </param>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <param name="c3"></param>
        /// <param name="c4"></param>
        private static void Divide(byte[] mapRgbValues, int bitmapWidth, double x, double y, double width, double height, double c1, double c2, double c3, double c4)
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
                Divide(mapRgbValues, bitmapWidth, x, y, newWidth, newHeight, c1, mid1, mid2, middle);
                Divide(mapRgbValues, bitmapWidth, x + newWidth, y, width - newWidth, newHeight, mid1, c2, middle, mid3);
                Divide(mapRgbValues, bitmapWidth, x, y + newHeight, newWidth, height - newHeight, mid2, middle, c3, mid4);
                Divide(mapRgbValues, bitmapWidth, x + newWidth, y + newHeight, width - newWidth, height - newHeight, middle, mid3, mid4, c4);
            }
            //If our rectangles are now 1px x 1px, we are ready to calculate final values and draw.
            else
            {
                //Average the points of the pixel sized rectangle down into a single number, which is that pixels final value.
                double finalVal = (c1 + c2 + c3 + c4) / 4;

                //Calculate where the position of the blue (remember BGR not RGB) byte for the pixel at positon (X, Y) on the bitmap image is in the 1D byte array.
                int bytePosition = ((int) (((int)y * bitmapWidth) + (int)x) * 3);

                //Only needs to set one of the RGB values in the byte array as it is grey, and the colour methods only look at the first one (Blue, remember BGR not RGB).
                mapRgbValues[bytePosition] = (byte) (finalVal * 255);
            }
        }

        public static Bitmap ColourBitmapBW(Bitmap map, Bitmap shaderMap = null, bool noise = true, bool rivers = true, int alpha = 255)
        {
            Bitmap colouredMap = new Bitmap(map);

            #region Rivers
            if (rivers)
            {
                for (int i = 0; i < 20; i++)
                {
                    Point currentPixel = new Point();
                    Color riverColour = Color.FromArgb(200, 200, 200);

                    //The lightest (lowest height) direction.
                    Point bestDirectionPoint = new Point();
                    int bestValue = 255;

                    Point lastMove = new Point();

                    //Find a suitable starting location/spring.
                    do
                    {
                        currentPixel.X = rand.Next(10, map.Width - 10);
                        currentPixel.Y = rand.Next(10, map.Height - 10);
                    }
                    while (map.GetPixel(currentPixel.X, currentPixel.Y).B > 150);  //Choose a position which is on high land. (blue heightmap)

                    //Colour the starting location.
                    colouredMap.SetPixel(currentPixel.X, currentPixel.Y, riverColour);   //Colour first pixel  

                    #region Carve River
                    while (map.GetPixel(currentPixel.X, currentPixel.Y).B < 155)  //WHile we're not at sea level..
                    {
                        Console.WriteLine(map.GetPixel(currentPixel.X, currentPixel.Y).B.ToString());

                        if (currentPixel.Y - 1 < 0 || currentPixel.X - 1 < 0 || currentPixel.Y + 1 >= colouredMap.Height || currentPixel.X + 1 >= colouredMap.Width)
                        {
                            break;
                        }

                        bestValue = 0;  //reset the highest value each iteration.

                        if (map.GetPixel(currentPixel.X, currentPixel.Y - 1).B > bestValue && colouredMap.GetPixel(currentPixel.X, currentPixel.Y - 1).ToArgb() != riverColour.ToArgb())
                        {
                            bestValue = map.GetPixel(currentPixel.X, currentPixel.Y - 1).B;
                            bestDirectionPoint = new Point(currentPixel.X, currentPixel.Y - 1);
                        }

                        if (map.GetPixel(currentPixel.X, currentPixel.Y + 1).B > bestValue && colouredMap.GetPixel(currentPixel.X, currentPixel.Y + 1).ToArgb() != riverColour.ToArgb())
                        {
                            bestValue = map.GetPixel(currentPixel.X, currentPixel.Y + 1).B;
                            bestDirectionPoint = new Point(currentPixel.X, currentPixel.Y + 1);
                        }
                        if (map.GetPixel(currentPixel.X - 1, currentPixel.Y).B > bestValue && colouredMap.GetPixel(currentPixel.X - 1, currentPixel.Y).ToArgb() != riverColour.ToArgb())
                        {
                            bestValue = map.GetPixel(currentPixel.X - 1, currentPixel.Y).B;
                            bestDirectionPoint = new Point(currentPixel.X - 1, currentPixel.Y);
                        }
                        if (map.GetPixel(currentPixel.X + 1, currentPixel.Y).B > bestValue && colouredMap.GetPixel(currentPixel.X + 1, currentPixel.Y).ToArgb() != riverColour.ToArgb())
                        {
                            bestValue = map.GetPixel(currentPixel.X + 1, currentPixel.Y).B;
                            bestDirectionPoint = new Point(currentPixel.X + 1, currentPixel.Y);
                        }

                        //If there is no where which has not been visited around us, 
                        //This will be repeated until we can move somewhere.
                        if (bestValue == 0)
                        {
                            colouredMap.SetPixel(bestDirectionPoint.X, bestDirectionPoint.Y, riverColour);   //Colour first pixel  

                            Point temp = new Point(bestDirectionPoint.X - lastMove.X, bestDirectionPoint.Y - lastMove.Y);
                            bestDirectionPoint.X += temp.X;
                            bestDirectionPoint.Y += temp.Y;
                            currentPixel.X += temp.X;
                            currentPixel.Y += temp.Y;
                            colouredMap.SetPixel(bestDirectionPoint.X, bestDirectionPoint.Y, riverColour);   //Colour first pixel  

                        }
                        else
                        {
                            colouredMap.SetPixel(bestDirectionPoint.X, bestDirectionPoint.Y, riverColour);   //Colour first pixel  
                            lastMove = currentPixel;
                            currentPixel = bestDirectionPoint;
                        }
                    }

                    #endregion
                }
            }
            #endregion

            #region Reading Main Map Bitmap.
            //Uses Lockbits, example here: http://msdn.microsoft.com/en-us/library/5ey6h79d(v=vs.110).aspx
            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, colouredMap.Width, colouredMap.Height);

            BitmapData mapBmpData = colouredMap.LockBits(rect,
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            // Get the address of the first line.
            IntPtr mapPtr = mapBmpData.Scan0;

            // Declare an array to hold the shaderBytes of the bitmap. 
            int mapBytes = Math.Abs(mapBmpData.Stride) * colouredMap.Height;
            byte[] mapRgbValues = new byte[mapBytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(mapPtr, mapRgbValues, 0, mapBytes);
            #endregion

            #region Reading Shader Bitmap, if needed.
            byte[] shaderRgbValues = null;

            if (shaderMap != null)
            {
                BitmapData shaderBmpData = shaderMap.LockBits(rect,
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                // Get the address of the first line.
                IntPtr shaderPtr = shaderBmpData.Scan0;

                // Declare an array to hold the shaderBytes of the bitmap. 
                int shaderBytes = Math.Abs(shaderBmpData.Stride) * colouredMap.Height;
                shaderRgbValues = new byte[shaderBytes];

                // Copy the RGB values into the array.
                System.Runtime.InteropServices.Marshal.Copy(shaderPtr, shaderRgbValues, 0, shaderBytes);
            }
            #endregion

       

            #region Colour Selecting.
            //Only need to do the B value, not R or B as it's grey (RGB are all the same value).
            for (int i = 0; i < mapRgbValues.Length; i += 3)
            {
                if (mapRgbValues[i] < 50)
                {
                    mapRgbValues[i] = 100;
                    mapRgbValues[i+1] = 100;
                    mapRgbValues[i+2] = 100;
                }
                else if (mapRgbValues[i] < 100)
                {
                    mapRgbValues[i]   = 120;
                    mapRgbValues[i+1] = 120;
                    mapRgbValues[i+2] = 120;
                }
                else if (mapRgbValues[i] < 150)
                {
                    mapRgbValues[i] = 140;
                    mapRgbValues[i + 1] = 140;
                    mapRgbValues[i + 2] = 140;
                }
                else if (mapRgbValues[i] < 200)
                {
                    mapRgbValues[i] = 160;
                    mapRgbValues[i + 1] = 160;
                    mapRgbValues[i + 2] = 160;
                }
                else if (mapRgbValues[i] < 250)
                {
                    mapRgbValues[i] = 180;
                    mapRgbValues[i + 1] = 180;
                    mapRgbValues[i + 2] = 180;
                }
                else if (mapRgbValues[i] < 150)
                {
                    mapRgbValues[i] = 200;
                    mapRgbValues[i + 1] = 200;
                    mapRgbValues[i + 2] = 200;
                }
                else if (mapRgbValues[i] < 175)
                {
                    mapRgbValues[i] = 220;
                    mapRgbValues[i + 1] = 220;
                    mapRgbValues[i + 2] = 220;
                }
                else if (mapRgbValues[i] < 200)
                {
                    mapRgbValues[i] = 240;
                    mapRgbValues[i + 1] = 240;
                    mapRgbValues[i + 2] = 240;
                }
                else if (mapRgbValues[i] < 225)
                {
                    mapRgbValues[i] = 240;
                    mapRgbValues[i + 1] = 240;
                    mapRgbValues[i + 2] = 240;
                }
                else
                {
                    mapRgbValues[i] = 250;
                    mapRgbValues[i + 1] = 250;
                    mapRgbValues[i + 2] = 250;
                }

                #region Shading.
                if (shaderMap != null)  //Interpolating the coloured map with another (black and white, so R = G = B) fractal to give it texture.
                {
                    mapRgbValues[i] = (byte)Math.Min(255, (int)(mapRgbValues[i] * ((float)shaderRgbValues[i] / 255)));
                    mapRgbValues[i + 1] = (byte)Math.Min(255, (int)(mapRgbValues[i + 1] * ((float)shaderRgbValues[i] / 255)));
                    mapRgbValues[i + 2] = (byte)Math.Min(255, (int)(mapRgbValues[i + 2] * ((float)shaderRgbValues[i] / 255)));
                }
                #endregion

                #region Noise Adding.
                if (noise)  //Displacing the pixel colour by a random amount.
                {
                    mapRgbValues[i] = (byte)Math.Min(255, (mapRgbValues[i] + (int)(((float)mapRgbValues[i] / 255) * rand.Next(-60, 60))));
                    mapRgbValues[i + 1] = (byte)Math.Min(255, (mapRgbValues[i + 1] + (int)(((float)mapRgbValues[i + 1] / 255) * rand.Next(-60, 60))));
                    mapRgbValues[i + 2] = (byte)Math.Min(255, (mapRgbValues[i + 2] + (int)(((float)mapRgbValues[i + 2] / 255) * rand.Next(-60, 60))));
                }
                #endregion

            }
            #endregion

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(mapRgbValues, 0, mapPtr, mapBytes);

            // Unlock the bits and return.
            colouredMap.UnlockBits(mapBmpData);
            return colouredMap;
        }

        public static Bitmap ColourBitmap(Bitmap map, Bitmap shaderMap = null, bool noise = true, bool rivers = true, int alpha = 255)
        {
            Bitmap colouredMap = new Bitmap(map);

            #region Rivers
            if (rivers)
            {
                for (int i = 0; i < 20; i++)
                {
                    Point currentPixel = new Point();
                    Color riverColour = Color.FromArgb(200, 200, 200);

                    //The lightest (lowest height) direction.
                    Point bestDirectionPoint = new Point();
                    int bestValue = 255;

                    Point lastMove = new Point();

                    //Find a suitable starting location/spring.
                    do
                    {
                        currentPixel.X = rand.Next(10, map.Width - 10);
                        currentPixel.Y = rand.Next(10, map.Height - 10);
                    }
                    while (map.GetPixel(currentPixel.X, currentPixel.Y).B > 150);  //Choose a position which is on high land. (blue heightmap)

                    //Colour the starting location.
                    colouredMap.SetPixel(currentPixel.X, currentPixel.Y, riverColour);   //Colour first pixel  

                    #region Carve River
                    while (map.GetPixel(currentPixel.X, currentPixel.Y).B < 155)  //WHile we're not at sea level..
                    {
                        Console.WriteLine(map.GetPixel(currentPixel.X, currentPixel.Y).B.ToString());

                        if (currentPixel.Y - 1 < 0 || currentPixel.X - 1 < 0 || currentPixel.Y + 1 >= colouredMap.Height || currentPixel.X + 1 >= colouredMap.Width)
                        {
                            break;
                        }

                        bestValue = 0;  //reset the highest value each iteration.

                        if (map.GetPixel(currentPixel.X, currentPixel.Y - 1).B > bestValue && colouredMap.GetPixel(currentPixel.X, currentPixel.Y - 1).ToArgb() != riverColour.ToArgb())
                        {
                            bestValue = map.GetPixel(currentPixel.X, currentPixel.Y - 1).B;
                            bestDirectionPoint = new Point(currentPixel.X, currentPixel.Y - 1);
                        }

                        if (map.GetPixel(currentPixel.X, currentPixel.Y + 1).B > bestValue && colouredMap.GetPixel(currentPixel.X, currentPixel.Y + 1).ToArgb() != riverColour.ToArgb())
                        {
                            bestValue = map.GetPixel(currentPixel.X, currentPixel.Y + 1).B;
                            bestDirectionPoint = new Point(currentPixel.X, currentPixel.Y + 1);
                        }
                        if (map.GetPixel(currentPixel.X - 1, currentPixel.Y).B > bestValue && colouredMap.GetPixel(currentPixel.X - 1, currentPixel.Y).ToArgb() != riverColour.ToArgb())
                        {
                            bestValue = map.GetPixel(currentPixel.X - 1, currentPixel.Y).B;
                            bestDirectionPoint = new Point(currentPixel.X - 1, currentPixel.Y);
                        }
                        if (map.GetPixel(currentPixel.X + 1, currentPixel.Y).B > bestValue && colouredMap.GetPixel(currentPixel.X + 1, currentPixel.Y).ToArgb() != riverColour.ToArgb())
                        {
                            bestValue = map.GetPixel(currentPixel.X + 1, currentPixel.Y).B;
                            bestDirectionPoint = new Point(currentPixel.X + 1, currentPixel.Y);
                        }

                        //If there is no where which has not been visited around us, 
                        //This will be repeated until we can move somewhere.
                        if (bestValue == 0)
                        {
                            colouredMap.SetPixel(bestDirectionPoint.X, bestDirectionPoint.Y, riverColour);   //Colour first pixel  

                            Point temp = new Point(bestDirectionPoint.X - lastMove.X, bestDirectionPoint.Y - lastMove.Y);
                            bestDirectionPoint.X += temp.X;
                            bestDirectionPoint.Y += temp.Y;
                            currentPixel.X += temp.X;
                            currentPixel.Y += temp.Y;
                            colouredMap.SetPixel(bestDirectionPoint.X, bestDirectionPoint.Y, riverColour);   //Colour first pixel  

                        }
                        else
                        {
                            colouredMap.SetPixel(bestDirectionPoint.X, bestDirectionPoint.Y, riverColour);   //Colour first pixel  
                            lastMove = currentPixel;
                            currentPixel = bestDirectionPoint;
                        }
                    }

                    #endregion
                }
            }
            #endregion

            #region Reading Main Map Bitmap.
            //Uses Lockbits, example here: http://msdn.microsoft.com/en-us/library/5ey6h79d(v=vs.110).aspx
            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, colouredMap.Width, colouredMap.Height);

            BitmapData mapBmpData = colouredMap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            // Get the address of the first line.
            IntPtr mapPtr = mapBmpData.Scan0;

            // Declare an array to hold the shaderBytes of the bitmap. 
            int mapBytes = Math.Abs(mapBmpData.Stride) * colouredMap.Height;
            byte[] mapRgbValues = new byte[mapBytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(mapPtr, mapRgbValues, 0, mapBytes);
            #endregion

            #region Reading Shader Bitmap, if needed.
            byte[] shaderRgbValues = null;

            if (shaderMap != null)
            {
                BitmapData shaderBmpData = shaderMap.LockBits(rect,
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                // Get the address of the first line.
                IntPtr shaderPtr = shaderBmpData.Scan0;

                // Declare an array to hold the shaderBytes of the bitmap. 
                int shaderBytes = Math.Abs(shaderBmpData.Stride) * colouredMap.Height;
                shaderRgbValues = new byte[shaderBytes];

                // Copy the RGB values into the array.
                System.Runtime.InteropServices.Marshal.Copy(shaderPtr, shaderRgbValues, 0, shaderBytes);
            }
            #endregion         

            #region Colour Selecting.
            //Is in the format BGR, NOT RGB!!!
            for (int i = 0; i < mapRgbValues.Length; i += 3)
            {
                //Snow Peak
                if (mapRgbValues[i] < 10)
                {
                    mapRgbValues[i] = 175;    //Blue component
                    mapRgbValues[i + 1] = 181;   //Green Component
                    mapRgbValues[i + 2] = 174;   //Red Component
                }
                //High Mountains
                else if (mapRgbValues[i] < 25)
                {
                    mapRgbValues[i] = 130;
                    mapRgbValues[i + 1] = 131;
                    mapRgbValues[i + 2] = 149;
                }
                //Low Mountains
                else if (mapRgbValues[i] < 50)
                {
                    mapRgbValues[i] = 99;
                    mapRgbValues[i + 1] = 102;
                    mapRgbValues[i + 2] = 102;
                }

                //Dark grass
                else if (mapRgbValues[i] < 70)
                {
                    mapRgbValues[i] = 48;
                    mapRgbValues[i + 1] = 79;
                    mapRgbValues[i + 2] = 40;
                }
                //Light Grass
                else if (mapRgbValues[i] < 145)
                {
                    mapRgbValues[i] = 60;
                    mapRgbValues[i + 1] = 95;
                    mapRgbValues[i + 2] = 48;
                }
                //Shore 1 - Inner Light Sand
                else if (mapRgbValues[i] < 150)
                {
                    mapRgbValues[i] = 110;
                    mapRgbValues[i + 1] = 163;
                    mapRgbValues[i + 2] = 140;
                }
                //Shore 2 - Outer Dark Sand
                else if (mapRgbValues[i] < 148)
                {
                    mapRgbValues[i] = 227;
                    mapRgbValues[i + 1] = 227;
                    mapRgbValues[i + 2] = 10;
                }
                //Shore 3 - Water
                else if (mapRgbValues[i] < 155)
                {
                    mapRgbValues[i] = 122;
                    mapRgbValues[i + 1] = 96;
                    mapRgbValues[i + 2] = 10;
                }
                //Reef
                else if (mapRgbValues[i] < 170)
                {
                    mapRgbValues[i] = 95;
                    mapRgbValues[i + 1] = 71;
                    mapRgbValues[i + 2] = 38;
                }
                //Sea
                else if (mapRgbValues[i] < 200)
                {
                    mapRgbValues[i] = 73;
                    mapRgbValues[i + 1] = 60;
                    mapRgbValues[i + 2] = 38;
                }
                //Deep Sea
                else
                {
                    mapRgbValues[i] = 78;
                    mapRgbValues[i + 1] = 59;
                    mapRgbValues[i + 2] = 40;
                }

                #region Shading.
                if (shaderMap != null)  //Interpolating the coloured map with another (black and white, so R = G = B) fractal to give it texture.
                {
                    mapRgbValues[i] = (byte)Math.Min(255, (int)(mapRgbValues[i] * ((float)shaderRgbValues[i] / 255)));
                    mapRgbValues[i + 1] = (byte)Math.Min(255, (int)(mapRgbValues[i + 1] * ((float)shaderRgbValues[i] / 255)));
                    mapRgbValues[i + 2] = (byte)Math.Min(255, (int)(mapRgbValues[i + 2] * ((float)shaderRgbValues[i] / 255)));
                }
                #endregion

                #region Noise Adding.
                if (noise)  //Displacing the pixel colour by a random amount.
                {
                    mapRgbValues[i] = (byte)Math.Min(255, (mapRgbValues[i] + (int)(((float)mapRgbValues[i] / 255) * rand.Next(-60, 60))));
                    mapRgbValues[i + 1] = (byte)Math.Min(255, (mapRgbValues[i + 1] + (int)(((float)mapRgbValues[i + 1] / 255) * rand.Next(-60, 60))));
                    mapRgbValues[i + 2] = (byte)Math.Min(255, (mapRgbValues[i + 2] + (int)(((float)mapRgbValues[i + 2] / 255) * rand.Next(-60, 60))));
                }
                #endregion
            }
            #endregion

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(mapRgbValues, 0, mapPtr, mapBytes);

            // Unlock the bits and return.
            colouredMap.UnlockBits(mapBmpData);
            return colouredMap;
        }

        public static Bitmap ColourBitmapHeightMapBW(Bitmap map, Bitmap shaderMap = null, bool noise = true, int alpha = 255)
        {
            Bitmap colouredMap = new Bitmap(map);

            #region Reading Main Map Bitmap.
            //Uses Lockbits, example here: http://msdn.microsoft.com/en-us/library/5ey6h79d(v=vs.110).aspx
            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, colouredMap.Width, colouredMap.Height);

            BitmapData mapBmpData = colouredMap.LockBits(rect,
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            // Get the address of the first line.
            IntPtr mapPtr = mapBmpData.Scan0;

            // Declare an array to hold the shaderBytes of the bitmap. 
            int mapBytes = Math.Abs(mapBmpData.Stride) * colouredMap.Height;
            byte[] mapRgbValues = new byte[mapBytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(mapPtr, mapRgbValues, 0, mapBytes);
            #endregion

            #region Reading Shader Bitmap, if needed.
            byte[] shaderRgbValues = null;

            if (shaderMap != null)
            {
                BitmapData shaderBmpData = shaderMap.LockBits(rect,
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                // Get the address of the first line.
                IntPtr shaderPtr = shaderBmpData.Scan0;

                // Declare an array to hold the shaderBytes of the bitmap. 
                int shaderBytes = Math.Abs(shaderBmpData.Stride) * colouredMap.Height;
                shaderRgbValues = new byte[shaderBytes];

                // Copy the RGB values into the array.
                System.Runtime.InteropServices.Marshal.Copy(shaderPtr, shaderRgbValues, 0, shaderBytes);
            }
            #endregion

            #region Colour Selecting.
            //Only need to do the B value, not R or B as it's grey (RGB are all the same value).
            for (int i = 0; i < mapRgbValues.Length; i += 3)
            {
                if (mapRgbValues[i] < 145)
                {
                    mapRgbValues[i + 1] = mapRgbValues[i];
                    mapRgbValues[i + 2] = mapRgbValues[i];
                }
                else
                {
                    mapRgbValues[i] = 145;
                    mapRgbValues[i + 1] = 145;
                    mapRgbValues[i + 2] = 145;
                }

                /*
                #region Shading.
                if (shaderMap != null)  //Interpolating the coloured map with another (black and white, so R = G = B) fractal to give it texture.
                {
                    mapRgbValues[i] = (byte)Math.Min(255, (int)(mapRgbValues[i] * ((float)shaderRgbValues[i] / 255)));
                    mapRgbValues[i + 1] = (byte)Math.Min(255, (int)(mapRgbValues[i + 1] * ((float)shaderRgbValues[i] / 255)));
                    mapRgbValues[i + 2] = (byte)Math.Min(255, (int)(mapRgbValues[i + 2] * ((float)shaderRgbValues[i] / 255)));
                }
                #endregion

                #region Noise Adding.
                if (noise)  //Displacing the pixel colour by a random amount.
                {
                    mapRgbValues[i] = (byte)Math.Min(255, (mapRgbValues[i] + (int)(((float)mapRgbValues[i] / 255) * rand.Next(-60, 60))));
                    mapRgbValues[i + 1] = (byte)Math.Min(255, (mapRgbValues[i + 1] + (int)(((float)mapRgbValues[i + 1] / 255) * rand.Next(-60, 60))));
                    mapRgbValues[i + 2] = (byte)Math.Min(255, (mapRgbValues[i + 2] + (int)(((float)mapRgbValues[i + 2] / 255) * rand.Next(-60, 60))));
                }
                #endregion
                 * */

            }
            #endregion

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(mapRgbValues, 0, mapPtr, mapBytes);

            // Unlock the bits and return.
            colouredMap.UnlockBits(mapBmpData);
            return colouredMap;
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

/*
        private static Color GenColourBWTrans(int value)
        {
            int transparency = 100; //Val between 0 and 255.

            //Snow Peak
            if (value < 10)
            {
                return Color.FromArgb(transparency, 250, 250, 250);
            }
            //High Mountains
            else if (value < 25)
            {
                return Color.FromArgb(transparency, 240, 240, 240);
            }
            //Low Mountains
            else if (value < 50)
            {
                return Color.FromArgb(transparency, 230, 230, 230);
            }
            //Dark grass
            else if (value < 70)
            {
                return Color.FromArgb(transparency, 220, 220, 220);
            }
            //Light Grass
            else if (value < 145)
            {
                return Color.FromArgb(transparency, 210, 210, 210);
            }
            //Shore 1 - Inner Light Sand
            else if (value < 150)
            {
                return Color.FromArgb(transparency, 200, 200, 200);
            }
            //Shore 2 - Outer Dark Sand
            // else if (value < 148)
            // {
            //     return Color.FromArgb(217, 217, 0);
            //}
            //Shore 3 - Water
            else if (value < 155)
            {
                return Color.FromArgb(transparency, 190, 190, 190);
            }
            //Reef
            else if (value < 170)
            {
                return Color.FromArgb(transparency, 180, 180, 180);
            }
            //Sea
            else if (value < 200)
            {
                return Color.FromArgb(transparency, 170, 170, 170);
            }
            //Deep Sea
            else
            {
                return Color.FromArgb(transparency, 160, 160, 160);
            }
        }

        private static Color GenColourOld(int value)
        {
            //Snow Peak
            if (value < 10)
            {
                return Color.FromArgb(245, 245, 245);
            }
            //High Mountains
            else if (value < 25)
            {
                return Color.FromArgb(169, 169, 169);
            }
            //Low Mountains
            else if (value < 50)
            {
                return Color.FromArgb(128, 128, 128);
            }
            //Dark grass
            else if (value < 70)
            {
                return Color.FromArgb(0, 100, 0);
            }
            //Light Grass
            else if (value < 145)
            {
                return Color.FromArgb(8, 128, 0);
            }
            //Shore 1 - Inner Light Sand
            else if (value < 150)
            {
                return Color.FromArgb(227, 227, 69);
            }
            //Shore 2 - Outer Dark Sand
            else if (value < 148)
            {
                return Color.FromArgb(217, 217, 0);
            }
            //Shore 3 - Water
            else if (value < 155)
            {
                return Color.FromArgb(0, 178, 178);
            }
            //Reef
            else if (value < 170)
            {
                return Color.FromArgb(0, 163, 217);
            }
            //Sea
            else if (value < 200)
            {
                return Color.FromArgb(0, 133, 178);
            }
            //Deep Sea
            else
            {
                return Color.FromArgb(0, 105, 140);
            }
        }#*/

/*
        public static Bitmap ColourBitmapOld(Bitmap map, Bitmap shaderMap = null, bool noise = true, int alpha = 255)
        {
            Bitmap colouredMap = new Bitmap(map.Width, map.Height);

            for (int i = 0; i < map.Width; i++)
            {
                for (int j = 0; j < map.Height; j++)
                {
                    int value = map.GetPixel(i, j).R;    //Could also be B or G, it's grey so all equal
                    Color colour;

                    //Snow Peak
                    if (value < 10)
                    {
                        colour = Color.FromArgb(174, 181, 175);
                    }
                    //High Mountains
                    else if (value < 25)
                    {
                        colour = Color.FromArgb(149, 131, 130);
                    }
                    //Low Mountains
                    else if (value < 50)
                    {
                        colour = Color.FromArgb(102, 102, 99);
                    }
                    //Dark grass
                    else if (value < 70)
                    {
                        colour = Color.FromArgb(40, 79, 48);
                    }
                    //Light Grass
                    else if (value < 145)
                    {
                        colour = Color.FromArgb(48, 95, 60);
                    }
                    //Shore 1 - Inner Light Sand
                    else if (value < 150)
                    {
                        colour = Color.FromArgb(140, 163, 110);
                    }
                    //Shore 2 - Outer Dark Sand
                    else if (value < 148)
                    {
                        colour = Color.FromArgb(227, 227, 10);
                    }
                    //Shore 3 - Water
                    else if (value < 155)
                    {
                        colour = Color.FromArgb(10, 96, 122);
                    }
                    //Reef
                    else if (value < 170)
                    {
                        colour = Color.FromArgb(38, 71, 95);
                    }
                    //Sea
                    else if (value < 200)
                    {
                        colour = Color.FromArgb(38, 60, 73);
                    }
                    //Deep Sea
                    else
                    {
                        colour = Color.FromArgb(40, 59, 78);
                    }

                    if (shaderMap != null)
                    {
                        Color shaderColor = shaderMap.GetPixel((int)i, (int)j);

                        colour = Color.FromArgb(
                            Math.Min(255, (int)(colour.R * ((float)shaderColor.R / 255))),
                            Math.Min(255, (int)(colour.G * ((float)shaderColor.G / 255))),
                            Math.Min(255, (int)(colour.B * ((float)shaderColor.B / 255))));
                    }

                    if (noise)
                    {
                        colour = Color.FromArgb(
                            Math.Min(255, (colour.R + (int)(((float)colour.R / 255) * rand.Next(-60, 60)))),
                            Math.Min(255, (colour.G + (int)(((float)colour.G / 255) * rand.Next(-60, 60)))),
                            Math.Min(255, (colour.B + (int)(((float)colour.B / 255) * rand.Next(-60, 60)))));
                    }

                    colouredMap.SetPixel((int)i, (int)j, Color.FromArgb(alpha, colour));
                }
            }
            return colouredMap;
        }
*/