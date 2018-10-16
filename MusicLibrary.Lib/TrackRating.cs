﻿using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MusicLibrary.Lib
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TrackRating
    {
        [JsonConstructor]
        public TrackRating(string rater, uint rating, ulong playCount) {
            Rater = rater;
            RawRating = rating;
            PlayCount = playCount;
            Dance = Dances.ByName[rater];
        }

        [JsonProperty]
        public string Rater { get; }

        [JsonProperty(PropertyName = "rating")]
        public uint RawRating { get; }

        [JsonProperty]
        public ulong PlayCount { get; }

        public double PercentRating => RawRatingToPercent(RawRating);
        public double FiveStarRating => RawRatingToFiveStar(RawRating);
        public Dance Dance { get; }

        public static double RawRatingToPercent(uint raw) {
            return Math.Round(((double)raw / (double)255), 2);
        }

        public static double RawRatingToFiveStar(uint raw) {
            return Math.Round(RawRatingToPercent(raw) * 10.0) / 2.0;
        }

        public static uint PercentRatingToRaw(double percent) {
            return (uint)Math.Round(255.0 * percent);
        }

        public static uint FiveStarRatingToRaw(double fiveStarRating) {
            return PercentRatingToRaw(fiveStarRating * 0.2);
        }

        public override bool Equals(object obj) {
            var other = obj as TrackRating;
            return (other != null) && (GetHashCode() == other.GetHashCode());
        }

        public override int GetHashCode() {
            var hash = Rater.GetHashCode();
            hash ^= RawRating.GetHashCode();
            hash ^= PlayCount.GetHashCode();
            hash ^= Dance?.Name.GetHashCode() ?? 0;
            return hash;
        }
    }
}
