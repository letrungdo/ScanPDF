using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using ScanPDF.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ScanPDF.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public ICommand SFCommand { get; set; }
        public ICommand NextPageCommand { get; set; }

        private int TotalPage = 20; // TODO used input
        public MainPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
            Title = Resources.TitlePDFScan;
            TextSF = Resources.Start;
            SFCommand = new DelegateCommand(async () => await HandleActionStartFinish());
            NextPageCommand = new DelegateCommand(async () => await HandleNextPage());
            ListImagePage = new List<ImageItem>();
            for (int i = 0; i < TotalPage; i++)
            {
                ListImagePage.Add(new ImageItem
                {
                    ImageSource = "Default"
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
            if (TextSF.Equals(Resources.Start))
            {
                // Start
                TextSF = Resources.Finish;
                await TaskPhoto();
                IsVisibleNextPage = true;
            }
            else
            {
                // Finish

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
                    await IPageDialogService.DisplayAlertAsync("No Camera", ":( No camera avaialble.", "OK");
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
                await IPageDialogService.DisplayAlertAsync("Permissions Denied", "Unable to take photos.", "OK");
                //On iOS you may want to send your user to the settings screen.
                //CrossPermissions.Current.OpenAppSettings();
            }
        }

    }
}