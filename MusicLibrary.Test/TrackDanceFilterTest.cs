using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

using Xunit;

using MusicLibrary.Lib;

namespace MusicLibrary.Test
{
    public class TrackDanceFilterTest
    {
        [Fact]
        public void ShouldFilterByDance() {
            var library = SampleData.GetTestLibrary();
            foreach (var dance in Dances.All) {
                var filter = new TrackDanceFilter(dance);
                var tracks = filter.Filter(library.Tracks);
                ExpectedDanceInfo expected = null;
                int gotCount = tracks.Count();

                if (SampleData.ExpectedDanceInfo.TryGetValue(dance, out expected)) {
                    Assert.Equal(expected.TotalTracks, gotCount);
                }

                foreach (var track in tracks) {
                    Assert.Contains<TrackDanceInfo>(track.Dances.Dances, (tdi) => tdi.Dance == dance);
                }
            }
        }

        [Fact]
        public void ShouldFilterByCategory() {
            var library = SampleData.GetTestLibrary();
            foreach (var dance in Dances.All) {
                foreach (var cat in dance.EnumerateCategories()) {
                    var mask = (DanceCategories)(1 << (int)cat);
                    var filter = new TrackDanceFilter(dance) { Categories = mask };
                    var tracks = filter.Filter(library.Tracks);
                    ExpectedDanceInfo expected = null;
                    int gotCount = tracks.Count();
                    if (SampleData.ExpectedDanceInfo.TryGetValue(dance, out expected)) {
                        Assert.Equal(expected.GetTotalForCategory(cat), gotCount);
                    }

                    foreach (var track in tracks) {
                        Assert.Contains<TrackDanceInfo>(track.Dances.Dances, (tdi) => ((tdi.Categories & mask) != 0));
                    }
                }
            }
        }
    }
}
