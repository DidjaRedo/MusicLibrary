using System;
using System.Collections.Generic;
using System.Text;

using Xunit;
using MusicLibrary.Lib;

namespace MusicLibrary.Test
{
    public class TrackRatingTest
    {
        [Fact]
        public void ShouldRoundTripRatingsValues() {
            for (double s = 0.0; s <= 5.0; s += 0.5) {
                uint r = TrackRating.FiveStarRatingToRaw(s);
                double ns = TrackRating.RawRatingToFiveStar(r);
                Assert.Equal(s, ns);
            }

            for (double p = 0.0; p <= 1.0; p = Math.Round(p + 0.01, 2)) {
                uint r = TrackRating.PercentRatingToRaw(p);
                double np = TrackRating.RawRatingToPercent(r);
                Assert.Equal(p, np);
            }
        }
    }
}
