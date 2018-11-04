using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Xunit;
using MusicLibrary.Lib;

namespace MusicLibrary.Test
{
    public class DanceInfoTagParserTest
    {
        [Fact]
        void ShouldExtractDances() {
            var parser = new DanceInfoTagParser("Foxtrot", "Waltz");
            Assert.Contains<Dance>(Dances.Foxtrot, parser.Dances);
            Assert.Contains<Dance>(Dances.Waltz, parser.Dances);
        }

        [Fact]
        void ShouldExtractCategories() {
            var parser = new DanceInfoTagParser("Waltz", "Smooth", "Standard");
            Assert.Equal(DanceCategories.Smooth | DanceCategories.Standard, parser.Categories);
        }

        [Fact]
        void ShouldExtractBallroomTag() {
            var parser = new DanceInfoTagParser("Waltz", "Smooth", "Standard");
            Assert.False(parser.IsBallroom);
            parser = new DanceInfoTagParser("Ballroom", "Waltz", "Smooth", "Standard");
            Assert.True(parser.IsBallroom);
        }

        [Fact]
        void ShouldReportUnusedTags() {
            var parser = new DanceInfoTagParser("Ballroom", "Quickstep", "Standard", "Big Band");
            Assert.Contains("Big Band", parser.UnusedTags);
        }

        [Fact]
        void ShouldExtractWhenRatingsArePresent() {
            var parser = new DanceInfoTagParser("Quickstep@3.5", "Standard@4", "Social", "Big Band@5");
            Assert.Contains<Dance>(Dances.Quickstep, parser.Dances);
            Assert.Equal(DanceCategories.Standard | DanceCategories.Social, parser.Categories);
            Assert.Contains("Big Band@5", parser.UnusedTags);
        }

        [Fact]
        void ShouldApplyAppropriateCategoriesToDances() {
            var parser = new DanceInfoTagParser("WC Swing", "Social", "Standard", "Swing");
            var tdis = parser.GetTagDanceInfo().ToList();
            Assert.Single(tdis);
            Assert.Equal(DanceCategories.Social | DanceCategories.Swing, tdis[0].Categories);
        }

        [Fact]
        void ShouldApplyDanceRatingsWhenSupplied() {
            var parser = new DanceInfoTagParser("WC Swing@3.5", "Social", "Swing");
            var tdis = parser.GetTagDanceInfo().ToList();
            Assert.Single(tdis);
            Assert.Equal(3.5, tdis[0].FiveStarRating);
        }

        [Fact]
        void ShouldApplyCategoryRatingsWhenSupplied() {
            var parser = new DanceInfoTagParser("WC Swing@4.0", "Social", "Swing@3.5");
            var tdis = parser.GetTagDanceInfo().ToList();

            Assert.Equal(2, tdis.Count);
            var social = tdis.Single((t) => (t.Categories & DanceCategories.Social) != DanceCategories.None);
            var swing = tdis.Single((t) => (t.Categories & DanceCategories.Swing) != DanceCategories.None);
            Assert.Equal(4.0, social.FiveStarRating);
            Assert.Equal(3.5, swing.FiveStarRating);
        }

        [Fact]
        void ShouldAllowMultipleDances() {
            var parser = new DanceInfoTagParser("Foxtrot", "WC Swing", "Smooth", "Social", "Swing");
            var tdis = parser.GetTagDanceInfo().ToList();

            Assert.Equal(2, tdis.Count);
            var swing = tdis.Single((t) => t.Dance == Dances.WestCoastSwing);
            var foxtrot = tdis.Single((t) => t.Dance == Dances.Foxtrot);
            Assert.Equal(DanceCategories.Social | DanceCategories.Swing, swing.Categories);
            Assert.Equal(DanceCategories.Social | DanceCategories.Smooth, foxtrot.Categories);
        }

        [Fact]
        void ShouldApplyDefaultRatings() {
            var parser = new DanceInfoTagParser(127, DanceCategories.None, "Foxtrot", "Smooth", "Social");
            var tdis = parser.GetTagDanceInfo().ToList();

            Assert.Single(tdis);
            Assert.True(tdis[0].RawRating.HasValue);
            Assert.Equal((uint)127, tdis[0].RawRating.Value);
        }

        [Fact]
        void ShouldMergeDefaultAndSuppliedRatings() {
            var defaultRating = TrackRating.FiveStarRatingToRaw(4.0);
            var parser = new DanceInfoTagParser(defaultRating, DanceCategories.None, "WC Swing@4.5", "Foxtrot", "Smooth", "Social@3.5", "Swing");
            var tdis = parser.GetTagDanceInfo().ToList();

            Assert.Equal(4, tdis.Count);
            var foxtrots = tdis.Where((t) => t.Dance == Dances.Foxtrot);
            var smooth = foxtrots.Single((t) => (t.Categories & DanceCategories.Smooth) != DanceCategories.None);
            var social = foxtrots.Single((t) => (t.Categories & DanceCategories.Social) != DanceCategories.None);
            Assert.Equal(defaultRating, smooth.RawRating);
            Assert.Equal(3.5, social.FiveStarRating);

            var swings = tdis.Where((t) => t.Dance == Dances.WestCoastSwing);
            var swing = swings.Single((t) => (t.Categories & DanceCategories.Swing) != DanceCategories.None);
            social = swings.Single((t) => (t.Categories & DanceCategories.Social) != DanceCategories.None);
            Assert.Equal(4.5, swing.FiveStarRating);
            Assert.Equal(3.5, social.FiveStarRating);
        }
    }
}
