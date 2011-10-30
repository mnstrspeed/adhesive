using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using Adhesive.Core;
using Adhesive.Core.Resizing;
using System.IO;
using System.Windows.Media.Imaging;

namespace Adhesive.Application
{
    public class WallpaperPreview : UserControl
    {
        private ScreenConfiguration screenConfiguration;

        /// <summary>
        /// ScreenConfiguration to preview the wallpaper on
        /// </summary>
        public ScreenConfiguration ScreenConfiguration
        { 
            get
            {
                return this.screenConfiguration;
            }
            set
            {
                if (this.screenConfiguration != null)
                {
                    this.screenConfiguration.Changed -= new ChangedEventHandler(ScreenConfigurationChanged);
                }
                this.screenConfiguration = value;
                this.screenConfiguration.Changed += new ChangedEventHandler(ScreenConfigurationChanged);
            }   
        }

        /// <summary>
        /// ImageResizer used to resize the wallpaper image
        /// </summary>
        public IImageResizer ImageResizer { get; set; }

        public static readonly DependencyProperty ImagePathProperty =
            DependencyProperty.Register("ImagePath", typeof(string), typeof(WallpaperPreview), 
            new UIPropertyMetadata(ImagePathPropertyChanged));

        public static void ImagePathPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WallpaperPreview wallpaperPreview = d as WallpaperPreview;
            wallpaperPreview.LoadImage();
        }

        /// <summary>
        /// Path to the wallpaper image to preview
        /// </summary>
        public string ImagePath 
        { 
            get
            {
                return GetValue(ImagePathProperty) as string;
            }
            set
            {
                SetValue(ImagePathProperty, value);
                //this.LoadImage();
            }
        }

        private System.Drawing.Image image;

        /// <summary>
        /// Triggered whenever the ScreenConfiguration has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ScreenConfigurationChanged(object sender, EventArgs e)
        {
            this.InvalidateVisual();
        }

        private void LoadImage()
        {   
            this.image = System.Drawing.Image.FromFile(this.ImagePath);
            this.InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            this.Clear(drawingContext);

            if (image == null)
            {
                this.DrawInstructions(drawingContext);
            }
            else
            {
                this.DrawPreview(drawingContext);
            }

            base.OnRender(drawingContext);
        }

        private void Clear(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(Brushes.Black, new Pen(Brushes.Black, 1),
                new Rect(0, 0, this.ActualWidth, this.ActualHeight));
        }

        private void DrawInstructions(DrawingContext drawingContext)
        {
            FormattedText instructionText = new FormattedText("Drag an image onto this surface",
                System.Globalization.CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight,
                new Typeface("Segoe UI"), 14, Brushes.White);

            drawingContext.DrawText(instructionText, new System.Windows.Point(
                (this.ActualWidth - instructionText.Width) / 2,
                (this.ActualHeight - instructionText.Height) / 2));
        }

        private void DrawPreview(DrawingContext drawingContext)
        {
            // Determine resized virtual screen bounds
            Rect controlBounds = new Rect(0, 0, this.ActualWidth, this.ActualHeight);
            double controlBoundsAspectRatio = controlBounds.Width / controlBounds.Height;
            double screenSurfaceAspectRatio = (double)this.ScreenConfiguration.VirtualScreenBounds.Width /
                (double)this.ScreenConfiguration.VirtualScreenBounds.Height;
 
            Rect previewBounds = controlBounds;
            if (screenSurfaceAspectRatio > controlBoundsAspectRatio)
            {
                previewBounds.Height = previewBounds.Width / screenSurfaceAspectRatio;
                previewBounds.Y = (controlBounds.Height - previewBounds.Height) / 2;
            }
            else if (screenSurfaceAspectRatio < controlBoundsAspectRatio)
            {
                previewBounds.Width = screenSurfaceAspectRatio * previewBounds.Height;
                previewBounds.X = (controlBounds.Width - previewBounds.Width) / 2;
            }
            double scale = previewBounds.Width / this.ScreenConfiguration.VirtualScreenBounds.Width;

            // Resize System.Drawing.Image and convert back into System.Windows.Drawing.Image
            System.Drawing.Image resizedImage = this.ImageResizer.ResizeImage(
                this.image, (int)previewBounds.Width, (int)previewBounds.Height);
            // TODO: scaling? [CenteringImageResizer]
            BitmapImage resizedBitmapImage = resizedImage.ToWpfBitmap();

            // Draw Image
            this.VisualBitmapScalingMode = BitmapScalingMode.HighQuality;
            drawingContext.DrawImage(resizedBitmapImage, previewBounds);

            Geometry combinedScreenGeometry = new RectangleGeometry(new Rect(0, 0, 0, 0));
            foreach (Adhesive.Core.Screen screen in this.ScreenConfiguration.Screens)
            {
                Rect boundsInPreview = new Rect(
                    previewBounds.X + scale * screen.BoundsInVirtualScreen.X,
                    previewBounds.Y + scale * screen.BoundsInVirtualScreen.Y,
                    scale * screen.BoundsInVirtualScreen.Width,
                    scale * screen.BoundsInVirtualScreen.Height);

                Geometry screenGeometry = new RectangleGeometry(boundsInPreview);
                // Add Screen Bounds to Geometry of all Screens
                combinedScreenGeometry = new CombinedGeometry(GeometryCombineMode.Union, 
                    combinedScreenGeometry, screenGeometry); ;

                // Draw Screen
                drawingContext.DrawRectangle(null, new Pen(Brushes.White, 2), boundsInPreview);
            }

            // Black out excluded region over image
            CombinedGeometry excludedRegion = new CombinedGeometry(
                GeometryCombineMode.Exclude, new RectangleGeometry(previewBounds), combinedScreenGeometry);
            Brush brush = new SolidColorBrush(Colors.Black);
            brush.Opacity = 0.7;

            drawingContext.DrawGeometry(brush, null, excludedRegion);
        }

    }
}
