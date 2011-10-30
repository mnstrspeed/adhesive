using System;
using System.Linq;
using System.Drawing;

namespace Adhesive.Core
{
    /// <summary>
    /// Distributes Screens over a VirtualScreen
    /// </summary>
    public class ScreenDistributor
    {
        private readonly ScreenConfiguration screenConfiguration;

        public ScreenDistributor(ScreenConfiguration screenConfiguration)
        {
            this.screenConfiguration = screenConfiguration;
        }

        /// <summary>
        /// Assigns the Screen.BoundsInVirtualScreen property for Screens in the ScreenConfiguration
        /// </summary>
        public void AssignScreenBoundsInVirtualScreen()
        {
            Point offset = this.DetermineOffset();
            foreach (Screen screen in this.screenConfiguration.Screens)
            {
                int leftNeighborCount = this.DetermineLeftNeighborCount(screen, offset);
                int topNeighborCount = this.DetermineTopNeighborCount(screen, offset);

                screen.BoundsInVirtualScreen = new Rectangle(
                    (screen.Bounds.X - offset.X) + (leftNeighborCount * this.screenConfiguration.BezelCompensation),
                    (screen.Bounds.Y - offset.Y) + (topNeighborCount * this.screenConfiguration.BezelCompensation),
                    screen.Bounds.Width, screen.Bounds.Height);
            }
        }

        /// <summary>
        /// Determines the offset (upper left point) of all Screens in the ScreenConfiguration
        /// </summary>
        /// <returns>The offset (upper left point) of all Screens in the ScreenConfiguration</returns>
        private Point DetermineOffset()
        {
            var screenBounds = this.screenConfiguration.Screens.Select(b => b.Bounds);
            return new Point(
                screenBounds.Min(b => b.X),
                screenBounds.Min(b => b.Y));
        }

        /// <summary>
        /// Determines the number of neighbor Screens on the left
        /// </summary>
        /// <param name="screen">The Screen to determine the number of neighbors on the left for</param>
        /// <param name="offset">The offset of all screens in the ScreenConfiguration</param>
        /// <returns>The number of neighbor Screens on the left of the given Screen</returns>
        private int DetermineLeftNeighborCount(Screen screen, Point offset)
        {
            Rectangle leftNeighborRegion = new Rectangle(
                    offset.X,
                    screen.Bounds.Y,
                    screen.Bounds.X - offset.X,
                    screen.Bounds.Height);

            return this.screenConfiguration.Screens.Where(
                    s => s.Bounds.IntersectsWith(leftNeighborRegion)).Count();
        }

        /// <summary>
        /// Determines the number of neighbor Screens on top
        /// </summary>
        /// <param name="screen">The Screen to determine the number of top neighbors for</param>
        /// <param name="offset">The offset of all screens in the ScreenConfiguration</param>
        /// <returns>The number of top neighbor Screen of the given Screen</returns>
        private int DetermineTopNeighborCount(Screen screen, Point offset)
        {
            Rectangle topNeighborRegion = new Rectangle(
                    screen.Bounds.X,
                    offset.Y,
                    screen.Bounds.Width,
                    screen.Bounds.Y - offset.Y);

            return this.screenConfiguration.Screens.Where(
                    s => s.Bounds.IntersectsWith(topNeighborRegion)).Count();
        }

    }
}
