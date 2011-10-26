using System;
using System.Drawing;

namespace Adhesive.Core.Resizing
{
    public abstract class ImageResizer : IImageResizer
    {
        public Image ResizeImage(Image original, int width, int height)
        {
            Bitmap resizedImage = new Bitmap(width, height);
            using (Graphics graphics = Graphics.FromImage(resizedImage))
            {
                this.ResizeImage(graphics, original);
            }

            return resizedImage;
        }

        public abstract void ResizeImage(Graphics graphics, Image original);
    }
}
