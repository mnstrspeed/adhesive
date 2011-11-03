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
        #region Properties

        public static readonly DependencyProperty ScreenConfigurationProperty =
            DependencyProperty.Register("ScreenConfiguration", typeof(ScreenConfiguration), typeof(WallpaperPreview),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(ScreenConfigurationPropertyChanged)));

        public static readonly DependencyProperty ImageResizerProperty =
            DependencyProperty.Register("ImageResizer", typeof(ImageResizer), typeof(WallpaperPreview),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ImagePathProperty =
           DependencyProperty.Register("ImagePath", typeof(string), typeof(WallpaperPreview),
           new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender,
               new PropertyChangedCallback(ImagePathPropertyChanged)));

        public ScreenConfiguration ScreenConfiguration
        {
            get { return GetValue(ScreenConfigurationProperty) as ScreenConfiguration; }
            set { SetValue(ScreenConfigurationProperty, value); }
        }

        public IImageResizer ImageResizer
        {
            get { return GetValue(ImageResizerProperty) as ImageResizer; }
            set { SetValue(ImageResizerProperty, value);  }
        }

        public string ImagePath 
        { 
            get { return GetValue(ImagePathProperty) as string; }
            set { SetValue(ImagePathProperty, value); }
        }

        #endregion

        #region Fields

        private System.Drawing.Image image;

        #endregion

        #region Methods

        public static void ScreenConfigurationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WallpaperPreview wallpaperPreview = d as WallpaperPreview;
            ScreenConfiguration screenConfiguration = e.NewValue as ScreenConfiguration;

            screenConfiguration.PropertyChanged += (sender, args) => wallpaperPreview.InvalidateVisual();
        }

        public static void ImagePathPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WallpaperPreview wallpaperPreview = d as WallpaperPreview;
            wallpaperPreview.LoadImage();
        }

        private void LoadImage()
        {
            if (this.ImagePath != null)
            {
                this.image = System.Drawing.Image.FromFile(this.ImagePath);
            }
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
            Rect previewBounds = DeterminePreviewBounds();

            this.RenderImage(drawingContext, previewBounds);
            this.RenderScreens(drawingContext, previewBounds);
        }

        private Rect DeterminePreviewBounds()
        {
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
            return previewBounds;
        }

        private void RenderImage(DrawingContext drawingContext, Rect previewBounds)
        {
            // Temporary fix for centering:
            System.Drawing.Image resizedImage;
            if (this.ImageResizer.GetType() == typeof(CenteringImageResizer))
            {
                double scale = this.ScreenConfiguration.VirtualScreenBounds.Width / previewBounds.Width;
                resizedImage = this.ImageResizer.ResizeImage(
                    this.image, (int)(previewBounds.Width * scale), (int)(previewBounds.Height * scale));
            }
            else
            {
                // Resize System.Drawing.Image and convert back into System.Windows.Drawing.Image
                resizedImage = this.ImageResizer.ResizeImage(
                    this.image, (int)previewBounds.Width, (int)previewBounds.Height);
            }
            BitmapImage resizedBitmapImage = resizedImage.ToWpfBitmap();

            this.VisualBitmapScalingMode = BitmapScalingMode.HighQuality;
            drawingContext.DrawImage(resizedBitmapImage, previewBounds);
        }

        private void RenderScreens(DrawingContext drawingContext, Rect previewBounds)
        {
            double scale = previewBounds.Width / this.ScreenConfiguration.VirtualScreenBounds.Width;

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

        #endregion
    }
}
