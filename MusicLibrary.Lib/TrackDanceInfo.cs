using System;
using System.Collections.Generic;
using System.Text;

namespace MusicLibrary.Lib
{
    public class TrackDanceInfo
    {
        [Flags]
        public enum DanceDifficulty
        {
            Beginner = 0x01,
            Intermediate = 0x02,
            Advanced = 0x04,
            Any = Beginner|Intermediate|Advanced
        }

        public TrackDanceInfo(Dance dance, Dance.DanceCategories categories, DanceDifficulty difficulty = DanceDifficulty.Any, uint? rating = null) {
            Dance = dance;
            Categories = categories;
            Difficulty = difficulty;
            RawRating = rating;
        }

        public Dance Dance { get; }
        public Dance.DanceCategories Categories { get; }
        public DanceDifficulty Difficulty { get; }
        public uint? RawRating { get; }
        public double? PercentRating => (RawRating.HasValue ? (double?)((double)RawRating.Value / (double)255) : null);
        public double? FiveStarRating => (RawRating.HasValue ? ((double?)Math.Round(PercentRating.Value * 10.0) / 2.0) : null);
    }
}
