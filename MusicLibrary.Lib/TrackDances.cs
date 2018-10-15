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
            _dances = InferDances(track);
        }

        protected List<TrackDanceInfo> _dances;
        public IReadOnlyList<TrackDanceInfo> Dances => _dances.AsReadOnly();

        protected static List<TrackDanceInfo> InferDances(ITrack track) {
            var dances = new List<Dance>();
            var genres = track.Genres.Select<string, string>((g) => g.ToLowerInvariant()).ToList();
            bool isBallroom = (genres.RemoveAll((s) => s.Equals("ballroom")) > 0);
            bool isSocial = (genres.RemoveAll((s) => s.Equals("social")) > 0);
            bool isCompetition = (genres.RemoveAll((s) => s.Equals("competition")) > 0);

            bool isStandard = (genres.RemoveAll((s) => s.Equals("standard")) > 0);
            bool isSmooth = (genres.RemoveAll((s) => s.Equals("smooth")) > 0);
            bool isLatin = (genres.RemoveAll((s) => s.Equals("latin")) > 0) && (isBallroom || isSocial || isCompetition);
            bool isRhythm = (genres.RemoveAll((s) => s.Equals("rhythm")) > 0);
            bool isSwing = (genres.RemoveAll((s) => s.Equals("swing")) > 0) && (isBallroom || isSocial || isCompetition);

            if (!String.IsNullOrEmpty(track.Grouping)) {
                var dance = MusicLibrary.Lib.Dances.ByName[track.Grouping];
                if (dance != null) {
                    isBallroom = true;
                    dances.Add(dance);
                }
            }

            if (isBallroom || isStandard || isSmooth || isLatin || isRhythm) {
                foreach (var genre in genres) {
                    var dance = MusicLibrary.Lib.Dances.ByName[genre];
                    if ((dance != null) && !dances.Contains(dance)) {
                        dances.Add(dance);
                    }
                }
            }

            var rating = track.Rating;

            var cat = DanceCategories.None;
            cat = cat | (isStandard ? DanceCategories.Standard : DanceCategories.None);
            cat = cat | (isSmooth ? DanceCategories.Smooth : DanceCategories.None);
            cat = cat | (isLatin ? DanceCategories.Latin : DanceCategories.None);
            cat = cat | (isRhythm ? DanceCategories.Rhythm : DanceCategories.None);
            cat = cat | (isSwing ? DanceCategories.Swing : DanceCategories.None);
            cat = cat | (isSocial ? DanceCategories.Social : DanceCategories.None);
            cat = cat | (isCompetition ? DanceCategories.Competition : DanceCategories.None);
            return dances.Select<Dance, TrackDanceInfo>((d) => {
                uint? danceRating = (track.Ratings.SingleOrDefault((r) => r.Dance == d) ?? rating)?.RawRating;
                return new TrackDanceInfo(d, cat, danceRating);
            }).ToList();
        }

        protected ITrack Track { get; }

        public override string ToString() {
            return String.Join("|", Dances.Select<TrackDanceInfo, string>((d) => d.ToString()));
        }
    }
}
