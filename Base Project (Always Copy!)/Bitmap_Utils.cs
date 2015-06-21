using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Plasma_Fractal
{
    public static class Bitmap_Utils
    {
        public static int[,] MakeCircularGradient(int width, int height, int maxValue = 255, int minValue = 0)
        {
            int[,] map = new int[width,height];

            // Lock the map's bits.  
            double maxDistance = 0, centerX = width / 2, centerY = height / 2;
            maxDistance = Math.Sqrt((Math.Pow(centerX, 2)) + (Math.Pow(centerY, 2)));

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {

                    double distX = Math.Abs(x - centerX), distY = Math.Abs(y - centerY);    //Distance fron center in x and y.
                    double distance = Math.Sqrt(Math.Pow(distX, 2) + Math.Pow(distY, 2));   //Distance from center.

                    map[x,y] = (int)((distance / maxDistance) * 255);
                }
            }
            return map;
        }

        public static int[,] InterpolateBitmaps(int[,] bmp1, int[,] bmp2, double bmp1Coeff = 0.5, double bmp2Coeff = 0.5, int offset = 0, int maxValue = 255, int minValue = 0)
        {
            int width = bmp1.GetLength(0), height = bmp1.GetLength(1);
            int[,] newBmp = new int[width, height];


            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {

                    double val = ((bmp1[x,y] * bmp1Coeff) + (bmp2[x,y] * bmp2Coeff));

                    if (val > maxValue)
                    {
                        val = maxValue;
                    }
                    else if (val < minValue)
                    {
                        val = minValue;
                    }

                    newBmp[x,y] = (int)(val);
                }
            }

            return newBmp;
        }

        public static int ConvertTo1DArr(int x, int y, int width)
        {
            return ((y * width) + (x));
        }

        public static Color ReturnColour(int x, int y, int width, byte[] colMapRgbValues)
        {
            int bluePos = ((((y * width) + (x + 1)) * 3) - 3);
            //Remember we're converting from BGR to RGB, so it's reversed (2, 1, 0, not 0, 1, 2)
            return Color.FromArgb(colMapRgbValues[bluePos + 2], colMapRgbValues[bluePos + 1], colMapRgbValues[bluePos]);
        }

        public static Bitmap ArrayToBitmap(int[,] array)
        {
            //Assuming square map (root to find width and height).
            int width = (array.GetLength(0)), height = array.GetLength(1);
            Bitmap bitmap = new Bitmap(width, height);

            byte[] paddedArray = new byte[width * height * 3];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    //Finding position in a 1D array that has been padded wit empty values for green and red.
                    paddedArray[(((y * width) + (x + 1)) * 3) - 3] = (byte)array[x, y];

                }
            }

            // Lock the map's bits.  
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData mapBmpData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr mapPtr = mapBmpData.Scan0;
            int mapBytes = Math.Abs(mapBmpData.Stride) * bitmap.Height;

            //Unlock and return
            Marshal.Copy(paddedArray, 0, mapPtr, mapBytes);
            bitmap.UnlockBits(mapBmpData);
            return bitmap;
        }

        public static Bitmap BiomeArrayToBitmap(int[,] array)
        {
            //Assuming square map (root to find width and height).
            int width = (array.GetLength(0)), height = array.GetLength(1);
            Bitmap bitmap = new Bitmap(width, height);

            // Lock the map's bits.  
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData mapBmpData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr mapPtr = mapBmpData.Scan0;
            int mapBytes = Math.Abs(mapBmpData.Stride) * bitmap.Height;
            byte[] paddedArray = new byte[width * height * 3];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < height; x++)
                {
                    int j = ConvertTo1DArr(x, y, width);
                    int i = j * 3;

                    switch (array[x,y])
                    {
                        case -2:
                            {
                                //Deep Sea
                                paddedArray[i] = 63;
                                paddedArray[i + 1] = 50;
                                paddedArray[i + 2] = 28;
                                break;
                            }
                        case -1:
                            {
                                //Shallow Sea.
                                paddedArray[i] = 73;
                                paddedArray[i + 1] = 60;
                                paddedArray[i + 2] = 38;
                                break;
                            }
                        case 0:
                            {
                                //Tropical Sand
                                paddedArray[i] = 24;
                                paddedArray[i + 1] = 148;
                                paddedArray[i + 2] = 250;
                                break;
                            }
                        case 1:
                            {
                                //Sand
                                paddedArray[i] = 7;
                                paddedArray[i + 1] = 219;
                                paddedArray[i + 2] = 250;
                                break;
                            }
                        case 2:
                            {
                                //Tropical Seasonal Forest/Savanna
                                paddedArray[i] = 7;
                                paddedArray[i + 1] = 219;
                                paddedArray[i + 2] = 250;
                                break;
                            }
                        case 3:
                            {
                                //Tropical Rain Forest
                                paddedArray[i] = 35;
                                paddedArray[i + 1] = 224;
                                paddedArray[i + 2] = 155;

                                break;
                            }
                        case 4:
                            {
                                //Dedious Forest
                                paddedArray[i] = 83;
                                paddedArray[i + 1] = 177;
                                paddedArray[i + 2] = 46;
                                break;
                            }
                        case 5:
                            {
                                //Temperate Rain Forest
                                paddedArray[i] = 162;
                                paddedArray[i + 1] = 249;
                                paddedArray[i + 2] = 7;
                                break;
                            }
                        case 6:
                            {
                                //Swamp
                                paddedArray[i] = 0;
                                paddedArray[i + 1] = 102;
                                paddedArray[i + 2] = 76;
                                break;
                            }
                        case 7:
                            {
                                //Taiga
                                paddedArray[i] = 33;
                                paddedArray[i + 1] = 102;
                                paddedArray[i + 2] = 5;
                                break;
                            }
                        case 8:
                            {
                                //Tundra
                                paddedArray[i] = 249;
                                paddedArray[i + 1] = 235;
                                paddedArray[i + 2] = 85;
                                break;
                            }
                    }
                }
            }

            //Unlock and return
            Marshal.Copy(paddedArray, 0, mapPtr, mapBytes);
            bitmap.UnlockBits(mapBmpData);
            return bitmap;
        }

        public static byte[] removeZeroes(byte[] array)
        {
            byte[] newArray = new byte[array.Count() / 3];
            for (int i = 0; i < array.Count(); i++)
            {
                if (array[i] != 0)
                {
                    newArray[i / 3] = array[i];

                }
            }
            return newArray;
        }
    }
}

/*
                int y = (int)Math.Floor(((double)i) / ((double)width)),
                    x = (i - (y * width));
*/