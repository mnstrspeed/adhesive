using System;
using System.Drawing;
using Microsoft.Win32;
using System.IO;
using System.Runtime.InteropServices;

namespace Adhesive.Core.Processors
{
    public class ApplyingWallpaperProcessor : IWallpaperProcessor
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinini);

        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        public void Process(Image wallpaper)
        {
            try
            {
                DirectoryInfo appData = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                DirectoryInfo applicationDirectory = appData.CreateSubdirectory("Adhesive");

                string wallpaperPath = applicationDirectory.FullName + "\\wallpaper.jpg";
                wallpaper.Save(wallpaperPath, System.Drawing.Imaging.ImageFormat.Bmp);

                RegistryKey desktop = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", true);
                desktop.SetValue("TileWallpaper", "1");
                desktop.SetValue("WallpaperStyle", "0");

                if (SystemParametersInfo(SPI_SETDESKWALLPAPER, 0,
                    wallpaperPath, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE) == 0)
                {
                    throw new Exception("SystemParametersInfo failed");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to set wallpaper: " + ex.Message);
            }
        }
    }
}
