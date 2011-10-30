using System;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing.Imaging;

namespace Adhesive.Application
{
    public static class BitmapConversion
    {
        public static Image ToWinFormsBitmap(this BitmapSource bitmapSource)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(stream);

                using (Bitmap bitmap = new Bitmap(stream))
                {
                    // Return a copy of the Bitmap so we don't have
                    // to keep our own MemoryStream open
                    return new Bitmap(bitmap);
                }
            }
        }

        public static BitmapImage ToWpfBitmap(this Image bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Bmp);
                stream.Seek(0, SeekOrigin.Begin);

                BitmapImage result = new BitmapImage();
                result.BeginInit();
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();

                return result;
            }
        }

    }
}
