using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Linq;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MusicLibrary.Lib;

namespace DanceDj.ViewModel
{
    public class FilterViewModel : ViewModelBase
    {
        public FilterViewModel(TrackDanceFilter filter) {
            Filter = filter;
            filter.PropertyChanged += Filter_PropertyChanged;

            Dances = new FilterDancesViewModel(filter);
            Categories = new FilterCategoriesViewModel(filter);
        }

        private void Filter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            RaisePropertyChanged(e.PropertyName);
            switch (e.PropertyName) {
                case "MinRating":
                    RaisePropertyChanged("ValidFiveStarMaxRatings");
                    break;
                case "MaxRating":
                    RaisePropertyChanged("ValidFiveStarMinRatings");
                    break;
            }
        }

        internal TrackDanceFilter Filter { get; }

        public string Name => Filter.Name;
        public string Description => Filter.Description;

        public DanceDifficulty Difficulty { get => Filter.Difficulty; set => Filter.Difficulty = value; }
        public DanceReviewStatusFlags ReviewStatus { get => Filter.ReviewStatus; set => Filter.ReviewStatus = value; }

        #region Categories

        internal class FilterCategoriesViewModel : Utils.IncludeExcludeViewModelBase<DanceCategory> {
            public FilterCategoriesViewModel(TrackDanceFilter filter)
                : base(GetCategoriesForDances(filter), Dance.EnumerateCategories(filter.Categories)) {
                Filter = filter;
                Filter.PropertyChanged += FilterPropertyChangedHandler;
                Filter.Dances.CollectionChanged += (o, e) => UpdateAllowed(GetCategoriesForDances(Filter));
            }

            private void FilterPropertyChangedHandler(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
                switch (e.PropertyName) {
                    case "Categories":
                        UpdateIncluded(Dance.EnumerateCategories(Filter.Categories));
                        break;
                    case "Dances":
                        UpdateAllowed(GetCategoriesForDances(Filter));
                        break;
                }
            }

            protected TrackDanceFilter Filter { get; }

            protected override void InnerAdd(DanceCategory category) {
                Filter.Categories |= Dance.ToCategoriesMask(category);
            }

            protected override void InnerRemove(DanceCategory category) {
                Filter.Categories &= ~Dance.ToCategoriesMask(category);
            }

            protected static IEnumerable<DanceCategory> GetCategoriesForDances(TrackDanceFilter filter) {
                var possible = (filter.Dances.Count > 0 ? DanceCategories.None : DanceCategories.Any);
                foreach (var dance in filter.Dances) {
                    possible |= dance.Categories;
                }
                return Dance.EnumerateCategories(possible);
            }
        }

        public Utils.IIncludeExcludeViewModelBase<DanceCategory> Categories { get; }
        public DanceCategories CategoriesMask { get => Filter.Categories; set => Filter.Categories = value; }

        #endregion

        #region Dances

        internal class FilterDancesViewModel : Utils.IncludeExcludeViewModelBase<DanceViewModel> {
            public FilterDancesViewModel(TrackDanceFilter filter) : base(DanceViewModel.GetAllDances(), filter.Dances.ToViewModel()) {
                Filter = filter;
                Filter.Dances.CollectionChanged += (o,e) => Update();
                filter.PropertyChanged += FilterPropertyChanged;
            }

            private void Update() {
                UpdateIncluded(Filter.Dances.ToViewModel());
            }

            private void FilterPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
                if (e.PropertyName == "Dances") {
                    Update();
                }
            }

            protected override void InnerAdd(DanceViewModel item) {
                Filter.Dances.Add(item.Dance);
            }

            protected override void InnerRemove(DanceViewModel item) {
                Filter.Dances.Remove(item.Dance);
            }

            private TrackDanceFilter Filter { get; }
        }

        public Utils.IIncludeExcludeViewModelBase<DanceViewModel> Dances { get; }
        public bool IncludeTracksWithNoDances { get => Filter.IncludeTracksWithNoDances; set => Filter.IncludeTracksWithNoDances = value; }

        #endregion

        public int MinBpm { get => Filter.MinBpm; set => Filter.MinBpm = value; }
        public int MaxBpm { get => Filter.MaxBpm; set => Filter.MaxBpm = value; }
        public bool IncludeTracksWithNoBpm { get => Filter.IncludeTracksWithNoBpm; set => Filter.IncludeTracksWithNoBpm = value; }

        public static readonly KeyValuePair<string,double>[] ValidFiveStarRatings = {
            new KeyValuePair<string,double>("0", 0.0),
            new KeyValuePair<string,double>("0.5", 0.5),
            new KeyValuePair<string, double>("1", 1.0),
            new KeyValuePair<string, double>("1.5", 1.5),
            new KeyValuePair<string, double>("2", 2.0),
            new KeyValuePair<string, double>("2.5", 2.5),
            new KeyValuePair<string, double>("3", 3.0),
            new KeyValuePair<string, double>("3.5", 3.5),
            new KeyValuePair<string, double>("4", 4.0),
            new KeyValuePair<string, double>("4.5", 4.5),
            new KeyValuePair<string, double>("5", 5.0)
        };

        public uint MinRating { get => Filter.MinRating; set => Filter.MinRating = value; }
        public uint MaxRating { get => Filter.MaxRating; set => Filter.MaxRating = value; }
        public double MinRatingFiveStars { get => Filter.MinRatingFiveStars; set => Filter.MinRatingFiveStars = value; }
        public double MaxRatingFiveStars { get => Filter.MaxRatingFiveStars; set => Filter.MaxRatingFiveStars = value; }
        public bool IncludeTracksWithNoRating { get => Filter.IncludeTracksWithNoRating; set => Filter.IncludeTracksWithNoRating = value; }

        public ICollection<KeyValuePair<string,double>> ValidFiveStarMinRatings {
            get {
                if (MaxRating == TrackRating.MaxFiveStarRating) {
                    return ValidFiveStarRatings;
                }
                return ValidFiveStarRatings.Where((kvp) => kvp.Value <= MaxRating).ToArray();
            }
        }

        public ICollection<KeyValuePair<string, double>> ValidFiveStarMaxRatings {
            get {
                if (MinRating == TrackRating.MinFiveStarRating) {
                    return ValidFiveStarRatings;
                }
                return ValidFiveStarRatings.Where((kvp) => kvp.Value >= MinRating).ToArray();
            }
        }

        private bool _showDetails = false;
        public bool ShowDetails {
            get => _showDetails;
            set => Set<bool>(ref _showDetails, value, "ShowDetails");
        }
    }
}
