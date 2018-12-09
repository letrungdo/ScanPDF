using Prism.Navigation;
using Prism.Services;

namespace ScanPDF.ViewModels
{
    public class PdfPageViewModel : ViewModelBase
    {
        private string webViewSource;
        public string WebViewSource
        {
            get { return webViewSource; }
            set { SetProperty(ref webViewSource, value); }
        }

        public PdfPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
        }

        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            base.OnNavigatingTo(parameters);
            WebViewSource = parameters.GetValue<string>("Source");

        }
    }
}
