using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ScanPDF.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public ICommand SFCommand { get; set; }
        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = Resources.TitlePDFScan;
            TextSF = Resources.Start;
            SFCommand = new DelegateCommand(HandleActionStartFinish);
        }

        private string textSF;
        public string TextSF
        {
            get { return textSF; }
            set { SetProperty(ref textSF, value); }
        }

        void HandleActionStartFinish()
        {

        }

    }
}