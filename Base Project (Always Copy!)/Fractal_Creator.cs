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
        private static double maxDistance = 0, centerX, centerY;

        public static Bitmap MakeFractal(int width, int height, int Roughness = 13, int maxValue = 255, int minValue = 0)
        {
            screenSize = width + height;
            centerX = width / 2;
            centerY = height / 2;
            maxDistance = Math.Sqrt((Math.Pow(centerX, 2)) + (Math.Pow(centerY, 2)));
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
            Divide(mapRgbValues, width, 0, 0, width, height, c1, c2, c3, c4, minValue, maxValue);

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(mapRgbValues, 0, mapPtr, mapBytes);

            // Unlock the bits and return.
            bitmap.UnlockBits(mapBmpData);
            return bitmap;
        }

        /// <summary>
        /// </summary>
        /// <param name="colMapRgbValues"> Array of BGR Bytes </param>
        /// <param name="bitmapWidth"> Width of the bitmap image </param>
        /// <param name="x"> X Pos </param>
        /// <param name="y"> Y Pos </param>
        /// <param name="width"> Width of current rectangle being worked on. </param>
        /// <param name="height"> Height of current rectangle being worked on. </param>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <param name="c3"></param>
        /// <param name="c4"></param>
        private static void Divide(byte[] mapRgbValues, int bitmapWidth, double x, double y, double width, double height, double c1,    
            double c2, double c3, double c4, int minValue, int maxValue)
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
                Divide(mapRgbValues, bitmapWidth, x, y, newWidth, newHeight, c1, mid1, mid2, middle, minValue, maxValue);
                Divide(mapRgbValues, bitmapWidth, x + newWidth, y, width - newWidth, newHeight, mid1, c2, middle, mid3, minValue, maxValue);
                Divide(mapRgbValues, bitmapWidth, x, y + newHeight, newWidth, height - newHeight, mid2, middle, c3, mid4, minValue, maxValue);
                Divide(mapRgbValues, bitmapWidth, x + newWidth, y + newHeight, width - newWidth, height - newHeight, middle, mid3, mid4, c4, minValue, maxValue);
            }
            //If our rectangles are now 1px x 1px, we are ready to calculate final values and draw.
            else
            {   
                //Calculate where the position of the blue (remember BGR not RGB) byte for the pixel at positon (X, Y) on the bitmap image is in the 1D byte array.
                int bytePosition = ConvertTo1DArr((int)x, (int)y, bitmapWidth);
                //Average the points of the pixel sized rectangle down into a single number, which is that pixels final gradientValue.
                double heightValue = (c1 + c2 + c3 + c4) / 4;   //Height value generated from random plasma noise.
                //Only needs to set one of the RGB values in the byte array as it is grey, and the colour methods only look at the first one (Blue, remember BGR not RGB).
                mapRgbValues[bytePosition] = (byte)((heightValue * (maxValue - minValue)) - minValue);
            }
        }
      
        public static Bitmap ShapeIsland(Bitmap fractal)
        {
            Bitmap bitmap = new Bitmap(fractal);
            maxDistance = Math.Sqrt((Math.Pow(centerX, 2)) + (Math.Pow(centerY, 2)));
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

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    #region Generating Island Shape
                    //Calculate where the position of the blue (remember BGR not RGB) byte for the pixel at positon (X, Y) on the bitmap image is in the 1D byte array.
                    int bytePosition = ConvertTo1DArr((int)x, (int)y, bitmap.Width);    //Convert to a 1D array from x, y position.
                    double distX = Math.Abs(x - centerX), distY = Math.Abs(y - centerY);    //Distance fron center in x and y.
                    double distance = Math.Sqrt(Math.Pow(distX, 2) + Math.Pow(distY, 2));   //Distance from center.
                    double fractalValue = mapRgbValues[bytePosition];   //Retrieve the fractal value at position (x, y).
                    double heightValue = fractalValue/255;   //Height value generated from random plasma noise. dictates how chaotic the island is.
                    double gradientValue = ((distance / maxDistance));  //Gradient used to get an island shape
                    int gradientStrength = 255; //how prevalent the Circular gradient is in the final value. Reduce to make reduce the centering effect. Not reccomended to change below 255.
                    int heightStrength = 100;   //How prevalant the heightmap is in the final value. Reduce for smaller, less chaotic islands.
                    int offset = 90;    //Offset used to make boost the value to make bigger islands. Reduce for smaller islands.

                    byte finalValue = (byte)((heightStrength * heightValue) - (gradientValue * gradientStrength) + offset); //Construct the final value for the island.
                    #endregion

                    #region Removing Fractal (blue for land, black for sea).
                    //Keep value between 0 and 255
                    if (finalValue > 109 || distance > maxDistance - 50)    //If we're high enough to be considered ocean or close to the edge.
                    {
                        //If we're close to the center and it's going to be water, instead make it land.  
                        if (distance < 170)
                        {
                            finalValue = 255;
                            //finalValue = (byte)Math.Min(255, (finalValue + (int)(((float)finalValue / 255) * rand.Next(-5, 5))));
                        }
                        else //Otherwise make it sea.
                        {
                            finalValue = 0;
                        }
                    }
                    else
                    {
                        finalValue = 255;
                        //finalValue = (byte)Math.Min(255, (finalValue + (int)(((float)finalValue / 255) * rand.Next(-5, 5))));
                    }
                    #endregion

                    //Only needs to set one of the RGB values in the byte array as it is grey, and the colour methods only look at the first one (Blue, remember BGR not RGB).
                    mapRgbValues[bytePosition] = finalValue;
                }
            }

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(mapRgbValues, 0, mapPtr, mapBytes);

            // Unlock the bits and return.
            bitmap.UnlockBits(mapBmpData);
            return bitmap;
        }

        public static Bitmap CalculateGradient(int width, int height, int maxValue, int minValue)
        {
            Bitmap bitmap = new Bitmap(width, height);

            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData mapBmpData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr mapPtr = mapBmpData.Scan0;
            int mapBytes = Math.Abs(mapBmpData.Stride) * bitmap.Height;
            byte[] mapRgbValues = new byte[mapBytes];
            System.Runtime.InteropServices.Marshal.Copy(mapPtr, mapRgbValues, 0, mapBytes);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    //Calculate where the position of the blue (remember BGR not RGB) byte for the pixel at positon (X, Y) on the bitmap image is in the 1D byte array.
                    int i = ConvertTo1DArr((int)x, (int)y, width);    //Convert to a 1D array from x, y position.
                    double distX = Math.Abs(x - centerX), distY = Math.Abs(y - centerY);    //Distance fron center in x and y.
                    double distance = Math.Sqrt(Math.Pow(distX, 2) + Math.Pow(distY, 2));   //Distance from center.

                    mapRgbValues[i] = (byte)((distance/maxDistance)*255);
                }
            }
            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(mapRgbValues, 0, mapPtr, mapBytes);
            // Unlock the bits and return.
            bitmap.UnlockBits(mapBmpData);
            return bitmap;
        }

        public static Bitmap CalculateBiomes(Bitmap islandFractal, Bitmap islandShape, Bitmap heightFractal, Bitmap tempFractal, Bitmap RainFractal)
        {
            Bitmap colouredIsland = new Bitmap(islandFractal.Width, islandFractal.Height);

            //All the bitmaps are the same size, so just use islandFractals height and width for all.
            Rectangle rect = new Rectangle(0, 0, islandFractal.Width, islandFractal.Height);
   
            //Lock all the bitmaps
            BitmapData islandFractalData = islandFractal.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb),
                islandShapeData = islandShape.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb),
                heightFractalData = heightFractal.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb),
                tempFractalData = tempFractal.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb),
                rainFractalData = RainFractal.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb),
                colouredIslandData = colouredIsland.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int mapBytes = Math.Abs(islandFractalData.Stride) * islandFractal.Height;

            #region Locking bitmaps
            //This is explained in the MakeFractal Method.
            IntPtr islandFractalPtr = islandFractalData.Scan0;
            byte[] islandFractalRbgValues = new byte[mapBytes];
            System.Runtime.InteropServices.Marshal.Copy(islandFractalPtr, islandFractalRbgValues, 0, mapBytes);

            IntPtr islandShapePtr = islandShapeData.Scan0;
            byte[] islandShapeRbgValues = new byte[mapBytes];
            System.Runtime.InteropServices.Marshal.Copy(islandShapePtr, islandShapeRbgValues, 0, mapBytes);

            IntPtr heightFractalPtr = heightFractalData.Scan0;
            byte[] heightFractalRbgValues = new byte[mapBytes];
            System.Runtime.InteropServices.Marshal.Copy(heightFractalPtr, heightFractalRbgValues, 0, mapBytes);

            IntPtr tempFractalPtr = tempFractalData.Scan0;
            byte[] tempFractalRbgValues = new byte[mapBytes];
            System.Runtime.InteropServices.Marshal.Copy(tempFractalPtr, tempFractalRbgValues, 0, mapBytes);

            IntPtr rainFractalPtr = rainFractalData.Scan0;
            byte[] rainFractalRbgValues = new byte[mapBytes];
            System.Runtime.InteropServices.Marshal.Copy(rainFractalPtr, rainFractalRbgValues, 0, mapBytes);

            IntPtr colouredIslandFractalPtr = colouredIslandData.Scan0;
            byte[] colouredIslandRbgValues = new byte[mapBytes];
            System.Runtime.InteropServices.Marshal.Copy(colouredIslandFractalPtr, colouredIslandRbgValues, 0, mapBytes);
            #endregion

            for (int x = 0; x < islandFractal.Width; x++)
            {
                for (int y = 0; y < islandFractal.Height; y++)
                {
                    int i = ConvertTo1DArr(x, y, islandFractal.Width);

                    if (islandShapeRbgValues[i] == 255)  //Land
                    {
                        if (heightFractalRbgValues[i] < 190) //Low areas
                        {
                            if (tempFractalRbgValues[i] < 20)
                            {   
                                //Tundra
                                colouredIslandRbgValues[i] = 249;
                                colouredIslandRbgValues[i + 1] = 235;
                                colouredIslandRbgValues[i + 2] = 85;
                            }
                            else if (tempFractalRbgValues[i] < 40)
                            {
                                if (rainFractalRbgValues[i] < 10)
                                {
                                    //Tropical Sand
                                    colouredIslandRbgValues[i] = 24;
                                    colouredIslandRbgValues[i + 1] = 148;
                                    colouredIslandRbgValues[i + 2] = 250;
                                }
                                else if (rainFractalRbgValues[i] < 20)
                                {
                                    //Sand
                                    colouredIslandRbgValues[i] = 7;
                                    colouredIslandRbgValues[i + 1] = 219;
                                    colouredIslandRbgValues[i + 2] = 250;
                                }
                                else if (rainFractalRbgValues[i] < 60)
                                {
                                    //Taiga
                                    colouredIslandRbgValues[i] = 33;
                                    colouredIslandRbgValues[i + 1] = 102;
                                    colouredIslandRbgValues[i + 2] = 5;
                                }
                                else
                                {
                                    //Swamp
                                    colouredIslandRbgValues[i] = 0;
                                    colouredIslandRbgValues[i + 1] = 102;
                                    colouredIslandRbgValues[i + 2] = 76;
                                }
                            }
                            else if (tempFractalRbgValues[i] < 50)
                            {
                                if (rainFractalRbgValues[i] < 10)
                                {
                                    //Tropical Sand
                                    colouredIslandRbgValues[i] = 24;
                                    colouredIslandRbgValues[i + 1] = 148;
                                    colouredIslandRbgValues[i + 2] = 250;
                                }
                                else if (rainFractalRbgValues[i] < 20)
                                {
                                    //Sand
                                    colouredIslandRbgValues[i] = 7;
                                    colouredIslandRbgValues[i + 1] = 219;
                                    colouredIslandRbgValues[i + 2] = 250;
                                }
                                else if (rainFractalRbgValues[i] < 50)
                                {
                                    //Dedious Forest
                                    colouredIslandRbgValues[i] = 83;
                                    colouredIslandRbgValues[i + 1] = 177;
                                    colouredIslandRbgValues[i + 2] = 46;
                                }
                                else if (rainFractalRbgValues[i] < 80)
                                {
                                    //Rain Forest
                                    colouredIslandRbgValues[i] = 162;
                                    colouredIslandRbgValues[i + 1] = 249;
                                    colouredIslandRbgValues[i + 2] = 7;
                                }
                                else
                                {
                                    //Swamp
                                    colouredIslandRbgValues[i] = 0;
                                    colouredIslandRbgValues[i + 1] = 102;
                                    colouredIslandRbgValues[i + 2] = 76;
                                }
                            }
                            else if (tempFractalRbgValues[i] < 60)
                            {
                                /*
                                if (rainFractalRbgValues[i] < 10)
                                {
                                    //Tropical Sand
                                    colouredIslandRbgValues[i] = 24;
                                    colouredIslandRbgValues[i + 1] = 148;
                                    colouredIslandRbgValues[i + 2] = 250;
                                }
                                else if (rainFractalRbgValues[i] < 30)
                                {
                                    //Sand
                                    colouredIslandRbgValues[i] = 7;
                                    colouredIslandRbgValues[i + 1] = 219;
                                    colouredIslandRbgValues[i + 2] = 250;
                                }
                                 * */
                                 if (rainFractalRbgValues[i] < 50)
                                {
                                    //Dedious Forest
                                    colouredIslandRbgValues[i] = 83;
                                    colouredIslandRbgValues[i + 1] = 177;
                                    colouredIslandRbgValues[i + 2] = 46;
                                }
                                else if (rainFractalRbgValues[i] < 80)
                                {
                                    //Rain Forest
                                    colouredIslandRbgValues[i] = 162;
                                    colouredIslandRbgValues[i + 1] = 249;
                                    colouredIslandRbgValues[i + 2] = 7;
                                }
                                else
                                {
                                    //Swamp
                                    colouredIslandRbgValues[i] = 0;
                                    colouredIslandRbgValues[i + 1] = 102;
                                    colouredIslandRbgValues[i + 2] = 76;
                                }
                            }
                            else if (tempFractalRbgValues[i] < 70)
                            {
                                if (rainFractalRbgValues[i] < 10)
                                {
                                    //Tropical Sand
                                    colouredIslandRbgValues[i] = 24;
                                    colouredIslandRbgValues[i + 1] = 148;
                                    colouredIslandRbgValues[i + 2] = 250;
                                }
                                else if (rainFractalRbgValues[i] < 30)
                                {
                                    //Sand
                                    colouredIslandRbgValues[i] = 7;
                                    colouredIslandRbgValues[i + 1] = 219;
                                    colouredIslandRbgValues[i + 2] = 250;
                                }
                                else if (rainFractalRbgValues[i] < 60)
                                {
                                    //Dedious Forest
                                    colouredIslandRbgValues[i] = 83;
                                    colouredIslandRbgValues[i + 1] = 177;
                                    colouredIslandRbgValues[i + 2] = 46;
                                }
                                else if (rainFractalRbgValues[i] < 80)
                                {
                                    //Rain Forest
                                    colouredIslandRbgValues[i] = 162;
                                    colouredIslandRbgValues[i + 1] = 249;
                                    colouredIslandRbgValues[i + 2] = 7;
                                }
                                else
                                {
                                    //Swamp
                                    colouredIslandRbgValues[i] = 0;
                                    colouredIslandRbgValues[i + 1] = 102;
                                    colouredIslandRbgValues[i + 2] = 76;
                                }
                            }
                            else if (tempFractalRbgValues[i] < 90)
                            {
                                if (rainFractalRbgValues[i] < 20)
                                {
                                    //Tropical Sand
                                    colouredIslandRbgValues[i] = 24;
                                    colouredIslandRbgValues[i + 1] = 148;
                                    colouredIslandRbgValues[i + 2] = 250;
                                }
                                else if (rainFractalRbgValues[i] < 60)
                                {
                                    //Tropical Seasonal Forest/Savanna
                                    colouredIslandRbgValues[i] = 7;
                                    colouredIslandRbgValues[i + 1] = 219;
                                    colouredIslandRbgValues[i + 2] = 250;
                                }
                                else
                                {
                                    //Tropical Forest
                                    colouredIslandRbgValues[i] = 35;
                                    colouredIslandRbgValues[i + 1] = 224;
                                    colouredIslandRbgValues[i + 2] = 155;
                                }
                            }
                            else
                            {
                                if (rainFractalRbgValues[i] < 30)
                                {
                                    //Tropical Sand
                                    colouredIslandRbgValues[i] = 24;
                                    colouredIslandRbgValues[i + 1] = 148;
                                    colouredIslandRbgValues[i + 2] = 250;
                                }
                                else if (rainFractalRbgValues[i] < 70)
                                {
                                    //Tropical Seasonal Forest/Savanna
                                    colouredIslandRbgValues[i] = 7;
                                    colouredIslandRbgValues[i + 1] = 219;
                                    colouredIslandRbgValues[i + 2] = 250;
                                }
                                else
                                {
                                    //Tropical Forest
                                    colouredIslandRbgValues[i] = 35;
                                    colouredIslandRbgValues[i + 1] = 224;
                                    colouredIslandRbgValues[i + 2] = 155;
                                }
                            }
/*
                            if (rainFractalRbgValues[i] < 30 && tempFractalRbgValues[i] > 10)   //Sand
                            {   
                                colouredIslandRbgValues[i] = 18;
                                colouredIslandRbgValues[i + 1] = 104;
                                colouredIslandRbgValues[i + 2] = 141;
                            }
                            else
                            {
                                //Follow Whittaker here.
                                colouredIslandRbgValues[i] = 60;
                                colouredIslandRbgValues[i + 1] = 95;
                                colouredIslandRbgValues[i + 2] = 48;
                            }
 * */
                        }
                        else //Mountains
                        {
                            colouredIslandRbgValues[i] = 130;
                            colouredIslandRbgValues[i + 1] = 131;
                            colouredIslandRbgValues[i + 2] = 149;
                        }

                    }
                    else //Sea
                    {
                        if (heightFractalRbgValues[i] < 58) //Deep areas
                        {
                            colouredIslandRbgValues[i] = 63;
                            colouredIslandRbgValues[i + 1] = 50;
                            colouredIslandRbgValues[i + 2] = 28;
                        }
                        else
                        {
                            //Inner sea
                        }
                        {
                            colouredIslandRbgValues[i] = 73;
                            colouredIslandRbgValues[i + 1] = 60;
                            colouredIslandRbgValues[i + 2] = 38;
                        }
                    }
                }
            }

            #region Unlocking Bitmaps.
            // Copy the RGB values back to the bitmap, and then unlocking.
            System.Runtime.InteropServices.Marshal.Copy(islandFractalRbgValues, 0, islandFractalPtr, mapBytes);
            System.Runtime.InteropServices.Marshal.Copy(islandShapeRbgValues, 0, islandShapePtr, mapBytes);
            System.Runtime.InteropServices.Marshal.Copy(heightFractalRbgValues, 0, heightFractalPtr, mapBytes);
            System.Runtime.InteropServices.Marshal.Copy(tempFractalRbgValues, 0, tempFractalPtr, mapBytes);
            System.Runtime.InteropServices.Marshal.Copy(colouredIslandRbgValues, 0, colouredIslandFractalPtr, mapBytes);

            islandFractal.UnlockBits(islandFractalData);
            islandShape.UnlockBits(islandShapeData);
            heightFractal.UnlockBits(heightFractalData);
            tempFractal.UnlockBits(tempFractalData);
            colouredIsland.UnlockBits(colouredIslandData);
            #endregion
            return colouredIsland;
        }

        public static Bitmap ColourBitmap(Bitmap map, Bitmap shaderMap = null, bool noise = true, int rivers = 0, int alpha = 255)
        {
            Bitmap colouredMap = new Bitmap(map);

            #region Reading Colour Map Bitmap.
            //Uses Lockbits, example here: http://msdn.microsoft.com/en-us/library/5ey6h79d(v=vs.110).aspx
            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, colouredMap.Width, colouredMap.Height);

            BitmapData colMapBmpData = colouredMap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            // Get the address of the first line.
            IntPtr colMapPtr = colMapBmpData.Scan0;

            // Declare an array to hold the shaderBytes of the bitmap. 
            int colMapBytes = Math.Abs(colMapBmpData.Stride) * colouredMap.Height;
            byte[] colMapRgbValues = new byte[colMapBytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(colMapPtr, colMapRgbValues, 0, colMapBytes);
            #endregion

            #region Reading Main Map Bitmap.
            //Uses Lockbits, example here: http://msdn.microsoft.com/en-us/library/5ey6h79d(v=vs.110).aspx
            // Lock the bitmap's bits. 
            BitmapData mapBmpData = map.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            // Get the address of the first line.
            IntPtr mapPtr = mapBmpData.Scan0;

            // Declare an array to hold the shaderBytes of the bitmap. 
            int mapBytes = Math.Abs(mapBmpData.Stride) * map.Height;
            byte[] mapRgbValues = new byte[mapBytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(mapPtr, mapRgbValues, 0, mapBytes);
            #endregion

            #region Reading Shader Bitmap, if needed.
            byte[] shaderRgbValues = null;
            BitmapData shaderBmpData = null;

            if (shaderMap != null)
            {
                shaderBmpData = shaderMap.LockBits(rect,
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

            #region Rivers
                for (int i = 0; i < rivers; i++)
                {
                    Point currentPixel = new Point();
                    Color riverColour = Color.FromArgb(10, 10, 10);
                    Color pixelColour;

                    //The lightest (lowest height) direction.
                    Point bestDirectionPoint = new Point();
                    int bestValue = 255;

                    Point lastMove = new Point();

                    #region Choose Start Location
                    //Find a suitable starting location/spring.
                    do
                    {
                        currentPixel.X = rand.Next(10, map.Width - 10);
                        currentPixel.Y = rand.Next(10, map.Height - 10);
                    }
                    while (ReturnColour(currentPixel.X, currentPixel.Y, map.Width, mapRgbValues).B < 150);  //Choose a position which is on high land. (blue heightmap)


                    Console.WriteLine("Found Mountain!");
                    //Colour the starting location.
                    colMapRgbValues[ConvertTo1DArr(currentPixel.X, currentPixel.Y, map.Width)] = riverColour.B;
                    colMapRgbValues[ConvertTo1DArr(currentPixel.X, currentPixel.Y, map.Width) + 1] = riverColour.G;
                    colMapRgbValues[ConvertTo1DArr(currentPixel.X, currentPixel.Y, map.Width) + 2] = riverColour.R;
                    #endregion

                    #region Carve River
                    while (ReturnColour(currentPixel.X, currentPixel.Y, map.Width, mapRgbValues).B > 8)  //While we're not at sea level..
                    {
                        //Stop if at edge of screen.
                        if (currentPixel.Y - 1 < 0 || currentPixel.X - 1 < 0 || currentPixel.Y + 1 >= colouredMap.Height || currentPixel.X + 1 >= colouredMap.Width)
                        {
                            break;
                        }

                        Point delta = new Point(bestDirectionPoint.X - lastMove.X, bestDirectionPoint.Y - lastMove.Y); //direction we're going in (straight line)
                        bestValue = 0;  //reset the highest gradientValue each iteration.

                        Point sameDir = new Point(currentPixel.X + delta.X, currentPixel.Y + delta.Y);
                        if (ReturnColour(sameDir.X, sameDir.Y, map.Width, colMapRgbValues).B > ReturnColour(currentPixel.X, currentPixel.Y, map.Width, colMapRgbValues).B + 2)
                        {
                            #region Checking around currentPixel for steepest drop.
                            pixelColour = ReturnColour(currentPixel.X, currentPixel.Y - 1, map.Width, colMapRgbValues); //Colour of pixel we're looking at.
                            if (mapRgbValues[ConvertTo1DArr(currentPixel.X, currentPixel.Y - 1, map.Width)] > bestValue && //If this is the steepest drop...
                                 pixelColour != riverColour)    //And it's not already a river pixel..
                            {
                                bestValue = colMapRgbValues[ConvertTo1DArr(currentPixel.X, currentPixel.Y - 1, map.Width)];
                                bestDirectionPoint = new Point(currentPixel.X, currentPixel.Y - 1);
                            }

                            pixelColour = ReturnColour(currentPixel.X, currentPixel.Y + 1, map.Width, colMapRgbValues);
                            if (mapRgbValues[ConvertTo1DArr(currentPixel.X, currentPixel.Y + 1, map.Width)] > bestValue
                                && pixelColour != riverColour)
                            {
                                bestValue = colMapRgbValues[ConvertTo1DArr(currentPixel.X, currentPixel.Y + 1, map.Width)];
                                bestDirectionPoint = new Point(currentPixel.X, currentPixel.Y + 1);
                            }

                            pixelColour = ReturnColour(currentPixel.X - 1, currentPixel.Y, map.Width, colMapRgbValues);
                            if (mapRgbValues[ConvertTo1DArr(currentPixel.X - 1, currentPixel.Y, map.Width)] > bestValue
                                && pixelColour != riverColour)
                            {
                                bestValue = mapRgbValues[ConvertTo1DArr(currentPixel.X - 1, currentPixel.Y, map.Width)];
                                bestDirectionPoint = new Point(currentPixel.X - 1, currentPixel.Y);
                            }

                            pixelColour = ReturnColour(currentPixel.X + 1, currentPixel.Y, map.Width, colMapRgbValues);
                            if (mapRgbValues[ConvertTo1DArr(currentPixel.X + 1, currentPixel.Y, map.Width)] > bestValue
                                && pixelColour != riverColour)
                            {
                                bestValue = mapRgbValues[ConvertTo1DArr(currentPixel.X + 1, currentPixel.Y, map.Width)];
                                bestDirectionPoint = new Point(currentPixel.X + 1, currentPixel.Y);
                            }
                            #endregion
                        }
                        //If there is no where which has not been visited around us, 
                        //This will be repeated until we can move somewhere.
                        if (bestValue == 0)
                        {

                            try
                            {
                                //Find direction we're going in (the direction we went last time).
                                
                                bestDirectionPoint.X = currentPixel.X + delta.X; //Move along the straight line one pixel.
                                bestDirectionPoint.Y = currentPixel.Y + delta.Y;
                                currentPixel.X += delta.X; //Move along the straight line one pixel.
                                currentPixel.Y += delta.Y;

                                //Colour pixel which is in the direction we were going last iteration.
                                colMapRgbValues[ConvertTo1DArr(bestDirectionPoint.X, bestDirectionPoint.Y, map.Width)] = riverColour.B;
                                colMapRgbValues[ConvertTo1DArr(bestDirectionPoint.X, bestDirectionPoint.Y, map.Width) + 1] = riverColour.G;
                                colMapRgbValues[ConvertTo1DArr(bestDirectionPoint.X, bestDirectionPoint.Y, map.Width) + 2] = riverColour.R;
                            }
                            catch (System.IndexOutOfRangeException)
                            {
                                break;  //At edge of screen: stop.
                            }

                        }
                        else
                        {
                            colMapRgbValues[ConvertTo1DArr(bestDirectionPoint.X, bestDirectionPoint.Y, map.Width)] = riverColour.B;
                            colMapRgbValues[ConvertTo1DArr(bestDirectionPoint.X, bestDirectionPoint.Y, map.Width) + 1] = riverColour.G;
                            colMapRgbValues[ConvertTo1DArr(bestDirectionPoint.X, bestDirectionPoint.Y, map.Width) + 2] = riverColour.R;   //Colour first pixel  

                            lastMove = currentPixel;
                            currentPixel = bestDirectionPoint;
                        }
                    }

                    #endregion
            }
            #endregion

            #region Colour Selecting.
            //Is in the format BGR, NOT RGB!!!
            for (int i = 0; i < colMapRgbValues.Length; i += 3)
            {
                double heightModifier = shaderRgbValues[i];

                //Sea
                if (colMapRgbValues[i] < 10)
                {
                    colMapRgbValues[i] = 73;
                    colMapRgbValues[i + 1] = 60;
                    colMapRgbValues[i + 2] = 38;
                }
                //Shore 2 - Outer Dark Sand
                else if (colMapRgbValues[i] < 11)
                {
                    colMapRgbValues[i] = 18;
                    colMapRgbValues[i + 1] = 104;
                    colMapRgbValues[i + 2] = 141;
                }
                //Shore 1 - Inner Light Sand
                else if (colMapRgbValues[i] < 14)
                {
                    colMapRgbValues[i] = 27;
                    colMapRgbValues[i + 1] = 140;
                    colMapRgbValues[i + 2] = 190;
                }
                //Light Grass
                else if (colMapRgbValues[i] < 105)
                {
                    colMapRgbValues[i] = (byte)(60);
                    colMapRgbValues[i + 1] = (byte)(95);
                    colMapRgbValues[i + 2] = (byte)(48);
                }
                //Light Grass
                else if (colMapRgbValues[i] < 105)
                {
                    colMapRgbValues[i] = (byte) (60 * ((heightModifier) / 105));
                    colMapRgbValues[i + 1] = (byte)(95 * ((heightModifier) / 105));
                    colMapRgbValues[i + 2] = (byte)(48 * ((heightModifier) / 105));
                }
                //Low Mountains
                else if (colMapRgbValues[i] < 130)
                {
                    colMapRgbValues[i] = 99;
                    colMapRgbValues[i + 1] = 102;
                    colMapRgbValues[i + 2] = 102;
                }
                //High Mountains
                else if (colMapRgbValues[i] < 155)
                {

                    colMapRgbValues[i] = 130;
                    colMapRgbValues[i + 1] = 131;
                    colMapRgbValues[i + 2] = 149;
                }
                //Mountain Top.
                else 
                {
                    colMapRgbValues[i] = 175;    //Blue component
                    colMapRgbValues[i + 1] = 181;   //Green Component
                    colMapRgbValues[i + 2] = 174;   //Red Component
                }




                /*
                #region Shading.
                if (shaderMap != null)  //Interpolating the coloured map with another (black and white, so R = G = B) fractal to give it texture.
                {
                    colMapRgbValues[i] = (byte)Math.Min(255, (int)(colMapRgbValues[i] * ((float)shaderRgbValues[i] / 180)));
                    colMapRgbValues[i + 1] = (byte)Math.Min(255, (int)(colMapRgbValues[i + 1] * ((float)shaderRgbValues[i] / 180)));
                    colMapRgbValues[i + 2] = (byte)Math.Min(255, (int)(colMapRgbValues[i + 2] * ((float)shaderRgbValues[i] / 180)));
                }
                 
                #endregion
                 * */
                
                #region Noise Adding.
                if (noise)  //Displacing the pixel colour by a random amount.
                {
                    colMapRgbValues[i] = (byte)Math.Min(255, (colMapRgbValues[i] + (int)(((float)colMapRgbValues[i] / 255) * rand.Next(-40, 40))));
                    colMapRgbValues[i + 1] = (byte)Math.Min(255, (colMapRgbValues[i + 1] + (int)(((float)colMapRgbValues[i + 1] / 255) * rand.Next(-40, 40))));
                    colMapRgbValues[i + 2] = (byte)Math.Min(255, (colMapRgbValues[i + 2] + (int)(((float)colMapRgbValues[i + 2] / 255) * rand.Next(-40, 40))));
                }
                #endregion
                 
            }
            #endregion

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(colMapRgbValues, 0, colMapPtr, colMapBytes);

            // Unlock the bits and return.
            colouredMap.UnlockBits(colMapBmpData);
            map.UnlockBits(mapBmpData);
            if (shaderMap != null)
            shaderMap.UnlockBits(shaderBmpData);

            return colouredMap;
        }

        public static Bitmap InterpolateBitmaps(Bitmap bmp1, Bitmap bmp2, double bmp1Coeff = 0.5, double bmp2Coeff = 0.5, int offset = 0)
        {
            Bitmap newBmp = new Bitmap(bmp1.Width, bmp1.Height);

            //All the bitmaps are the same size, so just use bmp1 height and width for all.
            Rectangle rect = new Rectangle(0, 0, bmp1.Width, bmp1.Height);

            #region Locking Bitmaps
            //Lock all the bitmaps
            BitmapData bmp1Data = bmp1.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb),
                bmp2Data = bmp2.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb),
                newBmpData = newBmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int mapBytes = Math.Abs(bmp1Data.Stride) * bmp1.Height;

            
            //This is explained in the MakeFractal Method.
            IntPtr bmp1Ptr = bmp1Data.Scan0;
            byte[] bmp1RbgValues = new byte[mapBytes];
            System.Runtime.InteropServices.Marshal.Copy(bmp1Ptr, bmp1RbgValues, 0, mapBytes);

            IntPtr bmp2Ptr = bmp2Data.Scan0;
            byte[] bmp2RbgValues = new byte[mapBytes];
            System.Runtime.InteropServices.Marshal.Copy(bmp2Ptr, bmp2RbgValues, 0, mapBytes);

            IntPtr newBmpPtr = newBmpData.Scan0;
            byte[] newBmpRbgValues = new byte[mapBytes];
            System.Runtime.InteropServices.Marshal.Copy(newBmpPtr, newBmpRbgValues, 0, mapBytes);
            #endregion

            for (int x = 0; x < bmp1.Width; x++)
            {
                for (int y = 0; y < bmp1.Height; y++)
                {
                    int i = ConvertTo1DArr(x, y, bmp1.Width);
                    double val = ((bmp1RbgValues[i] * bmp1Coeff) + (bmp2RbgValues[i] * bmp2Coeff));

                    if (val > 255)
                    {
                        val = 255;
                    }
                    else if (val < 0)
                    {
                        val = 0;
                    }

                    newBmpRbgValues[i] = (byte)(255 - val);
                }
            }
            #region Unlocking Bitmaps
            System.Runtime.InteropServices.Marshal.Copy(newBmpRbgValues, 0, newBmpPtr, mapBytes);
            System.Runtime.InteropServices.Marshal.Copy(bmp1RbgValues, 0, bmp1Ptr, mapBytes);
            System.Runtime.InteropServices.Marshal.Copy(bmp2RbgValues, 0, bmp2Ptr, mapBytes);

            bmp1.UnlockBits(bmp1Data);
            bmp2.UnlockBits(bmp2Data);
            newBmp.UnlockBits(newBmpData);
            #endregion
            return newBmp;
        }

        private static int ConvertTo1DArr(int x, int y, int width)
        {
            return ((((y * width) + (x + 1)) * 3) - 3);
        }

        private static Color ReturnColour(int x, int y, int width, byte[] colMapRgbValues)
        {
            int bluePos = ((((y * width) + (x + 1)) * 3) - 3);
            //Remember we're converting from BGR to RGB, so it's reversed (2, 1, 0, not 0, 1, 2)
            return Color.FromArgb(colMapRgbValues[bluePos + 2], colMapRgbValues[bluePos + 1], colMapRgbValues[bluePos]);
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

        //Displaces gradientValue a small amount.
        private static double Displace(double rectSize)
        {
            double Max = rectSize / screenSize * roughness;
            return (rand.NextDouble() - 0.5) * Max;
        }
    }
}