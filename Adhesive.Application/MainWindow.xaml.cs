using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Adhesive.Core;
using System.Threading;

namespace Adhesive.Application
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ScreenConfiguration ScreenConfiguration { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            this.ScreenConfiguration = new ScreenConfiguration(
                System.Windows.Forms.Screen.AllScreens.Select(s => new Adhesive.Core.Screen(s)).ToArray(), 120);
            wallpaperPreview.ScreenConfiguration = this.ScreenConfiguration;
            wallpaperPreview.ImageResizer = new Adhesive.Core.Resizing.FillingImageResizer();
        }

        private void bezelCompensationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.ScreenConfiguration = new ScreenConfiguration(System.Windows.Forms.Screen.AllScreens.Select(
                s => new Adhesive.Core.Screen(s)).ToArray(), (int)e.NewValue);
            this.wallpaperPreview.ScreenConfiguration = this.ScreenConfiguration;
            this.wallpaperPreview.InvalidateVisual();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                WallpaperComposerParameters parameters = new WallpaperComposerParameters();
                parameters.ImageProvider = new Adhesive.Core.Providers.StaticImageProvider(this.wallpaperPreview.ImagePath);
                parameters.ImageResizer = this.wallpaperPreview.ImageResizer;
                parameters.WallpaperProcessor = new Adhesive.Core.Processors.ApplyingWallpaperProcessor();

                WallpaperComposer composer = new WallpaperComposer(this.ScreenConfiguration);
                composer.Compose(parameters);

            }).Start();
        }
    }
}
