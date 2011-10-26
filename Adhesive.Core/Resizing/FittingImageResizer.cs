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
            int targetWidth = (int)graphics.VisibleClipBounds.Width;
            int targetHeight = (int)graphics.VisibleClipBounds.Height;
            Point targetPosition = new Point { X = 0, Y = 0 };

            if (originalAspectRatio > targetAspectRatio)
            {
                // Maintain aspect ratio by adjusting the height,
                // black borders on top and bottom
                targetHeight = (int)Math.Round(targetWidth / originalAspectRatio);
                targetPosition.Y = ((int)graphics.VisibleClipBounds.Height - targetHeight) / 2;
            }
            else if (originalAspectRatio < targetAspectRatio)
            {
                // Maintain aspect ratio by adjusting the width,
                // black borders on left and right
                targetWidth = (int)Math.Round(originalAspectRatio * targetHeight);
                targetPosition.X = ((int)graphics.VisibleClipBounds.Width - targetWidth) / 2;
            }

            graphics.DrawImage(original, targetPosition.X, targetPosition.Y,
                targetWidth, targetHeight);
        }
    }
}
