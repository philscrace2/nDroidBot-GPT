using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Common;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace nDroidBot_GPT.PythonRefactor.Adapter
{
    public class ImageUtils
    {
        // Rectangle intersection check
        public static bool Intersect(Rectangle rect1, Rectangle rect2)
        {
            // Check x
            bool xIntersect = false;
            if (rect1.Left <= rect2.Left && rect2.Left - rect1.Left < rect1.Width)
                xIntersect = true;
            if (rect2.Left <= rect1.Left && rect1.Left - rect2.Left < rect2.Width)
                xIntersect = true;

            // Check y
            bool yIntersect = false;
            if (rect1.Top <= rect2.Top && rect2.Top - rect1.Top < rect1.Height)
                yIntersect = true;
            if (rect2.Top <= rect1.Top && rect1.Top - rect2.Top < rect2.Height)
                yIntersect = true;

            return xIntersect && yIntersect;
        }

        // Load image from file path
        public static Bitmap LoadImageFromPath(string imgPath)
        {
            return new Bitmap(imgPath);
        }

        // Load image from byte array
        public static Bitmap LoadImageFromBuf(byte[] imgBytes)
        {
            using (MemoryStream ms = new MemoryStream(imgBytes))
            {
                return new Bitmap(ms);
            }
        }

        // Find views (rectangles) in a UI screenshot
        public static List<Rectangle> FindViews(Bitmap img)
        {
            int width = img.Width;
            int height = img.Height;
            int area = width * height;

            // Resize to smaller image for processing
            int xScale = 3;  // scale by 0.3
            int yScale = 3;  // scale by 0.3
            Bitmap resizedImg = new Bitmap(img, new Size(width / xScale, height / yScale));

            // Convert image to grayscale and then perform edge detection (Canny edges)
            Bitmap grayImg = ConvertToGrayscale(resizedImg);
            List<Rectangle> rectangleList = new List<Rectangle>();

            // Edge detection (Canny) applied to grayscale image
            // You would need a Canny edge detection algorithm or use a library like OpenCV (EmguCV in C#)
            // Here, we are simulating the edge detection process
            List<Rectangle> contours = GetContours(grayImg);

            foreach (var contour in contours)
            {
                if (contour.Width * contour.Height < area / 300 || contour.Width * contour.Height > area / 4)
                    continue;

                rectangleList.Add(contour);
            }

            return rectangleList;
        }

        // Convert image to grayscale
        private static Bitmap ConvertToGrayscale(Bitmap img)
        {
            Bitmap grayImg = new Bitmap(img.Width, img.Height);
            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    Color pixelColor = img.GetPixel(x, y);
                    int grayValue = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11);
                    grayImg.SetPixel(x, y, Color.FromArgb(grayValue, grayValue, grayValue));
                }
            }
            return grayImg;
        }

        // Get contours (simulating OpenCV contours)
        private static List<Rectangle> GetContours(Bitmap grayImg)
        {
            List<Rectangle> contours = new List<Rectangle>();

            // Simulate finding contours, you would typically use OpenCV or other libraries for this
            contours.Add(new Rectangle(10, 10, 50, 50));
            contours.Add(new Rectangle(60, 60, 100, 100));

            return contours;
        }

        // Calculate dHash value
        public static string CalculateDHash(Bitmap img)
        {
            var difference = CalculatePixelDifference(img);
            string hashString = "";

            int decimalValue = 0;
            for (int i = 0; i < difference.Length; i++)
            {
                if (difference[i])
                    decimalValue += (int)Math.Pow(2, i % 8);

                if (i % 8 == 7)
                {
                    hashString += decimalValue.ToString("x2");
                    decimalValue = 0;
                }
            }

            return hashString;
        }

        // Calculate pixel differences for dhash
        private static bool[] CalculatePixelDifference(Bitmap img)
        {
            int resizeWidth = 18;
            int resizeHeight = 16;

            Bitmap resizedImage = new Bitmap(img, new Size(resizeWidth, resizeHeight));
            bool[] difference = new bool[resizeHeight * (resizeWidth - 1)];

            for (int row = 0; row < resizeHeight; row++)
            {
                for (int col = 0; col < resizeWidth - 1; col++)
                {
                    Color pixel1 = resizedImage.GetPixel(col, row);
                    Color pixel2 = resizedImage.GetPixel(col + 1, row);
                    difference[row * (resizeWidth - 1) + col] = pixel1.R > pixel2.R;
                }
            }

            return difference;
        }

        // Calculate Hamming distance between two images
        public static int ImgHammingDistance(Bitmap img1, Bitmap img2)
        {
            string dhash1 = CalculateDHash(img1);
            string dhash2 = CalculateDHash(img2);
            return DHashHammingDistance(dhash1, dhash2);
        }

        // Calculate Hamming distance between two dHash values
        public static int DHashHammingDistance(string dhash1, string dhash2)
        {
            int difference = Convert.ToInt32(dhash1, 16) ^ Convert.ToInt32(dhash2, 16);
            return Convert.ToString(difference, 2).Count(c => c == '1');
        }
    }

}
