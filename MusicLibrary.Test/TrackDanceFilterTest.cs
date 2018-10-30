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
                var dtr = new DanceTestResult() { TotalTracks = new TrackDanceFilter(dance).Apply(library.Tracks).Count() };

                var categories = dance.EnumerateCategories().ToList();
                categories.AddRangeUnique(new DanceCategory[] { DanceCategory.Competition, DanceCategory.Social });

                foreach (var cat in categories) {
                    var mask = (DanceCategories)(1 << (int)cat);
                    var filter = new TrackDanceFilter(dance) { Categories = mask };
                    var tracks = filter.Apply(library.Tracks);

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
        public void ShouldRespectFlagToIncludeTracksWithNoDances() {
            var library = SampleData.GetTestLibrary();
            var filter = new TrackDanceFilter() { Options = DanceFilterFlags.IncludeTracksWithNoDances };
            var tracks = filter.Apply(library.Tracks);
            var totalTracks = tracks.Count();

            filter = new TrackDanceFilter();
            tracks = filter.Apply(library.Tracks);
            var tracksWithDances = tracks.Count();
            Assert.Equal(SampleData.NumTracksWithoutDances, totalTracks - tracksWithDances);
        }

        [Fact]
        public void ShouldFilterByRating() {
            var library = SampleData.GetTestLibrary();
            foreach (var rating in SampleData.ExpectedTracksByRating.Keys) {
                var filter = new TrackDanceFilter() {
                    MinRating = TrackRating.FiveStarRatingToRaw(rating),
                    MaxRating = TrackRating.FiveStarRatingToRaw(5.0)
                };
                var tracks = filter.Apply(library.Tracks);
                Assert.Equal(SampleData.ExpectedTracksByRating[rating], tracks.Count());
                foreach (var track in tracks) {
                    Assert.NotNull(track.Rating);
                    Assert.True(track.Rating.FiveStarRating >= rating);
                }
            }
        }
        
        [Fact]
        public void ShouldFilterByReviewStatus() {
            var library = SampleData.GetTestLibrary();
            foreach (var status in new DanceReviewStatus[] {  DanceReviewStatus.NeedsReview, DanceReviewStatus.Reviewed }) {
                var filter = new TrackDanceFilter() {
                    Difficulty = DanceDifficulty.Any,
                    ReviewStatus = TrackDanceInfo.ToStatusFlags(status)
                };
                var tmp = library.Tracks.Where((t) => t.Dances.Dances.Any((d) => d.Status == status));
                var tracks = filter.Apply(tmp);
                // var tracks = filter.Filter(library.Tracks);
                Assert.Equal(SampleData.ExpectedTracksByReviewStatus[status], tracks.Count());
            }
        }
    }
}
