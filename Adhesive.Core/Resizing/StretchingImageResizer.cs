using System;
using System.Drawing;

namespace Adhesive.Core.Resizing
{
    public class StretchingImageResizer : ImageResizer
    {
        public override void ResizeImage(Graphics graphics, Image original)
        {
            graphics.DrawImage(original, graphics.VisibleClipBounds);
        }
    }
}
