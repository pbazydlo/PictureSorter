using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace PictureSorter.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly string[] SupportedExtensions = new string[] { ".png", ".jpg", ".bmp", ".gif" };

        private string selectedFallbackDate;
        public string SelectedFallbackDate
        {
            get { return this.selectedFallbackDate; }
            set
            {
                if (value != this.selectedFallbackDate)
                {
                    this.selectedFallbackDate = value;
                    this.RaisePropertyChanged(() => this.SelectedFallbackDate);
                }
            }
        }

        private FallbackDatesEnum selectedFallbackDateAsEnum
        {
            get
            {
                return (FallbackDatesEnum)Enum.Parse(typeof(FallbackDatesEnum), this.selectedFallbackDate);
            }
        }

        private ObservableCollection<string> fallbackDates;
        public ObservableCollection<string> FallbackDates
        {
            get { return this.fallbackDates; }
            set
            {
                if (value != this.fallbackDates)
                {
                    this.fallbackDates = value;
                    this.RaisePropertyChanged(() => this.FallbackDates);
                }
            }
        }

        private Visibility progressBarVisibility;
        public Visibility ProgressBarVisibility
        {
            get { return this.progressBarVisibility; }
            set
            {
                if (value != this.progressBarVisibility)
                {
                    this.progressBarVisibility = value;
                    this.RaisePropertyChanged(() => this.ProgressBarVisibility);
                }
            }
        }

        private string sourceFolderName;
        public string SourceFolderName
        {
            get { return this.sourceFolderName; }
            set
            {
                if (value != this.sourceFolderName)
                {
                    this.sourceFolderName = value;
                    this.RaisePropertyChanged(() => this.SourceFolderName);
                }
            }
        }

        private bool isSourceFolderPicked;
        public bool IsSourceFolderPicked
        {
            get { return this.isSourceFolderPicked; }
            set
            {
                if (value != this.isSourceFolderPicked)
                {
                    this.isSourceFolderPicked = value;
                    this.RaisePropertyChanged(() => this.IsSourceFolderPicked);
                }
            }
        }

        private string destinationFolderName;
        public string DestinationFolderName
        {
            get { return this.destinationFolderName; }
            set
            {
                if (value != this.destinationFolderName)
                {
                    this.destinationFolderName = value;
                    this.RaisePropertyChanged(() => this.DestinationFolderName);
                }
            }
        }

        private bool isDestinationFolderPicked;
        public bool IsDestinationFolderPicked
        {
            get { return this.isDestinationFolderPicked; }
            set
            {
                if (value != this.isDestinationFolderPicked)
                {
                    this.isDestinationFolderPicked = value;
                    this.RaisePropertyChanged(() => this.IsDestinationFolderPicked);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            var fallbackDatesValues = Enum.GetNames(typeof(FallbackDatesEnum));
            this.fallbackDates = new ObservableCollection<string>(fallbackDatesValues);
            this.selectedFallbackDate = this.fallbackDates.First();
            this.progressBarVisibility = Visibility.Collapsed;
            this.sourceFolderName = string.Empty;
            this.isSourceFolderPicked = false;
            this.destinationFolderName = string.Empty;
            this.isDestinationFolderPicked = false;
        }

        public ICommand PickSourceFolderCommand
        {
            get { return new RelayCommand(() => this.PickSourceFolder()); }
        }

        private async void PickSourceFolder()
        {
            if (await this.EnsureUnsnapped())
            {
                FolderPicker folderPicker = new FolderPicker();
                folderPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                foreach (var supportedExtension in SupportedExtensions)
                {
                    folderPicker.FileTypeFilter.Add(supportedExtension);
                }

                StorageFolder folder = await folderPicker.PickSingleFolderAsync();
                if (folder != null)
                {
                    // Application now has read/write access to all contents in the picked folder (including other sub-folder contents)
                    StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedSourceFolder", folder);
                    this.SourceFolderName = folder.DisplayName;
                    this.IsSourceFolderPicked = true;
                    //OutputTextBlock.Text = "Picked folder: " + folder.Name;
                }
                else
                {
                    //OutputTextBlock.Text = "Operation cancelled.";
                }
            }
        }

        public ICommand PickDestinationFolderCommand
        {
            get { return new RelayCommand(() => this.PickDestinationFolder()); }
        }

        private async void PickDestinationFolder()
        {
            if (await this.EnsureUnsnapped())
            {
                FolderPicker folderPicker = new FolderPicker();
                folderPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                foreach (var supportedExtension in SupportedExtensions)
                {
                    folderPicker.FileTypeFilter.Add(supportedExtension);
                }

                StorageFolder folder = await folderPicker.PickSingleFolderAsync();
                if (folder != null)
                {
                    // Application now has read/write access to all contents in the picked folder (including other sub-folder contents)
                    StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedDestinationFolder", folder);
                    this.DestinationFolderName = folder.DisplayName;
                    this.IsDestinationFolderPicked = true;
                    //OutputTextBlock.Text = "Picked folder: " + folder.Name;
                }
                else
                {
                    //OutputTextBlock.Text = "Operation cancelled.";
                }
            }
        }

        internal async Task<bool> EnsureUnsnapped()
        {
            // FilePicker APIs will not work if the application is in a snapped state.
            // If an app wants to show a FilePicker while snapped, it must attempt to unsnap first
            bool unsnapped = ((ApplicationView.Value != ApplicationViewState.Snapped) || ApplicationView.TryUnsnap());
            if (!unsnapped)
            {
                await new Windows.UI.Popups.MessageDialog("Cannot unsnap the application.").ShowAsync();
            }

            return unsnapped;
        }

        private enum FallbackDatesEnum
        {
            FileCreated,
            LastModified
        }
    }
}