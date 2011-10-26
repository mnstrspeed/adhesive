using System;
using System.Drawing;

namespace Adhesive.Core.Resizing
{
    public interface IImageResizer
    {
        Image ResizeImage(Image original, int width, int height);
    }
}
