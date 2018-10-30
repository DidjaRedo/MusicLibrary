using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Xunit;
using MusicLibrary.Lib;

namespace MusicLibrary.Test
{
    public class LibraryFilterTest
    {
        [Fact]
        public void ShouldInitializeDancesByCategory() {
            var lib = new LibraryFilter(SampleData.GetTestLibrary());
            lib.AddDanceFiltersByCategory(DanceCategory.Standard, DanceCategory.Latin);
            Assert.Equal(lib.Filters.Keys, new string[] {
                "Waltz-Standard", "Tango-Standard", "Foxtrot-Standard", "Quickstep-Standard", "Viennese Waltz-Standard",
                "Samba-Latin", "Cha-Cha-Latin", "Rumba-Latin", "Paso Doble-Latin", "Jive-Latin"
            });
        }

        [Fact]
        public void ShouldPopulateFilterTracksFromLibrary() {
            var defaultFilter = new TrackDanceFilter() {
                Categories = DanceCategories.None,
                ReviewStatus = DanceReviewStatusFlags.Any,
                Difficulty = DanceDifficulty.Any,
            };
            var lib = new LibraryFilter(SampleData.GetTestLibrary(), defaultFilter);
            lib.AddDanceFiltersByCategory(Dance.AllCategories);
            foreach (var fi in lib.Filters.Values) {
                foreach (var dance in fi.Filter.Dances) {
                    var edi = SampleData.ExpectedDanceInfo[dance];
                    foreach (var category in Dance.EnumerateCategories(fi.Filter.Categories)) {
                        var expected = edi.GetTotalForCategory(category);
                        Assert.Equal(expected, fi.Tracks.Count);
                    }
                }
            }
        }

        [Fact]
        public void ShouldMergeDefaultCategories() {
            var minRating = TrackRating.FiveStarRatingToRaw(3.5);
            var maxRating = TrackRating.FiveStarRatingToRaw(5.0);
            var defaultFilter = new TrackDanceFilter() {
                ReviewStatus = DanceReviewStatusFlags.Any,
                Difficulty = DanceDifficulty.Any,
                MinRating = minRating, MaxRating = maxRating
            };
            var lib = new LibraryFilter(SampleData.GetTestLibrary(), defaultFilter);
            lib.AddDanceFiltersByCategory(DanceCategory.Standard, DanceCategory.Latin);

            foreach (var fi in lib.Filters.Values) {
                Assert.True(fi.Filter.MinRating.HasValue && (fi.Filter.MinRating.Value == minRating)
                            && fi.Filter.MaxRating.HasValue && (fi.Filter.MaxRating.Value == maxRating));
                Assert.All(fi.Tracks, (t) => {
                    Assert.NotNull(t.Rating);
                    Assert.InRange<uint>(t.Rating.RawRating, minRating, maxRating);
                });
            }
        }
    }
}
