using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using Newtonsoft.Json;

namespace MusicLibrary.Lib
{
    [Flags]
    public enum DanceDifficulty
    {
        Beginner = 0x01,
        Intermediate = 0x02,
        Advanced = 0x04,
        Any = Beginner | Intermediate | Advanced
    }

    public enum DanceReviewStatus
    {
        [Description("Not Reviewed")]
        NotReviewed = 0,

        [Description("Needs Review")]
        NeedsReview = 1,

        [Description("Reviewed")]
        Reviewed = 2,

        [Description("Rejected")]
        Rejected = 3
    }

    [Flags]
    public enum DanceReviewStatusFlags
    {
        [Description("None")]
        None = 0,

        [Description("Not Reviewed")]
        NotReviewed = (1 << DanceReviewStatus.NotReviewed),

        [Description("Needs Review")]
        NeedsReview = (1 << DanceReviewStatus.NeedsReview),

        [Description("Reviewed")]
        Reviewed = (1 << DanceReviewStatus.Reviewed),

        [Description("Rejected")]
        Rejected = (1 << DanceReviewStatus.Rejected),

        [Description("Any Not Rejected")]
        AnyNotRejected = NotReviewed | NeedsReview | Reviewed,

        [Description("Any")]
        Any = AnyNotRejected | Rejected
    }

    public class TrackDanceInfo {
        [JsonConstructor]
        public TrackDanceInfo(Dance dance, DanceCategories categories, uint? rawRating = null, DanceDifficulty difficulty = DanceDifficulty.Any, DanceReviewStatus status = DanceReviewStatus.NeedsReview) {
            Dance = dance;
            Categories = categories;
            Difficulty = difficulty;
            RawRating = rawRating;
            Status = status;
        }

        [JsonIgnore]
        public Dance Dance { get; protected set;  }

        public string DanceName {
            get { return Dance.Name; }
            set { Dance = Dances.ByName[value];  }
        }

        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public DanceCategories Categories { get; }

        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public DanceDifficulty Difficulty { get; }

        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public DanceReviewStatus Status { get; set; }

        public uint? RawRating { get; }

        [JsonIgnore]
        public double? PercentRating => (RawRating.HasValue ? (double?)((double)RawRating.Value / (double)255) : null);

        [JsonIgnore]
        public double? FiveStarRating => (RawRating.HasValue ? ((double?)Math.Round(PercentRating.Value * 10.0) / 2.0) : null);

        public static DanceReviewStatusFlags ToStatusFlags(DanceReviewStatus status) {
            return (DanceReviewStatusFlags)(1 << (int)status);
        }

        public override string ToString() {
            var rating = (FiveStarRating.HasValue ? $"@{FiveStarRating.Value.ToString()}" : String.Empty);
            var review = (Status == DanceReviewStatus.NeedsReview) ? "?" : "!";
            return $"{Dance}[{Categories}]{rating}{review}";
        }
    }
}
