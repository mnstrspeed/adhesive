using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Adhesive.Core.Resizing
{
    public class FillingImageResizer : AspectRatioDependentImageResizer
    {
        protected override void ResizeImage(Graphics graphics, Image original, 
            double originalAspectRatio, double targetAspectRatio)
        {
            int targetWidth = (int)graphics.VisibleClipBounds.Width;
            int targetHeight = (int)graphics.VisibleClipBounds.Height;
            Point targetPosition = new Point { X = 0, Y = 0 };

            if (targetAspectRatio > originalAspectRatio)
            {
                targetHeight = (int)(targetWidth / originalAspectRatio);
                targetPosition.Y = 0 - ((targetHeight - (int)graphics.VisibleClipBounds.Height) / 2);
            }
            else if (targetAspectRatio < originalAspectRatio)
            {
                targetWidth = (int)(originalAspectRatio * targetHeight);
                targetPosition.X = 0 - ((targetWidth - (int)graphics.VisibleClipBounds.Width) / 2);
            }

            graphics.DrawImage(original, targetPosition.X, targetPosition.Y,
                targetWidth, targetHeight);
        }
    }
}
