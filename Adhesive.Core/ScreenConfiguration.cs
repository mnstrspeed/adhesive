using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;

namespace Adhesive.Core
{
    /// <summary>
    /// Represents a collection of Screens and their properties in relation to one-another
    /// </summary>
    public class ScreenConfiguration : INotifyPropertyChanged
    {
        private readonly ScreenDistributor screenDistributor;

        /// <summary>
        /// Event triggered whenever a ScreenConfiguration is updated
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

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

                this.UpdateMergedBounds();
                this.UpdateVirtualScreen();
                // Trigger Changed event
                this.OnPropertyChanged("Screens");
            }
        }

        /// <summary>
        /// The merged bounds of Screens as they are assigned by Windows: the top left corner
        /// of the primary screen has the coordinates (0,0)
        /// </summary>
        public Rectangle MergedBounds { get; private set; }

        /// <summary>
        /// Bounds of the VirtualScreen that will fit all Screens in the ScreenConfiguration
        /// compensated for bezel size
        /// </summary>
        public Rectangle VirtualScreenBounds { get; private set; }

        private int bezelCompensation;

        /// <summary>
        /// The bezel compensation between screens in pixels. Negative values indicate overlapping displays
        /// </summary>
        public int BezelCompensation
        {
            get
            {
                return this.bezelCompensation;
            }
            set
            {
                this.bezelCompensation = value;
                this.UpdateVirtualScreen();
                // Trigger Changed event
                this.OnPropertyChanged("BezelCompensation");
            }
        }

        /// <summary>
        /// Initializes the ScreenConfiguration for a collection of Screens and
        /// a default BezelCompensation of 0
        /// </summary>
        /// <param name="screens">Screens in the ScreenConfiguration</param>
        public ScreenConfiguration(Screen[] screens)
            : this(screens, 0)
        { }

        /// <summary>
        /// Initializes the ScreenConfiguration for a collection of Screens
        /// with a specified BezelCompensation
        /// </summary>
        /// <param name="screens">Screens in the ScreenConfiguration</param>
        /// <param name="bezelCompensation">The BezelCompensation in pixels as an added margin to screen 
        /// bounds in a VirtualScreen to compensate for the display bezels in a multi-monitor setup</param>
        public ScreenConfiguration(Screen[] screens, int bezelCompensation) 
        {
            this.screenDistributor = new ScreenDistributor(this);

            this.bezelCompensation = bezelCompensation; // private field so we don't trigger UpdateVirtualScreen() twice
            this.Screens = screens;
        }

        /// <summary>
        /// Initializes a ScreenConfiguration using the screens as provided by
        /// System.Windows.Forms.Screen.AllScreens
        /// </summary>
        /// <returns>The initialized ScreenConfiguration</returns>
        public static ScreenConfiguration FromWindowsFormsScreens()
        {
            return new ScreenConfiguration(System.Windows.Forms.Screen.AllScreens.Select(
                screen => new Adhesive.Core.Screen(screen)).ToArray());
        }

        /// <summary>
        /// Updates the merged bounds of all the screens in the ScreenConfiguration
        /// </summary>
        private void UpdateMergedBounds()
        {
            var bounds = this.Screens.Select(s => s.Bounds);
            this.MergedBounds = this.DetermineMergedBounds(bounds);
        }

        /// <summary>
        /// Updates the properties related to the VirtualScreen, such as the
        /// Screen.BoundsInVirtualScreen properties and VirtualScreenBounds
        /// </summary>
        private void UpdateVirtualScreen()
        {
            this.AssignScreenBoundsInVirtualScreen();
            this.UpdateVirtualScreenBounds();
        }

        /// <summary>
        /// Assign the Screen.BoundsInVirtualScreen property of Screens in the ScreenConfiguration
        /// </summary>
        private void AssignScreenBoundsInVirtualScreen()
        {
            this.screenDistributor.AssignScreenBoundsInVirtualScreen();
        }

        /// <summary>
        /// Updates the VirtualScreenBounds to fit the Screen.BoundsInVirtualScreen values
        /// set by the ScreenDistributor
        /// </summary>
        private void UpdateVirtualScreenBounds()
        {
            var screenBounds = this.Screens.Select(s => s.BoundsInVirtualScreen);
            Rectangle mergedScreenBounds = this.DetermineMergedBounds(screenBounds);

            this.VirtualScreenBounds = new Rectangle(0, 0,
                mergedScreenBounds.Width, mergedScreenBounds.Height);
        }

        private Rectangle DetermineMergedBounds(IEnumerable<Rectangle> bounds) // To RectangleHelper
        {
            Point offset = new Point(
                bounds.Min(b => b.X),
                bounds.Min(b => b.Y));

            return new Rectangle(
                offset.X, offset.Y,
                bounds.Max(b => b.X - offset.X + b.Width),
                bounds.Max(b => b.Y - offset.Y + b.Height));
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
