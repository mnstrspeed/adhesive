using System;
using System.Drawing;

namespace Adhesive.Core.Providers
{
    public class StaticImageProvider : IImageProvider
    {
        public string Path { get; set; }

        public StaticImageProvider(string path)
        {
            this.Path = path;
        }

        public Image GetImage()
        {
            return Image.FromFile(this.Path);
        }
    }

}
