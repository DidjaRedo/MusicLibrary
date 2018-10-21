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
        public void ShouldFilterByDanceAndCategory() {
            var library = SampleData.GetTestLibrary();
            var results = new Dictionary<Dance,DanceTestResult>();

            foreach (var dance in Dances.All) {
                var dtr = new DanceTestResult() { TotalTracks = new TrackDanceFilter(dance).Filter(library.Tracks).Count() };

                var categories = dance.EnumerateCategories().ToList();
                categories.AddRangeUnique(new DanceCategory[] { DanceCategory.Competition, DanceCategory.Social });

                foreach (var cat in categories) {
                    var mask = (DanceCategories)(1 << (int)cat);
                    var filter = new TrackDanceFilter(dance) { Categories = mask };
                    var tracks = filter.Filter(library.Tracks);

                    dtr.SetTotalForCategory(cat, tracks.Count());

                    foreach (var track in tracks) {
                        Assert.Contains<TrackDanceInfo>(track.Dances.Dances, (tdi) => ((tdi.Categories & mask) != 0));
                    }
                }

                results[dance] = dtr;
            }

            foreach (var kvp in results) {
                var expected = SampleData.ExpectedDanceInfo[kvp.Key];
                Assert.Equal(expected, kvp.Value);
            }
        }

        [Fact]
        public void ShouldFilterByRating() {
            var library = SampleData.GetTestLibrary();
            foreach (var rating in SampleData.ExpectedTracksByRating.Keys) {
                var filter = new TrackDanceFilter() {
                    MinRating = TrackRating.FiveStarRatingToRaw(rating),
                    MaxRating = TrackRating.FiveStarRatingToRaw(5.0)
                };
                var tracks = filter.Filter(library.Tracks);
                Assert.Equal(SampleData.ExpectedTracksByRating[rating], tracks.Count());
                foreach (var track in tracks) {
                    Assert.NotNull(track.Rating);
                    Assert.True(track.Rating.FiveStarRating >= rating);
                }
            }
        }
    }
}
