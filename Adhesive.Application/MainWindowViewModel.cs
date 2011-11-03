using System;
using System.Collections.Generic;
using System.ComponentModel;
using Adhesive.Core;
using Adhesive.Core.Resizing;
using System.Windows;

namespace Adhesive.Application
{
    public enum ResizingType
    {
        Fitting,
        Filling,
        Stretching,
        Centering
    }

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private ScreenConfiguration screenConfiguration;
        public ScreenConfiguration ScreenConfiguration
        {
            get { return this.screenConfiguration; }
            set 
            {
                if (this.screenConfiguration != null)
                {
                    this.screenConfiguration.PropertyChanged -= new PropertyChangedEventHandler(
                        screenConfiguration_PropertyChanged);
                }
                this.screenConfiguration = value;
                this.screenConfiguration.PropertyChanged += new PropertyChangedEventHandler(
                    screenConfiguration_PropertyChanged);
            }
        }

        private string imagePath;
        public string ImagePath
        {
            get { return this.imagePath; }
            set
            {
                this.imagePath = value;
                this.OnPropertyChanged("ImagePath");
            }
        }

        private IImageResizer imageResizer;
        public IImageResizer ImageResizer
        {
            get { return this.imageResizer; }
            set
            {
                this.imageResizer = value;
                this.OnPropertyChanged("ImageResizer");
            }
        }

        private ResizingType resizingType;
        public ResizingType ResizingType
        {
            get { return this.resizingType; }
            set
            {
                this.resizingType = value;
                this.SelectImageResizer(this.resizingType);
            }
        }

        public Dictionary<ResizingType, string> ResizingTypes
        { 
            get; 
            set; 
        }

        public MainWindowViewModel(ScreenConfiguration screenConfiguration, 
            ResizingType resizingType)
        {
            this.ScreenConfiguration = screenConfiguration;
            this.ResizingType = resizingType;

            this.ResizingTypes = new Dictionary<ResizingType, string>()
            {
                { ResizingType.Filling, "Fill" },
                { ResizingType.Fitting, "Fit" },
                { ResizingType.Stretching, "Stretch" },
                { ResizingType.Centering, "Center" }
            };
        }

        public void screenConfiguration_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged("ScreenConfiguration");
        }

        private void SelectImageResizer(Application.ResizingType resizingType)
        {
            switch (resizingType)
            {
                case Application.ResizingType.Filling:
                    this.ImageResizer = new FillingImageResizer();
                    break;
                case Application.ResizingType.Fitting:
                    this.ImageResizer = new FittingImageResizer();
                    break;
                case Application.ResizingType.Stretching:
                    this.ImageResizer = new StretchingImageResizer();
                    break;
                case Application.ResizingType.Centering:
                    this.ImageResizer = new CenteringImageResizer();
                    break;
                default:
                    MessageBox.Show("Unsupported resizing type");
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
