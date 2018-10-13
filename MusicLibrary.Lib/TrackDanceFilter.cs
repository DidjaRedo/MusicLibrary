using System;
using System.Collections.Generic;
using System.Text;

namespace MusicLibrary.Lib
{
    public class TrackDanceFilter
    {
        public List<Dance> Dances { get; } = new List<Dance>();
        public Dance.DanceCategories? Categories { get; set; } = null;
        public int? MinBpm { get; set; }
        public int? MaxBpm { get; set; }
        public TrackDanceInfo.DanceDifficulty? Difficulty { get; set; }
        public TrackDanceInfo.ReviewStatus? ReviewStatus { get; set; }
        public uint? MinRating { get; set; }
        public uint? MaxRating { get; set; }

        public List<TrackDanceInfo> MatchingDances(Track track) {
            var matches = new List<TrackDanceInfo>();
            foreach (var dance in track.Dances.Dances) {
                bool match = (Dances.Count == 0) || Dances.Contains(dance.Dance);
                match = match && ((Categories == null) || ((dance.Categories & Categories.Value) != 0));
                match = match && ((Difficulty == null) || ((dance.Difficulty & Difficulty.Value) != 0));
                match = match && ((ReviewStatus == null)) || (dance.Status == this.ReviewStatus.Value);
                if (match) {
                    matches.Add(dance);
                }
            }
            return matches;
        }

        public List<TrackDanceInfo> Matches(Track track) {
            bool matches = true;

            matches = matches && ((MinBpm == null) || (track.BeatsPerMinute >= MinBpm.Value));
            matches = matches && ((MaxBpm == null) || (track.BeatsPerMinute <= MaxBpm.Value));
            matches = matches && ((MinRating == null) || ((track.Rating != null) && (track.Rating.RawRating >= MinRating.Value)));
            matches = matches && ((MaxRating == null) || ((track.Rating != null) && (track.Rating.RawRating >= MaxRating.Value)));
            return matches ? MatchingDances(track) : new List<TrackDanceInfo>();
        }

        public bool IsMatch(Track track) {
            return (Matches(track).Count > 0);
        }
    }
}
