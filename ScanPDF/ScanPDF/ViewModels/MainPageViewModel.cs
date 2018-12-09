using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using ScanPDF.Model;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ScanPDF.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public ICommand SFCommand { get; set; }
        public ICommand NextPageCommand { get; set; }

        private int TotalPage = 20; // TODO used input

        private string pdfPath = "";

        public MainPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
            Title = AppResources.TitlePDFScan;
            TextSF = AppResources.Start;
            SFCommand = new DelegateCommand(async () => await HandleActionStartFinish());
            NextPageCommand = new DelegateCommand(async () => await HandleNextPage());
            ListImagePage = new List<ImageItem>();
            for (int i = 0; i < TotalPage; i++)
            {
                ListImagePage.Add(new ImageItem
                {
                    ImageSource = "default_pic"
                });
            }
        }

        #region ----- Binding -----
        private string textSF;
        public string TextSF
        {
            get { return textSF; }
            set { SetProperty(ref textSF, value); }
        }

        private List<ImageItem> listImagePage;
        public List<ImageItem> ListImagePage
        {
            get { return listImagePage; }
            set { SetProperty(ref listImagePage, value); }
        }

        private int currentPagePosition;
        public int CurrentPagePosition
        {
            get { return currentPagePosition; }
            set { SetProperty(ref currentPagePosition, value); }
        }

        private bool isVisibleNextPage = false;
        public bool IsVisibleNextPage
        {
            get { return isVisibleNextPage; }
            set { SetProperty(ref isVisibleNextPage, value); }
        }
        #endregion

        private async Task HandleActionStartFinish()
        {
            if (TextSF.Equals(AppResources.Start))
            {
                // Start
                TextSF = AppResources.Finish;
                await TaskPhoto();
                IsVisibleNextPage = true;
            }
            else
            {
                // Finish
                await CreatePDF();

                await NavigationService.NavigateAsync(nameof(Views.PdfPage),
                new NavigationParameters($"Source={pdfPath}"));
            }
        }

        private async Task HandleNextPage()
        {
            await TaskPhoto();
        }

        private async Task TaskPhoto()
        {
            var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);

            if (cameraStatus != PermissionStatus.Granted || storageStatus != PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera, Permission.Storage });
                cameraStatus = results[Permission.Camera];
                storageStatus = results[Permission.Storage];
            }

            if (cameraStatus == PermissionStatus.Granted && storageStatus == PermissionStatus.Granted)
            {
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await PageDialogService.DisplayAlertAsync("No Camera", ":( No camera avaialble.", "OK");
                    return;
                }

                var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    PhotoSize = PhotoSize.Full,
                    Directory = "ImageTemp",
                    Name = "img.png",
                });

                if (file == null)
                    return;
                ListImagePage[CurrentPagePosition].ImageSource = file.Path;


            }
            else
            {
                await PageDialogService.DisplayAlertAsync("Permissions Denied", "Unable to take photos.", "OK");
                //On iOS you may want to send your user to the settings screen.
                //CrossPermissions.Current.OpenAppSettings();
            }
        }

        private async Task CreatePDF()
        {
            // Create a new PDF document
            PdfDocument document = new PdfDocument();

            foreach (var item in ListImagePage)
            {
                if (!item.ImageSource.Contains("img"))
                    continue;
                //Add a page to the document
                PdfPage page = document.Pages.Add();

                //Create PDF graphics for the page
                PdfGraphics graphics = page.Graphics;

                Stream imageStream = File.OpenRead(item.ImageSource);

                //Load the image from the stream 
                PdfBitmap image = new PdfBitmap(imageStream);

                //Draw the image 
                graphics.DrawImage(image, 0, 0, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height);
            }

            MemoryStream stream = new MemoryStream();

            document.Save(stream);

            //Close the document
            document.Close(true);

            //Save the stream as a file in the device and invoke it for viewing
            pdfPath = Xamarin.Forms.DependencyService.Get<ISave>().Save(stream);

            await Task.Delay(100);
        }
    }
}