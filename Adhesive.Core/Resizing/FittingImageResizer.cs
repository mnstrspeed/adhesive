using System;
using System.Drawing;

namespace Adhesive.Core.Resizing
{
    /// <summary>
    /// Implements an image resizer that fits the image
    /// to the screen, so that the aspect ratio of the
    /// original image is maintained.
    /// </summary>
    public class FittingImageResizer : AspectRatioDependentImageResizer
    {
        protected override void ResizeImage(Graphics graphics, Image original, 
            double originalAspectRatio, double targetAspectRatio)
        {
            Rectangle target = RectangleResizing.Fit(Rectangle.Round(graphics.VisibleClipBounds),
                targetAspectRatio, originalAspectRatio);
            graphics.DrawImage(original, target);
        }
    }
}
