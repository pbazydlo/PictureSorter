using GalaSoft.MvvmLight;
using System;
using System.Collections.ObjectModel;
using System.Linq;
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
        }

        private enum FallbackDatesEnum
        {
            FileCreated,
            LastModified
        }
    }
}