using System;
using System.Collections.Generic;
using System.Text;

namespace MusicLibrary.Lib
{
    public class TrackDanceInfo {
        [Flags]
        public enum DanceDifficulty {
            Beginner = 0x01,
            Intermediate = 0x02,
            Advanced = 0x04,
            Any = Beginner | Intermediate | Advanced
        }

        public enum ReviewStatus {
            NeedsReview,
            Reviewed
        }

        public TrackDanceInfo(Dance dance, Dance.DanceCategories categories, uint? rating = null, DanceDifficulty difficulty = DanceDifficulty.Any, ReviewStatus status = ReviewStatus.NeedsReview) {
            Dance = dance;
            Categories = categories;
            Difficulty = difficulty;
            RawRating = rating;
            Status = status;
        }

        public Dance Dance { get; }
        public Dance.DanceCategories Categories { get; }
        public DanceDifficulty Difficulty { get; }
        public ReviewStatus Status { get; set; }
        public uint? RawRating { get; }
        public double? PercentRating => (RawRating.HasValue ? (double?)((double)RawRating.Value / (double)255) : null);
        public double? FiveStarRating => (RawRating.HasValue ? ((double?)Math.Round(PercentRating.Value * 10.0) / 2.0) : null);

        public override string ToString() {
            var rating = (FiveStarRating.HasValue ? $"@{FiveStarRating.Value.ToString()}" : String.Empty);
            var review = (Status == ReviewStatus.NeedsReview) ? "?" : "!";
            return $"{Dance}[{Categories}]{rating}{review}";
        }
    }
}
