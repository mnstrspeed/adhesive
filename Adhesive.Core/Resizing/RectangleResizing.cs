using System;
using System.Drawing;

namespace Adhesive.Core.Resizing
{
    public static class RectangleResizing
    {
        /// <summary>
        /// Fits Rectangle b in Rectangle a
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Rectangle Fit(Rectangle a, Rectangle b)
        {
            double targetAspectRatio = (double)a.Width / (double)a.Height;
            double originalAspectRatio = (double)b.Width / (double)b.Height;

            return RectangleResizing.Fit(a, targetAspectRatio, originalAspectRatio);
        }

        /// <summary>
        /// Fits Rectangle b in Rectangle a
        /// </summary>
        /// <param name="a"></param>
        /// <param name="targetAspectRatio"></param>
        /// <param name="originalAspectRatio"></param>
        /// <returns></returns>
        public static Rectangle Fit(Rectangle a, double targetAspectRatio,
            double originalAspectRatio)
        {
            Rectangle result = a;
            if (originalAspectRatio > targetAspectRatio)
            {
                // Maintain aspect ratio by adjusting the height,
                // black borders on top and bottom
                result.Height = (int)(result.Width / originalAspectRatio);
                result.Y += ((int)a.Height - result.Height) / 2;
            }
            else if (originalAspectRatio < targetAspectRatio)
            {
                // Maintain aspect ratio by adjusting the width,
                // black borders on left and right
                result.Width = (int)(originalAspectRatio * result.Height);
                result.X = ((int)a.Width - result.Width) / 2;
            }

            return result;
        }

        /// <summary>
        /// Fills Rectangle b in Rectangle a
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Rectangle Fill(Rectangle a, Rectangle b)
        {
            double targetAspectRatio = (double)a.Width / (double)a.Height;
            double originalAspectRatio = (double)b.Width / (double)b.Height;

            return RectangleResizing.Fill(a, targetAspectRatio, originalAspectRatio);
        }

        /// <summary>
        /// Fills Rectangle b in Rectangle a
        /// </summary>
        /// <param name="a"></param>
        /// <param name="targetAspectRatio"></param>
        /// <param name="originalAspectRatio"></param>
        /// <returns></returns>
        public static Rectangle Fill(Rectangle a, double targetAspectRatio, 
            double originalAspectRatio)
        {
            Rectangle result = a;
            if (targetAspectRatio > originalAspectRatio)
            {
                result.Height = (int)(result.Width / originalAspectRatio);
                result.Y -= ((result.Height - (int)a.Height) / 2);
            }
            else if (targetAspectRatio < originalAspectRatio)
            {
                result.Width = (int)(originalAspectRatio * result.Height);
                result.X -= ((result.Width - (int)a.Width) / 2);
            }

            return result;
        }

        /// <summary>
        /// Center Rectangle a in Rectangle b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Rectangle Center(Rectangle a, Rectangle b)
        {
            Rectangle result = new Rectangle(a.X, b.X, b.Width, b.Height);

            result.X += (a.Width - b.Width) / 2;
            result.Y += (a.Height - b.Height) / 2;

            return result;
        }

        /// <summary>
        /// Scale the X, Y, Width, and Height component
        /// </summary>
        /// <param name="a"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static Rectangle Scale(Rectangle a, double scale)
        {
            Rectangle result = a;

            result.X = (int)(result.X * scale);
            result.Y = (int)(result.Y * scale);
            result.Width = (int)(result.Width * scale);
            result.Height = (int)(result.Height * scale);

            return result;
        }

    }
}
