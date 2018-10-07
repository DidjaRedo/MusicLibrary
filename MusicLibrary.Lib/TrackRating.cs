using System;
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

        public double PercentRating => ((double)RawRating / (double)255);
        public double FiveStarRating => Math.Round(PercentRating * 10.0) / 2.0;
        public Dance Dance { get; }
    }
}
