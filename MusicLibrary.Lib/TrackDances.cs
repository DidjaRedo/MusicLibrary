using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MusicLibrary.Lib
{
    public class TrackDances
    {
        public TrackDances(ITrack track) {
            Track = track;
            ApprovedTags = (track.MediaMonkey.Custom2 ?? "").SplitAndNormalize(';').ToArray();
            ReviewTags = (track.MediaMonkey.Custom3 ?? "").SplitAndNormalize(';').ToArray();
            _dances = InferDances(track, ApprovedTags, ReviewTags);
        }

        protected List<TrackDanceInfo> _dances;
        public IReadOnlyList<TrackDanceInfo> Dances => _dances.AsReadOnly();
        public string[] ApprovedTags { get; }
        public string[] ReviewTags { get; }

        protected static List<TrackDanceInfo> InferDances(ITrack track, string[] approvedTags, string[] reviewTags) {
            var dances = new List<Dance>();

            var mergedTags = new List<string>();
            if (!String.IsNullOrEmpty(track.Grouping)) {
                var dance = MusicLibrary.Lib.Dances.ByName[track.Grouping];
                if (dance != null) {
                    mergedTags.AddUnique("ballroom");
                    mergedTags.AddUnique(track.Grouping);
                }
            }

            if (approvedTags.Length > 0) {
                mergedTags.AddUnique("ballroom");
                mergedTags.AddRangeUnique(approvedTags);
            }

            mergedTags.AddRangeUnique(track.Genres);

            var approved = new CategoryFilter(mergedTags);

            var reviewFlags = (approved.Categories == DanceCategories.None) ? FilterFlags.SocialIsDefault : FilterFlags.None;
            var toReview = new CategoryFilter(reviewTags, reviewFlags);

            approved.Categories &= ~toReview.Categories;

            dances.AddRangeUnique(approved.Dances);
            dances.AddRangeUnique(toReview.Dances);

            var rating = track.Rating;

            var tdis = new List<TrackDanceInfo>();
            foreach (var dance in dances) {
                uint? danceRating = (track.Ratings.SingleOrDefault((r) => r.Dance == dance) ?? rating)?.RawRating;
                if (approved.Categories != DanceCategories.None) {
                    tdis.Add(new TrackDanceInfo(dance, approved.Categories, danceRating, status: DanceReviewStatus.Reviewed));
                }
                if (toReview.Categories != DanceCategories.None) {
                    tdis.Add(new TrackDanceInfo(dance, toReview.Categories, danceRating, status: DanceReviewStatus.NeedsReview));
                } 
            }
            return tdis;
        }

        protected ITrack Track { get; }

        public override string ToString() {
            return String.Join("|", Dances.Select<TrackDanceInfo, string>((d) => d.ToString()));
        }

        [Flags]
        protected enum FilterFlags {
            None = 0,
            BallroomIsImplied = 0x01,
            SocialIsDefault = 0x2
        }

        protected class CategoryFilter {
            public bool IsBallroom { get; }
            public DanceCategories Categories { get; set; }
            public IReadOnlyList<Dance> Dances { get; }
            public IReadOnlyList<string> Unused { get; }

            public CategoryFilter(IEnumerable <string> strings, FilterFlags flags = FilterFlags.None) {
                var remaining = strings.Select<string, string>((s) => s.ToLowerInvariant()).ToList();


                var isBallroomImplied = ((flags & FilterFlags.BallroomIsImplied) == FilterFlags.BallroomIsImplied);
                IsBallroom = isBallroomImplied || (remaining.RemoveAll((s) => s.Equals("ballroom")) > 0);
                bool isSocial = (remaining.RemoveAll((s) => s.Equals("social")) > 0);
                bool isCompetition = (remaining.RemoveAll((s) => s.Equals("competition")) > 0);

                bool isStandard = (remaining.RemoveAll((s) => s.Equals("standard")) > 0);
                bool isSmooth = (remaining.RemoveAll((s) => s.Equals("smooth")) > 0);
                bool isLatin = (remaining.RemoveAll((s) => s.Equals("latin")) > 0) && (IsBallroom || isSocial || isCompetition);
                bool isRhythm = (remaining.RemoveAll((s) => s.Equals("rhythm")) > 0);
                bool isSwing = (remaining.RemoveAll((s) => s.Equals("swing")) > 0) && (IsBallroom || isSocial || isCompetition);

                var cat = DanceCategories.None;
                cat = cat | (isStandard ? DanceCategories.Standard : DanceCategories.None);
                cat = cat | (isSmooth ? DanceCategories.Smooth : DanceCategories.None);
                cat = cat | (isLatin ? DanceCategories.Latin : DanceCategories.None);
                cat = cat | (isRhythm ? DanceCategories.Rhythm : DanceCategories.None);
                cat = cat | (isSwing ? DanceCategories.Swing : DanceCategories.None);
                cat = cat | (isSocial ? DanceCategories.Social : DanceCategories.None);
                cat = cat | (isCompetition ? DanceCategories.Competition : DanceCategories.None);
                if ((cat == DanceCategories.None) && ((flags & FilterFlags.SocialIsDefault) == FilterFlags.SocialIsDefault)) {
                    cat = DanceCategories.Social;
                }

                Categories = cat;

                var dances = new List<Dance>();
                var unused = new List<string>();

                if (IsBallroom || (Categories != DanceCategories.None)) {
                    foreach (var value in remaining) {
                        var dance = MusicLibrary.Lib.Dances.ByName[value];
                        if (dance != null) {
                            dances.AddUnique(dance);
                        }
                        else {
                            unused.AddUnique(value);
                        }
                    }
                }

                Dances = dances.AsReadOnly();
                Unused = unused.AsReadOnly();
            }
        }
    }
}
