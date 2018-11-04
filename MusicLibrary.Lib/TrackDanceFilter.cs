using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;
using System.Collections.ObjectModel;

namespace MusicLibrary.Lib
{
    public class TrackDanceFilter : ObservableBase
    {
        public const int MaxAllowedBpm = 240;

        protected string _name;
        protected DanceCategories _categories = DanceCategories.None;
        protected int _minBpm = 0;
        protected int _maxBpm = MaxAllowedBpm;
        protected DanceDifficulty _difficulty = DanceDifficulty.Any;
        protected DanceReviewStatusFlags _reviewStatus = DanceReviewStatusFlags.AnyNotRejected;
        protected uint _minRating = 0;
        protected uint _maxRating = 255;
        protected bool _includeTracksWithNoRating = true;
        protected bool _includeTracksWithNoBpm = true;
        protected bool _includeTracksWithNoDances = false;
        protected DateTimeOffset? _lastPlayedBefore;
        protected DateTimeOffset? _lastPlayedAfter;
        protected string _description;

        public string Name { get => _name; set => Set(ref _name, value, "Name"); }
        public string Description { get => _description ?? ToString(); set => Set(ref _description, value, "Description"); }

        public ObservableCollection<Dance> Dances { get; } = new ObservableCollection<Dance>();

        public DanceCategories Categories { get => _categories; set => Set(ref _categories, value, "Categories"); }

        public int MinBpm { get => _minBpm; set => Set(ref _minBpm, value, "MinBpm"); }
        public int MaxBpm { get => _maxBpm; set => Set(ref _maxBpm, value, "MaxBpm"); }
        public DanceDifficulty Difficulty { get => _difficulty; set => Set(ref _difficulty, value, "Difficulty"); }
        public DanceReviewStatusFlags ReviewStatus { get => _reviewStatus; set => Set(ref _reviewStatus, value, "ReviewStatus"); }
        public uint MinRating { get => _minRating; set => Set(ref _minRating, value, "MinRating", "MinRatingFiveStars"); }
        public uint MaxRating { get => _maxRating; set => Set(ref _maxRating, value, "MaxRating", "MaxRatingFiveStars"); }

        public double MinRatingFiveStars {
            get => TrackRating.RawRatingToFiveStar(MinRating);
            set => MinRating = TrackRating.FiveStarRatingToRaw(value);
        }

        public double MaxRatingFiveStars {
            get => TrackRating.RawRatingToFiveStar(MaxRating);
            set => MaxRating = TrackRating.FiveStarRatingToRaw(value);
        }

        public bool IncludeTracksWithNoRating { get => _includeTracksWithNoRating; set => Set(ref _includeTracksWithNoRating, value, "IncludeTracksWithNoRating"); }
        public bool IncludeTracksWithNoBpm { get => _includeTracksWithNoBpm; set => Set(ref _includeTracksWithNoBpm, value, "IncludeTracksWithNoBpm"); }
        public bool IncludeTracksWithNoDances { get => _includeTracksWithNoDances; set => Set(ref _includeTracksWithNoDances, value, "IncludeTracksWithNoDances"); }
        public DateTimeOffset? LastPlayedBefore { get => _lastPlayedBefore; set => Set(ref _lastPlayedBefore, value, "LastPlayedBefore"); }
        public DateTimeOffset? LastPlayedAfter { get => _lastPlayedAfter; set => Set(ref _lastPlayedAfter, value, "LastPlayedAfter"); }

        public TrackDanceFilter(IEnumerable<Dance> dances) {
            if (dances != null) {
                foreach (var dance in dances) {
                    Dances.Add(dance);
                }
            }
        }

        public TrackDanceFilter(params Dance[] dances) : this((IEnumerable<Dance>)dances) {

        }

        public List<TrackDanceInfo> MatchingDances(ITrack track) {
            var matches = new List<TrackDanceInfo>();
            if (track.Dances.Dances.Count > 0) {
                foreach (var dance in track.Dances.Dances) {
                    bool match = (Dances.Count == 0) || Dances.Contains(dance.Dance);
                    // For category we need to match all of the filtered categories
                    // For difficulty we need to match any of the filtered difficulties
                    match = match && ((dance.Categories & Categories) == Categories);
                    match = match && ((dance.Difficulty & Difficulty) != 0);
                    match = match && ((TrackDanceInfo.ToStatusFlags(dance.Status) & ReviewStatus) != 0);
                    if (dance.RawRating.HasValue) {
                        match = match && (dance.RawRating.Value >= MinRating);
                        match = match && (dance.RawRating.Value <= MaxRating);
                    }
                    else {
                        match = match && IncludeTracksWithNoRating;
                    }
                    if (match) {
                        matches.Add(dance);
                    }
                }

                matches = (matches.Count > 0) ? matches : null;
            }
            else {
                matches = IncludeTracksWithNoDances ? matches : null;
            }
            return matches;
        }

        public List<TrackDanceInfo> Matches(ITrack track) {
            bool matches = true;
            if (track.BeatsPerMinute != 0) {
                matches = matches && (track.BeatsPerMinute >= MinBpm);
                matches = matches && (track.BeatsPerMinute <= MaxBpm);
            }
            else {
                matches = matches && IncludeTracksWithNoBpm;
            }

            if (track.LastPlayed != null) {
                matches = matches && ((LastPlayedBefore == null) || (track.LastPlayed < LastPlayedBefore));
                matches = matches && ((LastPlayedAfter == null) || (track.LastPlayed > LastPlayedAfter));
            }
            else {
                // never played always matches LastPlayedBefore and never matches LastPlayedAfter
                matches = matches && (LastPlayedAfter == null);
            }

            return matches ? MatchingDances(track) : null;
        }

        public bool IsMatch(ITrack track) {
            return Matches(track) != null;
        }

        public IEnumerable<ITrack> Apply(IEnumerable<ITrack> all) {
            return all.Where((t) => IsMatch(t));
        }

        public override string ToString() {
            string dances = string.Join(",", Dances.Select((d) => d.ToString()));
            return $"[{dances}]{Categories.ToString()}";
        }
    }
}
