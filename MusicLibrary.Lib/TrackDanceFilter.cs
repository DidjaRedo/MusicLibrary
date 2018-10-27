using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace MusicLibrary.Lib
{
    [Flags] public enum DanceFilterFlags
    {
        Default = 0x00,
        IncludeTracksWithNoDances = 0x01,
        IncludeTracksWithNoBpm = 0x02,
        IncludeTracksWithNoRating = 0x4
    };

    public class TrackDanceFilter
    {
        public string Name { get; set; }
        public List<Dance> Dances { get; } = new List<Dance>();
        public DanceCategories? Categories { get; set; } = null;
        public int? MinBpm { get; set; }
        public int? MaxBpm { get; set; }
        public DanceDifficulty? Difficulty { get; set; }
        public DanceReviewStatus? ReviewStatus { get; set; }
        public uint? MinRating { get; set; }
        public uint? MaxRating { get; set; }
        public DanceFilterFlags Options { get; set; } = DanceFilterFlags.Default;

        public TrackDanceFilter(IEnumerable<Dance> dances) {
            if (dances != null) {
                Dances.AddRange(dances);
            }
        }

        public TrackDanceFilter(params Dance[] dances) : this((IEnumerable<Dance>)dances) {

        }

        public TrackDanceFilter Clone() {
            var result = new TrackDanceFilter() {
                Name = Name,
                Categories = Categories,
                MinBpm = MinBpm,
                MaxBpm = MaxBpm,
                Difficulty = Difficulty,
                ReviewStatus = ReviewStatus,
                MinRating = MinRating,
                MaxRating = MaxRating,
                Options = Options
            };
            result.Dances.AddRange(Dances);
            return result;
        }

        public static TrackDanceFilter Merge(params TrackDanceFilter[] filters) {
            return Merge((IEnumerable<TrackDanceFilter>)filters);
        }

        public static TrackDanceFilter Merge(IEnumerable<TrackDanceFilter> filters) {
            TrackDanceFilter merged = null;
            foreach (var filter in filters) {
                if (merged == null) {
                    merged = filter.Clone();
                }
                else {
                    merged.Name = merged.Name ?? filter.Name;
                    merged.Categories = merged.Categories ?? filter.Categories;
                    merged.MinBpm = merged.MinBpm ?? filter.MinBpm;
                    merged.MaxBpm = merged.MaxBpm ?? filter.MaxBpm;
                    merged.Difficulty = merged.Difficulty ?? filter.Difficulty;
                    merged.ReviewStatus = merged.ReviewStatus ?? filter.ReviewStatus;
                    merged.MinRating = merged.MinRating ?? filter.MinRating;
                    merged.MaxRating = merged.MaxRating ?? filter.MaxRating;
                    merged.Options |= filter.Options;
                    merged.Dances.AddRange(filter.Dances);
                }
            }
            return merged;
        }

        public List<TrackDanceInfo> MatchingDances(ITrack track) {
            var matches = new List<TrackDanceInfo>();
            if (track.Dances.Dances.Count > 0) {
                foreach (var dance in track.Dances.Dances) {
                    bool match = (Dances.Count == 0) || Dances.Contains(dance.Dance);
                    // For category we need to match all of the filtered categories
                    // For difficulty we need to match any of the filtered difficulties
                    match = match && ((!Categories.HasValue) || ((dance.Categories & Categories.Value) == Categories.Value));
                    match = match && ((!Difficulty.HasValue) || ((dance.Difficulty & Difficulty.Value) != 0));
                    match = match && ((!ReviewStatus.HasValue) || (dance.Status == ReviewStatus.Value));
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

        public IEnumerable<ITrack> Filter(IEnumerable<ITrack> all) {
            return all.Where((t) => IsMatch(t));
        }

        public override string ToString() {
            string dances = string.Join(",", Dances.Select((d) => d.ToString()));
            return $"[{dances}]{(Categories.HasValue ? Categories.ToString() : String.Empty)}";
        }
    }
}
