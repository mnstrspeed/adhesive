using System;
using System.Linq;
using System.Drawing;

namespace Adhesive.Core
{
    public class ScreenDistributor
    {
        private readonly ScreenConfiguration screenConfiguration;

        public int BezelCompensation { get; set; }

        public ScreenDistributor(ScreenConfiguration screenConfiguration)
        {
            this.screenConfiguration = screenConfiguration;
            this.BezelCompensation = 0;
        }

        public void AssignScreenImageBounds()
        {
            Point offset = this.DetermineOffset();
            foreach (Screen screen in this.screenConfiguration.Screens)
            {
                int leftNeighborCount = this.DetermineLeftNeighborCount(screen, offset);
                int topNeighborCount = this.DetermineTopNeighborCount(screen, offset);

                screen.BoundsInImage = new Rectangle(
                    (screen.BoundsInSurface.X - offset.X) + (leftNeighborCount * this.BezelCompensation),
                    (screen.BoundsInSurface.Y - offset.Y) + (topNeighborCount * this.BezelCompensation),
                    screen.Width, screen.Height);
            }
        }

        private Point DetermineOffset()
        {
            var bounds = this.screenConfiguration.Screens.Select(b => b.BoundsInSurface);
            return new Point(
                bounds.Min(b => b.X),
                bounds.Min(b => b.Y));
        }

        private int DetermineLeftNeighborCount(Screen screen, Point offset)
        {
            Rectangle leftNeighborRegion = new Rectangle(
                    offset.X,
                    screen.BoundsInSurface.Y,
                    screen.BoundsInSurface.X - offset.X,
                    screen.BoundsInSurface.Height);

            return this.screenConfiguration.Screens.Where(
                    s => s.BoundsInSurface.IntersectsWith(leftNeighborRegion)).Count();
        }

        private int DetermineTopNeighborCount(Screen screen, Point offset)
        {
            Rectangle topNeighborRegion = new Rectangle(
                    screen.BoundsInSurface.X,
                    offset.Y,
                    screen.BoundsInSurface.Width,
                    screen.BoundsInSurface.Y - offset.Y);

            return this.screenConfiguration.Screens.Where(
                    s => s.BoundsInSurface.IntersectsWith(topNeighborRegion)).Count();
        }

    }
}
