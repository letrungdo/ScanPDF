using System;
using Prism.Mvvm;

namespace ScanPDF.Model
{
    public class ImageItem : BindableBase
    {
        private string imageSource;
        public string ImageSource
        {
            get { return imageSource; }
            set { SetProperty(ref imageSource, value); }
        }
    }
}
