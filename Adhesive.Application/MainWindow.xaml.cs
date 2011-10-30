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
using System.ComponentModel;

namespace Adhesive.Application
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ScreenConfiguration ScreenConfiguration { get; private set; }

        private string imagePath;
        public string ImagePath
        { 
            get
            {
                return this.imagePath;
            }
            private set
            {
                this.imagePath = value;
                OnPropertyChanged("ImagePath");
            }
        }

        public MainWindow()
        {
            this.ScreenConfiguration = ScreenConfiguration.FromWindowsFormsScreens();

            this.InitializeComponent();
            this.wallpaperPreview.ScreenConfiguration = this.ScreenConfiguration;
            this.wallpaperPreview.ImageResizer = new Adhesive.Core.Resizing.CenteringImageResizer();

            this.AllowDrop = true;
            this.Drop += new DragEventHandler(MainWindow_Drop);
        }

        void MainWindow_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
                this.ImagePath = files[0]; // TODO: test file
            }
        }

        private void bezelCompensationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.ScreenConfiguration.BezelCompensation = (int)e.NewValue;
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
