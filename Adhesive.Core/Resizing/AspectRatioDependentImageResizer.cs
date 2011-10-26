using System;
using System.Drawing;

namespace Adhesive.Core.Resizing
{
    public abstract class AspectRatioDependentImageResizer : ImageResizer
    {
        public override void ResizeImage(Graphics graphics, Image original)
        {
            double originalAspectRatio = (double)original.Width / (double)original.Height;
            double targetAspectRatio = (double)graphics.VisibleClipBounds.Width / (double)graphics.VisibleClipBounds.Height;

            this.ResizeImage(graphics, original, originalAspectRatio, targetAspectRatio);
        }

        protected abstract void ResizeImage(Graphics graphics, Image original,
            double originalAspectRatio, double targetAspectRatio);
    }
}
