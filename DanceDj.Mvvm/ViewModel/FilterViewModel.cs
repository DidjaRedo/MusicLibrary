using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Linq;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MusicLibrary.Lib;

namespace DanceDj.Mvvm.ViewModel
{
    public class FilterViewModel : ViewModelBase
    {
        public FilterViewModel(TrackDanceFilter filter) {
            Filter = filter;
            filter.PropertyChanged += Filter_PropertyChanged;

            Dances = new FilterDancesViewModel(filter);
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
                case "Categories":
                    _categoryList = null;
                    RefreshCategories();
                    break;
            }
        }

        internal TrackDanceFilter Filter { get; }

        public string Name => Filter.Name;
        public string Description => Filter.Description;

        public DanceDifficulty Difficulty { get => Filter.Difficulty; set => Filter.Difficulty = value; }
        public DanceReviewStatusFlags ReviewStatus { get => Filter.ReviewStatus; set => Filter.ReviewStatus = value; }

        #region Categories

        public DanceCategories Categories { get => Filter.Categories; set => Filter.Categories = value; }

        private DanceCategory[] _categoryList;
        public DanceCategory[] CategoryList {
            get => _categoryList ?? (_categoryList = Dance.EnumerateCategories(Categories).ToArray());
        }

        private DanceCategories GetPossibleCategories() {
            var possible = DanceCategories.None;
            foreach (var dance in Dances.Included) {
                possible |= dance.CategoriesMask;
            }
            return possible;
        }

        private DanceCategories GetEligibleCategories() {
            return GetPossibleCategories() & ~Categories;
        }

        private DanceCategories? _eligibleCategories = DanceCategories.None;
        public DanceCategories EligibleCategories {
            get => (_eligibleCategories.HasValue ? _eligibleCategories : (_eligibleCategories = GetEligibleCategories())).Value;
        }

        private DanceCategory[] _eligibleCategoryList;
        public DanceCategory[] EligibleCategoryList {
            get => _eligibleCategoryList ?? (_eligibleCategoryList = Dance.EnumerateCategories(EligibleCategories).ToArray());
        }

        private void RefreshCategories() {
            _categoryList = null;
            RaisePropertyChanged("CategoryList");
            var eligible = GetEligibleCategories();
            if (!_eligibleCategories.HasValue || (eligible != _eligibleCategories.Value)) {
                _eligibleCategories = eligible;
                _eligibleCategoryList = null;
                RaisePropertyChanged("EligibleCategories");
                RaisePropertyChanged("EligibleCategoryList");
            }
        }

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
