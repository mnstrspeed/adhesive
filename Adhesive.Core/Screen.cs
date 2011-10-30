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
        public Rectangle Bounds { get; private set; }

        /// <summary>
        /// THe bounds of the screen in the image. The original image will
        /// be resized to match the combined bounds of the BoundsInImage property
        /// of all relevant screens. Bezel compensation can be implemented
        /// at this point.
        /// </summary>
        public Rectangle BoundsInVirtualScreen { get; set; }

        public Screen(System.Windows.Forms.Screen screen) 
            : this(screen.Bounds)
        { }

        public Screen(Rectangle bounds)
        {
            this.Bounds = bounds;

            // Default for the BoundsInImage
            this.BoundsInVirtualScreen = new Rectangle(0, 0, this.Bounds.Width, this.Bounds.Height);
        }

        public override string ToString()
        {
            return String.Format("Screen{0}", this.Bounds.ToString());
        }

    }
}
