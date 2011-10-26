using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Adhesive.Core
{
    public class FastBitmap : IDisposable
    {
        private static GraphicsUnit graphicsUnit = GraphicsUnit.Pixel;
        private BitmapData data;

        public Bitmap Bitmap { get; set; }

        public FastBitmap(Bitmap bitmap) :
            this(bitmap, System.Drawing.Rectangle.Round(bitmap.GetBounds(ref graphicsUnit)))
        { }

        public FastBitmap(Bitmap bitmap, Rectangle bounds)
        {
            this.Bitmap = bitmap;
            this.LockBounds(bounds);
        }

        private void LockBounds(Rectangle bounds)
        {
            this.data = this.Bitmap.LockBits(bounds, ImageLockMode.ReadWrite, 
                this.Bitmap.PixelFormat);
        }

        public struct Pixel
        {
            public byte blue;
            public byte green;
            public byte red;
            public byte alpha;
        }

        public unsafe Pixel GetPixel(int x, int y)
        {
            Pixel* pixel = this.GetPixelPointer(x, y);
            return *pixel;
        }

        private unsafe Pixel* GetPixelPointer(int x, int y)
        {
            return (Pixel*)(this.data.Scan0 + y * this.data.Stride + x * sizeof(Pixel));
        }

        public unsafe void SetPixel(int x, int y, Pixel value)
        {
            Pixel* destination = this.GetPixelPointer(x, y);
            this.SetPixel(destination, value);
        }

        public unsafe void SetPixel(Pixel* destination, Pixel value)
        {
            *destination = value;
        }

        public void Dispose()
        {
            this.UnlockBounds();
        }

        private void UnlockBounds()
        {
            this.Bitmap.UnlockBits(this.data);
        }
    }
}
