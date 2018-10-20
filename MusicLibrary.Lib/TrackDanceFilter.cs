using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace MusicLibrary.Lib
{
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
                MaxRating = MaxRating
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
                    merged.Categories = merged.Categories ?? filter.Categories;
                    merged.MinBpm = merged.MinBpm ?? filter.MinBpm;
                    merged.MaxBpm = merged.MaxBpm ?? filter.MaxBpm;
                    merged.Difficulty = merged.Difficulty ?? filter.Difficulty;
                    merged.ReviewStatus = merged.ReviewStatus ?? filter.ReviewStatus;
                    merged.MinRating = merged.MinRating ?? filter.MinRating;
                    merged.MaxRating = merged.MaxRating ?? filter.MaxRating;
                    merged.Dances.AddRange(filter.Dances);
                }
            }
            return merged;
        }

        public List<TrackDanceInfo> MatchingDances(ITrack track) {
            var matches = new List<TrackDanceInfo>();
            foreach (var dance in track.Dances.Dances) {
                bool match = (Dances.Count == 0) || Dances.Contains(dance.Dance);
                // For category we need to match all of the filtered categories
                // For difficulty we need to match any of the filtered difficulties
                match = match && ((!Categories.HasValue) || ((dance.Categories & Categories.Value) == Categories.Value));
                match = match && ((!Difficulty.HasValue) || ((dance.Difficulty & Difficulty.Value) != 0));
                match = match && ((!ReviewStatus.HasValue) || (dance.Status == ReviewStatus.Value));
                if (match) {
                    matches.Add(dance);
                }
            }
            return matches;
        }

        public List<TrackDanceInfo> Matches(ITrack track) {
            bool matches = true;

            matches = matches && ((MinBpm == null) || (track.BeatsPerMinute >= MinBpm.Value));
            matches = matches && ((MaxBpm == null) || (track.BeatsPerMinute <= MaxBpm.Value));
            matches = matches && ((MinRating == null) || ((track.Rating != null) && (track.Rating.RawRating >= MinRating.Value)));
            matches = matches && ((MaxRating == null) || ((track.Rating != null) && (track.Rating.RawRating >= MaxRating.Value)));
            return matches ? MatchingDances(track) : new List<TrackDanceInfo>();
        }

        public bool IsMatch(ITrack track) {
            return (Matches(track).Count > 0);
        }

        public IEnumerable<ITrack> Filter(IEnumerable<ITrack> all) {
            return all.Where((t) => IsMatch(t));
        }
    }
}
