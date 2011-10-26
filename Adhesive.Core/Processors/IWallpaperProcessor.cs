using System;
using System.Drawing;

namespace Adhesive.Core.Processors
{
    public interface IWallpaperProcessor
    {
        void Process(Image wallpaper);
    }
}
