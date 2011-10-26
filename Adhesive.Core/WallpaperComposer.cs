using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace Adhesive.Core
{
    public class WallpaperComposer
    {
        private readonly ScreenConfiguration screenConfiguration;

        public WallpaperComposer(ScreenConfiguration screenConfiguration)
        {
            this.screenConfiguration = screenConfiguration;
        }

        public void Compose(WallpaperComposerParameters parameters)
        {
            Image originalImage = parameters.ImageProvider.GetImage();
            Image resizedImage = parameters.ImageResizer.ResizeImage(originalImage,
                this.screenConfiguration.ImageSurfaceBounds.Width,
                this.screenConfiguration.ImageSurfaceBounds.Height);

            //this.SaveDebugImage(resizedImage);

            Image wallpaper = this.ComposeWallpaper(resizedImage);
            parameters.WallpaperProcessor.Process(wallpaper);
        }

        //private void SaveDebugImage(Image resizedImage)
        //{
        //    Bitmap debug = new Bitmap(resizedImage);
        //    using (Graphics graphics = Graphics.FromImage(debug))
        //    {
        //        foreach (Screen screen in this.screenConfiguration.Screens)
        //        {
        //            graphics.DrawRectangle(new Pen(Color.RoyalBlue, 5), screen.BoundsInImage);
        //        }
        //    }
        //    debug.Save("C:\\debug.jpg");
        //}

        private Image ComposeWallpaper(Image resizedImage)
        {
            Bitmap wallpaper = new Bitmap(
                this.screenConfiguration.SurfaceBounds.Width,
                this.screenConfiguration.SurfaceBounds.Height);
            Bitmap image = new Bitmap(resizedImage);

            foreach (Screen screen in this.screenConfiguration.Screens)
            {
                this.DrawScreen(wallpaper, image, screen);
            }

            return wallpaper;
        }

        private void DrawScreen(Bitmap wallpaper, Bitmap image, Screen screen)
        {
            if (screen.BoundsInSurface.X >= 0 && screen.BoundsInSurface.Y >= 0)
            {
                this.DrawScreenRectangle(wallpaper, image, screen);
            }
            else
            {
                this.DrawScreenPerPixel(wallpaper, image, screen);
            }
            
        }

        private void DrawScreenPerPixel(Bitmap wallpaper, Bitmap image, Screen screen)
        {
            Point screenPosition = new Point();
            Point imagePosition = new Point();
            Point wallpaperPosition = new Point();

            using (FastBitmap wallpaperBitmap = new FastBitmap(wallpaper))
            using (FastBitmap imageBitmap = new FastBitmap(image))
            {
                // Loop through all pixel rows
                for (screenPosition.Y = screen.BoundsInSurface.Y; screenPosition.Y <
                    screen.BoundsInSurface.Bottom; screenPosition.Y++)
                {
                    // Determine the vertical components of the image- and wallpaper positions
                    imagePosition.Y = screen.BoundsInImage.Y + (screenPosition.Y - screen.BoundsInSurface.Y);
                    wallpaperPosition.Y = screenPosition.Y >= 0 ? screenPosition.Y : wallpaper.Height + screenPosition.Y;

                    // Loop through all pixels in the current row
                    for (screenPosition.X = screen.BoundsInSurface.X; screenPosition.X <
                        screen.BoundsInSurface.Right; screenPosition.X++)
                    {
                        // Determine the horizontal components of the image- and wallpaper positions
                        imagePosition.X = screen.BoundsInImage.X + (screenPosition.X - screen.BoundsInSurface.X);
                        wallpaperPosition.X = screenPosition.X >= 0 ? screenPosition.X : wallpaper.Width + screenPosition.X;

                        // Assign the pixel data values at the image pixel position to the pixel at
                        // the wallpaper pixel position (copying it over)
                        FastBitmap.Pixel imagePixel = imageBitmap.GetPixel(imagePosition.X, imagePosition.Y);
                        wallpaperBitmap.SetPixel(wallpaperPosition.X, wallpaperPosition.Y, imagePixel);
                    }
                }
            }

        }

        //private struct PixelData
        //{
        //    public byte blue;
        //    public byte green;
        //    public byte red;
        //    public byte alpha;
        //}

        //private void DrawScreenPerPixelFast(Bitmap wallpaper, Bitmap image, Screen screen) 
        //{
        //    Point screenPosition = new Point();
        //    Point imagePosition = new Point();
        //    Point wallpaperPosition = new Point();

        //    BitmapData imageData = image.LockBits(screen.BoundsInImage, ImageLockMode.ReadOnly, image.PixelFormat);
        //    BitmapData wallpaperData = wallpaper.LockBits(new Rectangle(0, 0, wallpaper.Width, wallpaper.Height), 
        //        ImageLockMode.WriteOnly, wallpaper.PixelFormat);

        //    unsafe 
        //    {
        //        IntPtr imageDataOffset = imageData.Scan0;
        //        IntPtr wallpaperDataOffset = wallpaperData.Scan0;

        //        int wallpaperWidth = wallpaper.Width;
        //        int wallpaperHeight = wallpaper.Height;

        //        for (screenPosition.Y = screen.BoundsInSurface.Y; screenPosition.Y <
        //            screen.BoundsInSurface.Y + screen.BoundsInSurface.Height; screenPosition.Y++)
        //        {
        //            IntPtr imageYPosition = imageDataOffset + imagePosition.Y * imageData.Stride;
        //            wallpaperPosition.Y = screenPosition.Y >= 0 ?
        //                    screenPosition.Y : wallpaperHeight + screenPosition.Y;
        //            imagePosition.Y = screen.BoundsInImage.Y + (screenPosition.Y - screen.BoundsInSurface.Y);

        //            for (screenPosition.X = screen.BoundsInSurface.X; screenPosition.X <
        //                screen.BoundsInSurface.X + screen.BoundsInSurface.Width; screenPosition.X++)
        //            {
        //                // Positions in wallpaper (destination)
        //                wallpaperPosition.X = screenPosition.X >= 0 ?
        //                    screenPosition.X : wallpaperWidth + screenPosition.X;
        //                // Position in image (source) = BoundsInImage + offset
        //                imagePosition.X = screen.BoundsInImage.X + (screenPosition.X - screen.BoundsInSurface.X);

        //                PixelData imagePixel = *(PixelData*)(imageYPosition + imagePosition.X * sizeof(PixelData));
        //                PixelData* wallpaperPixel = (PixelData*)(wallpaperDataOffset + wallpaperPosition.Y * wallpaperData.Stride +
        //                    wallpaperPosition.X * sizeof(PixelData));
        //                *wallpaperPixel = imagePixel;
        //            }
        //        }
        //    }

        //    image.UnlockBits(imageData);
        //    wallpaper.UnlockBits(wallpaperData);
        //}

        private void DrawScreenRectangle(Bitmap wallpaper, Bitmap image, Screen screen)
        {
            System.Drawing.Rectangle destination = screen.BoundsInSurface;
            destination.X = destination.X >= 0 ? destination.X : wallpaper.Width + destination.X;
            destination.Y = destination.Y >= 0 ? destination.Y : wallpaper.Height + destination.Y;

            using (Graphics graphics = Graphics.FromImage(wallpaper))
            {
                graphics.DrawImage(image, destination, screen.BoundsInImage, GraphicsUnit.Pixel);
            }
        }


    }
}
