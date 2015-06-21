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

        public static int[,] MakeFractal(int width, int height, int Roughness = 13, int maxValue = 255, int minValue = 0)
        {
            screenSize = width + height;
            double maxDistance = 0, centerX = width / 2, centerY = height / 2;
            maxDistance = Math.Sqrt((Math.Pow(centerX, 2)) + (Math.Pow(centerY, 2)));
            roughness = Roughness;

            int[,] map = new int[width,height];

            //Calculate corner values (c1, c2, c3, c4).
            double c1 = rand.NextDouble();
            double c2 = rand.NextDouble();
            double c3 = rand.NextDouble();
            double c4 = rand.NextDouble();

            //Call Divide, begin the iteration.
            Divide(map, 0, 0, width, height, c1, c2, c3, c4, minValue, maxValue);

            return map;
        }

        /// <summary>
        /// </summary>
        /// <param name="colMapRgbValues"> Array of BGR Bytes </param>
        /// <param name="bitmapWidth"> Width of the map image </param>
        /// <param name="x"> X Pos </param>
        /// <param name="y"> Y Pos </param>
        /// <param name="width"> Width of current rectangle being worked on. </param>
        /// <param name="height"> Height of current rectangle being worked on. </param>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <param name="c3"></param>
        /// <param name="c4"></param>
        private static void Divide(int[,] mapRgbValues, double x, double y, double width, double height, double c1,
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
                Divide(mapRgbValues, x, y, newWidth, newHeight, c1, mid1, mid2, middle, minValue, maxValue);
                Divide(mapRgbValues, x + newWidth, y, width - newWidth, newHeight, mid1, c2, middle, mid3, minValue, maxValue);
                Divide(mapRgbValues, x, y + newHeight, newWidth, height - newHeight, mid2, middle, c3, mid4, minValue, maxValue);
                Divide(mapRgbValues, x + newWidth, y + newHeight, width - newWidth, height - newHeight, middle, mid3, mid4, c4, minValue, maxValue);
            }
            //If our rectangles are now 1px x 1px, we are ready to calculate final values and draw.
            else
            {
                //Average the points of the pixel sized rectangle down into a single number, which is that pixels final gradientValue.
                double heightValue = (c1 + c2 + c3 + c4) / 4;   //Height value generated from random plasma noise.
                //Setting the final value for this pixel.
                mapRgbValues[(int)x, (int)y] = (byte)((heightValue * (maxValue - minValue)) - minValue);
            }
        }

        public static int[,] ShapeIsland(int[,] fractal)
        {
            double maxDistance = 0, width = fractal.GetLength(0), height = fractal.GetLength(1), centerX = width / 2, centerY = height / 2;
            maxDistance = Math.Sqrt((Math.Pow(centerX, 2)) + (Math.Pow(centerY, 2)));
            maxDistance = Math.Sqrt((Math.Pow(centerX, 2)) + (Math.Pow(centerY, 2)));

            int[,] map = new int[(int)width, (int)height];


            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    #region Generating Island Shape
                    double distX = Math.Abs(x - centerX), distY = Math.Abs(y - centerY);    //Distance fron center in x and y.
                    double distance = Math.Sqrt(Math.Pow(distX, 2) + Math.Pow(distY, 2));   //Distance from center.
                    double fractalValue = fractal[x, y];   //Retrieve the fractal value at position (x, y).
                    double heightValue = fractalValue / 255;   //Height value generated from random plasma noise. Dictates how chaotic the island is.
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

                    map[x, y] = finalValue;
                }
            }

            return map;
        }  

        public static int[,] CalculateBiomes(int[,] islandFractal, int[,] islandShape, int[,] heightFractal, int[,] tempFractal, int[,] rainFractal)
        {
            int width = islandFractal.GetLength(0), height = islandFractal.GetLength(1);
            int[,] colouredIsland = new int[width, height];


            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int i = Bitmap_Utils.ConvertTo1DArr(x, y, width);

                    if (islandShape[x, y] == 255)  //Land
                    {
                        if (tempFractal[x, y] < 20)
                        {
                            //Tundra
                            colouredIsland[x, y] = 8;
                        }
                        else if (tempFractal[x, y] < 40)
                        {
                            if (rainFractal[x, y] < 10)
                            {
                                //Tropical Sand
                                colouredIsland[x, y] = 0;
                            }
                            else if (rainFractal[x, y] < 20)
                            {
                                //Desert
                                colouredIsland[x, y] = 1;
                            }
                            else if (rainFractal[x, y] < 60)
                            {
                                //Taiga
                                colouredIsland[x, y] = 7;
                            }
                            else
                            {
                                //Swamp
                                colouredIsland[x, y] = 6;
                            }
                        }
                        else if (tempFractal[x, y] < 50)
                        {
                            if (rainFractal[x, y] < 10)
                            {
                                //Tropical Sand
                                colouredIsland[x, y] = 0;
                            }
                            else if (rainFractal[x, y] < 20)
                            {
                                //Desert
                                colouredIsland[x, y] = 1;
                            }
                            else if (rainFractal[x, y] < 50)
                            {
                                //Dedious Forest
                                colouredIsland[x, y] = 4;
                            }
                            else if (rainFractal[x, y] < 80)
                            {
                                //Rain Forest
                                colouredIsland[x, y] = 5;
                            }
                            else
                            {
                                //Swamp
                                colouredIsland[x, y] = 6;
                            }
                        }
                        else if (tempFractal[x, y] < 60)
                        {
                            if (rainFractal[x, y] < 10)
                            {
                                //Tropical Sand
                                colouredIsland[x, y] = 0;
                            }
                            else if (rainFractal[x, y] < 30)
                            {
                                //Desert
                                colouredIsland[x, y] = 1;
                            }
                            if (rainFractal[x, y] < 50)
                            {
                                //Dedious Forest
                                colouredIsland[x, y] = 4;
                            }
                            else if (rainFractal[x, y] < 80)
                            {
                                //Rain Forest
                                colouredIsland[x, y] = 5;
                            }
                            else
                            {
                                //Swamp
                                colouredIsland[x, y] = 6;
                            }
                        }
                        else if (tempFractal[x, y] < 70)
                        {
                            if (rainFractal[x, y] < 10)
                            {
                                //Tropical Sand
                                colouredIsland[x, y] = 0;
                            }
                            else if (rainFractal[x, y] < 30)
                            {
                                //Desert
                                colouredIsland[x, y] = 1;
                            }
                            else if (rainFractal[x, y] < 60)
                            {
                                //Dedious Forest
                                colouredIsland[x, y] = 4;
                            }
                            else if (rainFractal[x, y] < 80)
                            {
                                //Rain Forest
                                colouredIsland[x, y] = 5;
                            }
                            else
                            {
                                //Swamp
                                colouredIsland[x, y] = 6;
                            }
                        }
                        else if (tempFractal[x, y] < 90)
                        {
                            if (rainFractal[x, y] < 20)
                            {
                                //Tropical Sand
                                colouredIsland[x, y] = 0;
                            }
                            else if (rainFractal[x, y] < 60)
                            {
                                //Tropical Seasonal Forest/Savanna
                                colouredIsland[x, y] = 2;
                            }
                            else
                            {
                                //Tropical Forest
                                colouredIsland[x, y] = 3;
                            }
                        }
                        else
                        {
                            if (rainFractal[x, y] < 30)
                            {
                                //Tropical Sand
                                colouredIsland[x, y] = 0;
                            }
                            else if (rainFractal[x, y] < 70)
                            {
                                //Tropical Seasonal Forest/Savanna
                                colouredIsland[x, y] = 2;
                            }
                            else
                            {
                                //Tropical Forest
                                colouredIsland[x, y] = 3;
                            }
                        }
                    }
                    else //Sea
                    {
                        if (heightFractal[x, y] < 58) //Deep areas
                        {
                            colouredIsland[x, y] = -2;
                        }
                        else
                        {
                            //Inner sea
                            colouredIsland[x, y] = -1;
                        }
                    }
                }
            }

            return colouredIsland;
        }

        /*
        public static byte[] ColourBitmap(Bitmap map, Bitmap shaderMap = null, bool noise = true, int rivers = 0, int alpha = 255)
        {
            Bitmap colouredMap = new Bitmap(map);

            #region Reading Colour Map Bitmap.
            //Uses Lockbits, example here: http://msdn.microsoft.com/en-us/library/5ey6h79d(v=vs.110).aspx
            // Lock the map's bits.  
            Rectangle rect = new Rectangle(0, 0, colouredMap.Width, colouredMap.Height);

            BitmapData colMapBmpData = colouredMap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            // Get the address of the first line.
            IntPtr colMapPtr = colMapBmpData.Scan0;

            // Declare an array to hold the shaderBytes of the map. 
            int colMapBytes = Math.Abs(colMapBmpData.Stride) * colouredMap.Height;
            byte[] colMapRgbValues = new byte[colMapBytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(colMapPtr, colMapRgbValues, 0, colMapBytes);
            #endregion

            #region Reading Main Map Bitmap.
            //Uses Lockbits, example here: http://msdn.microsoft.com/en-us/library/5ey6h79d(v=vs.110).aspx
            // Lock the map's bits. 
            BitmapData mapBmpData = map.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            // Get the address of the first line.
            IntPtr mapPtr = mapBmpData.Scan0;

            // Declare an array to hold the shaderBytes of the map. 
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

                // Declare an array to hold the shaderBytes of the map. 
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
                    while (Bitmap_Utils.ReturnColour(currentPixel.X, currentPixel.Y, map.Width, mapRgbValues).B < 150);  //Choose a position which is on high land. (blue heightmap)


                    Console.WriteLine("Found Mountain!");
                    //Colour the starting location.
                    colMapRgbValues[Bitmap_Utils.ConvertTo1DArr(currentPixel.X, currentPixel.Y, map.Width)] = riverColour.B;
                    colMapRgbValues[Bitmap_Utils.ConvertTo1DArr(currentPixel.X, currentPixel.Y, map.Width) + 1] = riverColour.G;
                    colMapRgbValues[Bitmap_Utils.ConvertTo1DArr(currentPixel.X, currentPixel.Y, map.Width) + 2] = riverColour.R;
                    #endregion

                    #region Carve River
                    while (Bitmap_Utils.ReturnColour(currentPixel.X, currentPixel.Y, map.Width, mapRgbValues).B > 8)  //While we're not at sea level..
                    {
                        //Stop if at edge of screen.
                        if (currentPixel.Y - 1 < 0 || currentPixel.X - 1 < 0 || currentPixel.Y + 1 >= colouredMap.Height || currentPixel.X + 1 >= colouredMap.Width)
                        {
                            break;
                        }

                        Point delta = new Point(bestDirectionPoint.X - lastMove.X, bestDirectionPoint.Y - lastMove.Y); //direction we're going in (straight line)
                        bestValue = 0;  //reset the highest gradientValue each iteration.

                        Point sameDir = new Point(currentPixel.X + delta.X, currentPixel.Y + delta.Y);
                        if (Bitmap_Utils.ReturnColour(sameDir.X, sameDir.Y, map.Width, colMapRgbValues).B > Bitmap_Utils.ReturnColour(currentPixel.X, currentPixel.Y, map.Width, colMapRgbValues).B + 2)
                        {
                            #region Checking around currentPixel for steepest drop.
                            pixelColour = Bitmap_Utils.ReturnColour(currentPixel.X, currentPixel.Y - 1, map.Width, colMapRgbValues); //Colour of pixel we're looking at.
                            if (mapRgbValues[Bitmap_Utils.ConvertTo1DArr(currentPixel.X, currentPixel.Y - 1, map.Width)] > bestValue && //If this is the steepest drop...
                                 pixelColour != riverColour)    //And it's not already a river pixel..
                            {
                                bestValue = colMapRgbValues[Bitmap_Utils.ConvertTo1DArr(currentPixel.X, currentPixel.Y - 1, map.Width)];
                                bestDirectionPoint = new Point(currentPixel.X, currentPixel.Y - 1);
                            }

                            pixelColour = Bitmap_Utils.ReturnColour(currentPixel.X, currentPixel.Y + 1, map.Width, colMapRgbValues);
                            if (mapRgbValues[Bitmap_Utils.ConvertTo1DArr(currentPixel.X, currentPixel.Y + 1, map.Width)] > bestValue
                                && pixelColour != riverColour)
                            {
                                bestValue = colMapRgbValues[Bitmap_Utils.ConvertTo1DArr(currentPixel.X, currentPixel.Y + 1, map.Width)];
                                bestDirectionPoint = new Point(currentPixel.X, currentPixel.Y + 1);
                            }

                            pixelColour = Bitmap_Utils.ReturnColour(currentPixel.X - 1, currentPixel.Y, map.Width, colMapRgbValues);
                            if (mapRgbValues[Bitmap_Utils.ConvertTo1DArr(currentPixel.X - 1, currentPixel.Y, map.Width)] > bestValue
                                && pixelColour != riverColour)
                            {
                                bestValue = mapRgbValues[Bitmap_Utils.ConvertTo1DArr(currentPixel.X - 1, currentPixel.Y, map.Width)];
                                bestDirectionPoint = new Point(currentPixel.X - 1, currentPixel.Y);
                            }

                            pixelColour = Bitmap_Utils.ReturnColour(currentPixel.X + 1, currentPixel.Y, map.Width, colMapRgbValues);
                            if (mapRgbValues[Bitmap_Utils.ConvertTo1DArr(currentPixel.X + 1, currentPixel.Y, map.Width)] > bestValue
                                && pixelColour != riverColour)
                            {
                                bestValue = mapRgbValues[Bitmap_Utils.ConvertTo1DArr(currentPixel.X + 1, currentPixel.Y, map.Width)];
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
                                colMapRgbValues[Bitmap_Utils.ConvertTo1DArr(bestDirectionPoint.X, bestDirectionPoint.Y, map.Width)] = riverColour.B;
                                colMapRgbValues[Bitmap_Utils.ConvertTo1DArr(bestDirectionPoint.X, bestDirectionPoint.Y, map.Width) + 1] = riverColour.G;
                                colMapRgbValues[Bitmap_Utils.ConvertTo1DArr(bestDirectionPoint.X, bestDirectionPoint.Y, map.Width) + 2] = riverColour.R;
                            }
                            catch (System.IndexOutOfRangeException)
                            {
                                break;  //At edge of screen: stop.
                            }

                        }
                        else
                        {
                            colMapRgbValues[Bitmap_Utils.ConvertTo1DArr(bestDirectionPoint.X, bestDirectionPoint.Y, map.Width)] = riverColour.B;
                            colMapRgbValues[Bitmap_Utils.ConvertTo1DArr(bestDirectionPoint.X, bestDirectionPoint.Y, map.Width) + 1] = riverColour.G;
                            colMapRgbValues[Bitmap_Utils.ConvertTo1DArr(bestDirectionPoint.X, bestDirectionPoint.Y, map.Width) + 2] = riverColour.R;   //Colour first pixel  

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




                
                #region Shading.
                if (shaderMap != null)  //Interpolating the coloured map with another (black and white, so R = G = B) fractal to give it texture.
                {
                    colMapRgbValues[i] = (byte)Math.Min(255, (int)(colMapRgbValues[i] * ((float)shaderRgbValues[i] / 180)));
                    colMapRgbValues[i + 1] = (byte)Math.Min(255, (int)(colMapRgbValues[i + 1] * ((float)shaderRgbValues[i] / 180)));
                    colMapRgbValues[i + 2] = (byte)Math.Min(255, (int)(colMapRgbValues[i + 2] * ((float)shaderRgbValues[i] / 180)));
                }
                 
                #endregion
                 
                
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

            // Copy the RGB values back to the map
            System.Runtime.InteropServices.Marshal.Copy(colMapRgbValues, 0, colMapPtr, colMapBytes);

            // Unlock the bits and return.
            colouredMap.UnlockBits(colMapBmpData);
            map.UnlockBits(mapBmpData);
            if (shaderMap != null)
            shaderMap.UnlockBits(shaderBmpData);

            return colouredMap;
        }
        */

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

