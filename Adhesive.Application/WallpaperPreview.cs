using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using Adhesive.Core;
using System.IO;
using System.Windows.Media.Imaging;

namespace Adhesive.Application
{
    public class WallpaperPreview : UserControl
    {
        public ScreenConfiguration ScreenConfiguration { get; set; }
        public Adhesive.Core.Resizing.IImageResizer ImageResizer { get; set; }
        public string ImagePath { get; private set; }

        private System.Drawing.Image image;

        public WallpaperPreview()
        {
            this.AllowDrop = true;
            this.Drop += new DragEventHandler(WallpaperPreview_Drop);
        }

        void WallpaperPreview_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];

                try
                {
                    this.image = System.Drawing.Image.FromFile(files[0]);
                    this.ImagePath = files[0];
                    this.InvalidateVisual();
                }
                catch { }
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
            Rect availableRegion = new Rect(0, 0, this.ActualWidth, this.ActualHeight);
            double availableRegionAspectRatio = availableRegion.Width / availableRegion.Height;
            double screenSurfaceAspectRatio = (double)this.ScreenConfiguration.ImageSurfaceBounds.Width /
                (double)this.ScreenConfiguration.ImageSurfaceBounds.Height;
 
            Rect targetRegion = availableRegion;
            if (screenSurfaceAspectRatio > availableRegionAspectRatio)
            {
                targetRegion.Height = targetRegion.Width / screenSurfaceAspectRatio;
                targetRegion.Y = (availableRegion.Height - targetRegion.Height) / 2;
            }
            else if (screenSurfaceAspectRatio < availableRegionAspectRatio)
            {
                targetRegion.Width = screenSurfaceAspectRatio * targetRegion.Height;
                targetRegion.X = (availableRegion.Width - targetRegion.Width) / 2;
            }
            double scale = targetRegion.Width / this.ScreenConfiguration.ImageSurfaceBounds.Width;

            Geometry geometry = new RectangleGeometry(new Rect(0, 0, 0, 0));

            System.Drawing.Image resizedImage = this.ImageResizer.ResizeImage(
                this.image, (int)targetRegion.Width, (int)targetRegion.Height);
            BitmapImage resizedBitmapImage = new BitmapImage();
            resizedBitmapImage.BeginInit();
            MemoryStream ms = new MemoryStream();
            resizedImage.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            ms.Seek(0, SeekOrigin.Begin);
            resizedBitmapImage.StreamSource = ms;
            resizedBitmapImage.EndInit();

            this.VisualBitmapScalingMode = BitmapScalingMode.HighQuality;
            drawingContext.DrawImage(resizedBitmapImage, targetRegion);

            foreach (Adhesive.Core.Screen screen in this.ScreenConfiguration.Screens)
            {
                Geometry screenGeometry = new RectangleGeometry(new Rect(
                    targetRegion.X + scale * screen.BoundsInImage.X,
                    targetRegion.Y + scale * screen.BoundsInImage.Y,
                    scale * screen.BoundsInImage.Width,
                    scale * screen.BoundsInImage.Height));
                CombinedGeometry combined = new CombinedGeometry(
                    GeometryCombineMode.Union, geometry, screenGeometry);
                geometry = combined;

                Rect destinationRect = new Rect(
                    targetRegion.X + scale * screen.BoundsInImage.X,
                    targetRegion.Y + scale * screen.BoundsInImage.Y,
                    scale * screen.BoundsInImage.Width,
                    scale * screen.BoundsInImage.Height);
                drawingContext.DrawRectangle(null, new Pen(Brushes.White, 2), destinationRect);
            }

            CombinedGeometry excludedRegion = new CombinedGeometry(
                GeometryCombineMode.Exclude, new RectangleGeometry(targetRegion), geometry);
            Brush brush = new SolidColorBrush(Colors.Black);
            brush.Opacity = 0.7;

            drawingContext.DrawGeometry(brush, null, excludedRegion);
            //drawingContext.DrawRectangle(Brushes.Black, new Pen(Brushes.White, 2), targetRegion);
            //drawingContext.DrawImage(this.image, new Rect(0, 0, this.ActualWidth, this.ActualHeight));
        }

    }
}
