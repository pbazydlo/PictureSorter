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

        private bool isSortCommandCanceled;

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

        private bool isProgressBarIndeterminate;
        public bool IsProgressBarIndeterminate
        {
            get { return this.isProgressBarIndeterminate; }
            set
            {
                if (value != this.isProgressBarIndeterminate)
                {
                    this.isProgressBarIndeterminate = value;
                    this.RaisePropertyChanged(() => this.IsProgressBarIndeterminate);
                }
            }
        }

        private int progressBarValue;
        public int ProgressBarValue
        {
            get { return this.progressBarValue; }
            set
            {
                if (value != this.progressBarValue)
                {
                    this.progressBarValue = value;
                    this.RaisePropertyChanged(() => this.ProgressBarValue);
                }
            }
        }

        private int progressBarMaxValue;
        public int ProgressBarMaxValue
        {
            get { return this.progressBarMaxValue; }
            set
            {
                if (value != this.progressBarMaxValue)
                {
                    this.progressBarMaxValue = value;
                    this.RaisePropertyChanged(() => this.ProgressBarMaxValue);
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
                    this.RaisePropertyChanged(() => this.SortCommand);
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
                    this.RaisePropertyChanged(() => this.SortCommand);
                }
            }
        }

        private bool isFallbackTypeSelectionEnabled;
        public bool IsFallbackTypeSelectionEnabled
        {
            get { return this.isFallbackTypeSelectionEnabled; }
            set
            {
                if (value != this.isFallbackTypeSelectionEnabled)
                {
                    this.isFallbackTypeSelectionEnabled = value;
                    this.RaisePropertyChanged(() => this.IsFallbackTypeSelectionEnabled);
                    this.RaisePropertyChanged(() => this.PickSourceFolderCommand);
                    this.RaisePropertyChanged(() => this.PickDestinationFolderCommand);
                    this.RaisePropertyChanged(() => this.SortCommand);
                    this.RaisePropertyChanged(() => this.CancelSortCommand);
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
            this.isFallbackTypeSelectionEnabled = true;
            this.isSortCommandCanceled = false;
        }

        public ICommand PickSourceFolderCommand
        {
            get { return new RelayCommand(() => this.PickSourceFolder(), () => this.IsFallbackTypeSelectionEnabled); }
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
            get { return new RelayCommand(() => this.PickDestinationFolder(), () => this.IsFallbackTypeSelectionEnabled); }
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

        public ICommand SortCommand
        {
            get
            {
                return new RelayCommand(() => this.Sort(), () => this.IsDestinationFolderPicked && this.IsSourceFolderPicked && this.IsFallbackTypeSelectionEnabled);
            }
        }

        private async void Sort()
        {
            this.isSortCommandCanceled = false;
            this.IsFallbackTypeSelectionEnabled = false;
            this.ProgressBarVisibility = Visibility.Visible;
            this.IsProgressBarIndeterminate = true;
            var sourceFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync("PickedSourceFolder");
            var destinationFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync("PickedDestinationFolder");
            var files = await sourceFolder.GetFilesAsync();
            this.ProgressBarValue = 0;
            this.ProgressBarMaxValue = files.Count;
            this.IsProgressBarIndeterminate = false;
            foreach (var file in files)
            {
                if (this.isSortCommandCanceled)
                {
                    break;
                }

                var fileProperties = await files[0].Properties.GetImagePropertiesAsync();
                string newName = "";
                DateTime chosenDate = file.DateCreated.LocalDateTime;
                if (fileProperties.DateTaken > new DateTime(1800, 01, 01))
                {
                    chosenDate = fileProperties.DateTaken.LocalDateTime;
                }
                else
                {
                    switch (this.selectedFallbackDateAsEnum)
                    {
                        case FallbackDatesEnum.FileCreated:
                            chosenDate = file.DateCreated.LocalDateTime;
                            break;
                        case FallbackDatesEnum.LastModified:
                            var basicProperties = await file.GetBasicPropertiesAsync();
                            chosenDate = basicProperties.DateModified.LocalDateTime;
                            break;
                    }
                }

                newName = string.Format("{0}_{1}", chosenDate.ToString("yyyyMMddHHmmssfff"), file.Name);

                this.ProgressBarValue++;
                await Task.Delay(500);
                //await file.CopyAsync(destinationFolder, "", NameCollisionOption.GenerateUniqueName);
            }

            this.ProgressBarVisibility = Visibility.Collapsed;
            this.IsFallbackTypeSelectionEnabled = true;
        }

        public ICommand CancelSortCommand
        {
            get
            {
                return new RelayCommand(() => this.isSortCommandCanceled = true, () => !this.IsFallbackTypeSelectionEnabled);
            }
        }

        private enum FallbackDatesEnum
        {
            FileCreated,
            LastModified
        }
    }
}