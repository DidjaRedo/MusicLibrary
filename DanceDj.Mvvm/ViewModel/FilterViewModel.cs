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

            var danceVms = filter.Dances.Select<Dance, DanceViewModel>((d) => DanceViewModel.GetOrAdd(d));
            _includedDances = new ObservableCollection<DanceViewModel>(danceVms);
            IncludedDances = new ReadOnlyObservableCollection<DanceViewModel>(_includedDances);
            SelectedIncludedDance = (IncludedDances.Count > 0) ? IncludedDances[0] : null;

            _excludedDances = new ObservableCollection<DanceViewModel>(DanceViewModel.ComputeMissingDances(_includedDances));
            ExcludedDances = new ReadOnlyObservableCollection<DanceViewModel>(_excludedDances);
            SelectedExcludedDance = (ExcludedDances.Count > 0) ? ExcludedDances[0] : null;
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
        public DanceCategories Categories { get => Filter.Categories; set => Filter.Categories = value; }

        public bool IncludeTracksWithNoDances { get => Filter.IncludeTracksWithNoDances; set => Filter.IncludeTracksWithNoDances = value; }

        #region Included Dances

        private ObservableCollection<DanceViewModel> _includedDances;
        private DanceViewModel _selectedIncludedDance;
  
        public ReadOnlyObservableCollection<DanceViewModel> IncludedDances { get; }
        public DanceViewModel SelectedIncludedDance { get => _selectedIncludedDance; set => Set("SelectedIncludedDance", ref _selectedIncludedDance, value); }

        private RelayCommand<DanceViewModel> _removeDanceCommand;
        public RelayCommand<DanceViewModel> RemoveDanceCommand {
            get => _removeDanceCommand ?? (_removeDanceCommand = new RelayCommand<DanceViewModel>(_RemoveDance, _CanRemoveDance));
        }

        private void _RemoveDance(DanceViewModel dance) {
            dance = dance ?? SelectedIncludedDance;
            if (!_CanRemoveDance(dance)) {
                throw new ApplicationException($"Cannot remove {dance.Name} from {Name} filter.");
            }
            Filter.Dances.Remove(dance.Dance);
            _excludedDances.Add(dance);
            _includedDances.Remove(dance);
            SelectedIncludedDance = (IncludedDances.Count > 0 ? IncludedDances[0] : null);
            SelectedExcludedDance = SelectedExcludedDance ?? dance;
        }

        private bool _CanRemoveDance(DanceViewModel dance) {
            dance = dance ?? SelectedIncludedDance;
            return (IncludedDances.Count > 0) && ((dance != null) && IncludedDances.Contains(dance));
        }

        #endregion

        #region Excluded Dances

        private ObservableCollection<DanceViewModel> _excludedDances;
        private DanceViewModel _selectedExcludedDance;
        public ReadOnlyObservableCollection<DanceViewModel> ExcludedDances { get; }
        public DanceViewModel SelectedExcludedDance { get => _selectedExcludedDance; set => Set("SelectedExcludedDance", ref _selectedExcludedDance, value); }

        private RelayCommand<DanceViewModel> _addDanceCommand;
        public RelayCommand<DanceViewModel> AddDanceCommand {
            get => _addDanceCommand ?? (_addDanceCommand = new RelayCommand<DanceViewModel>(_AddDance, _CanAddDance));
        }

        private void _AddDance(DanceViewModel dance) {
            dance = dance ?? SelectedExcludedDance;
            if (!_CanAddDance(dance)) {
                throw new ApplicationException($"Cannot add {dance.Name} to {Name} filter.");
            }
            Filter.Dances.Add(dance.Dance);
            _includedDances.Add(dance);
            _excludedDances.Remove(dance);
            SelectedExcludedDance = (ExcludedDances.Count > 0 ? ExcludedDances[0] : null);
            SelectedIncludedDance = SelectedIncludedDance ?? dance;
        }

        private bool _CanAddDance(DanceViewModel dance) {
            dance = dance ?? SelectedExcludedDance;
            return (ExcludedDances.Count > 0) && ((dance == null) || ExcludedDances.Contains(dance));
        }

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
