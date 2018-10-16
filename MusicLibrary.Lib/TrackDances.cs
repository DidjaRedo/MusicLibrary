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
            ReviewTags = (track.MediaMonkey.Custom2 ?? "").SplitAndNormalize(';').ToArray();
            _dances = InferDances(track, ReviewTags);
        }

        protected List<TrackDanceInfo> _dances;
        public IReadOnlyList<TrackDanceInfo> Dances => _dances.AsReadOnly();
        public string[] ReviewTags { get; }

        protected static List<TrackDanceInfo> InferDances(ITrack track, string[] reviewTags) {
            var dances = new List<Dance>();

            bool isBallroomImplied = false;
            if (!String.IsNullOrEmpty(track.Grouping)) {
                var dance = MusicLibrary.Lib.Dances.ByName[track.Grouping];
                if (dance != null) {
                    isBallroomImplied = true;
                    dances.Add(dance);
                }
            }

            var approved = new CategoryFilter(track.Genres, isBallroomImplied);
            var toReview = new CategoryFilter(reviewTags);

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

        protected class CategoryFilter {
            public bool IsBallroom { get; }
            public DanceCategories Categories { get; }
            public IReadOnlyList<Dance> Dances { get; }
            public IReadOnlyList<string> Unused { get; }

            public CategoryFilter(IEnumerable <string> strings, bool isBallroomImplied = true) {
                var remaining = strings.Select<string, string>((s) => s.ToLowerInvariant()).ToList();

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
