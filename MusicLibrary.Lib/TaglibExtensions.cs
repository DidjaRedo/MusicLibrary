using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using TagLib.Id3v2;

namespace MusicLibrary.Lib
{
    public static class TagLibExtensions
    {
        public static List<CommentsFrame> GetAllCommentFrames(this Tag tag, string language) {
            List<CommentsFrame> best_frames = new List<CommentsFrame>();

            foreach (Frame frame in tag.GetFrames("COMM")) {
                CommentsFrame comm = frame as CommentsFrame;

                if (comm == null)
                    continue;

                var commentLanguage = comm.Language;

                if ((commentLanguage == language) || (commentLanguage == "XXX") || String.IsNullOrEmpty(language) || (language == "XXX")) {
                    best_frames.Add(comm);
                }
            }

            return best_frames;
        }

        public static Dictionary<string,string> GetAllComments(this Tag tag,string language) {
            var comments = new Dictionary<string, string>();
            foreach (var frame in tag.GetAllCommentFrames(language)) {
                comments.Add(frame.Description, frame.Text);
            }
            return comments;
        }

        public static Dictionary<string, string> GetAllComments(this Tag tag) => tag.GetAllComments(Tag.Language);

        public static string GetComment(this Tag tag, string description, string language) {
            CommentsFrame f = CommentsFrame.GetPreferred(tag, description, language);
            return (f != null) && (f.Description == description) ? f.ToString() : null;
        }

        public static string GetComment(this Tag tag, string description) => tag.GetComment(description, Tag.Language);

        public static List<PopularimeterFrame> GetAllRatingsFrames(this Tag tag, params string[] users) {
            var ratings = new List<PopularimeterFrame>();

            PopularimeterFrame popm;
            foreach (Frame frame in tag) {
                popm = frame as PopularimeterFrame;

                if (popm != null && ((users.Length == 0) || users.Contains(popm.User))) {
                    ratings.Add(popm);
                }
            }

            return ratings;
        }

        public static List<TrackRating> GetAllRatings(this Tag tag, params string[] users) {
            return tag.GetAllRatingsFrames(users).Select<PopularimeterFrame, TrackRating>((f) => new TrackRating(f.User, f.Rating, f.PlayCount)).ToList();
        }
    }
}
