using System;
using System.Drawing;

namespace Adhesive.Core.Resizing
{
    public class CenteringImageResizer : ImageResizer
    {
        private static GraphicsUnit GraphicsUnit = GraphicsUnit.Pixel;

        public override void ResizeImage(Graphics graphics, Image original)
        {
            Rectangle target = RectangleResizing.Center(
                Rectangle.Round(graphics.VisibleClipBounds),
                Rectangle.Round(original.GetBounds(ref GraphicsUnit)));
            graphics.DrawImage(original, target);
        }
    }
}
