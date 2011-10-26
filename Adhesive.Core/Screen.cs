using System;
using System.Drawing;

namespace Adhesive.Core
{
    public class Screen
    {
        /// <summary>
        /// The bounds of the screen in the screen surface (the combined
        /// bounds of all relevant screens) as they are assigned by Windows
        /// </summary>
        public Rectangle BoundsInSurface { get; private set; }

        /// <summary>
        /// THe bounds of the screen in the image. The original image will
        /// be resized to match the combined bounds of the BoundsInImage property
        /// of all relevant screens. Bezel compensation can be implemented
        /// at this point.
        /// </summary>
        public Rectangle BoundsInImage { get; set; }

        /// <summary>
        /// The width of the screen
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// The height of the screen
        /// </summary>
        public int Height { get; private set; }

        public Screen(System.Windows.Forms.Screen screen) : 
            this(screen.Bounds, screen.Bounds.Width, screen.Bounds.Height)
        { }

        public Screen(Rectangle boundsInSurface, int width, int height)
        {
            this.BoundsInSurface = boundsInSurface;
            this.Width = width;
            this.Height = height;

            // Default for the BoundsInImage
            this.BoundsInImage = new Rectangle(0, 0, this.Width, this.Height);
        }

        public override string ToString()
        {
            return String.Format("Screen{0}", this.BoundsInSurface.ToString());
        }

    }
}
