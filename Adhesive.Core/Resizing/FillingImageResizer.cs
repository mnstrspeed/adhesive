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
            Rectangle target = RectangleResizing.Fill(Rectangle.Round(graphics.VisibleClipBounds),
                targetAspectRatio, originalAspectRatio);
            graphics.DrawImage(original, target);
        }
    }
}
