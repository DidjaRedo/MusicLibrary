using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;
using System.Collections.ObjectModel;

namespace MusicLibrary.Lib
{
    [Flags] public enum DanceFilterFlags
    {
        Default = 0x00,
        IncludeTracksWithNoDances = 0x01,
        IncludeTracksWithNoBpm = 0x02,
        IncludeTracksWithNoRating = 0x4
    };

    public class TrackDanceFilter : ObservableBase
    {

        protected string _name;
        protected DanceCategories _categories = DanceCategories.None;
        protected int? _minBpm;
        protected int? _maxBpm;
        protected DanceDifficulty _difficulty = DanceDifficulty.Any;
        protected DanceReviewStatusFlags _reviewStatus = DanceReviewStatusFlags.AnyNotRejected;
        protected uint? _minRating;
        protected uint? _maxRating;
        protected DanceFilterFlags _options = DanceFilterFlags.Default;
        protected string _description;

        public string Name { get => _name; set => Set(ref _name, value, "Name"); }
        public string Description { get => _description ?? ToString(); set => Set(ref _description, value, "Description"); }

        public ObservableCollection<Dance> Dances { get; } = new ObservableCollection<Dance>();

        public DanceCategories Categories { get => _categories; set => Set(ref _categories, value, "Categories"); }

        public int? MinBpm { get => _minBpm; set => Set(ref _minBpm, value, "MinBpm"); }
        public int? MaxBpm { get => _maxBpm; set => Set(ref _maxBpm, value, "MaxBpm"); }
        public DanceDifficulty Difficulty { get => _difficulty; set => Set(ref _difficulty, value, "Difficulty"); }
        public DanceReviewStatusFlags ReviewStatus { get => _reviewStatus; set => Set(ref _reviewStatus, value, "ReviewStatus"); }
        public uint? MinRating { get => _minRating; set => Set(ref _minRating, value, "MinRating", "MinRatingFiveStars"); }
        public uint? MaxRating { get => _maxRating; set => Set(ref _maxRating, value, "MaxRating", "MaxRatingFiveStars"); }

        public double? MinRatingFiveStars {
            get => TrackRating.RawRatingToFiveStar(MinRating.Value);
            set => MinRating = TrackRating.FiveStarRatingToRaw(value);
        }

        public double? MaxRatingFiveStars {
            get => TrackRating.RawRatingToFiveStar(MaxRating.Value);
            set => MaxRating = TrackRating.FiveStarRatingToRaw(value);
        }

        public DanceFilterFlags Options { get => _options; set => Set(ref _options, value, "Options"); }

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
                    if ((dance.RawRating.HasValue) || ((Options & DanceFilterFlags.IncludeTracksWithNoRating) == 0)) {
                        match = match && ((MinRating == null) || ((dance.RawRating.HasValue) && (dance.RawRating.Value >= MinRating.Value)));
                        match = match && ((MaxRating == null) || ((dance.RawRating.HasValue) && (dance.RawRating.Value <= MaxRating.Value)));
                    }
                    if (match) {
                        matches.Add(dance);
                    }
                }

                matches = (matches.Count > 0) ? matches : null;
            }
            else {
                matches = ((Options & DanceFilterFlags.IncludeTracksWithNoDances) != 0) ? matches : null;
            }
            return matches;
        }

        public List<TrackDanceInfo> Matches(ITrack track) {
            bool matches = true;
            if ((track.BeatsPerMinute != 0) || ((Options & DanceFilterFlags.IncludeTracksWithNoBpm) == 0)) {
                matches = matches && ((MinBpm == null) || (track.BeatsPerMinute >= MinBpm.Value));
                matches = matches && ((MaxBpm == null) || (track.BeatsPerMinute <= MaxBpm.Value));
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
