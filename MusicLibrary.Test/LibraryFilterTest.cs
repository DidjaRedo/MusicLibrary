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
            var names = lib.AllFilters.Select<FilteredTracks, string>((ft) => ft.Name);
            Assert.Equal(names, new string[] {
                "Waltz-Standard", "Tango-Standard", "Foxtrot-Standard", "Quickstep-Standard", "Viennese Waltz-Standard",
                "Samba-Latin", "Cha-Cha-Latin", "Rumba-Latin", "Paso Doble-Latin", "Jive-Latin"
            });
        }

        [Fact]
        public void ShouldPopulateFilterTracksFromLibrary() {
            var defaultFilter = new TrackDanceFilter() {
                Categories = DanceCategories.None,
                ReviewStatus = DanceReviewStatusFlags.Any,
                Difficulty = DanceDifficulty.Any
            };
            var lib = new LibraryFilter(SampleData.GetTestLibrary(), defaultFilter);
            lib.AddDanceFiltersByCategory(Dance.AllCategories);
            foreach (var fi in lib.AllFilters) {
                foreach (var dance in fi.Filter.Dances) {
                    var edi = SampleData.ExpectedDanceInfo[dance];
                    foreach (var category in Dance.EnumerateCategories(fi.Filter.Categories)) {
                        var expected = edi.GetTotalForCategory(category);
                        Assert.Equal(expected, fi.Tracks.Count);
                    }
                }
            }
        }
    }
}
