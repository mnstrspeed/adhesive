using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace Adhesive.Core
{
    public class ScreenConfiguration
    {
        private readonly ScreenDistributor screenDistributor;
        private Screen[] screens;

        /// <summary>
        /// All Screens in the ScreenConfiguration
        /// </summary>
        public Screen[] Screens
        {
            get
            {
                return this.screens;
            }
            set
            {
                this.screens = value;

                // Update bounds
                this.UpdateSurfaceBounds();

                this.AssignScreenImageBounds();
                this.UpdateImageSurfaceBounds();
            }
        }

        /// <summary>
        /// The bounds of the combined screen surfaces as they are
        /// assigned by Windows.
        /// </summary>
        public Rectangle SurfaceBounds { get; private set; }

        /// <summary>
        /// The bounds of the surface the original image will be resized to.
        /// </summary>
        public Rectangle ImageSurfaceBounds { get; private set; }

        public ScreenConfiguration(Screen[] screens)
            : this(screens, 0)
        { }

        public ScreenConfiguration(Screen[] screens, int bezelCompensation) 
        {
            this.screenDistributor = new ScreenDistributor(this);
            this.screenDistributor.BezelCompensation = bezelCompensation;

            this.Screens = screens;
        }

        /// <summary>
        /// Updates the merged surface bounds of all the screens
        /// in the ScreenConfiguration
        /// </summary>
        private void UpdateSurfaceBounds()
        {
            var bounds = this.Screens.Select(s => s.BoundsInSurface);
            this.SurfaceBounds = this.DetermineMergedBounds(bounds);
        }

        /// <summary>
        /// Assign Screen image bounds
        /// </summary>
        private void AssignScreenImageBounds()
        {
            this.screenDistributor.AssignScreenImageBounds();
        }

        /// <summary>
        /// Updates the merged image bounds of all the screens
        /// in the ScreenConfiguration
        /// </summary>
        private void UpdateImageSurfaceBounds()
        {
            var bounds = this.Screens.Select(s => s.BoundsInImage);
            Rectangle mergedBounds = this.DetermineMergedBounds(bounds);

            this.ImageSurfaceBounds = new Rectangle(0, 0,
                mergedBounds.Width, mergedBounds.Height);
        }

        private Rectangle DetermineMergedBounds(IEnumerable<Rectangle> bounds)
        {
            Point offset = new Point(
                bounds.Min(b => b.X),
                bounds.Min(b => b.Y));

            return new Rectangle(
                offset.X, offset.Y,
                bounds.Max(b => b.X - offset.X + b.Width),
                bounds.Max(b => b.Y - offset.Y + b.Height));
        }

    }
}
