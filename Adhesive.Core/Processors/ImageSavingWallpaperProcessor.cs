using System;
using System.Drawing;
using System.IO;

namespace Adhesive.Core.Processors
{
    public class ImageSavingWallpaperProcessor : IWallpaperProcessor
    {
        private string directoryPath;

        public string DirectoryPath
        {
            get { return this.directoryPath; }
            set 
            {
                if (Directory.Exists(value))
                {
                    this.directoryPath = value;
                } 
                else 
                {
                    throw new ArgumentException("Supplied path is not a valid directory");
                }
            }
        }

        public ImageSavingWallpaperProcessor(string path)
        {
            this.DirectoryPath = path;
        }

        public void Process(Image wallpaper)
        {
            wallpaper.Save(this.DirectoryPath + "\\" + this.GenerateFileName());
        }

        private string GenerateFileName()
        {
            return DateTime.Now.Ticks.ToString() + ".jpg";
        }

    }
}
