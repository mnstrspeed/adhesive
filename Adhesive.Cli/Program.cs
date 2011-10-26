using System;
using System.Linq;
using Adhesive.Core;
using Adhesive.Core.Providers;
using Adhesive.Core.Resizing;
using Adhesive.Core.Processors;

namespace Adhesive.Cli
{
    internal class Program
    {
        private const string imagePath = @"C:\Users\Tom\Desktop\trifecta_by_colecovizion-d49aijz.jpg";

        public void Run()
        {
            ScreenConfiguration configuration = new ScreenConfiguration(
                System.Windows.Forms.Screen.AllScreens.Select(s => new Screen(s)).ToArray(), -300); //120
            WallpaperComposer composer = new WallpaperComposer(configuration);

            WallpaperComposerParameters parameters = new WallpaperComposerParameters();
            parameters.ImageProvider = new StaticImageProvider(imagePath);
            parameters.ImageResizer = new FillingImageResizer();
            parameters.WallpaperProcessor = new ApplyingWallpaperProcessor();

            composer.Compose(parameters);
        }

        public static void Main()
        {
            new Program().Run();

            // Block until the user confirms
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
