using System;
using System.Collections.Generic;
using System.Text;

using GalaSoft.MvvmLight;
using MusicLibrary.Lib;

namespace DanceDj.Mvvm.ViewModel
{
    public class FilterViewModel : ViewModelBase
    {
        public FilterViewModel(TrackDanceFilter filter) {
            Filter = filter;
        }

        internal TrackDanceFilter Filter { get; }

        public string Name => Filter.Name;
        public string Description => Filter.ToString();

        public DanceDifficulty Difficulty {
            get => Filter.Difficulty;
            set {
                if (Filter.Difficulty != value) {
                    Filter.Difficulty = value;
                    RaisePropertyChanged("Difficulty");
                }
            }
        }

        public DanceReviewStatusFlags ReviewStatus {
            get => Filter.ReviewStatus;
            set {
                if (Filter.ReviewStatus != value) {
                    Filter.ReviewStatus = value;
                    RaisePropertyChanged("ReviewStatus");
                }
            }
        }

        private bool _showDetails = false;
        public bool ShowDetails {
            get => _showDetails;
            set => Set<bool>(ref _showDetails, value, "ShowDetails");
        }
    }
}
