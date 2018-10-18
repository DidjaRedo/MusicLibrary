using System;
using System.Collections.Generic;
using System.Text;

using Xunit;
using MusicLibrary.Lib;

namespace MusicLibrary.Test
{
    public class DanceInfoTagParserTest
    {
        [Fact]
        void ShouldExtractDances() {
            var parser = new DanceInfoTagParser(null, DanceCategories.None, "Foxtrot", "Waltz");
            Assert.Contains<Dance>(Dances.Foxtrot, parser.Dances);
            Assert.Contains<Dance>(Dances.Waltz, parser.Dances);
        }

        [Fact]
        void ShouldExtractCategories() {
            var parser = new DanceInfoTagParser(null, DanceCategories.None, "Waltz", "Smooth", "Standard");
            Assert.Equal(DanceCategories.Smooth | DanceCategories.Standard, parser.Categories);
        }

        [Fact]
        void ShouldExtractBallroomTag() {
            var parser = new DanceInfoTagParser(null, DanceCategories.None, "Waltz", "Smooth", "Standard");
            Assert.False(parser.IsBallroom);
            parser = new DanceInfoTagParser(null, DanceCategories.None, "Ballroom", "Waltz", "Smooth", "Standard");
            Assert.True(parser.IsBallroom);
        }

        [Fact]
        void ShouldReportUnusedTags() {
            var parser = new DanceInfoTagParser(null, DanceCategories.None, "Ballroom", "Quickstep", "Standard", "Big Band");
            Assert.Contains("Big Band", parser.UnusedTags);
        }

        [Fact]
        void ShouldExtractWhenRatingsArePresent() {
            var parser = new DanceInfoTagParser(null, DanceCategories.None, "Quickstep@3.5", "Standard@4", "Social", "Big Band@5");
            Assert.Contains<Dance>(Dances.Quickstep, parser.Dances);
            Assert.Equal(DanceCategories.Standard | DanceCategories.Social, parser.Categories);
            Assert.Contains("Big Band@5", parser.UnusedTags);
        }
    }
}
