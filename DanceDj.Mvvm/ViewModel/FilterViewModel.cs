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
            filter.PropertyChanged += Filter_PropertyChanged;
        }

        private void Filter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            RaisePropertyChanged(e.PropertyName);
        }

        internal TrackDanceFilter Filter { get; }

        public string Name => Filter.Name;
        public string Description => Filter.Description;

        public DanceDifficulty Difficulty { get => Filter.Difficulty; set => Filter.Difficulty = value; }
        public DanceReviewStatusFlags ReviewStatus { get => Filter.ReviewStatus; set => Filter.ReviewStatus = value; }
        public DanceCategories Categories { get => Filter.Categories; set => Filter.Categories = value; }
        public int? MinBpm { get => Filter.MinBpm; set => Filter.MinBpm = value; }
        public int? MaxBpm { get => Filter.MaxBpm; set => Filter.MaxBpm = value; }
        public uint? MinRating { get => Filter.MinRating; set => Filter.MinRating = value; }
        public uint? MaxRating { get => Filter.MaxRating; set => Filter.MaxRating = value; }
        public double? MinRatingFiveStars { get => Filter.MinRatingFiveStars; set => Filter.MinRatingFiveStars = value; }
        public double? MaxRatingFiveStars { get => Filter.MaxRatingFiveStars; set => Filter.MaxRatingFiveStars = value; }

        public bool IncludeTracksWithNoDances {
            get => (Filter.Options & DanceFilterFlags.IncludeTracksWithNoDances) == DanceFilterFlags.IncludeTracksWithNoDances;
            set => Filter.Options |= DanceFilterFlags.IncludeTracksWithNoDances;
        }

        public bool IncludeTracksWithNoBpm {
            get => (Filter.Options & DanceFilterFlags.IncludeTracksWithNoBpm) == DanceFilterFlags.IncludeTracksWithNoBpm;
            set => Filter.Options |= DanceFilterFlags.IncludeTracksWithNoBpm;
        }

        public bool IncludeTracksWithNoRating {
            get => (Filter.Options & DanceFilterFlags.IncludeTracksWithNoRating) == DanceFilterFlags.IncludeTracksWithNoRating;
            set => Filter.Options |= DanceFilterFlags.IncludeTracksWithNoRating;
        }

        private bool _showDetails = false;
        public bool ShowDetails {
            get => _showDetails;
            set => Set<bool>(ref _showDetails, value, "ShowDetails");
        }
    }
}
