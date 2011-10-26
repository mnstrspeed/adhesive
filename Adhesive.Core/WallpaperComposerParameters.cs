using System;
using Adhesive.Core.Providers;
using Adhesive.Core.Resizing;
using Adhesive.Core.Processors;

namespace Adhesive.Core
{
    /// <summary>
    /// Contains a set of parameters to pass on to the WallpaperComposer
    /// </summary>
    public class WallpaperComposerParameters
    {
        /// <summary>
        /// The ImageProvider to be used by the WallpaperComposer
        /// </summary>
        public IImageProvider ImageProvider { get; set; }

        /// <summary>
        /// The ImageResizer to be used by the WallpaperComposer
        /// </summary>
        public IImageResizer ImageResizer { get; set; }

        /// <summary>
        /// The WallpaperProcessor to tbe used by the WallpaperComposer
        /// </summary>
        public IWallpaperProcessor WallpaperProcessor { get; set; }

        /// <summary>
        /// Initializes the WallpaperComposerParameters without any properties. Properties
        /// must be set manually later on before calling the WallpaperComposer
        /// </summary>
        public WallpaperComposerParameters()
        { }

        /// <summary>
        /// Initializes the WallpaperComposerParameters with a set of properties
        /// </summary>
        /// <param name="imageProvider">ImageProvider to be used by the WallpaperComposer</param>
        /// <param name="imageResizer">ImageResizer to be used by the WallpaperComposer</param>
        /// <param name="wallpaperProcessor">WallpaperProcessor to be used by the WallpaperComposer</param>
        public WallpaperComposerParameters(IImageProvider imageProvider,
            IImageResizer imageResizer, IWallpaperProcessor wallpaperProcessor)
        {
            this.ImageProvider = imageProvider;
            this.ImageResizer = imageResizer;
            this.WallpaperProcessor = wallpaperProcessor;
        }
        
    }
}
