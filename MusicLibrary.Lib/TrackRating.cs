using System;
using System.Collections.Generic;
using System.Text;

namespace MusicLibrary.Lib
{
    public class TrackRating
    {
        public TrackRating(string rater, uint rating, ulong playCount) {
            Rater = rater;
            RawRating = rating;
            PlayCount = playCount;
        }

        public string Rater { get; }
        public uint RawRating { get; }
        public double PercentRating => ((double)RawRating / (double)255);
        public double FiveStarRating => Math.Round(PercentRating * 10.0) / 2.0;
        public ulong PlayCount { get; }
    }
}
